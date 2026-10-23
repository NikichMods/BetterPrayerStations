// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: AssemblyVersion(NikichMods.BPSFaithProbe.Plugin.Version + ".0")]
[assembly: AssemblyFileVersion(NikichMods.BPSFaithProbe.Plugin.Version + ".0")]
namespace NikichMods.BPSFaithProbe
{
    [BepInPlugin(Id, "Better Prayer Stations - Faith Probe", Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "nikichmods.bps.faithprobe";
        public const string Version = "0.1.1";
        const string Host = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        const int Limit = 12; // Diagnostic UI/object-count bound, not a product cap.
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static Plugin self;
        Harmony harmony;
        bool ready, effects = true, panel = true;
        int conversions, wreaths, measurement;
        WorldGameObject target, opened;
        Item body;
        GameSave session;
        GraveGUI gui;
        FieldInfo saveInventory, graveField, itemParams, resNames, resValues;
        FieldInfo[] resFloats;
        MethodInfo redraw;
        Action pending;
        int qualitySkullHits, qualityDecorHits, uiSkullHits, uiDecorHits;
        int lastRed, lastAvailable;
        float lastDecor;
        GUIStyle label, button;
        Rect window = new Rect(12, 80, 365, 550);
        Vector2 scroll;
        string status = "Откройте занятую могилу без текущей работы.";

        internal static MethodInfo Exact(Type type, string name, Type returns, params Type[] args)
        {
            var m = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, null, args, null);
            if (m == null || m.ReturnType != returns || m.GetMethodBody() == null) throw new MissingMethodException(type.FullName, name);
            return m;
        }
        internal static MethodInfo[] Targets()
        {
            return new[] {
                Exact(typeof(WorldGameObject), "get_quality", typeof(float)),
                Exact(typeof(GraveGUI), "RedrawGraveInventory", typeof(void)),
                Exact(typeof(GraveGUI), "Open", typeof(void), typeof(WorldGameObject)),
                Exact(typeof(MainGame), "OnGameLoaded", typeof(void), typeof(GameSave))
            };
        }
        static MethodInfo Own(string name) { return typeof(Plugin).GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic); }
        static readonly MethodInfo Skulls = Exact(typeof(Item), "GetBodySkulls", typeof(void), typeof(int).MakeByRefType(), typeof(int).MakeByRefType(), typeof(int).MakeByRefType(), typeof(bool));
        static readonly MethodInfo InventoryQuality = Exact(typeof(Item), "GetInventoryQuality", typeof(float), typeof(string));
        static readonly MethodInfo DrawPart = Exact(typeof(GraveGUI), "DrawGravePart", typeof(float), typeof(BaseItemCellGUI), typeof(Item), typeof(ItemDefinition.ItemType));
        static bool Calls(CodeInstruction i, MethodInfo m) { return (i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt) && Equals(i.operand, m); }
        static List<CodeInstruction> Copy(IEnumerable<CodeInstruction> source)
        {
            var result = new List<CodeInstruction>();
            foreach (var i in source) result.Add(new CodeInstruction(i));
            return result;
        }
        static void InsertOwner(List<CodeInstruction> result, CodeInstruction call, MethodInfo wrapper)
        {
            if (call.blocks.Count != 0) throw new InvalidOperationException("Unexpected exception boundary on projected call");
            var owner = new CodeInstruction(OpCodes.Ldarg_0);
            owner.labels.AddRange(call.labels); call.labels.Clear();
            result.Add(owner);
            call.opcode = OpCodes.Call; call.operand = wrapper;
            result.Add(call);
        }
        internal static List<CodeInstruction> RewriteQuality(IEnumerable<CodeInstruction> source)
        {
            var input = Copy(source);
            int skull = -1, inventory = -1, skullCount = 0, inventoryCount = 0;
            for (int i = 0; i < input.Count; i++)
            {
                if (Calls(input[i], Skulls)) { skull = i; skullCount++; }
                if (Calls(input[i], InventoryQuality)) { if (inventory < 0) inventory = i; inventoryCount++; }
            }
            // Structural semantic guard: one body skull call, first inventory input immediately
            // after it, then native floor/red subtraction/min. The second call stays generic.
            if (skullCount != 1 || inventoryCount != 2 || inventory != skull + 4 ||
                input[skull + 1].opcode != OpCodes.Ldarg_0 || input[skull + 2].opcode != OpCodes.Ldfld ||
                !(input[skull + 2].operand is FieldInfo) || ((FieldInfo)input[skull + 2].operand).Name != "_data" ||
                inventory + 8 >= input.Count || !Calls(input[inventory + 1], typeof(Mathf).GetMethod("Floor", new[] { typeof(float) })) ||
                input[inventory + 4].opcode != OpCodes.Sub || !Calls(input[inventory + 7], typeof(Mathf).GetMethod("Min", new[] { typeof(float), typeof(float) })) ||
                input[inventory + 8].opcode != OpCodes.Ret)
                throw new InvalidOperationException("Native grave rating input/cap shape mismatch");
            var output = new List<CodeInstruction>();
            for (int i = 0; i < input.Count; i++)
            {
                if (i == skull) InsertOwner(output, input[i], Own("QualitySkulls"));
                else if (i == inventory) InsertOwner(output, input[i], Own("QualityDecoration"));
                else output.Add(input[i]);
            }
            return output;
        }
        static int StoreLocal(CodeInstruction instruction)
        {
            if (instruction.opcode == OpCodes.Stloc_0) return 0;
            if (instruction.opcode == OpCodes.Stloc_1) return 1;
            if (instruction.opcode == OpCodes.Stloc_2) return 2;
            if (instruction.opcode == OpCodes.Stloc_3) return 3;
            if (instruction.opcode != OpCodes.Stloc && instruction.opcode != OpCodes.Stloc_S) return -1;
            var local = instruction.operand as LocalVariableInfo;
            return local != null ? local.LocalIndex : Convert.ToInt32(instruction.operand, Inv);
        }
        internal static List<CodeInstruction> RewriteUI(IEnumerable<CodeInstruction> source)
        {
            var input = Copy(source);
            var parts = new List<int>(); int skull = -1, skullCount = 0;
            for (int i = 0; i < input.Count; i++)
            {
                if (Calls(input[i], DrawPart)) parts.Add(i);
                if (Calls(input[i], Skulls)) { skull = i; skullCount++; }
            }
            if (parts.Count != 2 || skullCount != 1 || parts[1] + 2 >= input.Count || parts[1] >= skull)
                throw new InvalidOperationException("Native grave diagram shape mismatch");
            int firstStore = StoreLocal(input[parts[0] + 2]), secondStore = StoreLocal(input[parts[1] + 2]);
            if (input[parts[0] + 1].opcode != OpCodes.Add || input[parts[1] + 1].opcode != OpCodes.Add || firstStore < 0 || secondStore != firstStore ||
                Targets()[1].GetMethodBody().LocalVariables[secondStore].LocalType != typeof(float))
                throw new InvalidOperationException("Native decoration accumulator mismatch");
            var output = new List<CodeInstruction>();
            for (int i = 0; i < input.Count; i++)
            {
                if (i == skull) InsertOwner(output, input[i], Own("UISkulls"));
                else
                {
                    if (i == parts[1] + 2)
                    {
                        if (input[i].blocks.Count != 0) throw new InvalidOperationException("Unexpected UI exception boundary");
                        var owner = new CodeInstruction(OpCodes.Ldarg_0);
                        owner.labels.AddRange(input[i].labels); input[i].labels.Clear();
                        output.Add(owner); output.Add(new CodeInstruction(OpCodes.Call, Own("UIDecoration")));
                    }
                    output.Add(input[i]);
                }
            }
            return output;
        }
        static IEnumerable<CodeInstruction> QualityPatch(IEnumerable<CodeInstruction> instructions) { return RewriteQuality(instructions); }
        static IEnumerable<CodeInstruction> UIPatch(IEnumerable<CodeInstruction> instructions) { return RewriteUI(instructions); }
        internal static void ConvertSkulls(ref int red, ref int white, ref int available, float durability, int count)
        {
            int changed = Math.Min(count, red);
            if (changed <= 0) return;
            red -= changed; white += changed;
            int percent = Mathf.CeilToInt(durability * 100f);
            if (percent > 90) percent = 100;
            available = Mathf.FloorToInt((float)(white * percent) / 100f);
        }
        bool SessionReady()
        {
            return saveInventory != null && MainGame.me != null && MainGame.game_started && MainGame.me.save != null && MainGame.me.player != null &&
                MainGame.me.player.data != null && ReferenceEquals(saveInventory.GetValue(MainGame.me.save), MainGame.me.player.data);
        }
        bool Current() { return SessionReady() && ReferenceEquals(session, MainGame.me.save); }
        bool Bound(WorldGameObject grave)
        {
            return ready && ReferenceEquals(grave, target) && target != null && Current() && !target.is_removed && target.obj_id == "grave_ground" &&
                !target.components.craft.is_crafting && ReferenceEquals(target.GetBodyFromInventory(true), body) && body != null;
        }
        bool BoundUI(GraveGUI view) { return view != null && Bound(graveField.GetValue(view) as WorldGameObject); }
        static void QualitySkulls(Item source, out int red, out int white, out int available, bool dontCountSelf, WorldGameObject grave)
        {
            source.GetBodySkulls(out red, out white, out available, dontCountSelf);
            var p = self;
            if (p == null || !p.Bound(grave) || !ReferenceEquals(source, p.body) || dontCountSelf) return;
            p.qualitySkullHits++;
            if (p.effects) ConvertSkulls(ref red, ref white, ref available, source.durability, p.conversions);
        }
        static float QualityDecoration(Item inventory, string ignore, WorldGameObject grave)
        {
            float value = inventory.GetInventoryQuality(ignore);
            var p = self;
            if (p == null || !p.Bound(grave) || !ReferenceEquals(inventory, grave.data)) return value;
            p.qualityDecorHits++;
            return value + (p.effects ? p.wreaths : 0);
        }
        static void UISkulls(Item source, out int red, out int white, out int available, bool dontCountSelf, GraveGUI view)
        {
            source.GetBodySkulls(out red, out white, out available, dontCountSelf);
            var p = self;
            if (p == null || !p.BoundUI(view) || !ReferenceEquals(source, p.body) || dontCountSelf) return;
            p.uiSkullHits++;
            if (p.effects) ConvertSkulls(ref red, ref white, ref available, source.durability, p.conversions);
            p.lastRed = red; p.lastAvailable = available;
        }
        static float UIDecoration(float nativeSum, GraveGUI view)
        {
            var p = self;
            if (p == null || !p.BoundUI(view)) return nativeSum;
            p.uiDecorHits++;
            p.lastDecor = nativeSum + (p.effects ? p.wreaths : 0);
            return p.lastDecor;
        }
        void Awake()
        {
            self = this;
            try
            {
                string hash;
                using (var stream = File.OpenRead(typeof(WorldGameObject).Assembly.Location))
                using (var sha = SHA256.Create()) hash = Hex(sha.ComputeHash(stream));
                if (hash != Host) throw new InvalidOperationException("Unverified host " + hash);
                saveInventory = AccessTools.Field(typeof(GameSave), "_inventory");
                graveField = AccessTools.Field(typeof(GraveGUI), "_grave_wgo");
                itemParams = AccessTools.Field(typeof(Item), "_params");
                resNames = AccessTools.Field(typeof(GameRes), "_res_type");
                resValues = AccessTools.Field(typeof(GameRes), "_res_v");
                resFloats = new[] { AccessTools.Field(typeof(GameRes), "_hp"), AccessTools.Field(typeof(GameRes), "_progress"), AccessTools.Field(typeof(GameRes), "_money"), AccessTools.Field(typeof(GameRes), "_durability") };
                if (saveInventory == null || saveInventory.FieldType != typeof(Item) || graveField == null || graveField.FieldType != typeof(WorldGameObject) ||
                    itemParams == null || itemParams.FieldType != typeof(GameRes) || resNames == null || resNames.FieldType != typeof(List<string>) ||
                    resValues == null || resValues.FieldType != typeof(List<float>)) throw new MissingFieldException("Native owner/parameter fields");
                foreach (var field in resFloats) if (field == null || field.FieldType != typeof(float)) throw new MissingFieldException("Native scalar parameter field");
                redraw = Exact(typeof(GraveGUI), "Redraw", typeof(void));
                var targets = Targets();
                harmony = new Harmony(Id);
                harmony.Patch(targets[0], transpiler: new HarmonyMethod(Own("QualityPatch")));
                harmony.Patch(targets[1], transpiler: new HarmonyMethod(Own("UIPatch")));
                harmony.Patch(targets[2], postfix: new HarmonyMethod(Own("Opened")));
                harmony.Patch(targets[3], prefix: new HarmonyMethod(Own("Loading")));
                ready = true;
                Log("READY version=" + Version + " host=" + hash + " scope=one-grave synthetic_activation=true persistence=none payment=none coverage=none");
            }
            catch (Exception ex)
            {
                ready = false;
                if (harmony != null) harmony.UnpatchSelf();
                status = "Пробник не подключился. См. журнал.";
                Logger.LogError("BPS_FAITH INIT_FAILED action=disabled " + ex);
            }
        }
        static string Hex(byte[] bytes) { return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant(); }
        void Append(StringBuilder b, Item item, int depth)
        {
            if (item == null) { b.Append("null;"); return; }
            if (depth > 8) throw new InvalidOperationException("Unexpected inventory depth");
            b.Append(item.id).Append(':').Append(item.value).Append('{');
            var parameters = (GameRes)itemParams.GetValue(item);
            var names = (List<string>)resNames.GetValue(parameters); var values = (List<float>)resValues.GetValue(parameters);
            if (names.Count != values.Count) throw new InvalidOperationException("Native parameter lists differ");
            var ordered = new List<string>();
            for (int i = 0; i < names.Count; i++) ordered.Add(names[i] + "=" + values[i].ToString("R", Inv));
            ordered.Sort(StringComparer.Ordinal);
            foreach (string value in ordered) b.Append(value).Append(';');
            foreach (var field in resFloats) b.Append(field.Name).Append('=').Append(((float)field.GetValue(parameters)).ToString("R", Inv)).Append(';');
            b.Append("}[inventory:");
            if (item.inventory != null) foreach (var child in item.inventory) Append(b, child, depth + 1);
            b.Append("][secondary:");
            if (item.secondary_inventory != null) foreach (var child in item.secondary_inventory) Append(b, child, depth + 1);
            b.Append(']');
        }
        string PhysicalHash()
        {
            var b = new StringBuilder(); Append(b, target.data, 0);
            b.Append("money=").Append(MainGame.me.player.data.money.ToString("R", Inv)).Append(";faith=").Append(MainGame.me.player.data.GetItemsCount("faith", true));
            using (var sha = SHA256.Create()) return Hex(sha.ComputeHash(Encoding.UTF8.GetBytes(b.ToString())));
        }
        string Raw()
        {
            int red, white, available; body.GetBodySkulls(out red, out white, out available, false);
            var b = new StringBuilder("body=" + red + "/" + white + "/" + available + " durability=" + body.durability.ToString("R", Inv));
            foreach (var item in body.inventory) b.Append(" part=").Append(item.id).Append(':').Append(item.GetRedSkullsValue()).Append('/').Append(item.GetWhiteSkullsValue());
            return b.ToString();
        }
        void Select()
        {
            if (!SessionReady() || opened == null || opened.is_removed || opened.obj_id != "grave_ground" || opened.components.craft.is_crafting) { status = "Откройте занятую закопанную могилу без работы."; return; }
            if (target != null && !ReferenceEquals(target, opened)) { status = "Сначала очистите предыдущую могилу."; return; }
            var candidate = opened.GetBodyFromInventory(true);
            if (candidate == null) { status = "Нужна могила с телом."; return; }
            session = MainGame.me.save; target = opened; body = candidate;
            Log("SELECT target=" + target.unique_id + " " + Raw());
            Measure("select", delegate { conversions = 0; wreaths = 0; effects = true; });
        }
        void Measure(string reason, Action change)
        {
            if (!Bound(target) || gui == null || !gui.gameObject.activeInHierarchy || !ReferenceEquals(graveField.GetValue(gui), target)) { status = "Откройте окно выбранной могилы; изменения не применены."; return; }
            string before = PhysicalHash();
            change();
            qualitySkullHits = qualityDecorHits = uiSkullHits = uiDecorHits = 0;
            bool enabled = effects; float native;
            try { effects = false; native = target.quality; } finally { effects = enabled; }
            float effective = target.quality;
            Log("REDRAW_BEGIN n=" + (++measurement) + " reason=" + reason + " target=" + target.unique_id + " conversions=" + conversions + " wreaths=" + wreaths + " active=" + effects + " native=" + native.ToString("R", Inv) + " effective=" + effective.ToString("R", Inv) + " physical=" + before);
            redraw.Invoke(gui, null); // One real GUI redraw, no world-part recreation.
            bool intact = before == PhysicalHash();
            Log("REDRAW_END n=" + measurement + " physical_unchanged=" + intact + " " + Raw());
            if (!intact) throw new InvalidOperationException("Physical grave/body/organ/currency parameters changed");
            Queue(delegate { Observe(reason, effective); }); // Let native child destruction settle.
        }
        int Children(UITableOrGrid table, GameObject first, GameObject second)
        {
            int count = 0;
            foreach (Transform child in table.transform)
                if (child.gameObject != first && child.gameObject != second && child.gameObject.activeSelf) count++;
            return count;
        }
        internal static bool RedBarMatches(int red, int height, bool visible)
        {
            // Native GraveGUI hides this widget at zero red. NGUI can coerce height
            // through sprite minimums/anchors; invisible height has no displayed meaning.
            return visible == (red > 0) && (red == 0 || height == 7 + 13 * red);
        }
        void Observe(string reason, float rating)
        {
            if (!Bound(target) || gui == null || !gui.gameObject.activeInHierarchy) { Log("OBSERVE_SKIPPED reason=window-or-target-changed"); return; }
            int red, white, available; body.GetBodySkulls(out red, out white, out available, false);
            int slots = Children(gui.tb_slots, gui.prefab_slot_grey, gui.prefab_slot_red);
            int decorations = Children(gui.tb_skulls, gui.prefab_w, gui.prefab_w_red);
            bool rawPanel = gui.body_panel.skull_bar.negative == red && gui.body_panel.skull_bar.positive == white && gui.body_panel.skull_bar.durability == body.durability;
            bool slotsOk = slots == lastRed + lastAvailable;
            bool wreathsOk = decorations == Mathf.FloorToInt(lastDecor);
            int barHeight = gui.red_vertical_bar.height;
            int barMinimum = gui.red_vertical_bar.minHeight;
            bool barVisible = gui.red_vertical_bar.gameObject.activeSelf;
            bool barOk = RedBarMatches(lastRed, barHeight, barVisible);
            bool chart = slotsOk && wreathsOk && barOk;
            bool total = gui.txt_grave_total_q.text == rating.ToString("0");
            bool reached = qualitySkullHits > 0 && qualityDecorHits > 0 && uiSkullHits > 0 && uiDecorHits > 0;
            Log("OBSERVE n=" + measurement + " reason=" + reason + " target=" + target.unique_id + " rating=" + rating.ToString("R", Inv) + " label=" + gui.txt_grave_total_q.text + " effective_red=" + lastRed + " effective_available=" + lastAvailable + " decoration=" + lastDecor.ToString("R", Inv) + " actual_slots=" + slots + " actual_wreaths=" + decorations + " slots_ok=" + slotsOk + " wreaths_ok=" + wreathsOk + " red_bar_ok=" + barOk + " red_bar_height=" + barHeight + " red_bar_min=" + barMinimum + " red_bar_visible=" + barVisible + " raw_panel=" + rawPanel + " chart=" + chart + " total=" + total + " seams=" + qualitySkullHits + "," + qualityDecorHits + "," + uiSkullHits + "," + uiDecorHits);
            if (!rawPanel || !chart || !total || !reached) throw new InvalidOperationException("Native rating/diagram/raw-body observation mismatch");
            status = "Проверено: рейтинг " + rating.ToString("0") + "; шкала " + lastRed + " красн. / " + lastAvailable + " доступных бел.; венков " + Mathf.FloorToInt(lastDecor) + ". Тело без изменений.";
            if (reason == "cleanup") { target = null; body = null; conversions = wreaths = 0; Log("CLEANUP_DONE persistent_writes=0"); status = "Тест очищен. В сохранение ничего не записывалось."; }
        }
        void Queue(Action action) { if (pending == null) pending = action; }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8)) panel = !panel;
            var action = pending; pending = null;
            if (action == null) return;
            try { action(); } catch (Exception ex) { Fail(ex); }
        }
        void Fail(Exception ex)
        {
            bool wasReady = ready; ready = false; effects = false; pending = null;
            Logger.LogError("BPS_FAITH FAILED action=effects-disabled " + ex);
            status = "Эффекты отключены после ошибки. См. журнал; очистка доступна.";
            if (wasReady && Current() && gui != null && gui.gameObject.activeInHierarchy)
                Queue(delegate { Log("ROLLBACK_BEGIN"); redraw.Invoke(gui, null); Log("ROLLBACK_END fallback=vanilla-ui"); });
        }
        void Cleanup()
        {
            if (Bound(target) && gui != null && gui.gameObject.activeInHierarchy && ReferenceEquals(graveField.GetValue(gui), target)) Measure("cleanup", delegate { conversions = wreaths = 0; effects = false; });
            else { target = null; body = null; effects = false; conversions = wreaths = 0; status = "Тест очищен; постоянных записей нет."; Log("CLEANUP_DONE persistent_writes=0 ui_refresh=false"); }
        }
        void Log(string text) { Logger.LogInfo("BPS_FAITH " + text); }
        void OnGUI()
        {
            if (!panel) return;
            if (label == null) { label = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true }; button = new GUIStyle(GUI.skin.button) { fontSize = 14, wordWrap = true }; }
            window.width = Math.Min(365, Math.Max(260, Screen.width - 24)); window.height = Math.Min(550, Math.Max(260, Screen.height - 24));
            window.x = Mathf.Clamp(window.x, 0, Math.Max(0, Screen.width - window.width)); window.y = Mathf.Clamp(window.y, 0, Math.Max(0, Screen.height - window.height));
            window = GUI.Window(827402, window, Draw, "BPS: тест улучшений " + Version);
        }
        void Draw(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Только текущий сеанс. Без веры и радиуса. F8 — скрыть/показать.", label);
            GUILayout.Label(status, label);
            GUILayout.Label("Превращения: " + conversions + "; доп. венки: " + wreaths + "; эффекты: " + (effects ? "включены" : "отключены"), label);
            bool previous = GUI.enabled;
            GUI.enabled = ready && SessionReady() && pending == null;
            if (GUILayout.Button("Выбрать открытую могилу", button)) Queue(Select);
            GUI.enabled = ready && Current() && target != null && pending == null;
            if (GUILayout.Button("Добавить одно превращение", button)) Queue(delegate { if (conversions < Limit) Measure("conversion", delegate { conversions++; }); else status = "Тестовый предел 12."; });
            if (GUILayout.Button("Добавить один венок", button)) Queue(delegate { if (wreaths < Limit) Measure("wreath", delegate { wreaths++; }); else status = "Тестовый предел 12."; });
            if (GUILayout.Button(effects ? "Отключить эффекты" : "Включить эффекты", button)) Queue(delegate { Measure("toggle", delegate { effects = !effects; }); });
            if (GUILayout.Button("Обнулить улучшения", button)) Queue(delegate { Measure("reset", delegate { conversions = wreaths = 0; }); });
            if (GUILayout.Button("Повторно проверить рейтинг и шкалу", button)) Queue(delegate { Measure("explicit", delegate { }); });
            GUI.enabled = target != null && pending == null;
            if (GUILayout.Button("Очистить и завершить тест", button)) Queue(Cleanup);
            GUI.enabled = previous;
            GUILayout.Label("Панель тела показывает настоящие черепки; центральная шкала — результат улучшений. Не снимайте тело/украшения в этом узком тесте.", label);
            GUILayout.EndScrollView(); GUI.DragWindow(new Rect(0, 0, 10000, 22));
        }
        static void Opened(GraveGUI __instance, WorldGameObject __0)
        {
            var p = self; if (p == null || !p.ready) return;
            if (ReferenceEquals(p.graveField.GetValue(__instance), __0)) { p.gui = __instance; p.opened = __0; }
        }
        static void Loading()
        {
            var p = self; if (p == null) return;
            if (p.target != null) p.Log("RESET reason=load persistent_writes=0");
            p.target = p.opened = null; p.body = null; p.gui = null; p.session = null; p.pending = null; p.conversions = p.wreaths = 0; p.effects = true;
        }
        void OnDestroy()
        {
            ready = false; effects = false; pending = null;
            if (harmony != null) harmony.UnpatchSelf();
            if (self == this) self = null;
        }
    }
}
