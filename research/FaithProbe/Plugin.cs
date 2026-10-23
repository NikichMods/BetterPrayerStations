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
        public const string Version = "0.5.0";
        const string Host = "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8";
        const int Limit = SavedFaithState.Limit; // Diagnostic UI/object-count bound, not a product cap.
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static Plugin self;
        Harmony harmony;
        bool ready, effects = true, panel = true;
        int measurement, checkpoint, shownConversions, shownWreaths;
        readonly PrayerCoverage coverage=new PrayerCoverage();
        readonly FaithPurchase purchase=new FaithPurchase();
        readonly TwoGraves graves=new TwoGraves();
        readonly Dictionary<long,int> previousTier=new Dictionary<long,int>(),previousCovering=new Dictionary<long,int>();
        bool zoneCheckRequested,zoneSampling;
        string sampleZoneId;
        int zoneEvents;
        float zoneEventValue;
        readonly HashSet<long> zoneInputOwners=new HashSet<long>();
        FieldInfo zoneOwner;
        int shownFaith;
        string pairSummary="";
        int coverageTier=-1, coveringCount; long nearestStation; float nearestCells, nextCoveragePoll;
        bool ActiveFor(WorldGameObject grave)
        {long id;float distance;int count;return effects && coverage.Evaluate(grave,out id,out distance,out count)>0;}
        bool Active {get{return ActiveFor(target);}}
        int conversions { get { return target == null || target.data == null ? 0 : SavedFaithState.Read(target.data).Conversions; } }
        int wreaths { get { return target == null || target.data == null ? 0 : SavedFaithState.Read(target.data).Wreaths; } }
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
                Exact(typeof(MainGame), "OnGameLoaded", typeof(void), typeof(GameSave)),
                Exact(typeof(MainGame), "OnGameStartedPlaying", typeof(void)),
                CoverageTarget()
            };
        }
        internal static MethodInfo CoverageTarget()
        {
            var m=typeof(WorldMap).GetMethod("OnAddNewWGO",BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly,null,new[]{typeof(WorldGameObject)},null);
            if(m==null || m.ReturnType!=typeof(void) || m.GetMethodBody()==null)throw new MissingMethodException("WorldMap.OnAddNewWGO");
            return m;
        }
        static void Added(WorldGameObject __0)
        {
            var p=self;if(p==null || !p.ready || __0==null || PrayerCoverage.Tier(__0.obj_id)==0)return;
            try{if(p.Current() && WorldMap.objs.Contains(__0))p.coverage.Register(__0);}catch(Exception ex){p.Fail(ex);}
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
            SavedFaithState.Convert(ref red,ref white,ref available,durability,count);
        }
        bool SessionReady()
        {
            return saveInventory != null && MainGame.me != null && MainGame.game_started && MainGame.me.save != null && MainGame.me.player != null &&
                MainGame.me.player.data != null && ReferenceEquals(saveInventory.GetValue(MainGame.me.save), MainGame.me.player.data);
        }
        bool Current() { return SessionReady() && ReferenceEquals(session, MainGame.me.save); }
        bool Bound(WorldGameObject grave)
        {
            return ready && !ReferenceEquals(grave,null) && graves.Contains(grave) && Current() && grave!=null && !grave.is_removed && grave.obj_id == "grave_ground" &&
                !grave.components.craft.is_crafting && grave.GetBodyFromInventory(true) != null;
        }
        bool BoundUI(GraveGUI view) { return view != null && Bound(graveField.GetValue(view) as WorldGameObject); }
        static void QualitySkulls(Item source, out int red, out int white, out int available, bool dontCountSelf, WorldGameObject grave)
        {
            source.GetBodySkulls(out red, out white, out available, dontCountSelf);
            var p = self;
            if (p == null || !p.Bound(grave) || !ReferenceEquals(source, grave.GetBodyFromInventory(true)) || dontCountSelf) return;
            p.qualitySkullHits++;if(p.zoneSampling)p.zoneInputOwners.Add(grave.unique_id);
            int rawRed=red,rawWhite=white,rawAvailable=available;
            try { SavedFaithState.ApplySkulls(grave.data,source,p.ActiveFor(grave),ref red,ref white,ref available); }
            catch(Exception ex){red=rawRed;white=rawWhite;available=rawAvailable;p.Fail(ex);}
        }
        static float QualityDecoration(Item inventory, string ignore, WorldGameObject grave)
        {
            float value = inventory.GetInventoryQuality(ignore);
            var p = self;
            if (p == null || !p.Bound(grave) || !ReferenceEquals(inventory, grave.data)) return value;
            p.qualityDecorHits++;
            try{return SavedFaithState.ApplyDecoration(grave.data,value,p.ActiveFor(grave));}catch(Exception ex){p.Fail(ex);return value;}
        }
        static void UISkulls(Item source, out int red, out int white, out int available, bool dontCountSelf, GraveGUI view)
        {
            source.GetBodySkulls(out red, out white, out available, dontCountSelf);
            var p = self;
            if (p == null || !p.BoundUI(view)) return;
            var grave=p.graveField.GetValue(view) as WorldGameObject;
            if(!ReferenceEquals(source,grave.GetBodyFromInventory(true)) || dontCountSelf)return;
            p.uiSkullHits++;
            int rawRed=red,rawWhite=white,rawAvailable=available;
            try { SavedFaithState.ApplySkulls(grave.data,source,p.ActiveFor(grave),ref red,ref white,ref available); }
            catch(Exception ex){red=rawRed;white=rawWhite;available=rawAvailable;p.Fail(ex);}
            p.lastRed = red; p.lastAvailable = available;
        }
        static float UIDecoration(float nativeSum, GraveGUI view)
        {
            var p = self;
            if (p == null || !p.BoundUI(view)) return nativeSum;
            p.uiDecorHits++;
            var grave=p.graveField.GetValue(view) as WorldGameObject;
            try{p.lastDecor=SavedFaithState.ApplyDecoration(grave.data,nativeSum,p.ActiveFor(grave));}catch(Exception ex){p.lastDecor=nativeSum;p.Fail(ex);}
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
                zoneOwner=AccessTools.Field(typeof(WorldGameObject),"_zone");
                if(zoneOwner==null || zoneOwner.FieldType!=typeof(WorldZone))throw new MissingFieldException("Native zone ownership/members");
                Exact(typeof(WorldZone),"GetTotalQuality",typeof(float));Exact(typeof(WorldZone),"GetZoneWGOs",typeof(List<WorldGameObject>));
                redraw = Exact(typeof(GraveGUI), "Redraw", typeof(void));
                var targets = Targets();
                harmony = new Harmony(Id);
                harmony.Patch(targets[0], transpiler: new HarmonyMethod(Own("QualityPatch")));
                harmony.Patch(targets[1], transpiler: new HarmonyMethod(Own("UIPatch")));
                harmony.Patch(targets[2], postfix: new HarmonyMethod(Own("Opened")));
                harmony.Patch(targets[3], prefix: new HarmonyMethod(Own("Loading")));
                harmony.Patch(targets[4], postfix: new HarmonyMethod(Own("Started")));
                harmony.Patch(targets[5], postfix: new HarmonyMethod(Own("Added")));
                ready = true;
                Log("READY version=" + Version + " host=" + hash + " scope=two-graves selection=editing-only zone=explicit-closed-window-sample persistence=three-grave-params payment=player-and-bags native-debit=true diagnostic_price=N+1 coverage=real-station-positions radius_cells=3 metric=xy-origin-circle poll_seconds=0.25");
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
            for (int i = 0; i < names.Count; i++) if (depth != 0 || !SavedFaithState.IsOwned(names[i])) ordered.Add(names[i] + "=" + values[i].ToString("R", Inv));
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
        string RawBody(Item current)
        {
            if(current==null) return "body=none";
            int red,white,available;current.GetBodySkulls(out red,out white,out available,false);
            var b=new StringBuilder("body="+red+"/"+white+"/"+available+" durability="+current.durability.ToString("R",Inv));
            foreach(var item in current.inventory)
            {
                b.Append(" part=").Append(item.id).Append(':').Append(item.GetRedSkullsValue()).Append('/').Append(item.GetWhiteSkullsValue());
                b.Append(" part_probe=").Append(item.GetParam(SavedFaithState.SchemaKey,0)).Append(',').Append(item.GetParam(SavedFaithState.ConversionKey,0)).Append(',').Append(item.GetParam(SavedFaithState.WreathKey,0));
            }
            b.Append(" body_probe=").Append(current.GetParam(SavedFaithState.SchemaKey,0)).Append(',').Append(current.GetParam(SavedFaithState.ConversionKey,0)).Append(',').Append(current.GetParam(SavedFaithState.WreathKey,0));
            return b.ToString();
        }
        string Raw(){return RawBody(target==null?null:target.GetBodyFromInventory(true));}
        bool Trackable(WorldGameObject grave)
        {
            if(grave==null || grave.is_removed || grave.data==null)return false;
            return grave.obj_id=="grave_ground" || grave.obj_id=="grave_empty" || grave.obj_id=="grave_corp" || grave.obj_id=="grave_exhume";
        }
        void UpdateDisplay()
        {
            if(target==null || target.data==null){shownConversions=shownWreaths=0;return;}
            var saved=SavedFaithState.Read(target.data);shownConversions=saved.Conversions;shownWreaths=saved.Wreaths;
            RefreshSummary();
            if(Current())shownFaith=FaithPurchase.Count(MainGame.me.player.data);
        }
        void RefreshSummary()
        {
            var summary=new StringBuilder();
            foreach(var g in graves.Items)
            {var v=SavedFaithState.Read(g.data);summary.Append("№").Append(g.unique_id).Append(" C").Append(v.Conversions).Append(" / L").Append(v.Wreaths).Append(" / N").Append(v.Actions).Append("; цена ").Append(FaithPurchase.Price(v.Actions)).Append(Bound(g)&&ActiveFor(g)?"; активно":"; неактивно").Append(ReferenceEquals(g,target)?" — выбрана":"").AppendLine();}
            pairSummary=summary.ToString();
        }
        void Recover()
        {
            if(!ready || !SessionReady())return;
            session=MainGame.me.save;target=null;body=null;coverage.Clear();coverageTier=-1;graves.Clear();previousTier.Clear();previousCovering.Clear();
            foreach(var grave in WorldMap.objs)
            {
                if(grave==null || grave.data==null)continue;
                coverage.Register(grave);
                float schema=grave.data.GetParam(SavedFaithState.SchemaKey,0f);
                if(schema==0f)continue;
                SavedFaithState.Read(grave.data);
                if(!Trackable(grave))throw new InvalidOperationException("Saved faith-probe target is not a grave");
                graves.Add(grave);
            }
            if(graves.Items.Count>0)target=graves.Items[0];
            if(target==null){Log("RECOVER target=none");status="Откройте могилу и выберите её. Вложения теперь сохраняются у могилы.";return;}
            UpdateDisplay();
            Log("RECOVER target="+target.unique_id+" object="+target.obj_id+" conversions="+conversions+" wreaths="+wreaths+" actions="+(conversions+wreaths)+" faith="+shownFaith+" next_price="+FaithPurchase.Price(conversions+wreaths));
            Capture("load");
            zoneCheckRequested=graves.Items.Count==2;
            status="Вложения восстановлены после загрузки. Откройте могилу или запишите этап после эксгумации.";
        }
        void Select()
        {
            if(!SessionReady() || !Trackable(opened) || opened.components.craft.is_crafting){status="Откройте обычную могилу без текущей работы.";return;}
            if(!Current()){Recover();status="Дождитесь восстановления и повторите выбор.";return;}
            if(!graves.Contains(opened) && graves.Items.Count>=2){status="Две могилы уже отмечены; сначала завершите тест очисткой обеих.";return;}
            if(graves.Items.Count>0)
            {
                var z=zoneOwner.GetValue(graves.Items[0]) as WorldZone;
                if(z==null || !ReferenceEquals(z,zoneOwner.GetValue(opened))){status="Выберите вторую могилу в той же нативной зоне кладбища.";return;}
            }
            graves.Add(opened);
            target=opened;body=target.GetBodyFromInventory(true);
            var saved=SavedFaithState.Read(target.data);
            SavedFaithState.Write(target.data,saved.Conversions,saved.Wreaths);UpdateDisplay();
            effects=true;PollCoverage(true);
            Log("SELECT target="+target.unique_id+" conversions="+conversions+" wreaths="+wreaths+" "+Raw());
            if(Bound(target))Measure("select",delegate{});else Capture("select-empty");
        }
        void CaptureOne(WorldGameObject grave,string role)
        {
            if(grave==null || grave.data==null || grave.is_removed)return;
            var saved=SavedFaithState.Read(grave.data);
            bool was=effects;float native;try{effects=false;native=grave.quality;}finally{effects=was;}
            float actual=grave.quality;
            Log("CHECKPOINT n="+checkpoint+" role="+role+" target="+grave.unique_id+" object="+grave.obj_id+" conversions="+saved.Conversions+" wreaths="+saved.Wreaths+" actions="+saved.Actions+" active="+ActiveFor(grave)+" native="+native.ToString("R",Inv)+" effective="+actual.ToString("R",Inv)+" faith="+FaithPurchase.Count(MainGame.me.player.data)+" next_price="+FaithPurchase.Price(saved.Actions)+" "+RawBody(grave.GetBodyFromInventory(true)));
        }
        void Capture(string reason)
        {
            if(!Current() || target==null || target.data==null){Log("CHECKPOINT_SKIPPED reason=missing-target-or-session");status="Нет выбранной текущей могилы; см. журнал.";return;}
            PollCoverage(true);UpdateDisplay();checkpoint++;Log("CHECKPOINT_BEGIN n="+checkpoint+" reason="+reason);
            foreach(var grave in graves.Items)CaptureOne(grave,"invested");
            status="Этап №"+checkpoint+" записан для обеих отмеченных могил.";
        }
        void Buy(bool conversion,int quoteActions,int quoteCost,bool shortage=false)
        {
            if(!Bound(target) || gui==null || !gui.gameObject.activeInHierarchy || !ReferenceEquals(graveField.GetValue(gui),target) || !Active || MainGame.me.gui_elements.build_mode_gui.is_shown || FloatingWorldGameObject.IsFloating())
            {status="Покупка не выполнена: откройте выбранную могилу под готовой молельней вне режима строительства.";Log("PURCHASE_REJECT reason=context-or-coverage");return;}
            if(conversion && !shortage)
            {
                int r,w,a;target.GetBodyFromInventory(true).GetBodySkulls(out r,out w,out a,false);
                if(conversions>=r){status="Нет красных черепков для ещё одного превращения; вера не списана.";Log("PURCHASE_REJECT reason=no-eligible-red");return;}
            }
            string physicalBefore=GraveHash();string otherBefore=OtherState();float moneyBefore=MainGame.me.player.data.money;
            FaithPurchase.Result result;
            try{result=purchase.Buy(target.data,MainGame.me.player.data,conversion,quoteActions,quoteCost,shortage);}
            catch(Exception ex)
            {
                Log("PURCHASE_ABORT target="+target.unique_id+" action="+(conversion?"conversion":"wreath")+" cost="+quoteCost+" faith_now="+FaithPurchase.Count(MainGame.me.player.data)+" actions_now="+(conversions+wreaths)+" containment="+ex.Message);
                throw;
            }
            UpdateDisplay();
            Log("PURCHASE target="+target.unique_id+" action="+(conversion?"conversion":"wreath")+" result="+result.Status+" quote_kind="+(shortage?"balance-plus-one":"N-plus-one")+" cost="+result.Cost+" faith_before="+result.FaithBefore+" faith_after="+result.FaithAfter+" actions_before="+result.ActionsBefore+" actions_after="+result.ActionsAfter+" conversions="+conversions+" wreaths="+wreaths+" grave_physical_unchanged="+(physicalBefore==GraveHash())+" money_unchanged="+(moneyBefore==MainGame.me.player.data.money)+" other_unchanged="+(otherBefore==OtherState()));
            if(physicalBefore!=GraveHash() || otherBefore!=OtherState() || moneyBefore!=MainGame.me.player.data.money)throw new InvalidOperationException("Purchase changed physical grave or money");
            if(result.Status=="committed")Measure("purchase-"+(conversion?"conversion":"wreath"),delegate{});
            else status="Покупка не выполнена: "+result.Status+". Вера и общий счётчик не изменены.";
        }
        void QueueBuy(bool conversion,bool shortage=false)
        {
            int actions=shownConversions+shownWreaths;
            int price=shortage?checked(FaithPurchase.Count(MainGame.me.player.data)+1):FaithPurchase.Price(actions);
            Queue(delegate{Buy(conversion,actions,price,shortage);});
        }
        string OtherState()
        {
            var b=new StringBuilder();
            foreach(var g in graves.Items)if(!ReferenceEquals(g,target))
                b.Append(g.unique_id).Append(':').Append(g.data.GetParam(SavedFaithState.SchemaKey,0)).Append(':').Append(g.data.GetParam(SavedFaithState.ConversionKey,0)).Append(':').Append(g.data.GetParam(SavedFaithState.WreathKey,0)).Append(':').Append(GraveHash(g));
            return b.ToString();
        }
        string GraveHash(){return GraveHash(target);}
        string GraveHash(WorldGameObject grave)
        {
            var b=new StringBuilder();Append(b,grave.data,0);
            using(var sha=SHA256.Create())return Hex(sha.ComputeHash(Encoding.UTF8.GetBytes(b.ToString())));
        }
        void Measure(string reason, Action change)
        {
            if (!Bound(target) || gui == null || !gui.gameObject.activeInHierarchy || !ReferenceEquals(graveField.GetValue(gui), target)) { status = "Откройте окно выбранной могилы; изменения не применены."; return; }
            body = target.GetBodyFromInventory(true);
            string before = PhysicalHash();
            change();UpdateDisplay();
            qualitySkullHits = qualityDecorHits = uiSkullHits = uiDecorHits = 0;
            bool enabled = effects; float native;
            try { effects = false; native = target.quality; } finally { effects = enabled; }
            float effective = target.quality;
            Log("REDRAW_BEGIN n=" + (++measurement) + " reason=" + reason + " target=" + target.unique_id + " conversions=" + conversions + " wreaths=" + wreaths + " active=" + Active + " native=" + native.ToString("R", Inv) + " effective=" + effective.ToString("R", Inv) + " physical=" + before);
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
            if (!Bound(target) || gui == null || !gui.gameObject.activeInHierarchy || !ReferenceEquals(graveField.GetValue(gui),target)) { Log("OBSERVE_SKIPPED reason=window-or-target-changed"); return; }
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
            if (reason == "cleanup") { graves.Clear();previousTier.Clear();previousCovering.Clear();pairSummary="";target = null; body = null; shownConversions=shownWreaths=0; Log("CLEANUP_DONE grave_params=cleared save_required=true"); status = "Тест очищен. Сохраните игру, чтобы закрепить очистку."; }
        }
        void Queue(Action action) { if (pending == null) pending = action; }
        void PollCoverage(bool force)
        {
            if(!ready || !Current() || graves.Items.Count==0)return;
            bool refresh=force;
            foreach(var grave in graves.Items)
            {
                int count;long nearest;float distance;int oldTier,oldCount;
                int tier=coverage.Evaluate(grave,out nearest,out distance,out count);
                bool changed=!previousTier.TryGetValue(grave.unique_id,out oldTier) || !previousCovering.TryGetValue(grave.unique_id,out oldCount) || oldTier!=tier || oldCount!=count;
                previousTier[grave.unique_id]=tier;previousCovering[grave.unique_id]=count;
                if(ReferenceEquals(grave,target)){coverageTier=tier;coveringCount=count;nearestStation=nearest;nearestCells=distance;}
                if(!changed && !force)continue;
                refresh=true;
                var saved=SavedFaithState.Read(grave.data);
                Log("COVERAGE target="+grave.unique_id+" tier="+tier+" covering="+count+" registered="+coverage.Count+" nearest="+nearest+" distance_cells="+distance.ToString("R",Inv)+" "+coverage.Describe(grave)+" radius_cells=3 conversions="+saved.Conversions+" wreaths="+saved.Wreaths+" actions="+saved.Actions);
                if(changed && !force)
                {
                    CaptureOne(grave,"coverage-transition");
                    if(ReferenceEquals(grave,target) && Bound(target) && gui!=null && gui.gameObject.activeInHierarchy && ReferenceEquals(graveField.GetValue(gui),target))Queue(delegate{Measure("coverage",delegate{});});
                }
            }
            if(refresh)RefreshSummary();
        }
        static void ZoneComputed(string id,float value)
        {var p=self;if(p!=null && p.zoneSampling && id==p.sampleZoneId){p.zoneEvents++;p.zoneEventValue=value;}}
        bool NativeWindowsClosed()
        {return !MainGame.me.gui_elements.IsAnyMassiveWindowOpened() && (gui==null || !gui.gameObject.activeInHierarchy) && !FloatingWorldGameObject.IsFloating();}
        void CheckZone()
        {
            if(!ready || !Current() || graves.Items.Count!=2 || !NativeWindowsClosed())return;
            foreach(var g in graves.Items)if(!Bound(g)){Log("ZONE_SKIPPED reason=grave-not-idle-occupied");return;}
            var first=graves.Items[0];var zone=zoneOwner.GetValue(first) as WorldZone;
            if(zone==null || zone.definition==null || zone.definition.calc_method!=WorldZoneDefinition.QualityCalcMethod.Sum)throw new InvalidOperationException("Zone must use verified native sum");
            var members=zone.GetZoneWGOs();
            foreach(var g in graves.Items)if(!ReferenceEquals(zoneOwner.GetValue(g),zone) || !members.Contains(g) || g.obj_def.ignore_counting_at_zone)throw new InvalidOperationException("Marked grave missing from selected native zone sum");
            string physicalBefore=GraveHash(graves.Items[0])+GraveHash(graves.Items[1]);int faith=FaithPurchase.Count(MainGame.me.player.data);float money=MainGame.me.player.data.money;
            float baseline=0f,delta=0f;var raw=new float[2];var effective=new float[2];
            bool was=effects;
            try
            {
                effects=false;
                // Individual native leaf reads only: NEVER call GetTotalQuality with effects disabled.
                foreach(var g in members)if(!g.obj_def.ignore_counting_at_zone)baseline+=g.quality;
                for(int i=0;i<2;i++)raw[i]=graves.Items[i].quality;
            }
            finally{effects=was;}
            for(int i=0;i<2;i++){effective[i]=graves.Items[i].quality;delta+=effective[i]-raw[i];}
            zoneEvents=0;zoneInputOwners.Clear();sampleZoneId=zone.id;zoneSampling=true;
            float actual;
            WorldZone.OnQualityComputed+=ZoneComputed;
            try{actual=zone.GetTotalQuality();}finally{zoneSampling=false;WorldZone.OnQualityComputed-=ZoneComputed;}
            float expected=baseline+delta;
            bool total=Math.Abs(actual-expected)<=.001f,notified=zoneEvents>0 && Math.Abs(zoneEventValue-actual)<=.001f;
            bool reached=zoneInputOwners.Contains(graves.Items[0].unique_id) && zoneInputOwners.Contains(graves.Items[1].unique_id);
            bool intact=physicalBefore==GraveHash(graves.Items[0])+GraveHash(graves.Items[1]) && faith==FaithPurchase.Count(MainGame.me.player.data) && money==MainGame.me.player.data.money;
            Log("ZONE closed=True zone="+zone.id+" baseline_leaf_sum="+baseline.ToString("R",Inv)+" grave_delta="+delta.ToString("R",Inv)+" expected="+expected.ToString("R",Inv)+" actual="+actual.ToString("R",Inv)+" native_events="+zoneEvents+" event_value="+zoneEventValue.ToString("R",Inv)+" both_inputs="+reached+" total_ok="+total+" event_ok="+notified+" physical_money_faith_unchanged="+intact);
            for(int i=0;i<2;i++){var g=graves.Items[i];Log("ZONE_GRAVE target="+g.unique_id+" selected="+ReferenceEquals(g,target)+" native="+raw[i].ToString("R",Inv)+" effective="+effective[i].ToString("R",Inv));CaptureOne(g,"zone-closed");}
            if(!total || !notified || !reached || !intact)throw new InvalidOperationException("Native closed-window zone observation failed");
            status="Окна закрыты: обе могилы учтены. Качество зоны "+actual.ToString("0.###",Inv)+", добавка от вложений "+delta.ToString("0.###",Inv)+".";
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8)) panel = !panel;
            var action = pending; pending = null;
            try
            {
                if(action!=null)action();
                if(pending==null && Time.realtimeSinceStartup>=nextCoveragePoll){nextCoveragePoll=Time.realtimeSinceStartup+.25f;PollCoverage(false);}
                if(zoneCheckRequested && pending==null && ready && Current() && NativeWindowsClosed()){zoneCheckRequested=false;CheckZone();}
            } catch(Exception ex){Fail(ex);}
        }
        void Fail(Exception ex)
        {
            bool wasReady = ready; ready = false; effects = false; pending = null;
            Logger.LogError("BPS_FAITH FAILED action=effects-disabled " + ex);
            status = "Эффекты отключены после ошибки. См. журнал; очистка доступна.";
            if (wasReady && Current() && gui != null && gui.gameObject.activeInHierarchy)
                Queue(delegate { Log("ROLLBACK_BEGIN"); redraw.Invoke(gui, null); Log("ROLLBACK_END fallback=vanilla-ui"); });
        }
        void ClearAll(){foreach(var g in graves.Items)SavedFaithState.Clear(g.data);zoneCheckRequested=false;}
        void Cleanup()
        {
            if(!Current() || target==null || target.data==null){status="Нет текущей тестовой могилы.";return;}
            if(Bound(target) && gui!=null && gui.gameObject.activeInHierarchy && ReferenceEquals(graveField.GetValue(gui),target))
                Measure("cleanup",delegate{ClearAll();effects=false;});
            else
            {
                long id=target.unique_id;ClearAll();effects=false;graves.Clear();previousTier.Clear();previousCovering.Clear();pairSummary="";target=null;body=null;shownConversions=shownWreaths=0;
                status="Вложения очищены. Сохраните игру; тела и украшения остаются на своих местах.";
                Log("CLEANUP_DONE target="+id+" grave_params=cleared save_required=true ui_refresh=false");
            }
        }
        void Log(string text) { Logger.LogInfo("BPS_FAITH " + text); }
        void OnGUI()
        {
            if (!panel) return;
            if (label == null) { label = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true }; button = new GUIStyle(GUI.skin.button) { fontSize = 14, wordWrap = true }; }
            window.width = Math.Min(365, Math.Max(260, Screen.width - 24)); window.height = Math.Min(550, Math.Max(260, Screen.height - 24));
            window.x = Mathf.Clamp(window.x, 0, Math.Max(0, Screen.width - window.width)); window.y = Mathf.Clamp(window.y, 0, Math.Max(0, Screen.height - window.height));
            window = GUI.Window(827402, window, Draw, "BPS: две могилы и качество зоны " + Version);
        }
        void Draw(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Пробник СПИСЫВАЕТ веру у героя и из его сумок. Тестовая цена: N+1. F8 — скрыть/показать.", label);
            GUILayout.Label(status, label);
            GUILayout.Label("Превращения: " + shownConversions + "; доп. венки: " + shownWreaths + "; эффекты: " + (coverageTier>0 ? "покрытие есть" : "вне покрытия"), label);
            GUILayout.Label("Молельни: "+coverage.Count+"; покрывают: "+coveringCount+"; уровень: "+coverageTier+"; ближайшая №"+nearestStation+" на "+(float.IsPositiveInfinity(nearestCells)?"—":nearestCells.ToString("0.00",Inv))+" кл.",label);
            GUILayout.Label("Вера: "+shownFaith+"; общих действий: "+(shownConversions+shownWreaths)+"; следующая цена: "+FaithPurchase.Price(shownConversions+shownWreaths)+" веры (тест).",label);
            GUILayout.Label(pairSummary,label);
            bool previous = GUI.enabled;
            GUI.enabled = ready && SessionReady() && pending == null;
            if (GUILayout.Button("Отметить / выбрать открытую могилу (до двух)", button)) Queue(Select);
            GUI.enabled = ready && Current() && target != null && pending == null;
            if (GUILayout.Button("Купить превращение за "+FaithPurchase.Price(shownConversions+shownWreaths)+" веры",button))QueueBuy(true);
            if (GUILayout.Button("Купить венок за "+FaithPurchase.Price(shownConversions+shownWreaths)+" веры",button))QueueBuy(false);
            if (GUILayout.Button("Проверить нехватку: цена = весь запас + 1",button))QueueBuy(false,true);
            if (GUILayout.Button("Повторно проверить рейтинг и шкалу", button)) Queue(delegate { Measure("explicit", delegate { }); });
            GUI.enabled = target != null && pending == null;
            if (GUILayout.Button("Записать состояние обеих могил",button))Queue(delegate{Capture("manual");});
            if (GUILayout.Button("Очистить ОБЕ могилы и завершить тест", button)) Queue(Cleanup);
            GUI.enabled=ready && Current() && graves.Items.Count==2 && pending==null;
            if(GUILayout.Button("Проверить зону после закрытия игровых окон",button))Queue(delegate{zoneCheckRequested=true;status="Закройте окно могилы обычным способом; затем пробник один раз проверит нативную сумму зоны.";});
            GUI.enabled = previous;
            GUILayout.Label("Отметьте две могилы. Выбор меняет только адрес покупки; эффекты работают у обеих без окна. После переноса молельни закажите проверку зоны и закройте игровые окна. Очистка обеих могил — БЕЗ возврата веры; затем сохраните игру.", label);
            GUILayout.EndScrollView(); GUI.DragWindow(new Rect(0, 0, 10000, 22));
        }
        static void Opened(GraveGUI __instance, WorldGameObject __0)
        {
            var p = self; if (p == null || !p.ready) return;
            if (ReferenceEquals(p.graveField.GetValue(__instance), __0)) { p.gui = __instance; p.opened = __0; }
        }
        static void Loading()
        {
            var p=self;if(p==null)return;
            if(p.target!=null)p.Log("RESET reason=load saved_grave_params=retained");
            p.target=p.opened=null;p.body=null;p.gui=null;p.session=null;p.pending=null;p.effects=true;p.shownConversions=p.shownWreaths=0;p.coverage.Clear();p.graves.Clear();p.previousTier.Clear();p.previousCovering.Clear();p.zoneCheckRequested=false;p.zoneSampling=false;p.pairSummary="";p.coverageTier=-1;p.coveringCount=0;p.nearestStation=0;p.nearestCells=float.PositiveInfinity;
        }
        static void Started()
        {
            var p=self;if(p!=null && p.ready)p.Queue(p.Recover);
        }
        void OnDestroy()
        {
            ready = false; effects = false; pending = null;
            if (harmony != null) harmony.UnpatchSelf();
            if (self == this) self = null;
        }
    }
}
