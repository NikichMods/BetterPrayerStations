// SPDX-License-Identifier: MPL-2.0
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
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
}
