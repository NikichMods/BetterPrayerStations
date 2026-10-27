// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

static class PresentationTests
{
    static int checks;
    static void Assert(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static bool Calls(MethodInfo method,string type,string name)
    {
        foreach(var instruction in PatchProcessor.GetOriginalInstructions(method))
        {
            var call=instruction.operand as MethodBase;
            if(call!=null && call.DeclaringType.Name==type && call.Name==name)return true;
        }
        return false;
    }
    static MethodInfo Method(Type type,string name){return type.GetMethod(name,BindingFlags.Instance|BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public);}
    static int Int32BigEndian(byte[] bytes,int at){return bytes[at]<<24|bytes[at+1]<<16|bytes[at+2]<<8|bytes[at+3];}
    internal static void Run(Assembly probe)
    {
        string[] names={"BPS.Prayer.png","BPS.Place.png","BPS.Remove.png"};
        string[] hashes={"0f9324e6b6c40607856e922b750963675723f0551ad9337a2706f37a106e06c7","cdb90b9719f3efce7656b1b8403c4033eb407cf76dafa509b7a23919ff0adfbb","38c0dd4ec6a76170460e48227a60bb6a39631f6fd7db43b4276ba6401b46be4d"};
        for(int i=0;i<names.Length;i++)
        {
            using(var stream=probe.GetManifestResourceStream(names[i]))
            {
                Assert(stream!=null,"Selected icon must be embedded "+names[i]);
                byte[] bytes;using(var buffer=new MemoryStream()){stream.CopyTo(buffer);bytes=buffer.ToArray();}
                using(var sha=SHA256.Create())Assert(BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant()==hashes[i],"Embedded bytes match accepted artwork");
                Assert(bytes[0]==137 && bytes[1]==80 && bytes[2]==78 && bytes[3]==71,"Actual PNG payload");
                int expected=i==0?100:37;
                Assert(Int32BigEndian(bytes,16)==expected && Int32BigEndian(bytes,20)==expected,"Actual raster dimensions");
                Assert(bytes[25]==6,"Transparent RGBA PNG");
            }
        }
        var plugin=probe.GetType("NikichMods.BPSMenuProbe.Plugin",true);
        var view=probe.GetType("NikichMods.BPSMenuProbe.StationMenuView",true);
        var trace=probe.GetType("NikichMods.BPSMenuProbe.MenuLayoutTrace",true);
        CheckConfinement(plugin);
        foreach(var method in trace.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.DeclaredOnly))
        foreach(var instruction in PatchProcessor.GetOriginalInstructions(method))
        {
            var called=instruction.operand as MethodBase;
            Assert(called==null || !called.Name.StartsWith("set_"),"Read-only layout trace must never move, resize or re-anchor native objects");
            Assert(instruction.opcode!=System.Reflection.Emit.OpCodes.Stfld && instruction.opcode!=System.Reflection.Emit.OpCodes.Stsfld,"Layout trace must not write native or cached state");
        }
        var owner=FormatterServices.GetUninitializedObject(plugin);
        var stateType=probe.GetType("NikichMods.BPSMenuProbe.BeamState",true);
        var state=Activator.CreateInstance(stateType,true);
        plugin.GetField("state",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(owner,state);
        var card=(ObjectCraftDefinition)Method(plugin,"Card").Invoke(null,new object[]{"quote","icon_faith"});
        // Native Item construction initializes balance/bag metadata through Unity.
        // Seed the UI-only ingredient fixture; exercise the real repeated quote path.
        var quote=(Item)FormatterServices.GetUninitializedObject(typeof(Item));quote.id="faith";quote.value=1;card.needs.Add(quote);
        plugin.GetField("buyCard",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(owner,card);
        Item first=null;
        int[] prices={1,2,3,4,6,9,12,15,18,21,0};
        for(int i=0;i<prices.Length;i++)
        {
            Method(plugin,"UpdateQuote").Invoke(owner,null);
            Assert(card.needs.Count==1 && card.needs[0].id=="faith" && card.needs[0].value==prices[i],"Actual quote stays aligned with paid state");
            if(first==null)first=card.needs[0];else Assert(ReferenceEquals(first,card.needs[0]),"Redraw retains ingredient identity and slot count");
            Assert(card.output.Count==0,"Quote does not add craft output");
            if(i<10)Method(stateType,"Buy").Invoke(state,new object[]{15});
        }
        int referenceBranches=0;
        foreach(var instruction in PatchProcessor.GetOriginalInstructions(Method(plugin,"OwnedRow")))
            if(instruction.opcode==System.Reflection.Emit.OpCodes.Beq || instruction.opcode==System.Reflection.Emit.OpCodes.Beq_S)referenceBranches++;
        Assert(Calls(Method(plugin,"OwnedRow"),"Plugin","Current") && referenceBranches==3,"Presentation requires current session and three exact own-card reference branches");
        Assert(Calls(Method(plugin,"RowBefore"),"StationMenuView","BeforeRedraw") && Calls(Method(plugin,"RowAfter"),"StationMenuView","AfterRedraw"),"Native row patch delegates owned before/after presentation");
        Assert(Calls(Method(plugin,"RowAfter"),"CapacityPurchase","Count"),"Faith presentation uses same actual payer count as debit");
        Assert(Calls(Method(plugin,"MenuHidden"),"StationMenuView","Restore"),"Close restores pooled rows");
        Assert(Calls(Method(view,"BeforeRedraw"),"RowSnapshot","ResetGeometry"),"Each render resets geometry before native sizing");
        Assert(Calls(Method(view,"AfterRedraw"),"UI2DSprite","set_sprite2D"),"Custom sprites use native icon element");
        var nativeClick=typeof(CraftItemGUI).GetMethod("OnCraftPressed",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
        var il=PatchProcessor.GetOriginalInstructions(nativeClick);
        bool checkedDispatch=false;
        for(int i=0;i<il.Count-2;i++)
        {
            var call=il[i].operand as MethodBase;
            if(call==null || call.DeclaringType!=typeof(CraftInterface) || call.Name!="OnCraft")continue;
            if(il[i+2].opcode.FlowControl==System.Reflection.Emit.FlowControl.Branch){checkedDispatch=true;break;}
        }
        Assert(checkedDispatch,"Native custom action skips ordinary crafting branch; no second resource debit");
        Console.WriteLine("PASS "+checks+" presentation checks: selected embedded PNG bytes/dimensions, quote slot stability, scoped native final writers, payer count and restore routes. NGUI layout/readability require live acceptance.");
    }
    static void CheckConfinement(Type plugin)
    {
        var original=typeof(UIScrollView).GetMethod("RestrictWithinBounds",new[]{typeof(bool)});
        var axis=typeof(UIScrollView).GetMethod("RestrictWithinBounds",new[]{typeof(bool),typeof(bool),typeof(bool)});
        var native=PatchProcessor.GetOriginalInstructions(Method(typeof(CraftGUI),"LateUpdate"));
        var rewrite=Method(plugin,"ConfinementAxis");
        var wrapper=Method(plugin,"RestrictStationScroll");
        var rewritten=new List<CodeInstruction>((IEnumerable<CodeInstruction>)rewrite.Invoke(null,new object[]{native}));
        Assert(native.Count==rewritten.Count,"Confinement patch changes one call, not native control flow");
        int replacements=0;
        for(int i=0;i<native.Count;i++)
        {
            bool replaced=Equals(native[i].operand,original);
            if(replaced)replacements++;
            Assert(rewritten[i].opcode==(replaced?OpCodes.Call:native[i].opcode) && Equals(rewritten[i].operand,replaced?wrapper:native[i].operand),"Actual native instructions preserved outside one confinement dispatch");
            Assert(native[i].labels.Count==rewritten[i].labels.Count && native[i].blocks.Count==rewritten[i].blocks.Count,"Native control-flow labels and exception regions retained");
        }
        Assert(replacements==1,"Exact native confinement seam found once");
        var missing=new List<CodeInstruction>();
        foreach(var instruction in native)if(!Equals(instruction.operand,original))missing.Add(instruction);
        var duplicate=new List<CodeInstruction>(native);duplicate.Add(new CodeInstruction(OpCodes.Callvirt,original));
        foreach(var changed in new[]{missing,duplicate})
        {
            bool refused=false;
            try{rewrite.Invoke(null,new object[]{changed});}
            catch(TargetInvocationException ex){refused=ex.InnerException is InvalidOperationException;}
            Assert(refused,"Missing/ambiguous confinement seam must refuse patching");
        }
        var wrapperIL=PatchProcessor.GetOriginalInstructions(wrapper);
        bool verticalOnly=false,originalFallback=false;
        for(int i=0;i<wrapperIL.Count;i++)
        {
            if(Equals(wrapperIL[i].operand,axis))verticalOnly=i>=4 && wrapperIL[i-4].opcode==OpCodes.Ldarg_0 && wrapperIL[i-3].opcode==OpCodes.Ldarg_1 && wrapperIL[i-2].opcode==OpCodes.Ldc_I4_0 && wrapperIL[i-1].opcode==OpCodes.Ldc_I4_1;
            if(Equals(wrapperIL[i].operand,original))originalFallback=true;
        }
        Assert(verticalOnly && originalFallback,"Owned path restricts Y only; ordinary path retains original native overload");
        Assert(Calls(wrapper,"Plugin","Current") && Calls(wrapper,"BaseGUI","get_is_shown"),"Correction requires live current save and visible owned menu");
        var overload=PatchProcessor.GetOriginalInstructions(original);
        bool nativeBoth=false;
        for(int i=2;i<overload.Count;i++)if(Equals(overload[i].operand,axis))nativeBoth=overload[i-2].opcode==OpCodes.Ldc_I4_1 && overload[i-1].opcode==OpCodes.Ldc_I4_1;
        Assert(nativeBoth,"Pinned native single-argument overload really enables both axes");
    }
}
