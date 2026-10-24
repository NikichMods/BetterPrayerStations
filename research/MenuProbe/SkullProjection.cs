// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
namespace NikichMods.BPSMenuProbe
{
    // The narrow skull-input seam previously exercised by FaithProbe; no decoration or item projection.
    internal static class SkullProjection
    {
        internal static readonly FieldInfo Grave=AccessTools.Field(typeof(GraveGUI),"_grave_wgo");
        static readonly MethodInfo Skulls=AccessTools.Method(typeof(Item),"GetBodySkulls",new[]{typeof(int).MakeByRefType(),typeof(int).MakeByRefType(),typeof(int).MakeByRefType(),typeof(bool)});
        internal static void Install(Harmony harmony)
        {
            if(Grave==null || Grave.FieldType!=typeof(WorldGameObject) || Skulls==null)throw new MissingMemberException("Grave input seam");
            harmony.Patch(AccessTools.PropertyGetter(typeof(WorldGameObject),"quality"),transpiler:new HarmonyMethod(typeof(SkullProjection),nameof(Quality)));
            harmony.Patch(AccessTools.Method(typeof(GraveGUI),"RedrawGraveInventory",Type.EmptyTypes),transpiler:new HarmonyMethod(typeof(SkullProjection),nameof(Diagram)));
        }
        static bool Calls(CodeInstruction i,MethodInfo m){return (i.opcode==OpCodes.Call || i.opcode==OpCodes.Callvirt) && Equals(i.operand,m);}
        internal static List<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> source,bool ui)
        {
            var input=new List<CodeInstruction>();foreach(var i in source)input.Add(new CodeInstruction(i));
            int skull=-1,count=0,parts=0,inventory=0;
            foreach(var i in input){if(Calls(i,Skulls)){skull=input.IndexOf(i);count++;}var m=i.operand as MethodInfo;if(m!=null && m.DeclaringType==typeof(GraveGUI) && m.Name=="DrawGravePart")parts++;if(m!=null && m.DeclaringType==typeof(Item) && m.Name=="GetInventoryQuality")inventory++;}
            if(count!=1 || (ui?parts!=2:inventory!=2))throw new InvalidOperationException("Native skull input structure changed");
            if(!ui && (skull+12>=input.Count || input[skull+1].opcode!=OpCodes.Ldarg_0 || input[skull+2].opcode!=OpCodes.Ldfld || !(input[skull+2].operand is FieldInfo) || ((FieldInfo)input[skull+2].operand).Name!="_data" || !Calls(input[skull+4],AccessTools.Method(typeof(Item),"GetInventoryQuality",new[]{typeof(string)})) || !Calls(input[skull+5],AccessTools.Method(typeof(Mathf),"Floor",new[]{typeof(float)})) || input[skull+8].opcode!=OpCodes.Sub || !Calls(input[skull+11],AccessTools.Method(typeof(Mathf),"Min",new[]{typeof(float),typeof(float)})) || input[skull+12].opcode!=OpCodes.Ret))throw new InvalidOperationException("Native grave cap structure changed");
            var call=input[skull];if(call.blocks.Count!=0)throw new InvalidOperationException("Unexpected skull exception boundary");
            var owner=new CodeInstruction(OpCodes.Ldarg_0);owner.labels.AddRange(call.labels);call.labels.Clear();
            call.opcode=OpCodes.Call;call.operand=AccessTools.Method(typeof(SkullProjection),ui?nameof(UISkulls):nameof(QualitySkulls));input.Insert(skull,owner);return input;
        }
        static IEnumerable<CodeInstruction> Quality(IEnumerable<CodeInstruction> instructions){return Rewrite(instructions,false);}
        static IEnumerable<CodeInstruction> Diagram(IEnumerable<CodeInstruction> instructions){return Rewrite(instructions,true);}
        internal static void Convert(ref int red,ref int white,ref int available,float durability)
        {
            if(red<=0)return;
            white=checked(white+red);red=0;
            int percent=Mathf.CeilToInt(durability*100f);if(percent>90)percent=100;
            available=Mathf.FloorToInt((float)(white*percent)/100f);
        }
        static void QualitySkulls(Item body,out int red,out int white,out int available,bool dontCountSelf,WorldGameObject grave)
        {body.GetBodySkulls(out red,out white,out available,dontCountSelf);if(!dontCountSelf)Plugin.Project(body,grave,ref red,ref white,ref available);}
        static void UISkulls(Item body,out int red,out int white,out int available,bool dontCountSelf,GraveGUI view)
        {body.GetBodySkulls(out red,out white,out available,dontCountSelf);if(!dontCountSelf)Plugin.Project(body,Grave.GetValue(view) as WorldGameObject,ref red,ref white,ref available);}
    }
}
