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
        public const string Id="nikichmods.bps.menuprobe",Version="0.5.0";
        const string Host="e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        const string BuyId="bps_menu_probe_buy",PlaceId="bps_menu_probe_place",RemoveId="bps_menu_probe_remove";
        static Plugin self;
        readonly BeamState state=new BeamState();
        readonly CapacityPurchase purchase=new CapacityPurchase();
        readonly StationMenuView presentation=new StationMenuView();
        bool setupFailed;
        readonly List<WorldGameObject> stations=new List<WorldGameObject>();
        readonly List<WorldGameObject> graves=new List<WorldGameObject>();
        readonly FieldGeometry fieldGeometry=new FieldGeometry();
        sealed class EffectState{internal Item Body;internal bool Active;internal int Matches;internal float Quality;}
        readonly Dictionary<WorldGameObject,EffectState> previousEffects=new Dictionary<WorldGameObject,EffectState>();
        readonly Dictionary<WorldGameObject,float> badges=new Dictionary<WorldGameObject,float>();
        FieldInfo zoneField;
        GraveGUI graveView;
        int observeFrame=-1;
        bool suppressProjection;
        bool auditRequested;
        readonly Dictionary<ObjectDefinition,DefinitionSnapshot> definitions=new Dictionary<ObjectDefinition,DefinitionSnapshot>();
        sealed class DefinitionSnapshot{internal ObjectDefinition.InteractionType Interaction;internal string Zone;}
        Harmony harmony;
        GameSave session;
        Item savedOwner;
        WorldGameObject source;
        WorldGameObject previousDesk;
        CraftInterface previousController;
        CraftGUI menu;
        ObjectCraftDefinition buyCard,placeCard,removeCard;
        FieldInfo saveInventory,modeField,cameraField,hideBuildField;
        MethodInfo fitCamera,openMenu,graveRedraw;
        bool ready,ownMenu,editor,removing,painting,showPanel,validCell,cursorSet,refreshRequested,recoveryPending;
        bool menuQueueWasActive;
        UIWidget headerTableWidget;
        UILabel.Overflow headerOverflow;
        int headerHeight,headerDescriptionHeight,headerTop,headerBottom;
        Vector3 gridOrigin;
        int support,cursorX,cursorY,lastSupport=-1,editorStartFrame;
        float nextRefresh,gamepadRepeat,recoveryUntil;
        Vector2 gamepadCursor;
        Action pending;
        string status="Загрузите игру. Все изменения только в памяти.";
        Rect panel=new Rect(12,70,345,292);
        GUIStyle label;
        static bool Station(string id){return id=="pray_stand_wd" || id=="pray_stand_stn";}
        static bool StationCandidate(string id){return Station(id) || id=="pray_stand_wd_place" || id=="pray_stand_stn_place";}
        static bool GraveCandidate(string id){return id=="grave_ground" || id=="grave_empty" || id=="grave_corp" || id=="grave_exhume";}
        bool Current()
        {return ready && MainGame.me!=null && MainGame.game_started && MainGame.me.save!=null && MainGame.me.player!=null && MainGame.me.player.data!=null && ReferenceEquals(session,MainGame.me.save) && ReferenceEquals(savedOwner,MainGame.me.player.data) && ReferenceEquals(saveInventory.GetValue(session),savedOwner);}
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
                graveRedraw=AccessTools.Method(typeof(GraveGUI),"Redraw",Type.EmptyTypes);
                hideBuildField=AccessTools.Field(typeof(CraftGUI),"hide_after_build_gui");
                zoneField=AccessTools.Field(typeof(WorldGameObject),"_zone");
                if(zoneField==null || zoneField.FieldType!=typeof(WorldZone))throw new MissingFieldException("Native grave zone");
                if(saveInventory==null || modeField==null || cameraField==null || fitCamera==null || openMenu==null || hideBuildField==null)throw new MissingMemberException("Native session/editor members");
                presentation.Load();
                harmony=new Harmony(Id);
                Patch(typeof(MainGame),"OpenBuildObjectGUI",nameof(OpenPrefix),null,new[]{typeof(WorldGameObject)});
                Patch(typeof(MainGame),"OnGameLoaded",nameof(Loading),null,new[]{typeof(GameSave)});
                Patch(typeof(MainGame),"OnGameStartedPlaying",null,nameof(Started),Type.EmptyTypes);
                Patch(typeof(WorldMap),"OnAddNewWGO",null,nameof(Added),new[]{typeof(WorldGameObject)});
                Patch(typeof(BuildModeLogics),"Update",nameof(BuildUpdate),null,Type.EmptyTypes);
                Patch(typeof(BuildGrid),"IsCellBusy",nameof(CellBusy),null,new[]{typeof(Vector2),typeof(List<Collider2D>)});
                Patch(typeof(CraftGUI),"AddRemoveBuildingItem",nameof(NativeRemoveCard),null,Type.EmptyTypes);
                Patch(typeof(GJL),"L",nameof(Localize),null,new[]{typeof(string)});
                Patch(typeof(CraftItemGUI),"Redraw",nameof(RowBefore),nameof(RowAfter),Type.EmptyTypes);
                SkullProjection.Install(harmony);
                Patch(typeof(GraveGUI),"Redraw",null,nameof(GraveRedrawn),Type.EmptyTypes);
                ready=true;Log("READY version="+Version+" persistent=True faith_debit=True quality_effect=True test_resources=True native_presentation=True icons=100,37,37");
            }
            catch(Exception ex){Fail(ex);}
        }
        void Patch(Type t,string name,string prefix,string postfix,Type[] args)
        {var m=AccessTools.Method(t,name,args);if(m==null)throw new MissingMethodException(t.Name,name);harmony.Patch(m,prefix==null?null:new HarmonyMethod(typeof(Plugin),prefix),postfix==null?null:new HarmonyMethod(typeof(Plugin),postfix));}
        static bool Localize(string __0,ref string __result)
        {
            var p=self;if(p==null || !p.ready)return true;
            if(__0==BuyId)__result="Помолиться";
            else if(__0==PlaceId)__result="Благословить место";
            else if(__0==RemoveId)__result="Снять благословение";
            else if(__0==BuyId+"_d")__result=p.PrayerDescription();
            else if(__0==PlaceId+"_d")__result="На могиле превращает все красные черепа в белые.";
            else if(__0==RemoveId+"_d")__result="Снятое благословение можно использовать в другом месте.";
            else return true;
            return false;
        }
        string PrayerDescription()
        {
            if(state.NextPrice==0)return "Пробный ряд цен закончился.";
            if(state.Purchased>=support)return "Постройте или улучшите молельню.";
            int missing=state.NextPrice-CapacityPurchase.Count(savedOwner);
            return missing>0?"Не хватает "+missing+" веры.":"Дарует ещё одно благословение.";
        }
        void UpdateQuote()
        {
            if(buyCard==null)return;
            if(buyCard.needs.Count==0)buyCard.needs.Add(new Item("faith",state.NextPrice));
            else buyCard.needs[0].value=state.NextPrice;
        }
        int OwnedRow(CraftItemGUI row)
        {
            if(!Current() || !ownMenu || menu==null || !ReferenceEquals(menu.custom_craft_interface,this) || row==null)return -1;
            var craft=row.craft_definition;
            return ReferenceEquals(craft,buyCard)?0:ReferenceEquals(craft,placeCard)?1:ReferenceEquals(craft,removeCard)?2:-1;
        }
        static void RowBefore(CraftItemGUI __instance)
        {
            var p=self;if(p==null || p.OwnedRow(__instance)<0)return;
            try{p.UpdateQuote();p.presentation.BeforeRedraw(__instance);}catch(Exception ex){p.Fail(ex);}
        }
        static void RowAfter(CraftItemGUI __instance)
        {
            var p=self;if(p==null)return;int icon=p.OwnedRow(__instance);if(icon<0)return;
            try{p.presentation.AfterRedraw(__instance,icon,p.state.NextPrice,CapacityPurchase.Count(p.savedOwner));}
            catch(Exception ex){p.Fail(ex);}
        }
        static bool OpenPrefix(WorldGameObject __0)
        {
            var p=self;if(p==null || !p.Current() || __0==null || !Station(__0.obj_id))return true;
            try{p.Open(__0,false);}catch(Exception ex){p.Fail(ex);}return false;
        }
        static void Loading(){if(self!=null)self.ResetSession("load");}
        static void Started(){if(self!=null && self.ready){self.recoveryPending=true;self.recoveryUntil=Time.realtimeSinceStartup+30f;}}
        static void Added(WorldGameObject __0)
        {var p=self;if(p==null || !p.Current() || __0==null)return;if(GraveCandidate(__0.obj_id) && !p.graves.Contains(__0))p.graves.Add(__0);if(!StationCandidate(__0.obj_id))return;if(!p.stations.Contains(__0))p.stations.Add(__0);p.refreshRequested=true;}
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
            savedOwner=MainGame.me.player.data;
            var restored=SavedBeamState.Read(savedOwner);state.Restore(restored.Purchased,restored.Points);
            foreach(var w in WorldMap.objs)if(w!=null){if(StationCandidate(w.obj_id))stations.Add(w);if(GraveCandidate(w.obj_id))graves.Add(w);}
            foreach(string id in new[]{"pray_stand_wd","pray_stand_stn"})
            {
                var d=GameBalance.me.GetData<ObjectDefinition>(id);
                definitions.Add(d,new DefinitionSnapshot{Interaction=d.interaction_type,Zone=d.zone_id});
                d.interaction_type=ObjectDefinition.InteractionType.Builder;d.zone_id="graveyard";
            }
            RefreshSupport();status="Подойдите к молельне и взаимодействуйте. F8 — панель теста.";
            Log("SESSION station_candidates="+stations.Count+" support="+support+" recovered=True purchased="+state.Purchased+" placed="+state.Points.Count+" active="+state.Active(support)+" faith="+CapacityPurchase.Count(savedOwner)+" next_price="+state.NextPrice+" order="+PointOrder());
        }
        string PointOrder()
        {var cells=new List<string>();for(int i=0;i<state.Points.Count;i++){var p=state.Points[i];cells.Add((i+1)+":"+p.X+","+p.Y);}return string.Join(";",cells.ToArray());}
        void SaveState(string reason)
        {
            if(!Current())throw new InvalidOperationException("Saved beam owner is no longer the current player/save");
            SavedBeamState.Write(savedOwner,state);
            Log("PERSIST reason="+reason+" purchased="+state.Purchased+" placed="+state.Points.Count+" order="+PointOrder()+" disk=native-next-save");
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
            if(support!=lastSupport){auditRequested=true;lastSupport=support;Log("SUPPORT capacity="+support+" purchased="+state.Purchased+" placed="+state.Points.Count+" active="+state.Active(support));if(ownMenu)refreshRequested=true;}
        }
        static ObjectCraftDefinition Card(string id,string icon)
        {return new ObjectCraftDefinition{id=id,custom_name=id,icon=icon,build_type=ObjectCraftDefinition.BuildType.None,craft_type=CraftDefinition.CraftType.None,needs_unlock=false,disable_multi_craft=true,force_multi_craft=false};}
        void Open(WorldGameObject station,bool shortcut)
        {
            if(!Current() || station==null || station.is_removed || station.is_removing || station.components.craft.is_crafting || !Station(station.obj_id) || editor || FloatingWorldGameObject.IsFloating())return;
            if(MainGame.me.gui_elements.IsAnyMassiveWindowOpened())return;
            RefreshSupport();source=station;menu=MainGame.me.gui_elements.craft;
            var inventory=new CraftsInventory();
            buyCard=Card(BuyId,"icon_faith");placeCard=Card(PlaceId,"icon_faith");removeCard=Card(RemoveId,"icon_faith");
            UpdateQuote();
            inventory.additional_crafts=new List<ObjectCraftDefinition>{buyCard,placeCard,removeCard};
            previousController=menu.custom_craft_interface;previousDesk=BuildModeLogics.last_build_desk;
            menu.custom_craft_interface=this;menu.has_amount_buttons=false;hideBuildField.SetValue(menu,true);
            BuildModeLogics.last_build_desk=station;ownMenu=true;MainGame.paused=true;
            openMenu.Invoke(menu,new object[]{station,inventory,null});
            menu.ON_CRAFT_GUI_HIDE=MenuHidden;
            if(menu.queue!=null){menuQueueWasActive=menu.queue.gameObject.activeSelf;menu.queue.gameObject.SetActive(false);}
            CaptureHeader();
            RefreshMenu();Log("OPEN station="+station.unique_id+" shortcut="+shortcut+" cards="+menu.list_items.Count);
            if(menu.list_items.Count!=3)throw new InvalidOperationException("Expected exactly three native cards");
        }
        void MenuHidden()
        {
            presentation.Restore();
            ownMenu=false;
            if(menu!=null)
            {
                RestoreHeader();
                if(ReferenceEquals(menu.custom_craft_interface,this))menu.custom_craft_interface=previousController;
                if(menu.queue!=null)menu.queue.gameObject.SetActive(menuQueueWasActive);
            }
            if(ReferenceEquals(BuildModeLogics.last_build_desk,source))BuildModeLogics.last_build_desk=previousDesk;
            Log("MENU_CLOSED controller_restored=True");
        }
        string Counts()
        {
            int active=state.Active(support),dark=state.Points.Count-active;
            return "Даровано благословений: "+state.Purchased+"\nМолельни позволяют использовать до "+support+" благословений одновременно.\nДействуют: "+active+" · Доступно: "+state.Free(support)+(dark>0?" · Угасло: "+dark:"");
        }
        void CaptureHeader()
        {
            var info=menu.object_info;
            if(info==null || info.text_descr==null || info.table==null || menu.header_height_widget==null)throw new MissingMemberException("Native header layout");
            headerTableWidget=info.table.GetComponent<UIWidget>();
            if(headerTableWidget==null)throw new MissingMemberException("Native header table bounds");
            headerOverflow=info.text_descr.overflowMethod;headerDescriptionHeight=info.text_descr.height;
            headerHeight=menu.header_height_widget.height;
            headerTop=headerTableWidget.topAnchor.absolute;headerBottom=headerTableWidget.bottomAnchor.absolute;
            info.text_descr.overflowMethod=UILabel.Overflow.ResizeHeight;
        }
        void RestoreHeader()
        {
            if(headerTableWidget==null)return;
            var info=menu.object_info;
            info.text_descr.overflowMethod=headerOverflow;info.text_descr.height=headerDescriptionHeight;
            headerTableWidget.topAnchor.absolute=headerTop;headerTableWidget.bottomAnchor.absolute=headerBottom;
            menu.header_height_widget.height=headerHeight;headerTableWidget.UpdateAnchors();
            info.table.Reposition();headerTableWidget=null;
        }
        void RefreshMenu()
        {
            if(!ownMenu || menu==null || !menu.is_shown)return;
            UpdateQuote();
            menu.UpdateAllAnchors();
            if(menu.object_info!=null)
            {
                menu.object_info.text_header.text="Благословения";
                menu.object_info.text_descr.gameObject.SetActive(true);
                menu.object_info.text_descr.text=Counts();
                var description=menu.object_info.text_descr;
                description.UpdateAnchors();description.ProcessText(false,true);
                int extra=Math.Max(0,description.height-headerDescriptionHeight);
                // UITable is vertically centred: move its anchor range by half the growth to retain its top.
                headerTableWidget.topAnchor.absolute=headerTop-(extra+1)/2;
                headerTableWidget.bottomAnchor.absolute=headerBottom-(extra+1)/2;
                menu.header_height_widget.height=headerHeight+extra;
                headerTableWidget.UpdateAnchors();
                menu.object_info.table.Reposition();
            }
            menu.UpdateAllAnchors();
            foreach(var row in menu.list_items)row.Redraw();
            menu.craft_list_grid.Reposition();menu.craft_list_grid.repositionNow=true;
            menu.UpdateAllAnchors();
            Log("LAYOUT header_height="+menu.header_height_widget.height+" description_height="+menu.object_info.text_descr.height+" rows="+string.Join(",",menu.list_items.ConvertAll(row=>row.GetComponent<UIWidget>().height.ToString()).ToArray()));
            refreshRequested=false;
        }
        public bool CanCraft(CraftDefinition cd,List<string> multi,int amount,List<Item> needs)
        {
            if(!Current() || !ownMenu || editor || pending!=null || amount!=1)return false;
            if(cd.id==BuyId)return state.CanBuy(support) && CapacityPurchase.Count(savedOwner)>=state.NextPrice;
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
                if(cd.id==BuyId)
                {
                    var result=purchase.Buy(player,state,support,state.Purchased,state.NextPrice);
                    Log("PURCHASE result="+result.Status+" cost="+result.Cost+" faith_before="+result.Before+" faith_after="+result.After+" purchased="+result.Purchased+" money_unchanged="+(money==player.money)+" order="+PointOrder());
                    if(result.Status!="committed")return false;
                    status="Вместимость +1. Потрачено "+result.Cost+" веры.";refreshRequested=true;
                    return true;
                }
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
            gridOrigin=BuildGrid.me.transform.position;
            game.gui_elements.hud.Hide();game.gui_elements.build_mode_gui.Open();
            auditRequested=true;
            var cameraTarget=cameraField.GetValue(game.build_mode_logics) as Transform;
            gamepadCursor=cameraTarget!=null?(Vector2)cameraTarget.position:(Vector2)game.player.transform.position;
            status=remove?"Выберите точку для удаления. Вера и оплаченная вместимость сохраняются.":"Любая клетка могилы подходит. Активная точка конвертирует все красные черепа.";
            Log("EDITOR mode="+(remove?"remove":"place")+" persistent=True grid_origin="+gridOrigin);
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
                // The texture was sampled at gridOrigin. Only the camera target moves during editing.
            }
            if(!removing && state.Free(support)==0)status="Свободных мест нет. Уберите точку или выйдите и увеличьте вместимость.";
            bool click=pad?LazyInput.GetKeyDown(GameKey.Interaction):Input.GetMouseButtonDown(0);
            if(click && !(showPanel && panel.Contains(new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y))))
            {
                if(removing)
                {
                    if(state.Remove(x,y)){SaveState("remove");auditRequested=true;status="Точка убрана. Вместимость сохранена.";Log("REMOVE x="+x+" y="+y+" purchased="+state.Purchased);}
                }
                else if(validCell && state.Place(x,y,support)){SaveState("place");auditRequested=true;status="Точка поставлена. Порядок: последняя.";Log("PLACE x="+x+" y="+y+" placed="+state.Points.Count+" active="+state.Active(support));}
            }
            MainGame.me.gui_elements.build_mode_gui.RedrawScriptMode(false);
        }
        void EndEditor(bool reopen)
        {
            if(!editor)return;editor=false;painting=false;cursorSet=false;
            if(BuildGrid.me!=null)Log("GRID_END origin_delta="+(BuildGrid.me.transform.position-gridOrigin));
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
                if(Time.realtimeSinceStartup>=nextRefresh){nextRefresh=Time.realtimeSinceStartup+.25f;RefreshSupport();RefreshEffects();}
                if(ownMenu && refreshRequested)RefreshMenu();
                if(editor)EditorUpdate();
                if(observeFrame>=0 && Time.frameCount>=observeFrame){observeFrame=-1;ObserveGrave();}
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
                    GUI.Box(new Rect(370,10,Math.Max(260,Screen.width-390),72),(removing?"Убрать свет":"При размещении на могиле превращает все красные черепа в белые.")+"\nСохранение — обычным способом в игре. "+Counts().Replace("\n"," · ")+"\nЛКМ / действие — выбрать; ПКМ / назад — к меню. Мышь у края — камера.");
                }
                if(showPanel)
                {
                    GUI.Box(panel,"BPS Menu Probe "+Version+" — данные сохраняются");
                    GUI.Label(new Rect(panel.x+10,panel.y+24,panel.width-20,65),Counts()+"\n"+status,label);
                    if(GUI.Button(new Rect(panel.x+10,panel.y+100,panel.width-20,28),"Очистить точки и покупки"))pending=()=>{EndEditor(false);if(ownMenu && menu!=null)menu.Hide(true);state.Clear();SaveState("cleanup");RefreshEffects();RedrawOpenGrave();status="Тест очищен. Сохраните игру для закрепления очистки.";Log("CLEANUP saved_group=zeroed save_required=True");};
                    GUI.enabled=!editor && !ownMenu && !MainGame.me.gui_elements.IsAnyMassiveWindowOpened();
                    if(GUI.Button(new Rect(panel.x+10,panel.y+134,panel.width-20,28),"Запасной вход: ближайшая молельня"))pending=OpenNearest;
                    GUI.enabled=!editor && !ownMenu && pending==null && !setupFailed && !MainGame.me.gui_elements.IsAnyMassiveWindowOpened();
                    if(GUI.Button(new Rect(panel.x+10,panel.y+168,panel.width-20,28),"Материалы: деревянная молельня"))pending=()=>GrantResources(0);
                    if(GUI.Button(new Rect(panel.x+10,panel.y+202,panel.width-20,28),"Материалы: каменная молельня"))pending=()=>GrantResources(1);
                    if(GUI.Button(new Rect(panel.x+10,panel.y+236,panel.width-20,28),"Добавить +50 веры (тест)"))pending=()=>GrantResources(2);
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
        void GrantResources(int kind)
        {
            if(!Current() || setupFailed || editor || ownMenu || MainGame.me.gui_elements.IsAnyMassiveWindowOpened())return;
            try
            {
                bool granted=kind==2?TestResources.GrantFaith(savedOwner,Log):NikichMods.BPSFaithProbe.StationMaterials.Grant(savedOwner,kind==1,Log);
                status=granted?"Тестовые ресурсы добавлены. Они сохраняются вместе с игрой.":"Не хватает места в инвентаре. Освободите место.";
            }
            catch(Exception ex){setupFailed=true;status="Выдача ресурсов отключена после ошибки. Смотрите журнал.";Logger.LogError("BPS_MENU SETUP_FAILED setup_disabled=True "+ex);}
        }
        bool CemeteryGrave(WorldGameObject grave)
        {
            if(grave==null || grave.is_removed || grave.GetComponent<FloatingWorldGameObject>()!=null || !GraveCandidate(grave.obj_id))return false;
            var zone=zoneField.GetValue(grave) as WorldZone;return zone!=null && zone.id=="graveyard";
        }
        internal static void Project(Item body,WorldGameObject grave,ref int red,ref int white,ref int available)
        {
            var p=self;if(p==null || !p.Current() || p.suppressProjection)return;
            int r=red,w=white,a=available;
            try
            {
                if(!p.CemeteryGrave(grave) || grave.obj_id!="grave_ground" || !ReferenceEquals(body,grave.GetBodyFromInventory(true)) || p.fieldGeometry.Matches(grave,p.state,p.support)==0)return;
                SkullProjection.Convert(ref red,ref white,ref available,body.durability);
            }
            catch(Exception ex){red=r;white=w;available=a;p.Fail(ex);}
        }
        void RefreshBadge(WorldGameObject grave)
        {
            if(!grave.show_quality_hint){badges.Remove(grave);return;}
            float value=grave.quality,old;if(badges.TryGetValue(grave,out old) && value==old)return;
            var data=grave.GetQualityWidgetData();if(data==null)return;
            grave.SetBubbleWidgetData(data,data.widget_id);grave.bubble.Redraw();
            var view=grave.bubble.GetBubbleGUI();if(view!=null)view.RefreshAlign(grave);badges[grave]=value;
        }
        void RefreshEffects()
        {
            bool changed=false;
            for(int i=graves.Count-1;i>=0;i--)
            {
                var grave=graves[i];
                if(grave==null || grave.is_removed || !GraveCandidate(grave.obj_id)){if(!ReferenceEquals(grave,null)){fieldGeometry.Forget(grave);previousEffects.Remove(grave);badges.Remove(grave);}graves.RemoveAt(i);continue;}
                bool eligible=CemeteryGrave(grave);var body=eligible?grave.GetBodyFromInventory(true):null;
                int matches=eligible?fieldGeometry.Matches(grave,state,support):0;
                bool active=matches>0 && body!=null && grave.obj_id=="grave_ground";
                float quality=grave.quality;EffectState old;bool had=previousEffects.TryGetValue(grave,out old);
                bool transition=had?(old.Active!=active || old.Matches!=matches || !ReferenceEquals(old.Body,body) || (active && old.Quality!=quality)):matches>0;
                if(transition){RecordEffect(grave,body,matches,active,quality);changed=true;}
                if(!had){old=new EffectState();previousEffects.Add(grave,old);}
                old.Body=body;old.Active=active;old.Matches=matches;old.Quality=quality;RefreshBadge(grave);
            }
            if(auditRequested)
            {
                auditRequested=false;int checkedGraves=0,unavailableGraves=0;
                foreach(var grave in graves)if(CemeteryGrave(grave))
                {
                    if(state.Active(support)==0)continue;
                    int projected=fieldGeometry.Matches(grave,state,support),native;string unavailable;
                    if(!fieldGeometry.TryNativeMatches(grave,state,support,out native,out unavailable))
                    {unavailableGraves++;Log("MEMBERSHIP_UNAVAILABLE target="+grave.unique_id+" projected="+projected+" reason="+unavailable);continue;}
                    if(projected!=native)throw new InvalidOperationException("Cell membership mismatch target="+grave.unique_id+" projected="+projected+" native="+native);
                    checkedGraves++;
                }
                Log("MEMBERSHIP_CHECK graves="+checkedGraves+" unavailable="+unavailableGraves+" active_fields="+state.Active(support)+" native_physics_match="+(checkedGraves>0?"True":"not-tested"));
            }
            if(changed)
            {
                RedrawOpenGrave();var zone=WorldZone.GetZoneByID("graveyard",true);
                if(zone!=null)Log("ZONE actual="+zone.GetTotalQuality()+" source=native-members");
            }
        }
        void RecordEffect(WorldGameObject grave,Item body,int matches,bool active,float quality)
        {
            string before=PhysicalSnapshot.Read(grave.data);float native;bool prior=suppressProjection;
            try{suppressProjection=true;native=grave.quality;}finally{suppressProjection=prior;}
            int red=0,white=0,available=0;if(body!=null)body.GetBodySkulls(out red,out white,out available,false);
            int effectiveRed=red,effectiveWhite=white,effectiveAvailable=available;
            if(body!=null)Project(body,grave,ref effectiveRed,ref effectiveWhite,ref effectiveAvailable);
            bool intact=before==PhysicalSnapshot.Read(grave.data);
            Rect rect;string bounds=fieldGeometry.TryRect(grave,out rect)?rect.ToString():"unavailable";
            Log("FIELD_EFFECT target="+grave.unique_id+" active="+active+" matches="+matches+" native="+native+" effective="+quality+" raw_skulls="+red+","+white+","+available+" effective_skulls="+effectiveRed+","+effectiveWhite+","+effectiveAvailable+" physical_unchanged="+intact+" footprint="+bounds);
            if(!intact || (active && effectiveRed!=0))throw new InvalidOperationException("Field projection physical/conversion invariant");
        }
        static void GraveRedrawn(GraveGUI __instance)
        {var p=self;if(p==null || !p.Current())return;p.graveView=__instance;p.observeFrame=Time.frameCount+2;}
        void RedrawOpenGrave()
        {if(graveView!=null && graveView.is_shown)graveRedraw.Invoke(graveView,null);}
        static int Children(UITableOrGrid table,GameObject first,GameObject second)
        {int count=0;foreach(Transform t in table.transform)if(t.gameObject!=first && t.gameObject!=second && t.gameObject.activeSelf)count++;return count;}
        void ObserveGrave()
        {
            if(graveView==null || !graveView.is_shown)return;
            var grave=SkullProjection.Grave.GetValue(graveView) as WorldGameObject;if(!CemeteryGrave(grave))return;
            var body=grave.GetBodyFromInventory(true);if(body==null)return;
            string before=PhysicalSnapshot.Read(grave.data);int r,w,a;body.GetBodySkulls(out r,out w,out a,false);
            int er=r,ew=w,ea=a;Project(body,grave,ref er,ref ew,ref ea);
            bool rawPanel=graveView.body_panel.skull_bar.negative==r && graveView.body_panel.skull_bar.positive==w && graveView.body_panel.skull_bar.durability==body.durability;
            int slots=Children(graveView.tb_slots,graveView.prefab_slot_grey,graveView.prefab_slot_red);
            bool diagram=slots==er+ea && graveView.red_vertical_bar.gameObject.activeSelf==(er>0);
            bool total=graveView.txt_grave_total_q.text==grave.quality.ToString("0");bool intact=before==PhysicalSnapshot.Read(grave.data);
            Log("GRAVE_VIEW target="+grave.unique_id+" matches="+fieldGeometry.Matches(grave,state,support)+" effective_red="+er+" effective_white="+ew+" slots="+slots+" diagram="+diagram+" raw_panel="+rawPanel+" total="+total+" physical_unchanged="+intact);
            if(!rawPanel || !diagram || !total || !intact)throw new InvalidOperationException("Native grave diagram/body/result mismatch");
        }
        void ResetSession(string reason)
        {
            pending=null;recoveryPending=false;
            try{EndEditor(false);if(ownMenu && menu!=null)menu.Hide(true);}finally
            {
                presentation.Restore();
                if(menu!=null && ReferenceEquals(menu.custom_craft_interface,this))menu.custom_craft_interface=previousController;
                if(ReferenceEquals(BuildModeLogics.last_build_desk,source))BuildModeLogics.last_build_desk=previousDesk;
                foreach(var pair in definitions){pair.Key.interaction_type=pair.Value.Interaction;pair.Key.zone_id=pair.Value.Zone;}
                state.Clear();
                foreach(var grave in graves)if(grave!=null && !grave.is_removed)RefreshBadge(grave);
                RedrawOpenGrave();fieldGeometry.Clear();graves.Clear();previousEffects.Clear();badges.Clear();graveView=null;observeFrame=-1;auditRequested=false;
                definitions.Clear();stations.Clear();ownMenu=false;session=null;savedOwner=null;source=null;support=0;lastSupport=-1;
            }
            if(ready)Log("RESET reason="+reason+" runtime_only=True saved_group_untouched=True");
        }
        void Fail(Exception ex)
        {
            ready=false;try{ResetSession("failure");}catch(Exception cleanup){Logger.LogError("BPS_MENU cleanup_failed "+cleanup);}
            Logger.LogError("BPS_MENU FAILED action=disabled-for-session "+ex);
        }
        void OnDestroy(){try{ResetSession("unload");}finally{if(harmony!=null)harmony.UnpatchSelf();presentation.Dispose();if(ReferenceEquals(self,this))self=null;}}
    }
}
