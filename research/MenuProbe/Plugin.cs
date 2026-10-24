// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: AssemblyVersion(NikichMods.BPSMenuProbe.Plugin.Version+".0")]
[assembly: AssemblyFileVersion(NikichMods.BPSMenuProbe.Plugin.Version+".0")]
namespace NikichMods.BPSMenuProbe
{
    [BepInPlugin(Id,"Better Prayer Stations - Menu Probe",Version)]
    public sealed class Plugin:BaseUnityPlugin,CraftInterface
    {
        public const string Id="nikichmods.bps.menuprobe",Version="0.1.0";
        const string Host="e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        const string BuyId="bps_menu_probe_buy",PlaceId="bps_menu_probe_place",RemoveId="bps_menu_probe_remove";
        static Plugin self;
        readonly BeamState state=new BeamState();
        readonly List<WorldGameObject> stations=new List<WorldGameObject>();
        readonly Dictionary<ObjectDefinition,DefinitionSnapshot> definitions=new Dictionary<ObjectDefinition,DefinitionSnapshot>();
        sealed class DefinitionSnapshot{internal ObjectDefinition.InteractionType Interaction;internal string Zone;}
        Harmony harmony;
        GameSave session;
        WorldGameObject source;
        WorldGameObject previousDesk;
        CraftInterface previousController;
        CraftGUI menu;
        ObjectCraftDefinition buyCard;
        FieldInfo saveInventory,modeField,cameraField,hideBuildField;
        MethodInfo fitCamera,openMenu;
        bool ready,ownMenu,editor,removing,painting,showPanel=true,validCell,cursorSet,refreshRequested,recoveryPending;
        bool menuQueueWasActive;
        int support,cursorX,cursorY,lastSupport=-1,editorStartFrame;
        float nextRefresh,gamepadRepeat,recoveryUntil;
        Vector2 gamepadCursor;
        Action pending;
        string status="Загрузите игру. Все изменения только в памяти.";
        Rect panel=new Rect(12,70,345,190);
        GUIStyle label;
        static bool Station(string id){return id=="pray_stand_wd" || id=="pray_stand_stn";}
        static bool StationCandidate(string id){return Station(id) || id=="pray_stand_wd_place" || id=="pray_stand_stn_place";}
        bool Current()
        {return ready && MainGame.me!=null && MainGame.game_started && MainGame.me.save!=null && MainGame.me.player!=null && MainGame.me.player.data!=null && ReferenceEquals(session,MainGame.me.save) && ReferenceEquals(saveInventory.GetValue(session),MainGame.me.player.data);}
        void Log(string value){Logger.LogInfo("BPS_MENU "+value);}
        void Awake()
        {
            self=this;
            try
            {
                string hash;using(var s=SHA256.Create())using(var f=File.OpenRead(typeof(MainGame).Assembly.Location))hash=BitConverter.ToString(s.ComputeHash(f)).Replace("-","").ToLowerInvariant();
                if(hash!=Host)throw new InvalidOperationException("Unverified host");
                saveInventory=AccessTools.Field(typeof(GameSave),"_inventory");
                modeField=AccessTools.Field(typeof(BuildModeLogics),"_mode");cameraField=AccessTools.Field(typeof(BuildModeLogics),"_zone_camera_tf");
                fitCamera=AccessTools.Method(typeof(BuildModeLogics),"GetFitCameraPos",new[]{typeof(Vector3)});
                openMenu=AccessTools.Method(typeof(CraftGUI),"Open",new[]{typeof(WorldGameObject),typeof(CraftsInventory),typeof(string)});
                hideBuildField=AccessTools.Field(typeof(CraftGUI),"hide_after_build_gui");
                if(saveInventory==null || modeField==null || cameraField==null || fitCamera==null || openMenu==null || hideBuildField==null)throw new MissingMemberException("Native session/editor members");
                harmony=new Harmony(Id);
                Patch(typeof(MainGame),"OpenBuildObjectGUI",nameof(OpenPrefix),null,new[]{typeof(WorldGameObject)});
                Patch(typeof(MainGame),"OnGameLoaded",nameof(Loading),null,new[]{typeof(GameSave)});
                Patch(typeof(MainGame),"OnGameStartedPlaying",null,nameof(Started),Type.EmptyTypes);
                Patch(typeof(WorldMap),"OnAddNewWGO",null,nameof(Added),new[]{typeof(WorldGameObject)});
                Patch(typeof(BuildModeLogics),"Update",nameof(BuildUpdate),null,Type.EmptyTypes);
                Patch(typeof(BuildGrid),"IsCellBusy",nameof(CellBusy),null,new[]{typeof(Vector2),typeof(List<Collider2D>)});
                Patch(typeof(CraftGUI),"AddRemoveBuildingItem",nameof(NativeRemoveCard),null,Type.EmptyTypes);
                Patch(typeof(GJL),"L",nameof(Localize),null,new[]{typeof(string)});
                ready=true;Log("READY version="+Version+" persistent=False faith_debit=False quality_effect=False");
            }
            catch(Exception ex){Fail(ex);}
        }
        void Patch(Type t,string name,string prefix,string postfix,Type[] args)
        {var m=AccessTools.Method(t,name,args);if(m==null)throw new MissingMethodException(t.Name,name);harmony.Patch(m,prefix==null?null:new HarmonyMethod(typeof(Plugin),prefix),postfix==null?null:new HarmonyMethod(typeof(Plugin),postfix));}
        static bool Localize(string __0,ref string __result)
        {
            var p=self;if(p==null || !p.ready)return true;
            if(__0==BuyId)__result="Купить +1 вместимость (тест: бесплатно)";
            else if(__0==PlaceId)__result="Поставить свет";
            else if(__0==RemoveId)__result="Убрать свет";
            else if(__0==BuyId+"_d")__result="Цена для будущей игры: "+(p.state.NextPrice==0?"предел теста":p.state.NextPrice+" веры")+". В этом тесте вера не списывается.";
            else if(__0==PlaceId+"_d")__result="При размещении на могиле превращает все красные черепа в белые. Сейчас проверяем только точки, без эффекта.";
            else if(__0==RemoveId+"_d")__result="Убирает точку бесплатно. Оплаченная вместимость сохраняется.";
            else return true;
            return false;
        }
        static bool OpenPrefix(WorldGameObject __0)
        {
            var p=self;if(p==null || !p.Current() || __0==null || !Station(__0.obj_id))return true;
            try{p.Open(__0,false);}catch(Exception ex){p.Fail(ex);}return false;
        }
        static void Loading(){if(self!=null)self.ResetSession("load");}
        static void Started(){if(self!=null && self.ready){self.recoveryPending=true;self.recoveryUntil=Time.realtimeSinceStartup+30f;}}
        static void Added(WorldGameObject __0)
        {var p=self;if(p==null || !p.Current() || __0==null || !StationCandidate(__0.obj_id))return;if(!p.stations.Contains(__0))p.stations.Add(__0);p.refreshRequested=true;}
        static bool BuildUpdate(){return self==null || !self.editor;}
        static bool CellBusy(ref bool __result)
        {if(self==null || !self.editor || !self.painting)return true;__result=false;return false;}
        static bool NativeRemoveCard(CraftGUI __instance)
        {return self==null || !self.ownMenu || !ReferenceEquals(self.menu,__instance);}
        void StartSession()
        {
            ResetSession("start");
            if(MainGame.me==null || MainGame.me.save==null || MainGame.me.player==null || !ReferenceEquals(saveInventory.GetValue(MainGame.me.save),MainGame.me.player.data))return;
            session=MainGame.me.save;
            foreach(var w in WorldMap.objs)if(w!=null && StationCandidate(w.obj_id))stations.Add(w);
            foreach(string id in new[]{"pray_stand_wd","pray_stand_stn"})
            {
                var d=GameBalance.me.GetData<ObjectDefinition>(id);
                definitions.Add(d,new DefinitionSnapshot{Interaction=d.interaction_type,Zone=d.zone_id});
                d.interaction_type=ObjectDefinition.InteractionType.Builder;d.zone_id="graveyard";
            }
            RefreshSupport();status="Подойдите к молельне и взаимодействуйте. F8 — панель теста.";
            Log("SESSION station_candidates="+stations.Count+" support="+support+" state_reset=True");
        }
        void RefreshSupport()
        {
            support=0;
            for(int i=stations.Count-1;i>=0;i--)
            {
                var w=stations[i];if(w==null || w.is_removed || !StationCandidate(w.obj_id)){stations.RemoveAt(i);continue;}
                if(!Station(w.obj_id) || w.GetComponent<FloatingWorldGameObject>()!=null || w.is_removing || w.components.craft.is_crafting)continue;
                var z=WorldZone.GetZoneByID("graveyard",true);if(z==null || !z.GetZoneWGOs().Contains(w))continue;
                support+=w.obj_id=="pray_stand_wd"?5:10;
            }
            if(support!=lastSupport){lastSupport=support;Log("SUPPORT capacity="+support+" purchased="+state.Purchased+" placed="+state.Points.Count+" active="+state.Active(support));if(ownMenu)refreshRequested=true;}
        }
        static ObjectCraftDefinition Card(string id,string icon)
        {return new ObjectCraftDefinition{id=id,custom_name=id,icon=icon,build_type=ObjectCraftDefinition.BuildType.None,craft_type=CraftDefinition.CraftType.None,needs_unlock=false,disable_multi_craft=true,force_multi_craft=false};}
        void Open(WorldGameObject station,bool shortcut)
        {
            if(!Current() || station==null || station.is_removed || station.is_removing || station.components.craft.is_crafting || !Station(station.obj_id) || editor || FloatingWorldGameObject.IsFloating())return;
            if(MainGame.me.gui_elements.IsAnyMassiveWindowOpened())return;
            RefreshSupport();source=station;menu=MainGame.me.gui_elements.craft;
            var inventory=new CraftsInventory();
            string faithIcon=GameBalance.me.GetData<ItemDefinition>("faith").icon;
            buyCard=Card(BuyId,faithIcon);
            inventory.additional_crafts=new List<ObjectCraftDefinition>{buyCard,Card(PlaceId,faithIcon),Card(RemoveId,"i_b_remove")};
            previousController=menu.custom_craft_interface;previousDesk=BuildModeLogics.last_build_desk;
            menu.custom_craft_interface=this;menu.has_amount_buttons=false;hideBuildField.SetValue(menu,true);
            BuildModeLogics.last_build_desk=station;ownMenu=true;MainGame.paused=true;
            openMenu.Invoke(menu,new object[]{station,inventory,null});
            menu.ON_CRAFT_GUI_HIDE=MenuHidden;
            if(menu.queue!=null){menuQueueWasActive=menu.queue.gameObject.activeSelf;menu.queue.gameObject.SetActive(false);}
            RefreshMenu();Log("OPEN station="+station.unique_id+" shortcut="+shortcut+" cards="+menu.list_items.Count);
            if(menu.list_items.Count!=3)throw new InvalidOperationException("Expected exactly three native cards");
        }
        void MenuHidden()
        {
            ownMenu=false;
            if(menu!=null)
            {
                if(ReferenceEquals(menu.custom_craft_interface,this))menu.custom_craft_interface=previousController;
                if(menu.queue!=null)menu.queue.gameObject.SetActive(menuQueueWasActive);
            }
            if(ReferenceEquals(BuildModeLogics.last_build_desk,source))BuildModeLogics.last_build_desk=previousDesk;
            Log("MENU_CLOSED controller_restored=True");
        }
        string Counts()
        {return "Оплачено: "+state.Purchased+" / поддержка молельнями: "+support+"\nТочек: "+state.Points.Count+" · работают: "+state.Active(support)+" · свободно: "+state.Free(support);}
        void RefreshMenu()
        {
            if(!ownMenu || menu==null || !menu.is_shown)return;
            foreach(var row in menu.list_items)row.Redraw();
            if(menu.object_info!=null)
            {
                menu.object_info.text_header.text="Благословения — прототип";
                menu.object_info.text_descr.gameObject.SetActive(true);
                menu.object_info.text_descr.text=Counts()+"\nСледующая цена: "+(state.NextPrice==0?"предел теста":state.NextPrice+" веры")+". Сейчас бесплатно.\nТест ограничен 10 покупками. Сохранение и качество не меняются.";
                menu.object_info.table.Reposition();
            }
            refreshRequested=false;
        }
        public bool CanCraft(CraftDefinition cd,List<string> multi,int amount,List<Item> needs)
        {
            if(!Current() || !ownMenu || editor || pending!=null || amount!=1)return false;
            if(cd.id==BuyId)return state.CanBuy(support);
            if(cd.id==PlaceId)return state.Free(support)>0;
            if(cd.id==RemoveId)return state.Points.Count>0;
            return false;
        }
        public bool OnCraft(CraftDefinition cd,Item body,List<string> multi,int amount,List<Item> needs,WorldGameObject other)
        {
            try
            {
                RefreshSupport();if(!CanCraft(cd,multi,amount,needs))return false;
                var player=MainGame.me.player.data;int faith=player.GetTotalCount("faith",true);float money=player.money;
                if(cd.id==BuyId){int quote=state.NextPrice;if(!state.Buy(support))return false;status="Вместимость +1. Вера не списана.";Log("BUY simulated=True quote="+quote+" purchased="+state.Purchased);refreshRequested=true;}
                else if(cd.id==PlaceId)pending=()=>BeginEditor(false);
                else if(cd.id==RemoveId)pending=()=>BeginEditor(true);
                else return false;
                if(faith!=player.GetTotalCount("faith",true) || money!=player.money)throw new InvalidOperationException("Resource invariant changed");
                Log("ACTION id="+cd.id+" faith_unchanged=True money_unchanged=True");return true;
            }
            catch(Exception ex){Fail(ex);return false;}
        }
        public void OnRightClick(){if(ownMenu && menu!=null)menu.OnClosePressed();}
        void Paint(Action action){painting=true;try{action();}finally{painting=false;}}
        void BeginEditor(bool remove)
        {
            if(!Current() || !ownMenu || source==null)return;
            menu.Hide(true);removing=remove;editor=true;cursorSet=false;editorStartFrame=Time.frameCount;
            var game=MainGame.me;game.build_mode_logics.SetCurrentBuildZone("graveyard","");
            if(game.build_mode_logics.cur_build_zone==null)throw new InvalidOperationException("Missing native cemetery build zone");
            game.EnterScriptBuilding();game.player.components.character.control_enabled=false;
            Paint(()=>BuildGrid.ShowBuildGrid(true,true,""));
            game.gui_elements.hud.Hide();game.gui_elements.build_mode_gui.Open();
            var cameraTarget=cameraField.GetValue(game.build_mode_logics) as Transform;
            gamepadCursor=cameraTarget!=null?(Vector2)cameraTarget.position:(Vector2)game.player.transform.position;
            status=remove?"Выберите точку для удаления. Вера и оплаченная вместимость сохраняются.":"Выберите клетку для света. Это только маркер без эффекта.";
            Log("EDITOR mode="+(remove?"remove":"place")+" persistent=False");
        }
        bool Allowed(Vector2 pos)
        {
            foreach(var collider in Physics2D.OverlapBoxAll(pos,BuildGrid.GRID_CHECK_BOX_SIZE,0))
            {var z=collider.GetComponent<WorldZone>();if(z!=null && z.id=="graveyard" && z.IsAvailableForBuild())return true;}
            return false;
        }
        void EditorUpdate()
        {
            if(!Current()){EndEditor(false);return;}
            if(Time.frameCount<=editorStartFrame)return;
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || LazyInput.GetKeyDown(GameKey.Back)){EndEditor(true);return;}
            bool pad=LazyInput.gamepad_active;
            var b=MainGame.me.build_mode_logics;var target=cameraField.GetValue(b) as Transform;
            Vector2 aim;
            if(pad)
            {
                if(Time.unscaledTime>=gamepadRepeat)
                {
                    foreach(var direction in b.gamepad_directions)
                        if(LazyInput.GetKeyDown(direction.Key)){gamepadCursor+=new Vector2(Math.Sign(direction.Value.x),Math.Sign(direction.Value.y))*32f;gamepadRepeat=Time.unscaledTime+.12f;}
                }
                aim=gamepadCursor;
            }
            else aim=MainGame.me.world_cam.ScreenToWorldPoint(Input.mousePosition);
            int x=Mathf.RoundToInt(aim.x/32f),y=Mathf.RoundToInt(aim.y/32f);
            if(!cursorSet || cursorX!=x || cursorY!=y){cursorX=x;cursorY=y;cursorSet=true;validCell=Allowed(new Vector2(x*32f,y*32f));}
            if(target!=null)
            {
                Vector3 position=target.position;
                if(pad)position=new Vector3(aim.x,aim.y,position.z);
                else
                {
                    float speed=360f*Time.unscaledDeltaTime;
                    if(Input.mousePosition.x<18)position.x-=speed;
                    if(Input.mousePosition.x>Screen.width-18)position.x+=speed;
                    if(Input.mousePosition.y<18)position.y-=speed;
                    if(Input.mousePosition.y>Screen.height-18)position.y+=speed;
                }
                target.position=(Vector3)fitCamera.Invoke(b,new object[]{position});
                Paint(()=>BuildGrid.me.MoveBuildGridTo(target.position));
            }
            if(!removing && state.Free(support)==0)status="Свободных мест нет. Уберите точку или выйдите и увеличьте вместимость.";
            bool click=pad?LazyInput.GetKeyDown(GameKey.Interaction):Input.GetMouseButtonDown(0);
            if(click && !(showPanel && panel.Contains(new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y))))
            {
                if(removing)
                {
                    if(state.Remove(x,y)){status="Точка убрана. Вместимость сохранена.";Log("REMOVE x="+x+" y="+y+" purchased="+state.Purchased);}
                }
                else if(validCell && state.Place(x,y,support)){status="Точка поставлена. Порядок: последняя.";Log("PLACE x="+x+" y="+y+" placed="+state.Points.Count+" active="+state.Active(support));}
            }
            MainGame.me.gui_elements.build_mode_gui.RedrawScriptMode(false);
        }
        void EndEditor(bool reopen)
        {
            if(!editor)return;editor=false;painting=false;cursorSet=false;
            if(MainGame.me!=null)
            {
                var game=MainGame.me;var zone=game.build_mode_logics.cur_build_zone;game.gui_elements.build_mode_gui.Hide(true);
                modeField.SetValue(game.build_mode_logics,Enum.ToObject(modeField.FieldType,0));
                game.build_mode_logics.SetCurrentBuildZone("","");BuildGrid.ShowBuildGrid(false,true,"");game.ExitBuildMode();game.gui_elements.hud.Open();
                if(zone!=null)zone.RedrawQualities(null,false);
            }
            Log("EDITOR_END grid_hidden=True control_restored=True");
            if(reopen && Current() && source!=null)pending=()=>Open(source,false);
        }
        void Update()
        {
            if(!ready)return;
            try
            {
                if(Input.GetKeyDown(KeyCode.F8))showPanel=!showPanel;
                if(recoveryPending)
                {
                    if(MainGame.me!=null && MainGame.game_started && MainGame.me.save!=null && MainGame.me.player!=null && ReferenceEquals(saveInventory.GetValue(MainGame.me.save),MainGame.me.player.data))StartSession();
                    else if(Time.realtimeSinceStartup>recoveryUntil)throw new InvalidOperationException("Current-player session recovery timeout");
                }
                if(!Current())return;
                if(pending!=null){var action=pending;pending=null;action();}
                if(Time.realtimeSinceStartup>=nextRefresh){nextRefresh=Time.realtimeSinceStartup+.25f;RefreshSupport();}
                if(ownMenu && refreshRequested)RefreshMenu();
                if(editor)EditorUpdate();
            }
            catch(Exception ex){Fail(ex);}
        }
        void OnGUI()
        {
            if(!ready || !Current())return;
            try
            {
                if(label==null)label=new GUIStyle(GUI.skin.label){wordWrap=true};
                if(!ownMenu)
                {
                    for(int i=0;i<state.Points.Count;i++){var pt=state.Points[i];DrawCell(pt.X,pt.Y,i<state.Active(support)?new Color(.2f,1f,.7f,.65f):new Color(.65f,.65f,.65f,.6f),false);}
                }
                if(editor)
                {
                    if(cursorSet)DrawCell(cursorX,cursorY,(removing?state.Index(cursorX,cursorY)>=0:validCell && state.Free(support)>0)?Color.yellow:Color.red,true);
                    GUI.Box(new Rect(370,10,Math.Max(260,Screen.width-390),72),(removing?"Убрать свет":"При размещении на могиле превращает все красные черепа в белые.")+"\nПрототип: качество не меняется. "+Counts().Replace("\n"," · ")+"\nЛКМ / действие — выбрать; ПКМ / назад — к меню. Мышь у края — камера.");
                }
                if(showPanel)
                {
                    GUI.Box(panel,"BPS Menu Probe "+Version+" — только текущая сессия");
                    GUI.Label(new Rect(panel.x+10,panel.y+24,panel.width-20,65),Counts()+"\n"+status,label);
                    if(GUI.Button(new Rect(panel.x+10,panel.y+100,panel.width-20,28),"Очистить временные точки и покупки"))pending=()=>{EndEditor(false);if(ownMenu && menu!=null)menu.Hide(true);state.Clear();status="Тест очищен. Можно снова открыть молельню.";Log("CLEANUP local_state=True");};
                    GUI.enabled=!editor && !ownMenu && !MainGame.me.gui_elements.IsAnyMassiveWindowOpened();
                    if(GUI.Button(new Rect(panel.x+10,panel.y+134,panel.width-20,28),"Запасной вход: ближайшая молельня"))pending=OpenNearest;
                    GUI.enabled=true;
                }
            }
            catch(Exception ex){Fail(ex);}
        }
        void DrawCell(int x,int y,Color color,bool cursor)
        {
            if(Event.current.type!=EventType.Repaint)return;
            var cam=MainGame.me.world_cam;var a=cam.WorldToScreenPoint(new Vector3(x*32f-14,y*32f-14,0));var b=cam.WorldToScreenPoint(new Vector3(x*32f+14,y*32f+14,0));
            var r=Rect.MinMaxRect(Math.Min(a.x,b.x),Screen.height-Math.Max(a.y,b.y),Math.Max(a.x,b.x),Screen.height-Math.Min(a.y,b.y));
            var old=GUI.color;GUI.color=color;
            if(!cursor)GUI.DrawTexture(new Rect(r.center.x-4,r.center.y-4,8,8),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(r.x,r.y,r.width,2),Texture2D.whiteTexture);GUI.DrawTexture(new Rect(r.x,r.yMax-2,r.width,2),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(r.x,r.y,2,r.height),Texture2D.whiteTexture);GUI.DrawTexture(new Rect(r.xMax-2,r.y,2,r.height),Texture2D.whiteTexture);GUI.color=old;
        }
        void OpenNearest()
        {
            WorldGameObject best=null;float distance=float.PositiveInfinity;
            foreach(var w in stations)if(w!=null && !w.is_removed && Station(w.obj_id)){float d=(w.transform.position-MainGame.me.player.transform.position).sqrMagnitude;if(d<distance){distance=d;best=w;}}
            if(best==null){status="Нужна готовая молельня на кладбище.";return;}Open(best,true);
        }
        void ResetSession(string reason)
        {
            pending=null;recoveryPending=false;
            try{EndEditor(false);if(ownMenu && menu!=null)menu.Hide(true);}finally
            {
                if(menu!=null && ReferenceEquals(menu.custom_craft_interface,this))menu.custom_craft_interface=previousController;
                if(ReferenceEquals(BuildModeLogics.last_build_desk,source))BuildModeLogics.last_build_desk=previousDesk;
                foreach(var pair in definitions){pair.Key.interaction_type=pair.Value.Interaction;pair.Key.zone_id=pair.Value.Zone;}
                definitions.Clear();stations.Clear();state.Clear();ownMenu=false;session=null;source=null;support=0;lastSupport=-1;
            }
            if(ready)Log("RESET reason="+reason+" persistent=False");
        }
        void Fail(Exception ex)
        {
            ready=false;try{ResetSession("failure");}catch(Exception cleanup){Logger.LogError("BPS_MENU cleanup_failed "+cleanup);}
            Logger.LogError("BPS_MENU FAILED action=disabled-for-session "+ex);
        }
        void OnDestroy(){try{ResetSession("unload");}finally{if(harmony!=null)harmony.UnpatchSelf();if(ReferenceEquals(self,this))self=null;}}
    }
}
