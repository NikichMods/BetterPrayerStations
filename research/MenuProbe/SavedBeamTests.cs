// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
internal static class SavedBeamTests
{
    const BindingFlags Flags=BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic;
    const string Prefix="nikichmods.bps.menu_probe.beams.";
    static Type stateType,backend;
    static int checks;
    static void Assert(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static object Call(object owner,string name,params object[] args){return owner.GetType().GetMethod(name,Flags).Invoke(owner,args);}
    static object Store(string name,params object[] args){return backend.GetMethod(name,Flags).Invoke(null,args);}
    static object Fresh(){return Activator.CreateInstance(stateType,true);}
    static int Purchased(object state){return (int)stateType.GetProperty("Purchased",Flags).GetValue(state,null);}
    static IList Points(object state){return (IList)stateType.GetField("Points",Flags).GetValue(state);}
    static string Signature(object state)
    {
        string value=Purchased(state)+"|";
        foreach(var p in Points(state))value+=p.GetType().GetField("X").GetValue(p)+","+p.GetType().GetField("Y").GetValue(p)+";";
        return value;
    }
    static Item Player()
    {
        var item=(Item)FormatterServices.GetUninitializedObject(typeof(Item));item.id="inventory";item.value=1;
        item.inventory=new List<Item>();item.secondary_inventory=new List<Item>();typeof(Item).GetField("_params",Flags).SetValue(item,new GameRes());
        return item;
    }
    static GameSave Save(Item player){var save=(GameSave)FormatterServices.GetUninitializedObject(typeof(GameSave));typeof(GameSave).GetField("_inventory",Flags).SetValue(save,player);save.day=27;return save;}
    static Item Owner(GameSave save){return (Item)typeof(GameSave).GetField("_inventory",Flags).GetValue(save);}
    static byte[] Serialize<T>(T value)
    {
        foreach(var m in typeof(SmartSerializer).GetMethods())if(m.Name=="Serialize" && m.IsGenericMethodDefinition && m.GetParameters().Length==1)return (byte[])m.MakeGenericMethod(typeof(T)).Invoke(null,new object[]{value});
        throw new MissingMethodException("Native serializer");
    }
    static T Roundtrip<T>(T value)
    {
        byte[] bytes=Serialize(value);
        foreach(var m in typeof(SmartSerializer).GetMethods())if(m.Name=="Deserialize" && m.IsGenericMethodDefinition && m.GetParameters().Length==1 && m.GetParameters()[0].ParameterType==typeof(byte[]))return (T)m.MakeGenericMethod(typeof(T)).Invoke(null,new object[]{bytes});
        throw new MissingMethodException("Native deserializer");
    }
    static void Reject(Action action,string message)
    {
        bool rejected=false;try{action();}catch(TargetInvocationException ex){rejected=ex.InnerException is InvalidOperationException;}Assert(rejected,message);
    }
    internal static void Run(Assembly probe)
    {
        stateType=probe.GetType("NikichMods.BPSMenuProbe.BeamState",true);backend=probe.GetType("NikichMods.BPSMenuProbe.SavedBeamState",true);
        var player=Player();player.SetParam("fixture.unrelated",42);player.money=12.5f;var child=Player();child.id="fixture_item";child.value=17;player.inventory.Add(child);
        var beforeChild=Convert.ToBase64String(Serialize(child));var state=Fresh();
        Assert(Purchased(Store("Read",player))==0,"Fresh player should have no saved investment");
        for(int i=0;i<3;i++)Assert((bool)Call(state,"Buy",5),"Fixture capacity buy");
        Call(state,"Place",0,-40,5);Call(state,"Place",68,0,5);Call(state,"Place",-10,-11,5);
        Call(state,"Remove",0,-40);Call(state,"Place",0,-40,5);
        Assert(Signature(state)=="3|68,0;-10,-11;0,-40;","Removal/replacement order fixture");
        Store("Write",player,state);player.RemoveZeroParams();var restoredSave=Roundtrip(Save(player));var restored=Owner(restoredSave);var recovered=Store("Read",restored);
        Assert(restoredSave.day==27 && !ReferenceEquals(restored,player),"Actual GameSave roundtrip independent owner");
        Assert(Signature(recovered)==Signature(state),"Purchased count, signed/zero coordinates and list priority lost");
        Assert((int)stateType.GetProperty("NextPrice",Flags).GetValue(recovered,null)==4,"Recovered purchase progression changed");
        Assert(restored.GetParam("fixture.unrelated",0)==42 && restored.money==12.5f && restored.inventory.Count==1 && Convert.ToBase64String(Serialize(restored.inventory[0]))==beforeChild,"Foreign metadata/money/inventory changed");
        for(int i=0;i<Points(recovered).Count;i++)Assert((long)Points(recovered)[i].GetType().GetField("Order").GetValue(Points(recovered)[i])==i+1,"Recovered rank not sequential");
        Assert((int)Call(recovered,"Active",0)==0 && (int)Call(recovered,"Active",1)==1 && Purchased(recovered)==3 && Points(recovered).Count==3,"Support loss erased/clamped investment or points");
        Assert((int)Call(recovered,"Active",10)==3,"Support restoration did not reactivate retained points");
        Call(recovered,"Remove",68,0);Call(recovered,"Place",68,0,5);Store("Write",restored,recovered);
        var second=Store("Read",Owner(Roundtrip(Save(restored))));Assert(Signature(second)=="3|-10,-11;0,-40;68,0;","Post-load placement did not become last");
        var other=Owner(Roundtrip(Save(Player())));Assert(Purchased(Store("Read",other))==0 && Signature(Store("Read",player))==Signature(state),"Global state leaked between saves");
        Call(state,"Clear");Assert(Purchased(Store("Read",player))==3,"Runtime-only clear erased native save carrier");
        // A native setter failure after partial progress must restore the previously readable payload.
        int writes=0;var next=Store("Read",restored);Call(next,"Buy",5);string old=Signature(Store("Read",restored));
        Func<string,float> read=key=>restored.GetParam(key,0);
        Action<string,float> failOnce=(key,value)=>{if(++writes==5)throw new IOException("Controlled setter failure");restored.SetParam(key,value);};
        bool failed=false;try{Store("Commit",read,failOnce,next);}catch(TargetInvocationException ex){failed=ex.InnerException is IOException;}
        Assert(failed && Signature(Store("Read",restored))==old && restored.GetParam("fixture.unrelated",0)==42,"Partial write rollback lost state or changed foreign values");
        var bads=new List<Action<Item>>{
            p=>p.SetParam(Prefix+"schema",2),p=>p.SetParam(Prefix+"purchased",1.5f),p=>p.SetParam(Prefix+"purchased",float.NaN),
            p=>p.SetParam(Prefix+"purchased",11),p=>p.SetParam(Prefix+"count",4),p=>p.SetParam(Prefix+"x0",float.PositiveInfinity),
            p=>p.SetParam(Prefix+"x0",1000001),p=>p.SetParam(Prefix+"x0",.25f),
            p=>{p.SetParam(Prefix+"x1",p.GetParam(Prefix+"x0",0));p.SetParam(Prefix+"y1",p.GetParam(Prefix+"y0",0));},
            p=>p.SetParam(Prefix+"schema",0),p=>p.SetParam(Prefix+"x9",1)
        };
        foreach(var corrupt in bads)
        {
            var bad=Roundtrip(restored);corrupt(bad);string bytes=Convert.ToBase64String(Serialize(bad));
            Reject(()=>Store("Read",bad),"Malformed/unknown saved data accepted");
            Reject(()=>Store("Write",bad,next),"Malformed data was overwritten");
            Assert(bytes==Convert.ToBase64String(Serialize(bad)),"Rejection changed stored data");
        }
        Reject(()=>backend.GetMethod("SetOwned",Flags).Invoke(null,new object[]{restored,"fixture.unrelated",0f}),"Foreign parameter write allowed");
        var invalid=Store("Read",restored);Points(invalid).Add(Points(invalid)[0]);string validBytes=Convert.ToBase64String(Serialize(restored));
        Reject(()=>Store("Write",restored,invalid),"Invalid proposed state was committed");
        Assert(validBytes==Convert.ToBase64String(Serialize(restored)),"Proposed-state rejection changed saved owner");
        Store("Write",restored,Fresh());restored.RemoveZeroParams();var clean=Owner(Roundtrip(Save(restored)));
        Assert(Purchased(Store("Read",clean))==0 && Points(Store("Read",clean)).Count==0 && clean.GetParam("fixture.unrelated",0)==42 && clean.money==12.5f,"Cleanup did not survive native save or touched unrelated state");
        Assert(Signature(Store("Read",player))=="3|68,0;-10,-11;0,-40;","Cleanup/another owner changed original save");
        var reset=probe.GetType("NikichMods.BPSMenuProbe.Plugin").GetMethod("ResetSession",Flags);
        foreach(var i in PatchProcessor.GetOriginalInstructions(reset)){var m=i.operand as MethodBase;Assert(m==null || (m.Name!="SaveState" && m.DeclaringType!=backend),"Session transition writes old save state");}
        var inventory=typeof(GameSave).GetField("_inventory",Flags);Assert(inventory.IsDefined(typeof(UnityEngine.SerializeField),false),"Native player carrier is not serialized");
        Console.WriteLine("PASS "+checks+" persistence checks: actual GameSave roundtrip, priority/reinsert, zero pruning, support loss/recovery, cross-save graph isolation, unrelated data, cleanup, malformed-data preservation and controlled partial-write rollback. Live load/restart remains pending.");
    }
}
