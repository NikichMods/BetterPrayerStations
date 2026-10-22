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
        public const string Version = "0.1.1";
        const string Marker = "nikichmods.bps.appearance_probe.selected";
        const string Look = "nikichmods.bps.appearance_probe.look";
        const string HostHash = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        static readonly string[] Looks = { "", "grave_top_wd_cross_1", "grave_top_stn_cross_1" };
        static readonly string[] Names = { "Исходный вид", "Деревянный крест", "Каменный крест" };
        static Plugin self;
        Harmony harmony;
        FieldInfo savedInventory, openedGrave;
        GameSave session;
        WorldGameObject target, opened;
        int preview = -1;
        bool active, panel = true, resourcesChecked;
        Action pending;
        Rect window = new Rect(12, 80, 340, 470);
        GUIStyle label, button;
        string status = "Откройте обычную могилу с установленным надгробием.";

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
                Patch(targets[2], null, "Closed");
                Patch(targets[3], "Loading", null);
                Patch(targets[4], null, "Started");
                active = true;
                Log("READY version=" + Version + " host=" + hash + " scope=one-gravestone save_keys=2");
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
                DeclaredTarget(typeof(GraveGUI), "Hide", typeof(void), new[] { typeof(bool) }),
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
            return MainGame.me != null && MainGame.game_started && MainGame.me.save != null && MainGame.me.player != null &&
                MainGame.me.player.data != null && ReferenceEquals(savedInventory.GetValue(MainGame.me.save), MainGame.me.player.data);
        }

        bool Current() { return SessionReady() && ReferenceEquals(session, MainGame.me.save); }

        bool Eligible(WorldGameObject grave)
        {
            if (!Current() || grave == null || grave.is_removed || grave.obj_id != "grave_ground" || grave.data == null ||
                grave.components.craft.is_crafting || grave.GetBodyFromInventory(true) == null) return false;
            var item = grave.data.GetItemOfType(ItemDefinition.ItemType.GraveStone, true);
            return item != null && item.definition != null && grave.data.GetParam(item.id, 0f) >= 1f;
        }

        void CheckResources()
        {
            if (resourcesChecked) return;
            for (int i = 1; i < Looks.Length; i++)
            {
                var definition = GameBalance.me.GetData<ItemDefinition>(Looks[i]);
                if (definition == null || definition.type != ItemDefinition.ItemType.GraveStone ||
                    SmartResourceHelper.GetResource<WorldObjectPart>("objects/grave parts/" + Looks[i] + "_stg_1") == null)
                    throw new InvalidOperationException("Missing selected native look: " + Looks[i]);
            }
            resourcesChecked = true;
        }

        void Recover()
        {
            if (!active || !SessionReady()) return;
            session = MainGame.me.save;
            target = null;
            int found = 0;
            foreach (var grave in WorldMap.objs)
            {
                if (grave != null && grave.data != null && grave.data.GetParam(Marker, 0f) > 0f)
                { target = grave; found++; }
            }
            if (found > 1) throw new InvalidOperationException("Multiple probe-marked graves; refusing ambiguous target");
            if (target != null)
            {
                CheckResources();
                if (!Eligible(target)) { status = "Тестовая могила загружена, но сейчас недоступна. Откройте её для очистки."; Log("LOAD target=" + target.unique_id + " eligible=false"); return; }
                Log("LOAD target=" + target.unique_id + " committed=" + Committed());
                Refresh("load");
                status = "Выбор восстановлен после загрузки. Проверьте вид могилы.";
            }
            else { status = "Откройте могилу и выберите её для теста."; Log("LOAD target=none"); }
        }

        int Committed()
        {
            if (target == null || target.data == null) return 0;
            float value = target.data.GetParam(Look, 0f);
            if (value != 0f && value != 1f && value != 2f) throw new InvalidOperationException("Unsupported probe appearance code");
            return (int)value;
        }

        void Select()
        {
            if (!Current() && SessionReady()) Recover();
            if (!Eligible(opened)) { status = "Нужна открытая могила с телом и готовым надгробием, без текущей работы."; return; }
            if (target != null && target != opened) { status = "Сначала очистите предыдущую тестовую могилу."; return; }
            CheckResources();
            target = opened;
            target.data.SetParam(Marker, 1f);
            Log("SELECT target=" + target.unique_id + " state=" + Snapshot());
            status = "Могила выбрана. Сохранённые тестовые настройки можно очистить кнопкой ниже.";
        }

        void Commit(int code)
        {
            RequireTarget();
            CheckResources();
            preview = -1;
            float old = target.data.GetParam(Look, 0f);
            target.data.SetParam(Look, code);
            try { Refresh("commit"); }
            catch { target.data.SetParam(Look, old); throw; }
            status = "Сохранён тестовый вид: " + Names[code] + ". Для проверки загрузки сохраните игру обычным способом.";
        }

        void Fit()
        {
            RequireTarget();
            if (opened != target) { status = "Для примерки откройте окно выбранной могилы."; return; }
            CheckResources();
            preview = Committed() == 1 ? 2 : 1;
            Refresh("preview");
            status = "Примерка: " + Names[preview] + ". Закройте окно могилы обычной кнопкой: вернётся сохранённый вид.";
        }

        void RequireTarget()
        {
            if (!Eligible(target) || target.data.GetParam(Marker, 0f) <= 0f) throw new InvalidOperationException("Selected grave is not current/idle/complete");
        }

        void Cancel(string reason)
        {
            if (preview < 0) return;
            preview = -1;
            Log("CANCEL reason=" + reason + " persisted_look_unchanged=true");
            Queue(delegate { if (Eligible(target)) Refresh("cancel-" + reason); });
            status = "Примерка отменена: " + reason + ". Сохранённый выбор не изменён.";
        }

        void Cleanup()
        {
            if (!Current() || target == null || target.data == null) { status = "Нет текущей тестовой могилы."; return; }
            preview = -1;
            var grave = target;
            grave.data.SetParam(Marker, 0f);
            grave.data.SetParam(Look, 0f);
            if (grave.obj_id == "grave_ground" && !grave.components.craft.is_crafting) Refresh("cleanup");
            target = null;
            status = "Тест отключён, исходный вид возвращён. Сохраните игру, чтобы сохранить очистку.";
            Log("CLEANUP target=" + grave.unique_id + " active_keys=0 inert_zero_keys=2");
        }

        string Snapshot()
        {
            var b = new StringBuilder();
            b.Append("quality:").Append(target.quality.ToString("R", CultureInfo.InvariantCulture));
            foreach (var item in target.data.inventory)
            {
                b.Append('|').Append(item.id).Append(':').Append(item.value).Append(':').Append(item.durability.ToString("R", CultureInfo.InvariantCulture));
                foreach (var child in item.inventory)
                    b.Append('/').Append(child.id).Append(':').Append(child.value).Append(':').Append(child.durability.ToString("R", CultureInfo.InvariantCulture));
            }
            b.Append("|money:").Append(MainGame.me.player.data.money.ToString("R", CultureInfo.InvariantCulture));
            b.Append("|faith:").Append(MainGame.me.player.data.GetItemsCount("faith", true));
            return b.ToString();
        }

        void Refresh(string reason)
        {
            if (!Current() || target == null) throw new InvalidOperationException("Stale refresh target");
            string before = Snapshot();
            Log("REDRAW_BEGIN reason=" + reason + " target=" + target.unique_id + " committed=" + Committed() + " preview=" + preview + " state=" + before);
            target.Redraw(false, false, false);
            string after = Snapshot();
            var actual = new List<string>();
            foreach (var part in target.additional_wops) if (part != null) actual.Add(part.gameObject.name.Replace(" ", "_"));
            bool intact = before == after;
            Log("REDRAW_END reason=" + reason + " invariants=" + intact + " actual_parts=" + string.Join(",", actual.ToArray()) + " state=" + after);
            if (!intact) throw new InvalidOperationException("Native item/body/quality/currency invariant changed");
        }

        void Queue(Action action) { if (pending == null) pending = action; }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8)) { panel = !panel; if (!panel) Cancel("panel-hidden"); }
            var action = pending;
            pending = null;
            if (action == null) return;
            try { action(); }
            catch (Exception ex) { Contain(ex); }
        }

        void Contain(Exception ex)
        {
            active = false;
            preview = -1;
            pending = null;
            status = "Пробник отключён после ошибки. Очистка остаётся доступной; см. журнал.";
            Logger.LogError("BPS_PROBE FAILED action=projection-disabled " + ex);
            if (Current() && target != null)
            {
                try { Log("ROLLBACK_BEGIN"); target.Redraw(false, false, false); Log("ROLLBACK_END fallback=vanilla"); }
                catch (Exception rollback) { Logger.LogError("BPS_PROBE ROLLBACK_FAILED " + rollback); }
            }
        }

        void Log(string message) { Logger.LogInfo("BPS_PROBE " + message); }

        void OnGUI()
        {
            if (!panel) return;
            if (label == null) { label = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true }; button = new GUIStyle(GUI.skin.button) { fontSize = 14, wordWrap = true }; }
            window.width = Math.Min(340, Math.Max(260, Screen.width - 24));
            window.x = Mathf.Clamp(window.x, 0, Math.Max(0, Screen.width - window.width));
            window.y = Mathf.Clamp(window.y, 0, Math.Max(0, Screen.height - 470));
            window = GUI.Window(827401, window, DrawPanel, "BPS: тест внешности " + Version);
        }

        void DrawPanel(int id)
        {
            GUILayout.Label("Диагностика одной могилы. F8 — скрыть/показать.", label);
            GUILayout.Label("«Сохранить вид» входит в сохранение игры. Вера, деньги и предметы не расходуются.", label);
            GUILayout.Label(status, label);
            bool previous = GUI.enabled;
            GUI.enabled = active && Current() && pending == null;
            if (GUILayout.Button("Выбрать открытую могилу", button)) Queue(Select);
            GUI.enabled = active && Current() && target != null && pending == null;
            if (GUILayout.Button("Сохранить вид: деревянный крест", button)) Queue(delegate { Commit(1); });
            if (GUILayout.Button("Сохранить вид: каменный крест", button)) Queue(delegate { Commit(2); });
            if (GUILayout.Button("Проверить перерисовку", button)) Queue(delegate { RequireTarget(); Refresh("explicit"); });
            if (GUILayout.Button("Примерить другой вид до закрытия окна", button)) Queue(Fit);
            if (GUILayout.Button("Отменить примерку", button)) Cancel("button");
            GUI.enabled = target != null && Current() && pending == null;
            if (GUILayout.Button("Очистить тест и вернуть исходный вид", button)) Queue(Cleanup);
            GUI.enabled = previous;
            GUILayout.Label("После очистки сохраните игру. При загрузке незавершённая примерка отменяется.", label);
            GUI.DragWindow(new Rect(0, 0, 10000, 22));
        }

        static void Opened(GraveGUI __instance, WorldGameObject __0)
        {
            var grave_wgo = __0;
            var p = self;
            if (p == null || !p.active) return;
            try
            {
                if (!p.Current() && p.SessionReady()) p.Recover();
                if (ReferenceEquals(p.openedGrave.GetValue(__instance), grave_wgo) && !grave_wgo.components.craft.is_crafting)
                { if (p.opened != grave_wgo) p.Cancel("target-change"); p.opened = grave_wgo; }
            }
            catch (Exception ex) { p.Contain(ex); }
        }

        static void Closed()
        {
            var p = self;
            if (p == null) return;
            p.opened = null;
            p.Cancel("grave-close");
        }

        static void Loading()
        {
            var p = self;
            if (p == null) return;
            if (p.preview >= 0) p.Log("CANCEL reason=load preview_not_saved=true");
            p.preview = -1; p.opened = null; p.target = null; p.session = null; p.pending = null; p.resourcesChecked = false;
        }

        static void Started()
        {
            var p = self;
            if (p != null && p.active) p.Queue(p.Recover);
        }

        static void Project(CustomDrawers __instance, ref List<WOPPrefabName> __result)
        {
            var p = self;
            if (p == null || !p.active || !p.Current() || p.target == null || __instance.wobj != p.target || __result == null || p.target.data.GetParam(Marker, 0f) <= 0f) return;
            try
            {
                int code = p.preview >= 0 && p.opened == p.target ? p.preview : p.Committed();
                if (code == 0) return;
                var item = p.target.data.GetItemOfType(ItemDefinition.ItemType.GraveStone, true);
                if (item == null) return;
                ProjectParts(__result, item.id, Looks[code]);
            }
            catch (Exception ex)
            {
                // Avoid recursively redrawing inside the native descriptor builder.
                p.active = false; p.preview = -1; p.pending = null;
                p.Logger.LogError("BPS_PROBE PROJECTION_FAILED action=disabled " + ex);
                p.status = "Подмена отключена после ошибки. Используйте очистку.";
            }
        }

        internal static void ProjectParts(IList<WOPPrefabName> parts, string actualId, string lookId)
        {
            string original = actualId + "_stg_";
            for (int i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                if (part == null || part.part_1 != original || part.stage <= 0) continue;
                parts[i] = new WOPPrefabName { part_1 = lookId + "_stg_", part_2 = part.part_2, stage = part.stage, need_herb = part.need_herb };
                break;
            }
        }

        void OnDestroy()
        {
            active = false; preview = -1; pending = null;
            if (harmony != null) harmony.UnpatchSelf();
            if (self == this) self = null;
        }
    }
}
