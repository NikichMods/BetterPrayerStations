// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using HarmonyLib;
class MenuTests
{
    static int assertions;
    static string managed,core;
    static void Assert(bool test,string message){assertions++;if(!test)throw new Exception(message);}
    static object Invoke(object state,string name,params object[] args)
    {return state.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).Invoke(state,args);}
    static int Value(object state,string name)
    {return (int)state.GetType().GetProperty(name,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).GetValue(state,null);}
    static bool Calls(MethodBase m,string type,string name)
    {foreach(var i in PatchProcessor.GetOriginalInstructions(m)){var call=i.operand as MethodBase;if(call!=null && call.DeclaringType.Name==type && call.Name==name)return true;}return false;}
    static void Main(string[] args)
    {
        managed=args[0];core=args[1];
        AppDomain.CurrentDomain.AssemblyResolve+=(s,e)=>{string file=new AssemblyName(e.Name).Name+".dll";foreach(var dir in new[]{managed,core}){string path=Path.Combine(dir,file);if(File.Exists(path))return Assembly.LoadFrom(path);}return null;};
        Run(args);
    }
    static void Run(string[] args)
    {
        string hash;using(var sha=SHA256.Create())using(var file=File.OpenRead(Path.Combine(managed,"Assembly-CSharp.dll")))hash=BitConverter.ToString(sha.ComputeHash(file)).Replace("-","").ToLowerInvariant();
        Assert(hash=="e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8","Host identity");
        var probe=Assembly.LoadFrom(args[2]);var type=probe.GetType("NikichMods.BPSMenuProbe.BeamState",true);var state=Activator.CreateInstance(type,true);
        EffectTests.Run(probe);
        SavedBeamTests.Run(probe);
        CapacityPurchaseTests.Run(probe);
        PresentationTests.Run(probe);
        var points=(IList)type.GetField("Points",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(state);
        Assert(Value(state,"Purchased")==0 && (int)Invoke(state,"Free",5)==0,"Initial zero paid capacity");
        int[] prices={1,2,3,4,6,9,12,15,18,21};
        for(int i=0;i<5;i++){Assert(Value(state,"NextPrice")==prices[i],"Trial price");Assert((bool)Invoke(state,"Buy",5),"Buy within support");}
        Assert(!(bool)Invoke(state,"Buy",5) && Value(state,"Purchased")==5,"Support cap refusal");
        for(int i=0;i<5;i++)Assert((bool)Invoke(state,"Place",i,0,5),"Place within paid capacity");
        Assert(!(bool)Invoke(state,"Place",0,0,10),"Duplicate/cap refusal");
        Assert((int)Invoke(state,"Active",2)==2 && points.Count==5 && Value(state,"Purchased")==5,"Support loss retains paid state and points");
        Assert((int)Invoke(state,"Active",0)==0 && (int)Invoke(state,"Free",0)==0,"No support dormant");
        Assert((int)Invoke(state,"Active",10)==5,"Free reactivation");
        long oldLast=(long)points[4].GetType().GetField("Order").GetValue(points[4]);
        Assert((bool)Invoke(state,"Remove",0,0) && Value(state,"Purchased")==5,"Removal preserves payment");
        Assert((bool)Invoke(state,"Place",0,0,5),"Free replacement");
        Assert((int)points[0].GetType().GetField("X").GetValue(points[0])==1,"Oldest surviving point first");
        Assert((long)points[4].GetType().GetField("Order").GetValue(points[4])>oldLast,"Replaced point newest");
        for(int i=5;i<10;i++){Assert(Value(state,"NextPrice")==prices[i],"Remaining adopted trial prices");Assert((bool)Invoke(state,"Buy",15),"Later buys");}
        Assert(!(bool)Invoke(state,"Place",0,0,15) && points.Count==5,"Duplicate refusal even with spare capacity");
        Assert(!(bool)Invoke(state,"Buy",15) && Value(state,"Purchased")==10,"Explicit diagnostic bound after known prices");
        Invoke(state,"Clear");Assert(points.Count==0 && Value(state,"Purchased")==0,"Cleanup");
        Assert(Calls(typeof(CraftItemGUI).GetMethod("CanCraft",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic),"CraftInterface","CanCraft"),"Actual native affordability dispatch");
        Assert(Calls(typeof(CraftItemGUI).GetMethod("OnCraftPressed",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic),"CraftInterface","OnCraft"),"Actual native click dispatch");
        var plugin=probe.GetType("NikichMods.BPSMenuProbe.Plugin",true);
        var fieldGeometry=probe.GetType("NikichMods.BPSMenuProbe.FieldGeometry",true);
        var nativeAudit=fieldGeometry.GetMethod("TryNativeMatches",BindingFlags.Instance|BindingFlags.NonPublic);
        Assert(nativeAudit.ReturnType==typeof(bool),"Unavailable physics must differ from a tested zero match");
        bool activeGuard=false,enabledGuard=false,boundsGuard=false;
        foreach(var instruction in PatchProcessor.GetOriginalInstructions(nativeAudit))
        {
            var call=instruction.operand as MethodBase;if(call==null)continue;
            if(call.Name=="get_activeInHierarchy")activeGuard=true;
            if(call.Name=="get_enabled")enabledGuard=true;
            if(call.Name=="get_bounds")boundsGuard=true;
            if(call.DeclaringType==typeof(BuildGrid) && (call.Name=="SkipCollider" || call.Name=="IsCellBusy"))
                Assert(activeGuard && enabledGuard && boundsGuard,"Physics/native active-only cache must not be used before readiness inspection");
        }
        Assert(activeGuard && enabledGuard && boundsGuard,"Audit checks hierarchy, collider and transformed physics bounds");
        Assert(Calls(typeof(ChunkedGameObject).GetMethod("UpdateVisibility",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic),"GameObject","SetActive"),"Native visibility legitimately removes distant owners from physics");
        var geometry=probe.GetType("NikichMods.BPSFaithProbe.GridFootprint",true);
        var globalGeometry=Activator.CreateInstance(fieldGeometry,true);
        var footprint=fieldGeometry.GetField("geometry",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(globalGeometry);
        var inactiveOption=geometry.GetField("includeInactivePlacement",BindingFlags.Instance|BindingFlags.NonPublic);
        Assert((bool)inactiveOption.GetValue(footprint),"Global field geometry must include hidden placement boxes");
        Assert(!(bool)inactiveOption.GetValue(Activator.CreateInstance(geometry,new object[]{false})),"Historical Faith geometry retains its native-filter default");
        Assert(!Calls(fieldGeometry.GetMethod("Matches",BindingFlags.Instance|BindingFlags.NonPublic),"FieldGeometry","TryNativeMatches"),"Functional field membership must not depend on physics visibility");
        foreach(var entry in new[]{new[]{"NikichMods.BPSMenuProbe.TestResources","GrantFaith"},new[]{"NikichMods.BPSFaithProbe.StationMaterials","Grant"}})
        {
            var setupType=probe.GetType(entry[0],true);var grant=setupType.GetMethod(entry[1],BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
            bool capacity=false,added=false;
            foreach(var instruction in PatchProcessor.GetOriginalInstructions(grant))
            {
                var call=instruction.operand as MethodBase;if(call==null || call.DeclaringType!=typeof(Item))continue;
                if(call.Name=="CanAddItems")capacity=true;
                if(call.Name=="AddItems"){Assert(capacity,"Setup native grant must follow capacity inspection");added=true;}
            }
            Assert(capacity && added,"Setup must use actual native capacity/add path");
        }
        var materials=probe.GetType("NikichMods.BPSFaithProbe.StationMaterials",true);
        Assert((string)materials.GetMethod("Recipe").Invoke(null,new object[]{false})=="graveyard_builddesk:p:pray_stand_wd_place" && (string)materials.GetMethod("Recipe").Invoke(null,new object[]{true})=="graveyard_builddesk:p:pray_stand_stn_place","Both native station recipe identities");
        Assert((int)probe.GetType("NikichMods.BPSMenuProbe.TestResources").GetField("FaithAmount",BindingFlags.Static|BindingFlags.NonPublic).GetRawConstantValue()==50,"Diagnostic faith button amount");
        Assert(Calls(plugin.GetMethod("CanCraft"),"CapacityPurchase","Count"),"Native menu affordability must inspect actual carried faith");
        Assert(Calls(plugin.GetMethod("OnCraft"),"CapacityPurchase","Buy") && !Calls(plugin.GetMethod("OnCraft"),"BeamState","Buy"),"Native menu purchase must use paid transaction rather than free state increment");
        var editorUpdate=plugin.GetMethod("EditorUpdate",BindingFlags.Instance|BindingFlags.NonPublic);
        Assert(!Calls(editorUpdate,"BuildGrid","MoveBuildGridTo") && !Calls(editorUpdate,"BuildGrid","RefreshGrid"),"Camera pan must not move or repaint the world-sampled grid");
        Assert(Calls(typeof(BuildGrid).GetMethod("RefreshGrid",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic),"Transform","get_position"),"Native grid painting samples its transform origin");
        Assert(!Calls(typeof(BuildGrid).GetMethod("MoveBuildGridTo"),"BuildGrid","RefreshGrid"),"Native grid movement does not repaint its texture");
        var refresh=plugin.GetMethod("RefreshMenu",BindingFlags.Instance|BindingFlags.NonPublic);
        bool redrawn=false,repositioned=false;
        foreach(var instruction in PatchProcessor.GetOriginalInstructions(refresh))
        {
            var call=instruction.operand as MethodBase;if(call==null)continue;
            if(call.DeclaringType==typeof(CraftItemGUI) && call.Name=="Redraw")redrawn=true;
            if(redrawn && call.DeclaringType==typeof(UITable) && call.Name=="Reposition")repositioned=true;
        }
        Assert(repositioned,"Reposition the outer native list after row heights have been redrawn");
        Assert(Calls(plugin.GetMethod("CaptureHeader",BindingFlags.Instance|BindingFlags.NonPublic),"UILabel","set_overflowMethod"),"Capture temporarily enables native text height growth");
        Assert(Calls(plugin.GetMethod("MenuHidden",BindingFlags.Instance|BindingFlags.NonPublic),"Plugin","RestoreHeader"),"Shared header settings restored at actual menu-hide callback");
        var card=(ObjectCraftDefinition)plugin.GetMethod("Card",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{"test_action","test_icon"});
        Assert(card.build_type==ObjectCraftDefinition.BuildType.None && !card.needs_unlock,"Virtual card does not resolve a physical output object");
        Assert(card.output.Count==0 && card.needs.Count==0 && card.disable_multi_craft,"Virtual card has no products/materials/batching");
        foreach(var ownerType in probe.GetTypes())
        foreach(var method in ownerType.GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly))
        {
            if(method.GetMethodBody()==null)continue;
            foreach(var instruction in PatchProcessor.GetOriginalInstructions(method))
            {
                var call=instruction.operand as MethodBase;if(call==null)continue;
                string owner=call.DeclaringType.Name,name=call.Name;
                bool ownedMetadata=ownerType.FullName=="NikichMods.BPSMenuProbe.SavedBeamState" && method.Name=="SetOwned" && name=="SetParam";
                bool paidDebit=ownerType.FullName=="NikichMods.BPSMenuProbe.CapacityPurchase" && method.Name=="NativeDebit" && name=="RemoveItem";
                bool setupGrant=(ownerType.FullName=="NikichMods.BPSMenuProbe.TestResources" && method.Name=="GrantFaith" || ownerType.FullName=="NikichMods.BPSFaithProbe.StationMaterials" && method.Name=="Grant") && name=="AddItems";
                Assert(!(owner=="Item" && ((name.StartsWith("RemoveItem") && !paidDebit) || (name.StartsWith("AddItem") && !setupGrant) || (name=="SetParam" && !ownedMetadata) || name=="SetParamAdd")),"Only owned metadata, native faith adapter and explicit setup grants allowed: "+method.Name);
                Assert(!(owner=="WorldGameObject" && (name=="SetParam" || name=="MarkForRemoval" || name=="SetObject")),"No world-data mutation: "+method.Name);
                Assert(!(owner=="FloatingWorldGameObject" && (name.StartsWith("Create") || name.StartsWith("Stop"))),"No floating object or commit: "+method.Name);
                Assert(!(owner=="BuildModeLogics" && (name=="DoPlace" || name=="DoRemove" || name=="CraftBuilding")),"No native physical edit: "+method.Name);
                Assert(!(owner=="GameSave" && name.StartsWith("Save")),"No save request");
            }
        }
        Console.WriteLine("PASS "+assertions+" assertions: state policy, native dispatch, narrow owned metadata/payment/setup adapters; no forced-save/world-write paths. Live UI/input/payment remains pending.");
    }
}
