// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: AssemblyVersion(NikichMods.BPSProbe.Plugin.Version + ".0")]
[assembly: AssemblyFileVersion(NikichMods.BPSProbe.Plugin.Version + ".0")]

namespace NikichMods.BPSProbe
{
    [BepInPlugin(Id, "Better Prayer Stations - Appearance Probe", Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "nikichmods.bps.appearanceprobe";
        public const string Version = "0.1.2";
        const string Marker = "nikichmods.bps.appearance_probe.selected";
        const string Look = "nikichmods.bps.appearance_probe.look";
        const string FenceLook = "nikichmods.bps.appearance_probe.fence";
        const string HostHash = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        static readonly string[] Looks = { "", "grave_top_wd_cross_1", "grave_top_stn_cross_1" };
        static readonly string[] Fences = { "", "grave_bot_stn_1", "grave_bot_stn_2" };
        static Plugin self;
        Harmony harmony;
        FieldInfo savedInventory, openedGrave;
        GameSave session;
        readonly WorldGameObject[] graves = new WorldGameObject[2];
        int selected, checkpoint;
        WorldGameObject opened;
        WorldGameObject target { get { return graves[selected]; } }
        bool active, panel = true, resourcesChecked;
        Action pending;
        Rect window = new Rect(12, 80, 350, 510);
        GUIStyle label, button;
        Vector2 scroll;
        string status = "Откройте первую занятую могилу и запомните её как А.";

        void Awake()
        {
            self = this;
            try
            {
                string hash;
                using (var stream = File.OpenRead(typeof(WorldGameObject).Assembly.Location))
                using (var sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                if (hash != HostHash) throw new InvalidOperationException("Unverified host assembly: " + hash);
                savedInventory = AccessTools.Field(typeof(GameSave), "_inventory");
                openedGrave = AccessTools.Field(typeof(GraveGUI), "_grave_wgo");
                if (savedInventory == null || savedInventory.FieldType != typeof(Item) || openedGrave == null || openedGrave.FieldType != typeof(WorldGameObject))
                    throw new MissingFieldException("Save/grave ownership fields");
                harmony = new Harmony(Id);
                var targets = ResolveTargets();
                Patch(targets[0], null, "Project");
                Patch(targets[1], null, "Opened");
                Patch(targets[2], "Loading", null);
                Patch(targets[3], null, "Started");
                active = true;
                Log("READY version=" + Version + " host=" + hash + " scope=two-graves-two-slots save_keys_per_grave=3 native_item_actions=manual");
            }
            catch (Exception ex)
            {
                active = false;
                if (harmony != null) harmony.UnpatchSelf();
                status = "Не удалось подключить пробник. Причина записана в журнал.";
                Logger.LogError("BPS_PROBE INIT_FAILED action=disabled " + ex);
            }
        }

        internal static MethodInfo DeclaredTarget(Type type, string name, Type returns, Type[] arguments)
        {
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, arguments, null);
            if (method == null || method.DeclaringType != type || method.ReturnType != returns || method.IsAbstract || method.GetMethodBody() == null)
                throw new MissingMethodException(type.FullName, name + " (exact declared signature)");
            return method;
        }

        internal static MethodInfo[] ResolveTargets()
        {
            return new[]
            {
                DeclaredTarget(typeof(CustomDrawers), "GetNewWOPPrefabsNames", typeof(List<WOPPrefabName>), Type.EmptyTypes),
                DeclaredTarget(typeof(GraveGUI), "Open", typeof(void), new[] { typeof(WorldGameObject) }),
                DeclaredTarget(typeof(MainGame), "OnGameLoaded", typeof(void), new[] { typeof(GameSave) }),
                DeclaredTarget(typeof(MainGame), "OnGameStartedPlaying", typeof(void), Type.EmptyTypes)
            };
        }

        void Patch(MethodInfo method, string prefix, string postfix)
        {
            harmony.Patch(method,
                prefix == null ? null : new HarmonyMethod(typeof(Plugin), prefix),
                postfix == null ? null : new HarmonyMethod(typeof(Plugin), postfix));
        }

        bool SessionReady()
        {
            return savedInventory != null && MainGame.me != null && MainGame.game_started && MainGame.me.save != null && MainGame.me.player != null &&
                MainGame.me.player.data != null && ReferenceEquals(savedInventory.GetValue(MainGame.me.save), MainGame.me.player.data);
        }
        bool Current() { return SessionReady() && ReferenceEquals(session, MainGame.me.save); }
        bool Idle(WorldGameObject grave)
        {
            return Current() && grave != null && !grave.is_removed && grave.obj_id == "grave_ground" && grave.data != null &&
                !grave.components.craft.is_crafting && grave.GetBodyFromInventory(true) != null;
        }
        bool Known(WorldGameObject grave)
        {
            return Current() && grave != null && grave.data != null && !grave.is_removed &&
                (grave == graves[0] || grave == graves[1]) && grave.data.GetParam(Marker, 0f) > 0f;
        }
        void CheckResources()
        {
            if (resourcesChecked) return;
            for (int slot = 0; slot < 2; slot++)
            for (int code = 1; code <= 2; code++)
            {
                string id = LookId(slot == 1, code);
                var definition = GameBalance.me.GetData<ItemDefinition>(id);
                var kind = slot == 0 ? ItemDefinition.ItemType.GraveStone : ItemDefinition.ItemType.GraveFence;
                if (definition == null || definition.type != kind || SmartResourceHelper.GetResource<WorldObjectPart>("objects/grave parts/" + id + "_stg_1") == null)
                    throw new InvalidOperationException("Missing selected native look: " + id);
            }
            resourcesChecked = true;
        }
        internal static string LookId(bool fence, int code)
        {
            if (code < 0 || code > 2) throw new InvalidOperationException("Unsupported probe appearance code");
            return (fence ? Fences : Looks)[code];
        }
        int Committed(WorldGameObject grave, bool fence)
        {
            int code = ReadCode(grave.data.GetParam(fence ? FenceLook : Look, 0f));
            return code;
        }
        internal static int ReadCode(float value)
        {
            if (value != 0f && value != 1f && value != 2f) throw new InvalidOperationException("Unsupported probe appearance code");
            return (int)value;
        }
        void Recover()
        {
            if (!active || !SessionReady()) return;
            session = MainGame.me.save;
            graves[0] = null; graves[1] = null;
            foreach (var grave in WorldMap.objs)
            {
                if (grave == null || grave.data == null) continue;
                float marker = grave.data.GetParam(Marker, 0f);
                if (marker == 0f) continue;
                int index = ReadCode(marker) - 1;
                if (index < 0 || graves[index] != null) throw new InvalidOperationException("Invalid/duplicate probe grave slot");
                graves[index] = grave;
                Committed(grave, false); Committed(grave, true);
            }
            CheckResources();
            Log("LOAD_RECOVERY A=" + (graves[0] == null ? "none" : graves[0].unique_id.ToString()) + " B=" + (graves[1] == null ? "none" : graves[1].unique_id.ToString()));
            Queue(delegate { Visit(0, "load", false); });
            status = "Сохранённые тестовые могилы восстановлены. Можно продолжать или очистить обе.";
        }
        void Visit(int index, string reason, bool cleanup)
        {
            if (!Current()) return;
            var grave = graves[index];
            if (grave != null && grave.data != null)
            {
                if (cleanup)
                {
                    grave.data.SetParam(Marker, 0f); grave.data.SetParam(Look, 0f); grave.data.SetParam(FenceLook, 0f);
                }
                Log(reason.ToUpperInvariant() + " slot=" + (index == 0 ? "A" : "B") + " target=" + grave.unique_id + " top=" + Committed(grave, false) + " fence=" + Committed(grave, true));
                if (Idle(grave)) Refresh(grave, reason);
                else Log("REFRESH_SKIPPED reason=" + reason + " target=" + grave.unique_id + " idle=false");
                if (cleanup) graves[index] = null;
            }
            if (index == 0) Queue(delegate { Visit(1, reason, cleanup); });
            else if (cleanup) { opened = null; status = "Обе могилы очищены. Предметы остаются на своих местах. Сохраните игру."; Log("CLEANUP_DONE active_keys=0 save_required=true"); }
        }
        void Select(int index)
        {
            if (!Current() && SessionReady()) { Recover(); status = "Дождитесь восстановления и повторите выбор."; return; }
            if (!Idle(opened)) { status = "Откройте занятую закопанную могилу без текущей работы. Украшения могут отсутствовать."; return; }
            if (opened == graves[1-index]) { status = "А и Б должны быть разными могилами."; return; }
            if (graves[index] != null && graves[index] != opened) { status = "Эта буква уже занята. Сначала очистите обе могилы."; return; }
            CheckResources();
            selected = index; graves[index] = opened;
            target.data.SetParam(Marker, index + 1f);
            Log("SELECT slot=" + (index == 0 ? "A" : "B") + " target=" + target.unique_id + " state=" + Snapshot(target));
            status = "Выбрана могила " + (index == 0 ? "А" : "Б") + ". Задайте отдельно надгробие и оградку.";
        }
        void Commit(bool fence, int code)
        {
            if (!Known(target) || !Idle(target)) { status = "Выберите доступную могилу без текущей работы."; return; }
            CheckResources();
            string key = fence ? FenceLook : Look;
            float old = target.data.GetParam(key, 0f);
            target.data.SetParam(key, code);
            try { Refresh(target, fence ? "commit-fence" : "commit-top"); }
            catch { target.data.SetParam(key, old); throw; }
            status = "Вид сохранён у могилы " + (selected == 0 ? "А" : "Б") + ". Пустой слот остаётся пустым. Снятие и установка — обычными кнопками игры.";
        }
        string Snapshot(WorldGameObject grave)
        {
            var b = new StringBuilder();
            b.Append("quality:").Append(grave.quality.ToString("R", CultureInfo.InvariantCulture));
            foreach (var item in grave.data.inventory) AppendItem(b, item, true);
            b.Append("|money:").Append(MainGame.me.player.data.money.ToString("R", CultureInfo.InvariantCulture));
            b.Append("|faith:").Append(MainGame.me.player.data.GetItemsCount("faith", true));
            return b.ToString();
        }
        void AppendItem(StringBuilder b, Item item, bool children)
        {
            b.Append('|').Append(item.id).Append(':').Append(item.value).Append(':').Append(item.durability.ToString("R", CultureInfo.InvariantCulture));
            b.Append(":probe=").Append(item.GetParam(Marker,0f)).Append(',').Append(item.GetParam(Look,0f)).Append(',').Append(item.GetParam(FenceLook,0f));
            if (children) foreach (var child in item.inventory) AppendItem(b, child, false);
        }
        string Parts(WorldGameObject grave)
        {
            var actual = new List<string>();
            foreach (var part in grave.additional_wops) if (part != null) actual.Add(part.gameObject.name.Replace(" ", "_"));
            return string.Join(",", actual.ToArray());
        }
        void Capture()
        {
            if (!Current()) return;
            checkpoint++;
            for (int i=0;i<2;i++)
            {
                var grave=graves[i];
                if (grave == null || grave.data == null) continue;
                Log("CHECKPOINT n="+checkpoint+" slot="+(i==0?"A":"B")+" target="+grave.unique_id+" top="+Committed(grave,false)+" fence="+Committed(grave,true)+" crafting="+grave.components.craft.is_crafting+" actual_parts="+Parts(grave)+" state="+Snapshot(grave));
            }
            var player = new StringBuilder();
            foreach (var item in MainGame.me.player.data.inventory)
                if (item.definition != null && (item.definition.type == ItemDefinition.ItemType.GraveStone || item.definition.type == ItemDefinition.ItemType.GraveFence)) AppendItem(player,item,false);
            Log("CHECKPOINT_PLAYER n="+checkpoint+" decorations="+player);
            status="Снимок этапа №"+checkpoint+" записан. Снимайте, устанавливайте и переносите предметы обычным способом; после завершения работ сделайте следующий снимок.";
        }
        void Refresh(WorldGameObject grave, string reason)
        {
            if (!Current() || grave == null) throw new InvalidOperationException("Stale refresh target");
            string before = Snapshot(grave);
            Log("REDRAW_BEGIN reason=" + reason + " target=" + grave.unique_id + " top=" + Committed(grave,false) + " fence=" + Committed(grave,true) + " state=" + before);
            grave.Redraw(false, false, false);
            string after = Snapshot(grave);
            bool intact = before == after;
            Log("REDRAW_END reason=" + reason + " invariants=" + intact + " actual_parts=" + Parts(grave) + " state=" + after);
            if (!intact) throw new InvalidOperationException("Native item/body/quality/currency invariant changed");
        }
        void Queue(Action action) { if (pending == null) pending = action; }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8)) panel = !panel;
            var action = pending; pending = null;
            if (action == null) return;
            try { action(); } catch (Exception ex) { Contain(ex); }
        }
        void Contain(Exception ex)
        {
            bool wasActive = active;
            active = false; pending = null;
            status = "Подмена отключена после ошибки. Очистите обе могилы; см. журнал.";
            Logger.LogError("BPS_PROBE FAILED action=projection-disabled " + ex);
            // Native appearances are restored one grave per frame, without clearing persistent choices.
            if (wasActive && Current()) Queue(delegate { Visit(0, "fallback", false); });
        }
        void Log(string message) { Logger.LogInfo("BPS_PROBE " + message); }
        void OnGUI()
        {
            if (!panel) return;
            if (label == null) { label = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true }; button = new GUIStyle(GUI.skin.button) { fontSize = 14, wordWrap = true }; }
            window.width = Math.Min(350, Math.Max(260, Screen.width - 24));
            window.height = Math.Min(560, Math.Max(260, Screen.height - 24));
            window.x = Mathf.Clamp(window.x, 0, Math.Max(0, Screen.width - window.width));
            window.y = Mathf.Clamp(window.y, 0, Math.Max(0, Screen.height - window.height));
            window = GUI.Window(827401, window, DrawPanel, "BPS: две могилы " + Version);
        }
        void DrawPanel(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("F8 — скрыть/показать. Виды сохраняются у могил, предметы не меняются.",label);
            GUILayout.Label("Выбрана: "+(selected==0?"А":"Б")+". "+status,label);
            bool previous=GUI.enabled;
            GUI.enabled=active && SessionReady() && pending==null;
            if(GUILayout.Button("Запомнить открытую могилу как А",button)) Queue(delegate{Select(0);});
            if(GUILayout.Button("Запомнить открытую могилу как Б",button)) Queue(delegate{Select(1);});
            GUI.enabled=active && Current() && target!=null && pending==null;
            GUILayout.Label("При открытии А или Б выбор переключается автоматически.",label);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Надгробие: дерево",button)) Queue(delegate{Commit(false,1);});
            if(GUILayout.Button("Надгробие: камень",button)) Queue(delegate{Commit(false,2);});
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Оградка: камень I",button)) Queue(delegate{Commit(true,1);});
            if(GUILayout.Button("Оградка: камень II",button)) Queue(delegate{Commit(true,2);});
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Записать состояние обеих могил и инвентаря",button)) Queue(Capture);
            GUI.enabled=Current() && (graves[0]!=null || graves[1]!=null) && pending==null;
            if(GUILayout.Button("Очистить обе могилы и вернуть исходный вид",button)) Queue(delegate{Visit(0,"cleanup",true);});
            GUI.enabled=previous;
            GUILayout.Label("Пробник не снимает и не перемещает предметы. После очистки сохраните игру. Плата за тестовые виды отсутствует.",label);
            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0,0,10000,22));
        }
        static void Opened(GraveGUI __instance, WorldGameObject __0)
        {
            var p=self;
            if(p==null || !p.active) return;
            try
            {
                if(!p.Current() && p.SessionReady()) p.Recover();
                if(ReferenceEquals(p.openedGrave.GetValue(__instance),__0))
                {
                    p.opened=__0;
                    for(int i=0;i<2;i++) if(p.graves[i]==__0) p.selected=i;
                }
            }
            catch(Exception ex){p.Contain(ex);}
        }
        static void Loading()
        {
            var p=self;
            if(p==null) return;
            p.opened=null; p.graves[0]=null; p.graves[1]=null; p.session=null; p.pending=null; p.resourcesChecked=false;
        }
        static void Started()
        {
            var p=self;
            if(p!=null && p.active) p.Queue(p.Recover);
        }
        static void Project(CustomDrawers __instance, ref List<WOPPrefabName> __result)
        {
            var p=self;
            if(p==null || !p.active || !p.Known(__instance.wobj) || __result==null) return;
            try
            {
                var grave=__instance.wobj;
                for(int slot=0;slot<2;slot++)
                {
                    int code=p.Committed(grave,slot==1);
                    if(code==0) continue;
                    var item=grave.data.GetItemOfType(slot==0 ? ItemDefinition.ItemType.GraveStone : ItemDefinition.ItemType.GraveFence,true);
                    if(item!=null) ProjectParts(__result,item.id,LookId(slot==1,code));
                }
            }
            catch(Exception ex)
            {
                p.active=false; p.pending=null;
                p.Logger.LogError("BPS_PROBE PROJECTION_FAILED action=disabled " + ex);
                p.status="Подмена отключена после ошибки. Очистите обе могилы.";
                if(p.Current()) p.Queue(delegate{p.Visit(0,"fallback",false);});
            }
        }
        internal static void ProjectParts(IList<WOPPrefabName> parts, string actualId, string lookId)
        {
            string original=actualId+"_stg_";
            for(int i=0;i<parts.Count;i++)
            {
                var part=parts[i];
                if(part==null || part.part_1!=original || part.stage<=0) continue;
                parts[i]=new WOPPrefabName{part_1=lookId+"_stg_",part_2=part.part_2,stage=part.stage,need_herb=part.need_herb};
                break;
            }
        }
        void OnDestroy()
        {
            active=false; pending=null;
            if(harmony!=null) harmony.UnpatchSelf();
            if(self==this) self=null;
        }
    }
}
