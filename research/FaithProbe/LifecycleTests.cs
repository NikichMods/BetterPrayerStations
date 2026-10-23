// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using HarmonyLib;

// Headless research: actual native getter IL + actual shared saved-state backend.
// Scene/save/target authorization is an explicit synthetic fixture boundary, never
// claimed as live exhumation, native SetObject or Unity restoration acceptance.
static class LifecycleTests
{
    static Assembly probe;
    static Type state;
    static MethodInfo applySkulls, applyDecoration;
    static WorldGameObject selected,secondarySelected,dormant;
    static bool enabled;
    static Func<WorldGameObject,float> nativeReplay, projectedReplay;
    static readonly BindingFlags Flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
    static readonly Dictionary<string,ItemDefinition> Definitions=new Dictionary<string,ItemDefinition>();
    static readonly string Schema="nikichmods.bps.faith_probe.schema", C="nikichmods.bps.faith_probe.conversions", L="nikichmods.bps.faith_probe.wreaths";
    static void Check(bool condition,string text){if(!condition)throw new Exception(text);}
    static void Set(object owner,string name,object value){owner.GetType().GetField(name,Flags).SetValue(owner,value);}
    static T Bare<T>(){return (T)FormatterServices.GetUninitializedObject(typeof(T));}
    static ItemDefinition Definition(string id,ItemDefinition.ItemType kind,float quality)
    {
        ItemDefinition result;
        if(Definitions.TryGetValue(id,out result))return result;
        result=Bare<ItemDefinition>();result.id=id;result.type=kind;result.quality=quality;
        Definitions.Add(id,result);return result;
    }
    static void Bind(Item item)
    {
        Set(item,"_definition",Definitions[item.id]);Set(item,"_definition_set",true);Set(item,"_definition_set_id",item.id);
        if(item.inventory!=null)foreach(var child in item.inventory)Bind(child);
        if(item.secondary_inventory!=null)foreach(var child in item.secondary_inventory)Bind(child);
    }
    static Item Item(string id,ItemDefinition.ItemType kind,float quality=0f)
    {
        Definition(id,kind,quality);
        var result=Bare<Item>();result.id=id;result.value=1;result.inventory=new List<Item>();result.secondary_inventory=new List<Item>();
        Set(result,"_params",new GameRes());Bind(result);result.durability=1f;return result;
    }
    static Item Body(int red,int white,float durability)
    {
        var result=Item("body",ItemDefinition.ItemType.Body);
        result.SetParam("bp_minus_mod",red);result.SetParam("bp_plus_mod",white);result.durability=durability;
        // Include a real nested native inventory; its zero-skull organ data must survive.
        result.inventory.Add(Item("fixture_organ",ItemDefinition.ItemType.BodyUniversalPart));
        return result;
    }
    static WorldGameObject Grave(Item data,bool generic=false)
    {
        var result=Bare<WorldGameObject>();result.totem_effect=new GameRes();result.obj_id=data.id;result.obj_def=Bare<ObjectDefinition>();
        result.obj_def.quality_type=generic?(ObjectDefinition.QualityType)0:ObjectDefinition.QualityType.Grave;
        result.obj_def.quality_multiplier=1f;result.obj_def.quality=Bare<SmartExpression>();Set(result.obj_def.quality,"default_value",-2f);
        Set(result,"_data",data);
        var manager=Bare<ComponentsManager>();var craft=Bare<CraftComponent>();
        Set(manager,"_components",new Dictionary<Type,WorldGameObjectComponent>{{typeof(CraftComponent),craft}});
        Set(result,"_components_inited",true);Set(result,"_components_manager",manager);return result;
    }
    static WorldGameObject Fixture(int red,int white,float durability,float decoration,bool hasBody=true)
    {
        var data=Item("grave_ground",(ItemDefinition.ItemType)0);
        if(hasBody)data.inventory.Add(Body(red,white,durability));
        data.inventory.Add(Item("fixture_top_"+decoration.ToString(System.Globalization.CultureInfo.InvariantCulture),ItemDefinition.ItemType.GraveStone,decoration));
        return Grave(data);
    }
    static MethodInfo Backend(string name){return state.GetMethod(name,BindingFlags.Public|BindingFlags.Static);}
    static void Write(Item owner,int c,int l){Backend("Write").Invoke(null,new object[]{owner,c,l});}
    static float Count(Item owner,string key){return owner.GetParam(key,0f);}
    static void Investment(Item owner,int c,int l)
    {
        Check(Count(owner,Schema)==1 && Count(owner,C)==c && Count(owner,L)==l,"Saved investment changed/lost");
        var value=Backend("Read").Invoke(null,new object[]{owner});
        Check((int)value.GetType().GetProperty("Actions").GetValue(value,null)==c+l,"Shared diagnostic action sum lost");
    }
    static bool Scope(WorldGameObject grave)
    {
        return (ReferenceEquals(grave,selected) || ReferenceEquals(grave,secondarySelected)) && !ReferenceEquals(grave,dormant) && enabled && grave.obj_id=="grave_ground" && !grave.components.craft.is_crafting && grave.GetBodyFromInventory(true)!=null;
    }
    static void SkullInput(Item body,out int red,out int white,out int available,bool dontCountSelf,WorldGameObject grave)
    {
        body.GetBodySkulls(out red,out white,out available,dontCountSelf);
        if(!Scope(grave) || dontCountSelf)return;
        var values=new object[]{grave.data,body,true,red,white,available};applySkulls.Invoke(null,values);
        red=(int)values[3];white=(int)values[4];available=(int)values[5];
    }
    static float DecorationInput(Item owner,string ignore,WorldGameObject grave)
    {
        float raw=owner.GetInventoryQuality(ignore);
        return (float)applyDecoration.Invoke(null,new object[]{owner,raw,Scope(grave)});
    }
    static Func<WorldGameObject,float> Replay(bool project)
    {
        var getter=typeof(WorldGameObject).GetProperty("quality").GetGetMethod();
        var dynamic=new DynamicMethod(project?"ProjectedNativeGrave":"OriginalNativeGrave",typeof(float),new[]{typeof(WorldGameObject)},typeof(LifecycleTests).Module,true);
        var generator=dynamic.GetILGenerator();
        var instructions=PatchProcessor.GetOriginalInstructions(getter,generator);
        if(project)
        {
            var type=probe.GetType("NikichMods.BPSFaithProbe.Plugin",true);
            instructions=(List<CodeInstruction>)type.GetMethod("RewriteQuality",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{instructions});
            foreach(var instruction in instructions)
            {
                var m=instruction.operand as MethodInfo;
                if(m!=null && m.DeclaringType==type && m.Name=="QualitySkulls")instruction.operand=typeof(LifecycleTests).GetMethod("SkullInput",BindingFlags.Static|BindingFlags.NonPublic);
                if(m!=null && m.DeclaringType==type && m.Name=="QualityDecoration")instruction.operand=typeof(LifecycleTests).GetMethod("DecorationInput",BindingFlags.Static|BindingFlags.NonPublic);
            }
        }
        foreach(var instruction in instructions)
        {
            Check(instruction.blocks.Count==0,"Unexpected native getter exception region");
            foreach(var label in instruction.labels)generator.MarkLabel(label);
            var op=instruction.opcode;var value=instruction.operand;
            if(value==null)generator.Emit(op);
            else if(value is Label)generator.Emit(op,(Label)value);
            else if(value is Label[])generator.Emit(op,(Label[])value);
            else if(value is LocalBuilder)generator.Emit(op,(LocalBuilder)value);
            else if(value is MethodInfo)generator.Emit(op,(MethodInfo)value);
            else if(value is ConstructorInfo)generator.Emit(op,(ConstructorInfo)value);
            else if(value is FieldInfo)generator.Emit(op,(FieldInfo)value);
            else if(value is Type)generator.Emit(op,(Type)value);
            else if(value is string)generator.Emit(op,(string)value);
            else if(value is int)generator.Emit(op,(int)value);
            else if(value is byte)generator.Emit(op,(byte)value);
            else if(value is sbyte)generator.Emit(op,(sbyte)value);
            else if(value is float)generator.Emit(op,(float)value);
            else if(value is double)generator.Emit(op,(double)value);
            else if(value is long)generator.Emit(op,(long)value);
            else throw new InvalidOperationException("Unsupported native getter operand "+value.GetType());
        }
        return (Func<WorldGameObject,float>)dynamic.CreateDelegate(typeof(Func<WorldGameObject,float>));
    }
    static byte[] Serialize<T>(T item)
    {
        var methods=typeof(SmartSerializer).GetMethods(BindingFlags.Public|BindingFlags.Static);
        foreach(var m in methods)if(m.Name=="Serialize" && m.IsGenericMethodDefinition && m.GetParameters().Length==1)
            return (byte[])m.MakeGenericMethod(typeof(T)).Invoke(null,new object[]{item});
        throw new MissingMethodException("Native serializer");
    }
    static T Restore<T>(byte[] bytes)
    {
        foreach(var m in typeof(SmartSerializer).GetMethods(BindingFlags.Public|BindingFlags.Static))
            if(m.Name=="Deserialize" && m.IsGenericMethodDefinition && m.GetParameters().Length==1 && m.GetParameters()[0].ParameterType==typeof(byte[]))
                return (T)m.MakeGenericMethod(typeof(T)).Invoke(null,new object[]{bytes});
        throw new MissingMethodException("Native deserializer");
    }
    static string Fingerprint(Item item){return Convert.ToBase64String(Serialize(item));}
    static WorldGameObject[] Roundtrip(WorldGameObject a,WorldGameObject b)
    {
        var map=new SerializableGameMap();
        var list=(List<SerializableWGO>)typeof(SerializableGameMap).GetField("_wgos",Flags).GetValue(map);
        var ra=new SerializableWGO{unique_id=101,obj_id=a.obj_id,item=a.data};
        var rb=new SerializableWGO{unique_id=202,obj_id=b.obj_id,item=b.data};list.Add(ra);list.Add(rb);
        var restored=Restore<SerializableGameMap>(Serialize(map));
        var records=(List<SerializableWGO>)typeof(SerializableGameMap).GetField("_wgos",Flags).GetValue(restored);
        Check(records.Count==2 && records[0].unique_id==101 && records[1].unique_id==202,"Native map saved identity lost");
        Bind(records[0].item);Bind(records[1].item);
        Check(!ReferenceEquals(records[0].item,a.data),"Native restore reused original Item reference");
        Check(records[0].obj_id==a.obj_id && records[1].obj_id==b.obj_id,"Native object-state IDs lost");
        var ga=Grave(records[0].item);var gb=Grave(records[1].item);ga.obj_id=records[0].obj_id;gb.obj_id=records[1].obj_id;return new[]{ga,gb};
    }
    static void NoExport(Item body)
    {
        Check(Count(body,Schema)==0 && Count(body,C)==0 && Count(body,L)==0,"Grave parameters exported to body/item");
        foreach(var child in body.inventory)NoExport(child);
    }
    static void TwoOwnerAndNativeZone()
    {
        var a=Fixture(3,3,1f,6f);a.unique_id=101;var b=Fixture(1,5,.8f,3f);b.unique_id=202;
        var setType=probe.GetType("NikichMods.BPSFaithProbe.TwoGraves",true);var set=Activator.CreateInstance(setType);var add=setType.GetMethod("Add");var contains=setType.GetMethod("Contains");
        add.Invoke(set,new object[]{b});add.Invoke(set,new object[]{a});add.Invoke(set,new object[]{a});
        var items=(List<WorldGameObject>)setType.GetField("Items").GetValue(set);
        Check(items.Count==2 && ReferenceEquals(items[0],a) && ReferenceEquals(items[1],b),"Two-owner set lost identity/order");
        Check((bool)contains.Invoke(set,new object[]{a}) && (bool)contains.Invoke(set,new object[]{b}),"Membership tied to selected editor");
        var c=Fixture(0,9,1f,9f);c.unique_id=303;bool refused=false;
        try{add.Invoke(set,new object[]{c});}catch(TargetInvocationException){refused=true;}Check(refused,"Third diagnostic owner accepted");
        var duplicateSet=Activator.CreateInstance(setType);add.Invoke(duplicateSet,new object[]{a});var duplicate=Fixture(0,9,1f,9f);duplicate.unique_id=101;refused=false;
        try{add.Invoke(duplicateSet,new object[]{duplicate});}catch(TargetInvocationException){refused=true;}Check(refused,"Duplicate live ID accepted");
        Write(a.data,1,1);Write(b.data,1,0);selected=a;secondarySelected=b;dormant=null;enabled=true;
        Check(projectedReplay(a)==4 && projectedReplay(b)==3,"Two simultaneous projected native getters");
        dormant=a;Check(projectedReplay(a)==3 && projectedReplay(b)==3,"A coverage loss affected B");
        dormant=b;Check(projectedReplay(a)==4 && projectedReplay(b)==2,"B coverage loss affected A");
        dormant=null;var pair=Roundtrip(a,b);a=pair[0];b=pair[1];selected=a;secondarySelected=b;
        Investment(a.data,1,1);Investment(b.data,1,0);Check(projectedReplay(a)==4 && projectedReplay(b)==3,"Both saved owners not projected after reload");
        var zone=Bare<WorldZone>();var definition=Bare<WorldZoneDefinition>();definition.calc_method=WorldZoneDefinition.QualityCalcMethod.Sum;
        Set(zone,"_definition",definition);zone.id="synthetic_graveyard";c.obj_def.ignore_counting_at_zone=true;
        Set(zone,"_wgos",new List<WorldGameObject>{a,b,c});
        int events=0;string eventId=null;float eventValue=0;Action<string,float> listener=(id,value)=>{events++;eventId=id;eventValue=value;};
        WorldZone.OnQualityComputed+=listener;
        try
        {
            float total=zone.GetTotalQuality();
            Check(total==5 && events==1 && eventId==zone.id && eventValue==total,"Actual native zone sum/ignored member/notification");
            Check(zone.GetZoneWGOs().Count==3,"Native member getter changed membership");
        }
        finally{WorldZone.OnQualityComputed-=listener;}
        selected=a;secondarySelected=null;dormant=null;
        Console.WriteLine("PASS two-owner set and projected native getter IL: simultaneous independent C/L, selective dormant/active result, both-owner native serialization including mixed C/L. PASS actual native WorldZone sum, ignored member and one native quality callback on synthetic objects.");
        Console.WriteLine("LIMIT native zone fixture calls unpatched native leaf getters; combined live two-owner Harmony activation/closed-window zone/Unity coverage still requires0.5.0 installed return.");
    }
    public static void Run(Assembly assembly)
    {
        probe=assembly;state=probe.GetType("NikichMods.BPSFaithProbe.SavedFaithState",true);
        applySkulls=Backend("ApplySkulls");applyDecoration=Backend("ApplyDecoration");
        nativeReplay=Replay(false);projectedReplay=Replay(true);
        int cases=0;
        foreach(var pair in new[]{new[]{3,3},new[]{0,8},new[]{1,5},new[]{7,0},new[]{-2,-1}})
        foreach(float durability in new[]{1f,.9001f,.9f,.8f,.05f,0f})
        foreach(float decoration in new[]{0f,3f,6f})
        foreach(int c in new[]{0,1,3,4,12})
        foreach(int l in new[]{0,1,3})
        foreach(bool active in new[]{false,true})
        {
            var actual=Fixture(pair[0],pair[1],durability,decoration);Write(actual.data,c,l);selected=actual;enabled=active;
            var source=actual.GetBodyFromInventory(true);string before=Fingerprint(source);
            float direct=actual.quality;
            Check(nativeReplay(actual)==direct,"Native IL replay differs from actual native getter");
            int r,w,available;source.GetBodySkulls(out r,out w,out available,false);
            int changed=active?Math.Min(c,r):0;
            var oracle=Fixture(r-changed,w+changed,durability,decoration);
            if(active && l>0)oracle.data.inventory.Add(Item("fixture_wreath_"+l,(ItemDefinition.ItemType)0,l));
            Check(projectedReplay(actual)==oracle.quality,"Projected native getter differs from independent native input fixture");
            Check(before==Fingerprint(source),"Real original synthetic anatomy data changed by projection");NoExport(source);
            Investment(actual.data,c,l);cases++;
        }
        // Ensure no-body and generic branches preserve their actual native result.
        var empty=Fixture(0,0,1,6,false);Write(empty.data,3,2);selected=empty;enabled=true;
        Check(projectedReplay(empty)==empty.quality && empty.quality==-2,"Empty grave received investment bonus");
        var generic=Grave(Fixture(3,3,1,6).data,true);Write(generic.data,3,2);selected=generic;
        Check(projectedReplay(generic)==generic.quality,"Generic quality branch changed");
        var busy=Fixture(3,3,1,6);Write(busy.data,3,2);busy.components.craft.is_crafting=true;
        busy.components.craft.current_craft=new CraftDefinition{id="set_fixture",needs=new List<Item>{busy.data.inventory[1]}};selected=busy;
        Check(projectedReplay(busy)==busy.quality,"Native busy/craft-exclusion pass-through changed");
        busy.components.craft.current_craft=new CraftDefinition{id="rem_fixture",output=new List<Item>{busy.data.inventory[1]}};
        Check(projectedReplay(busy)==busy.quality,"Native removal-craft exclusion pass-through changed");
        Console.WriteLine("PASS headless execution of actual native getter IL: "+cases+" projected/independent-native cases; original getter equivalence, no-body/generic/busy pass-through, unchanged original anatomy.");
        // Native serializer + deliberately synthetic inventory lifecycle; not native exhumation.
        var a=Fixture(3,3,1,6);var b=Fixture(1,5,.8f,3);Write(a.data,3,2);selected=a;enabled=true;
        var bodyA=a.GetBodyFromInventory(true);var bodyB=b.GetBodyFromInventory(true);
        string originalA=Fingerprint(bodyA),originalB=Fingerprint(bodyB);
        var loaded=Roundtrip(a,b);a=loaded[0];b=loaded[1];selected=a;
        Investment(a.data,3,2);Check(projectedReplay(a)==6,"Saved occupied-grave effect lost");
        bodyA=a.GetBodyFromInventory(true);bodyB=b.GetBodyFromInventory(true);
        a.data.inventory.Remove(bodyA);a.obj_id="grave_empty";
        Investment(a.data,3,2);Check(projectedReplay(a)==a.quality,"Removed-body grave gets a bonus");
        loaded=Roundtrip(a,b);a=loaded[0];a.obj_id="grave_empty";b=loaded[1];selected=a;
        Investment(a.data,3,2);Check(a.GetBodyFromInventory(true)==null,"Empty saved inventory gained a body");
        b.data.inventory.Remove(b.GetBodyFromInventory(true));b.data.inventory.Add(bodyA);
        Check(projectedReplay(b)==b.quality,"Body exported source grave bonus into another grave");NoExport(bodyA);
        a.data.inventory.Add(bodyB);a.obj_id="grave_ground";
        Investment(a.data,3,2);Check(projectedReplay(a)==4,"Replacement body does not use its own red bound/decay");
        loaded=Roundtrip(a,b);a=loaded[0];b=loaded[1];selected=a;
        Investment(a.data,3,2);Check(projectedReplay(a)==4 && projectedReplay(b)==b.quality,"Effects lost or exported after replaced-body save");
        Check(originalA==Fingerprint(b.GetBodyFromInventory(true)) && originalB==Fingerprint(a.GetBodyFromInventory(true)),"Transferred anatomy changed across native serialization");
        NoExport(a.GetBodyFromInventory(true));NoExport(b.GetBodyFromInventory(true));
        a.data.SetParam("fixture.other_owner",7);a.data.RemoveZeroParams();Investment(a.data,3,2);
        Backend("Clear").Invoke(null,new object[]{a.data});a.data.RemoveZeroParams();
        loaded=Roundtrip(a,b);a=loaded[0];selected=a;
        Check(Count(a.data,Schema)==0 && Count(a.data,C)==0 && Count(a.data,L)==0 && a.data.GetParam("fixture.other_owner",0)==7,"Cleanup lost another owner's data or left investment");
        Check(projectedReplay(a)==a.quality,"Cleanup native rating not restored");
        foreach(float invalid in new[]{-1f,1.5f,13f,float.NaN,float.PositiveInfinity})
        {
            Write(a.data,1,1);a.data.SetParam(C,invalid);bool rejected=false;
            try{Backend("Read").Invoke(null,new object[]{a.data});}catch(TargetInvocationException ex){rejected=ex.InnerException is InvalidOperationException;}
            Check(rejected,"Corrupt saved count accepted");
        }
        Write(a.data,1,1);a.data.SetParam(Schema,2);bool unknown=false;
        try{Backend("Read").Invoke(null,new object[]{a.data});}catch(TargetInvocationException ex){unknown=ex.InnerException is InvalidOperationException;}
        Check(unknown,"Unknown saved schema accepted");
        TwoOwnerAndNativeZone();
        Console.WriteLine("PASS native binary map serialization + synthetic inventory lifecycle: occupied/empty/replaced grave investment and IDs, original raw anatomy, no cross-grave export, red-bound/decay recalculation, shared action sum, positive-state pruning, cleanup roundtrip and unrelated data, invalid state rejection.");
        Console.WriteLine("LIMIT synthetic definitions/owners and list relocation, explicit headless authorization; actual getter IL and shared backend executed, but no Unity scene, native exhumation/SetObject/scene restore, live context recovery, UI or runtime save acceptance.");
    }
}
