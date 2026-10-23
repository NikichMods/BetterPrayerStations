// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using HarmonyLib;

static class FaithTests
{
    static Type plugin;
    static MethodInfo Own(string name) { return plugin.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static); }
    static void Assert(bool value, string text) { if (!value) throw new Exception(text); }
    static List<CodeInstruction> Rewrite(string name, List<CodeInstruction> input) { return (List<CodeInstruction>)Own(name).Invoke(null, new object[] { input }); }
    static int Count(List<CodeInstruction> list, string owner, string name)
    {
        int count = 0;
        foreach (var i in list) { var m = i.operand as MethodInfo; if (m != null && m.DeclaringType.Name == owner && m.Name == name && (i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt)) count++; }
        return count;
    }
    static void Reject(string name, List<CodeInstruction> input)
    {
        bool rejected = false;
        try { Rewrite(name, input); } catch (TargetInvocationException ex) { rejected = ex.InnerException is InvalidOperationException; }
        Assert(rejected, "Unexpected IL shape accepted: " + name);
    }
    static Item Body(int red, int white, float durability)
    {
        var item = (Item)FormatterServices.GetUninitializedObject(typeof(Item));
        var definition = (ItemDefinition)FormatterServices.GetUninitializedObject(typeof(ItemDefinition));
        definition.id = "body"; definition.q_minus = red; definition.q_plus = white;
        item.id = "body"; item.value = 1; item.inventory = new List<Item>(); item.secondary_inventory = new List<Item>();
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        typeof(Item).GetField("_params", flags).SetValue(item, new GameRes());
        typeof(Item).GetField("_definition", flags).SetValue(item, definition);
        typeof(Item).GetField("_definition_set", flags).SetValue(item, true);
        typeof(Item).GetField("_definition_set_id", flags).SetValue(item, "body");
        item.durability = durability;
        return item;
    }
    static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
        {
            string file = new AssemblyName(e.Name).Name + ".dll";
            foreach (string dir in new[] { args[0], args[1], Path.GetDirectoryName(args[2]) })
            { string path = Path.Combine(dir, file); if (File.Exists(path)) return Assembly.LoadFrom(path); }
            return null;
        };
        try { Run(args[2]); return 0; } catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    static void Run(string path)
    {
        var assembly = Assembly.LoadFrom(path);
        plugin = assembly.GetType("NikichMods.BPSFaithProbe.Plugin", true);
        string version = (string)plugin.GetField("Version").GetRawConstantValue();
        Assert(assembly.GetName().Version.ToString() == version + ".0", "Version mismatch");
        var targets = (MethodInfo[])Own("Targets").Invoke(null, null);
        Assert(targets.Length == 6 && targets[0].DeclaringType == typeof(WorldGameObject) && targets[1].DeclaringType == typeof(GraveGUI) && targets[2].GetParameters()[0].ParameterType == typeof(WorldGameObject), "Wrong exact targets");
        bool rejected = false;
        try { Own("Exact").Invoke(null, new object[] { typeof(GraveGUI), "Open", typeof(void), Type.EmptyTypes }); }
        catch (TargetInvocationException ex) { rejected = ex.InnerException is MissingMethodException; }
        Assert(rejected, "Inherited parameterless Open accepted");
        Assert(Own("Opened").GetParameters()[1].Name == "__0", "Open must use indexed argument");
        ILGenerator qualityGenerator;
        var quality = PatchProcessor.GetOriginalInstructions(targets[0], out qualityGenerator);
        var rewritten = Rewrite("RewriteQuality", quality);
        Assert(Count(rewritten,"Plugin","QualitySkulls") == 1 && Count(rewritten,"Plugin","QualityDecoration") == 1, "Missing rating projections");
        Assert(Count(rewritten,"Item","GetBodySkulls") == 0 && Count(rewritten,"Item","GetInventoryQuality") == 1, "Generic branch was projected or grave input missed");
        Assert(Count(rewritten,"Mathf","Floor") == Count(quality,"Mathf","Floor") && Count(rewritten,"Mathf","Min") == Count(quality,"Mathf","Min"), "Native cap altered");
        ILGenerator uiGenerator;
        var ui = PatchProcessor.GetOriginalInstructions(targets[1], out uiGenerator);
        var uiNew = Rewrite("RewriteUI", ui);
        Assert(Count(uiNew,"Plugin","UISkulls") == 1 && Count(uiNew,"Plugin","UIDecoration") == 1 && Count(uiNew,"GraveGUI","DrawGravePart") == 2, "UI card sum/body projection mismatch");
        Assert(Count(uiNew,"Item","GetBodySkulls") == 0, "Raw central skull call remained");
        var broken = new List<CodeInstruction>(quality);
        broken.RemoveAll(i => i.operand is MethodInfo && ((MethodInfo)i.operand).Name == "GetBodySkulls");
        Reject("RewriteQuality",broken);
        broken = new List<CodeInstruction>(ui);
        broken.RemoveAll(i => i.operand is MethodInfo && ((MethodInfo)i.operand).Name == "DrawGravePart");
        Reject("RewriteUI",broken);
        // Transfer branch labels to the inserted owner load, and leave source metadata intact.
        var testLabel = qualityGenerator.DefineLabel();
        CodeInstruction marked = null;
        foreach (var i in quality) if (i.operand is MethodInfo && ((MethodInfo)i.operand).Name == "GetBodySkulls") { marked = i; break; }
        marked.labels.Add(testLabel);
        var labelled = Rewrite("RewriteQuality",quality);
        bool moved = false;
        foreach (var i in labelled) if (i.labels.Contains(testLabel)) { Assert(i.opcode == OpCodes.Ldarg_0,"Label skips inserted owner argument"); moved = true; }
        Assert(moved && marked.labels.Contains(testLabel),"Source label metadata lost/mutated");
        Console.WriteLine("PASS exact native targets and actual installed-method IL rewrite: only grave inputs projected; generic branch, native floor/min and physical item-card calls retained; changed shapes rejected; branch label preserved.");
        var bar = Own("RedBarMatches");
        foreach (int hiddenHeight in new[] {2,7,8,12,46})
            Assert((bool)bar.Invoke(null,new object[]{0,hiddenHeight,false}),"Invisible zero-red bar rejected because of meaningless height");
        Assert(!(bool)bar.Invoke(null,new object[]{0,7,true}),"Visible zero-red bar accepted");
        Assert((bool)bar.Invoke(null,new object[]{1,20,true}),"Correct one-red bar rejected");
        Assert((bool)bar.Invoke(null,new object[]{3,46,true}),"Correct three-red bar rejected");
        Assert(!(bool)bar.Invoke(null,new object[]{1,20,false}),"Missing visible red bar accepted");
        Assert(!(bool)bar.Invoke(null,new object[]{1,7,true}),"Wrong visible bar geometry accepted");
        Console.WriteLine("PASS native red-bar observation contract: zero red requires hidden state, ignores hidden geometry; positive red requires visible correct-height bar.");
        var convert = Own("ConvertSkulls"); int cases = 0;
        foreach (float durability in new[] {1f,.97f,.9001f,.9f,.8999f,.8f,.01f,0f})
        foreach (var pair in new[] {new[]{3,3},new[]{0,6},new[]{7,0},new[]{1,11},new[]{-2,-1}})
        foreach (int capacity in new[] {0,1,3,12})
        {
            var original = Body(pair[0],pair[1],durability);
            int r,w,a; original.GetBodySkulls(out r,out w,out a,false);
            var values = new object[]{r,w,a,durability,capacity};
            convert.Invoke(null,values);
            int c = Math.Min(capacity,r);
            var oracle = Body(r-c,w+c,durability);
            int er,ew,ea; oracle.GetBodySkulls(out er,out ew,out ea,false);
            Assert((int)values[0] == er && (int)values[1] == ew && (int)values[2] == ea,"Compiled projection differs from native reference skulls");
            int rr,ww,aa; original.GetBodySkulls(out rr,out ww,out aa,false);
            Assert(rr==r && ww==w && aa==a && original.durability==durability,"Original synthetic body changed");
            foreach (string wrapper in new[]{"QualitySkulls","UISkulls"})
            {
                var outside = new object[]{original,0,0,0,false,null};
                Own(wrapper).Invoke(null,outside);
                Assert((int)outside[1]==r && (int)outside[2]==w && (int)outside[3]==a,"Unbound body affected by wrapper");
            }
            cases++;
        }
        Console.WriteLine("PASS compiled conversion against independent native skull oracle: "+cases+" cases; red bound, decay thresholds, unchanged original and unbound body pass-through.");
        var coverage=assembly.GetType("NikichMods.BPSFaithProbe.PrayerCoverage",true);
        var previewEligible=coverage.GetMethod("PreviewEligibleTier");
        Assert((int)previewEligible.Invoke(null,new object[]{"pray_stand_wd_place",288f,288f})==1,"Native construction preview rejected at square corner");
        Assert((int)previewEligible.Invoke(null,new object[]{"pray_stand_stn_place",384.01f,0f})==0,"Native construction preview accepted outside square");
        Assert((int)previewEligible.Invoke(null,new object[]{"grave_ground",0f,0f})==0,"Nonstation cursor contributes coverage");
        var accumulate=PatchProcessor.GetOriginalInstructions(coverage.GetMethod("Accumulate",BindingFlags.NonPublic|BindingFlags.Instance));
        Assert(Count(accumulate,"PrayerCoverage","PreviewEligibleTier")==1,"Actual coverage accumulator bypasses placeholder preview eligibility");
        var eligible=coverage.GetMethod("EligibleTier");
        Func<string,bool,bool,float,float,int> tier=(id,removed,busy,x,y)=>(int)eligible.Invoke(null,new object[]{id,removed,busy,x,y});
        Assert(tier("pray_stand_wd",false,false,288,0)==1,"Boundary not inclusive");
        Assert(tier("pray_stand_stn",false,false,0,-384)==2,"Stone boundary");
        Assert(tier("pray_stand_stn",false,false,384.01f,0)==0,"Outside square accepted");
        Assert(tier("pray_stand_wd",false,false,288,288)==1,"Square corner rejected");
        Assert(tier("pray_stand_wd",false,false,172.8f,230.4f)==1,"Square interior rejected");
        Assert(tier("pray_stand_stn",true,false,0,0)==0,"Removed station active");
        Assert(tier("pray_stand_stn",false,true,0,0)==0,"Busy station active");
        Assert(tier("pray_stand_wd_place",false,false,0,0)==0,"Placeholder station accepted");
        Assert(tier("grave_ground",false,false,0,0)==0,"Nonstation accepted");
        foreach(float offset in new[]{-500f,-288.01f,-288f,0f,288f,288.01f,500f})
        {
            int expected=Math.Abs(offset)<=288f?1:0;
            Assert(tier("pray_stand_wd",false,false,offset,0)==expected,"Move out/return");
        }
        Console.WriteLine("PASS diagnostic coverage geometry: native32-unit cell scale, inclusive square corners, tier-specific half-side, wood/stone, absent/busy/removed, movement out/back. No Unity registry or live move/demolition acceptance.");
        var zoneCheck=PatchProcessor.GetOriginalInstructions(plugin.GetMethod("CheckZone",BindingFlags.NonPublic|BindingFlags.Instance));
        Assert(Count(zoneCheck,"WorldZone","GetTotalQuality")==1 && Count(zoneCheck,"WorldZone","GetZoneWGOs")==1,"Zone probe must use one real native total and native membership, never a fake-baseline total");
        Console.WriteLine("PASS compiled zone observer uses one actual native GetTotalQuality call and verified native member list; baseline is leaf-only.");
        PreviewTests.Run(assembly);LifecycleTests.Run(assembly);
        var purchase=assembly.GetType("NikichMods.BPSFaithProbe.FaithPurchase",true);
        var debitIL=PatchProcessor.GetOriginalInstructions(purchase.GetMethod("NativeDebit",BindingFlags.NonPublic|BindingFlags.Static));
        Assert(Count(debitIL,"Item","RemoveItem")==1,"Purchase does not call actual native single-resource RemoveItem");
        var countIL=PatchProcessor.GetOriginalInstructions(purchase.GetMethod("Count"));
        Assert(Count(countIL,"Item","GetTotalCount")==1 && Count(countIL,"Item","GetItemsCount")==0,"Payment count owner changed");
        Console.WriteLine("PASS compiled payment native adapter: Item.RemoveItem and player/bag GetTotalCount, no synthetic inventory replacement in the installed debit path.");
        PurchaseTests.Run(assembly);
        Console.WriteLine("LIMIT no Unity scene/GUI instantiation, live Harmony startup, Unity-native lifecycle/UI, payment, coverage or real-save acceptance. Headless native getter execution is reported separately above.");
    }
}
