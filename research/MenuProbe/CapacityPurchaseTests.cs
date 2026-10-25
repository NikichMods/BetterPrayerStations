// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
internal static class CapacityPurchaseTests
{
    const BindingFlags Flags=BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic;
    static Type serviceType,stateType,store;static int checks,calls;
    static readonly Dictionary<string,ItemDefinition> definitions=new Dictionary<string,ItemDefinition>();
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static object Call(object target,string name,params object[] args){return target.GetType().GetMethod(name,Flags).Invoke(target,args);}
    static object Fresh(){return Activator.CreateInstance(stateType,true);}
    static int P(object state){return (int)stateType.GetProperty("Purchased",Flags).GetValue(state,null);}
    static int Price(object state){return (int)stateType.GetProperty("NextPrice",Flags).GetValue(state,null);}
    static object Read(Item payer){return store.GetMethod("Read",Flags).Invoke(null,new object[]{payer});}
    static void Write(Item payer,object state){store.GetMethod("Write",Flags).Invoke(null,new object[]{payer,state});}
    static string Signature(object state)
    {
        string value=P(state)+"|";foreach(var pt in (IList)stateType.GetField("Points",Flags).GetValue(state))value+=pt.GetType().GetField("X").GetValue(pt)+","+pt.GetType().GetField("Y").GetValue(pt)+";";return value;
    }
    static Item Item(string id,int count=1,bool bag=false)
    {
        if(!definitions.ContainsKey(id)){var d=(ItemDefinition)FormatterServices.GetUninitializedObject(typeof(ItemDefinition));d.id=id;d.type=bag?ItemDefinition.ItemType.Bag:ItemDefinition.ItemType.None;d.stack_count=1000;definitions.Add(id,d);}
        var item=(Item)FormatterServices.GetUninitializedObject(typeof(Item));item.id=id;item.value=count;item.inventory=new List<Item>();item.secondary_inventory=new List<Item>();
        typeof(Item).GetField("_params",Flags).SetValue(item,new GameRes());Bind(item);return item;
    }
    static void Bind(Item item)
    {
        typeof(Item).GetField("_definition",Flags).SetValue(item,definitions[item.id]);typeof(Item).GetField("_definition_set",Flags).SetValue(item,true);typeof(Item).GetField("_definition_set_id",Flags).SetValue(item,item.id);
        foreach(var child in item.inventory)Bind(child);foreach(var child in item.secondary_inventory)Bind(child);
    }
    static Item Payer(int root,int bag)
    {
        var payer=Item("payer_fixture");payer.money=12.5f;payer.SetParam("fixture.foreign",7);
        if(root>0)payer.inventory.Add(Item("faith",root));payer.inventory.Add(Item("foreign_fixture",7));
        var pouch=Item("pouch_fixture",1,true);if(bag>0)pouch.inventory.Add(Item("faith",bag));payer.inventory.Add(pouch);payer.secondary_inventory.Add(Item("faith",99));return payer;
    }
    static int Faith(Item payer){return payer.GetTotalCount("faith",true);}
    static object Service(Func<Item,int,bool> debit,Action<string> phase=null){return Activator.CreateInstance(serviceType,Flags,null,new object[]{debit,phase},null);}
    static bool Debit(Item payer,int amount)
    {
        calls++;int left=amount;var owners=new List<Item>{payer};foreach(var item in payer.inventory)if(item.is_bag)owners.Add(item);
        foreach(var owner in owners)for(int i=owner.inventory.Count-1;i>=0 && left>0;i--){var item=owner.inventory[i];if(item.id!="faith")continue;int take=Math.Min(left,item.value);item.value-=take;left-=take;if(item.value==0)owner.inventory.RemoveAt(i);}return left==0;
    }
    static object Buy(object service,Item payer,object state,int support=10,int? quoteP=null,int? cost=null){return Call(service,"Buy",payer,state,support,quoteP??P(state),cost??Price(state));}
    static string Status(object result){return (string)result.GetType().GetField("Status",Flags).GetValue(result);}
    static byte[] Bytes<T>(T value) where T:class {return SmartSerializer.Serialize<T>(value);}
    static string Inventory(Item p){return Convert.ToBase64String(Bytes(p.inventory[0]))+"|"+Faith(p)+"|"+p.inventory.Count+"|"+p.money;}
    static Item Reload(Item payer)
    {
        var save=(GameSave)FormatterServices.GetUninitializedObject(typeof(GameSave));typeof(GameSave).GetField("_inventory",Flags).SetValue(save,payer);
        var restored=SmartSerializer.Deserialize<GameSave>(Bytes(save));var owner=(Item)typeof(GameSave).GetField("_inventory",Flags).GetValue(restored);Bind(owner);return owner;
    }
    internal static void Run(Assembly assembly)
    {
        serviceType=assembly.GetType("NikichMods.BPSMenuProbe.CapacityPurchase",true);stateType=assembly.GetType("NikichMods.BPSMenuProbe.BeamState",true);store=assembly.GetType("NikichMods.BPSMenuProbe.SavedBeamState",true);
        var payer=Payer(50,51);var state=Fresh();var service=Service(Debit);int[] prices={1,2,3,4,6,9,12,15,18,21};int remaining=101;
        for(int i=0;i<prices.Length;i++)
        {
            int before=Faith(payer);Check(Price(state)==prices[i],"Adopted global price differs");Check(Status(Buy(service,payer,state))=="committed","Purchase rejected");remaining-=prices[i];
            Check(Faith(payer)==before-prices[i] && Faith(payer)==remaining && P(state)==i+1 && P(Read(payer))==i+1,"Exact faith/P commit");
            if(i==0)Call(state,"Place",68,-49,10);
            // Normal placement persists independently, as in the real editor.
            Write(payer,state);payer=Reload(payer);state=Read(payer);Check(P(state)==i+1 && Faith(payer)==remaining,"Actual GameSave payment/ledger recovery");
            Check(payer.money==12.5f && payer.GetParam("fixture.foreign",0)==7 && payer.secondary_inventory[0].value==99,"Unrelated money/metadata/secondary changed");
        }
        int debitCalls=calls;Check(Status(Buy(service,payer,state))=="limit" && calls==debitCalls,"Ten-price bound debits");
        var exact=Payer(0,1);var one=Fresh();Check(Status(Buy(service,exact,one))=="committed" && Faith(exact)==0,"Exact bag-only funds");
        foreach(int amount in new[]{0,1})
        {
            var poor=Payer(amount,0);var two=Fresh();Call(two,"Buy",10);Write(poor,two);string before=Convert.ToBase64String(Bytes(poor));debitCalls=calls;
            Check(Status(Buy(service,poor,two))=="insufficient" && calls==debitCalls && before==Convert.ToBase64String(Bytes(poor)) && P(two)==1,"Shortage mutated ledger/inventory");
        }
        var rich=Payer(100,0);var current=Fresh();Write(rich,current);debitCalls=calls;
        Check(Status(Buy(service,rich,current,0))=="limit" && calls==debitCalls,"Zero support debits");
        Check(Status(Buy(service,rich,current,10,1,2))=="stale-quote" && calls==debitCalls,"Stale count debits");
        Check(Status(Buy(service,rich,current,10,0,2))=="stale-quote" && calls==debitCalls,"Wrong price debits");
        Call(current,"Buy",10);Check(Status(Buy(service,rich,current))=="stale-quote" && calls==debitCalls,"Saved/local mismatch debits");
        int faults=0;
        foreach(string phase in new[]{"before-debit","after-debit","after-write","after-state"})
        {
            var p=Payer(1,20);var s=Fresh();Call(s,"Buy",10);Call(s,"Place",68,-49,10);Write(p,s);string signature=Signature(s),bytes=Convert.ToBase64String(Bytes(p));
            var root=p.inventory[0];var pouch=p.inventory[2];
            try{Buy(Service(Debit,at=>{if(at==phase)throw new Exception("Injected "+phase);}),p,s);throw new Exception("No fault");}
            catch(TargetInvocationException ex){Check(ex.InnerException.Message.Contains("rollback=verified"),"Rollback unverified "+phase);faults++;}
            Check(bytes==Convert.ToBase64String(Bytes(p)) && Signature(s)==signature && Signature(Read(p))==signature && ReferenceEquals(root,p.inventory[0]) && ReferenceEquals(pouch,p.inventory[2]),"Fault did not restore original payer/points/ledger");
        }
        foreach(Func<Item,int,bool> bad in new Func<Item,int,bool>[] {(p,n)=>false,(p,n)=>{Debit(p,n);return false;},(p,n)=>Debit(p,n+1),(p,n)=>{Debit(p,n);throw new Exception("Native fault");}})
        {
            var p=Payer(1,20);var s=Fresh();Write(p,s);string signature=Signature(s),inventory=Inventory(p);
            try{Buy(Service(bad),p,s);throw new Exception("Bad debit accepted");}catch(TargetInvocationException ex){Check(ex.InnerException.Message.Contains("rollback=verified"),"Bad debit not contained");faults++;}
            Check(Inventory(p)==inventory && Signature(s)==signature && Signature(Read(p))==signature,"Bad debit state leak");
        }
        object reentrant=null;var rp=Payer(3,0);var rs=Fresh();bool blocked=false;
        reentrant=Service((p,n)=>{blocked=Status(Buy(reentrant,p,rs))=="busy";return Debit(p,n);});
        Check(Status(Buy(reentrant,rp,rs))=="committed" && blocked && P(rs)==1 && Faith(rp)==2,"Reentry duplicate commit");
        var adapter=serviceType.GetMethod("NativeDebit",Flags);bool native=false;foreach(var i in PatchProcessor.GetOriginalInstructions(adapter)){var m=i.operand as MethodInfo;if(m!=null && m.DeclaringType==typeof(Item) && m.Name=="RemoveItem")native=true;}Check(native,"Native adapter bypassed");
        Check(faults==8,"Fault coverage");
        Console.WriteLine("PASS "+checks+" global payment checks: ten exact adopted prices/actual GameSave reload, exact bag/zero/shortage, support and trial bounds, stale/local mismatch, reentry, eight rollback faults with inventory refs/points/ledger. Debit is simulated; native adapter IL checked, live payment remains pending.");
    }
}
