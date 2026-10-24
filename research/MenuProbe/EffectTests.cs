// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using HarmonyLib;
using UnityEngine;
internal static class EffectTests
{
    static void Assert(bool value,string message){if(!value)throw new Exception(message);}
    static Item Body(int red,int white,float durability)
    {
        var item=(Item)FormatterServices.GetUninitializedObject(typeof(Item));
        var definition=(ItemDefinition)FormatterServices.GetUninitializedObject(typeof(ItemDefinition));
        definition.id="body";definition.q_minus=red;definition.q_plus=white;
        item.id="body";item.value=1;item.inventory=new List<Item>();item.secondary_inventory=new List<Item>();
        var flags=BindingFlags.Instance|BindingFlags.NonPublic;
        typeof(Item).GetField("_params",flags).SetValue(item,new GameRes());
        typeof(Item).GetField("_definition",flags).SetValue(item,definition);
        typeof(Item).GetField("_definition_set",flags).SetValue(item,true);
        typeof(Item).GetField("_definition_set_id",flags).SetValue(item,"body");item.durability=durability;return item;
    }
    static int Count(IEnumerable<CodeInstruction> list,Type owner,string name)
    {int n=0;foreach(var i in list){var m=i.operand as MethodInfo;if(m!=null && m.DeclaringType==owner && m.Name==name)n++;}return n;}
    internal static void Run(Assembly probe)
    {
        var type=probe.GetType("NikichMods.BPSMenuProbe.SkullProjection",true);
        var convert=type.GetMethod("Convert",BindingFlags.Static|BindingFlags.NonPublic);
        var rewrite=type.GetMethod("Rewrite",BindingFlags.Static|BindingFlags.NonPublic);
        var snapshot=probe.GetType("NikichMods.BPSMenuProbe.PhysicalSnapshot").GetMethod("Read",BindingFlags.Static|BindingFlags.NonPublic);
        int cases=0;
        foreach(float durability in new[]{1f,.97f,.9001f,.9f,.8999f,.8f,.01f,0f})
        foreach(var pair in new[]{new[]{3,3},new[]{0,6},new[]{7,0},new[]{1,11},new[]{-2,-1}})
        {
            var body=Body(pair[0],pair[1],durability);int r,w,a;body.GetBodySkulls(out r,out w,out a,false);
            var before=(string)snapshot.Invoke(null,new object[]{body});var args=new object[]{r,w,a,durability};convert.Invoke(null,args);
            var oracle=Body(0,r+w,durability);int er,ew,ea;oracle.GetBodySkulls(out er,out ew,out ea,false);
            Assert((int)args[0]==er && (int)args[1]==ew && (int)args[2]==ea,"Full conversion differs from native Item oracle");
            Assert(before==(string)snapshot.Invoke(null,new object[]{body}),"Conversion changed source parameters");
            body.GetBodySkulls(out er,out ew,out ea,false);Assert(er==r && ew==w && ea==a,"Raw body changed");
            var twice=new object[]{args[0],args[1],args[2],durability};convert.Invoke(null,twice);
            Assert(Equals(args[0],twice[0]) && Equals(args[1],twice[1]) && Equals(args[2],twice[2]),"Duplicate conversion stacks");cases++;
        }
        var quality=AccessTools.PropertyGetter(typeof(WorldGameObject),"quality");var ui=AccessTools.Method(typeof(GraveGUI),"RedrawGraveInventory",Type.EmptyTypes);
        foreach(var target in new[]{quality,ui})
        {
            bool isUI=target==ui;ILGenerator generator;var input=PatchProcessor.GetOriginalInstructions(target,out generator);
            var label=generator.DefineLabel();CodeInstruction original=null;foreach(var i in input)if(i.operand is MethodInfo && ((MethodInfo)i.operand).Name=="GetBodySkulls"){original=i;break;}
            original.labels.Add(label);
            var output=(List<CodeInstruction>)rewrite.Invoke(null,new object[]{input,isUI});
            Assert(Count(output,typeof(Item),"GetBodySkulls")==0,"Skull input was not replaced");
            Assert(Count(output,typeof(Item),"GetInventoryQuality")==Count(input,typeof(Item),"GetInventoryQuality"),"Decoration input changed");
            Assert(Count(output,typeof(Mathf),"Floor")==Count(input,typeof(Mathf),"Floor") && Count(output,typeof(Mathf),"Min")==Count(input,typeof(Mathf),"Min"),"Native caps changed");
            Assert(Count(output,typeof(GraveGUI),"DrawGravePart")==Count(input,typeof(GraveGUI),"DrawGravePart"),"Native decoration card rendering changed");
            bool ownerLabel=false;foreach(var i in output)if(i.labels.Contains(label))ownerLabel=i.opcode==OpCodes.Ldarg_0;
            Assert(ownerLabel && original.labels.Contains(label),"Owner branch labels skipped/lost");
            var broken=new List<CodeInstruction>(input);broken.Remove(original);bool rejected=false;
            try{rewrite.Invoke(null,new object[]{broken,isUI});}catch(TargetInvocationException ex){rejected=ex.InnerException is InvalidOperationException;}
            Assert(rejected,"Changed native input structure accepted");
        }
        var contains=probe.GetType("NikichMods.BPSMenuProbe.FieldGeometry").GetMethod("Contains",BindingFlags.Static|BindingFlags.NonPublic);
        // Independent schematic2x3 cell fixture, not a claimed native grave size.
        var rect=Rect.MinMaxRect(-16,-80,48,16);
        for(int x=-1;x<=2;x++)for(int y=-3;y<=1;y++)Assert((bool)contains.Invoke(null,new object[]{rect,x,y})==(x>=0 && x<=1 && y>=-2 && y<=0),"Interior/edge/adjacent cell membership mismatch");
        Console.WriteLine("PASS effect gates: "+cases+" actual native skull/decay oracle cases, source snapshots, idempotence, native input/cap/label guards and schematic interior/edge/outside cells.");
    }
}
