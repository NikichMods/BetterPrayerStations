// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
static class PurchaseTests
{
    static Type backend,state;
    static readonly BindingFlags Fields=BindingFlags.Instance|BindingFlags.NonPublic;
    static readonly Dictionary<string,ItemDefinition> definitions=new Dictionary<string,ItemDefinition>();
    static int calls;
    static void Check(bool value,string text){if(!value)throw new Exception(text);}
    static T Bare<T>(){return (T)FormatterServices.GetUninitializedObject(typeof(T));}
    static void Bind(Item item)
    {
        typeof(Item).GetField("_definition",Fields).SetValue(item,definitions[item.id]);
        typeof(Item).GetField("_definition_set",Fields).SetValue(item,true);
        typeof(Item).GetField("_definition_set_id",Fields).SetValue(item,item.id);
        foreach(var child in item.inventory)Bind(child);foreach(var child in item.secondary_inventory)Bind(child);
    }
    static Item New(string id,int count=1,bool bag=false)
    {
        if(!definitions.ContainsKey(id)){var d=Bare<ItemDefinition>();d.id=id;d.type=bag?ItemDefinition.ItemType.Bag:ItemDefinition.ItemType.None;d.stack_count=100;definitions.Add(id,d);}
        var item=Bare<Item>();item.id=id;item.value=count;item.inventory=new List<Item>();item.secondary_inventory=new List<Item>();
        typeof(Item).GetField("_params",Fields).SetValue(item,new GameRes());Bind(item);return item;
    }
    static Item Payer(int root,int bag)
    {
        var p=New("payer_fixture");if(root>0)p.inventory.Add(New("faith",root));p.inventory.Add(New("unrelated",7));
        var b=New("bag_fixture",1,true);if(bag>0)b.inventory.Add(New("faith",bag));p.inventory.Add(b);p.secondary_inventory.Add(New("faith",99));return p;
    }
    static int Faith(Item p){return (int)backend.GetMethod("Count").Invoke(null,new object[]{p});}
    static int Field(object result,string name){return (int)result.GetType().GetField(name).GetValue(result);}
    static string Status(object result){return (string)result.GetType().GetField("Status").GetValue(result);}
    static int Actions(Item g){var value=state.GetMethod("Read").Invoke(null,new object[]{g});return (int)value.GetType().GetProperty("Actions").GetValue(value,null);}
    static int Price(int actions){return (int)backend.GetMethod("Price").Invoke(null,new object[]{actions});}
    static object Service(Func<Item,int,bool> debit,Action<string> checkpoint=null){return Activator.CreateInstance(backend,new object[]{debit,checkpoint});}
    static object Buy(object service,Item g,Item p,bool c,int actions,int cost,bool shortage=false)
    {return backend.GetMethod("Buy").Invoke(service,new object[]{g,p,c,actions,cost,shortage});}
    // Deliberately simulated debit. Tests transaction boundaries, NOT native RemoveItem.
    static bool Debit(Item payer,int cost)
    {
        calls++;int left=cost;var owners=new List<Item>{payer};foreach(var item in payer.inventory)if(item.is_bag)owners.Add(item);
        foreach(var owner in owners)for(int i=owner.inventory.Count-1;i>=0 && left>0;i--)
        {var item=owner.inventory[i];if(item.id!="faith")continue;int take=Math.Min(left,item.value);item.value-=take;left-=take;if(item.value==0)owner.inventory.RemoveAt(i);}
        return left==0;
    }
    static byte[] Bytes<T>(T value) where T:class {return SmartSerializer.Serialize<T>(value);}
    static bool Same(byte[] a,byte[] b){if(a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
    static void Roundtrip(ref Item g,ref Item p)
    {
        var map=new SerializableGameMap();var list=(List<SerializableWGO>)typeof(SerializableGameMap).GetField("_wgos",Fields).GetValue(map);
        list.Add(new SerializableWGO{unique_id=101,obj_id=g.id,item=g});list.Add(new SerializableWGO{unique_id=202,obj_id=p.id,item=p});
        var restored=SmartSerializer.Deserialize<SerializableGameMap>(Bytes(map));
        var records=(List<SerializableWGO>)typeof(SerializableGameMap).GetField("_wgos",Fields).GetValue(restored);
        Check(records[0].unique_id==101 && records[1].unique_id==202,"Serialization owner identity changed");g=records[0].item;p=records[1].item;Bind(g);Bind(p);
    }
    public static void Run(Assembly assembly)
    {
        backend=assembly.GetType("NikichMods.BPSFaithProbe.FaithPurchase",true);state=assembly.GetType("NikichMods.BPSFaithProbe.SavedFaithState",true);
        var g=New("grave_fixture");g.SetParam("unrelated.owner",7);var p=Payer(2,8);var service=Service(Debit);
        var r=Buy(service,g,p,true,0,1);Check(Status(r)=="committed" && Faith(p)==9 && Actions(g)==1 && Field(r,"FaithBefore")==10 && Field(r,"FaithAfter")==9,"First purchase");
        r=Buy(service,g,p,false,1,2);Check(Status(r)=="committed" && Faith(p)==7 && Actions(g)==2 && Price(Actions(g))==3,"Shared action quote");
        Roundtrip(ref g,ref p);Check(Faith(p)==7 && Actions(g)==2 && Price(Actions(g))==3,"Saved pair state/quote");
        r=Buy(service,g,p,true,2,3);Check(Status(r)=="committed" && Faith(p)==4 && Actions(g)==3,"Purchase after roundtrip");
        Check(g.GetParam("unrelated.owner",0)==7 && p.secondary_inventory[0].value==99 && p.inventory.Find(x=>x.id=="unrelated").value==7,"Unrelated or secondary inventory touched");
        byte[] inventory=Bytes(p);int beforeCalls=calls;
        r=Buy(service,g,p,false,3,5,true);Check(Status(r)=="insufficient" && calls==beforeCalls && Actions(g)==3 && Same(inventory,Bytes(p)),"Shortage branch changed state");
        r=Buy(service,g,p,false,2,3);Check(Status(r)=="stale-quote" && calls==beforeCalls,"Stale action quote accepted");
        r=Buy(service,g,p,false,3,1);Check(Status(r)=="stale-quote" && calls==beforeCalls,"Wrong cost accepted");
        var exactPayer=Payer(0,1);var exactGrave=New("grave_fixture");r=Buy(service,exactGrave,exactPayer,true,0,1);Check(Status(r)=="committed" && Faith(exactPayer)==0,"Exact funds/bag-only debit");
        var empty=Payer(0,0);beforeCalls=calls;r=Buy(service,New("grave_fixture"),empty,false,0,1);Check(Status(r)=="insufficient" && calls==beforeCalls,"Actual zero funds accepted");
        int failures=0;
        foreach(string phase in new[]{"before-debit","after-debit","after-state"})
        {
            var fg=New("grave_fixture");var fp=Payer(1,2);byte[] initial=Bytes(fp);var fs=Service(Debit,at=>{if(at==phase)throw new Exception("Injected "+phase);});
            try{Buy(fs,fg,fp,true,0,1);throw new Exception("Failure not raised");}catch(TargetInvocationException ex){Check(ex.InnerException.Message.Contains("rollback=verified"),"Rollback not verified: "+phase);failures++;}
            Check(Faith(fp)==3 && Actions(fg)==0 && Same(initial,Bytes(fp)),"Fault changed inventory/action state");
        }
        foreach(Func<Item,int,bool> badDebit in new Func<Item,int,bool>[] { (owner,cost)=>false,(owner,cost)=>{Debit(owner,cost);return false;},(owner,cost)=>Debit(owner,cost+1),(owner,cost)=>{Debit(owner,cost);throw new Exception("Debit fault");}})
        {
            var fg=New("grave_fixture");var fp=Payer(1,2);byte[] initial=Bytes(fp);
            try{Buy(Service(badDebit),fg,fp,true,0,1);throw new Exception("Bad debit accepted");}catch(TargetInvocationException ex){Check(ex.InnerException.Message.Contains("rollback=verified"),"Bad debit rollback");failures++;}
            Check(Faith(fp)==3 && Actions(fg)==0 && Same(initial,Bytes(fp)),"Partial/wrong debit not restored");
        }
        object reentrant=null;var rg=New("grave_fixture");var rp=Payer(3,0);bool blocked=false;
        reentrant=Service((owner,cost)=>{blocked=Status(Buy(reentrant,rg,rp,false,0,1))=="busy";return Debit(owner,cost);});
        r=Buy(reentrant,rg,rp,true,0,1);Check(blocked && Status(r)=="committed" && Actions(rg)==1 && Faith(rp)==2,"Reentrant duplicate purchase");
        state.GetMethod("Write").Invoke(null,new object[]{rg,12,0});beforeCalls=calls;r=Buy(service,rg,rp,true,12,13);Check(Status(r)=="limit" && calls==beforeCalls,"Bounded action debited");
        Console.WriteLine("PASS purchase transaction: exact/bag/zero funds, shared conversion/wreath quote, stale quote, insufficient without debit, reentry guard, count bound, seven injected rollback classes, original inventory references/order/amounts and native map serialization of synthetic payer/grave pair.");
        Check(failures==7,"Fault test coverage");
        Console.WriteLine("LIMIT purchase tests use simulated debit with actual native counting/serializer; actual Item.RemoveItem, live payer/save/UI and cross-mod debit behavior require installed0.4.0 return. Synthetic map is not real player GameSave restoration.");
    }
}
