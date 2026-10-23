// SPDX-License-Identifier: MPL-2.0
using System;
using System.Reflection;
using HarmonyLib;
internal static class PreviewTests
{
    static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    public static void Run(Assembly probe)
    {
        var type=probe.GetType("NikichMods.BPSFaithProbe.StationSet`1",true).MakeGenericType(typeof(object));
        var set=Activator.CreateInstance(type);var register=type.GetMethod("Register");var begin=type.GetMethod("Begin");var end=type.GetMethod("End");var suppressed=type.GetMethod("Suppressed");
        var field=type.GetField("Built");Func<int> count=()=>((System.Collections.ICollection)field.GetValue(set)).Count;
        object a=new object(),b=new object();register.Invoke(set,new[]{a});register.Invoke(set,new[]{a});Check(count()==1,"Duplicate committed ref counted twice");
        for(int i=0;i<8;i++)
        {
            object preview=new object();begin.Invoke(set,new[]{preview,a});register.Invoke(set,new[]{preview});
            Check(count()==1 && (bool)suppressed.Invoke(set,new object[]{a,true}),"Move preview adds a station or keeps source position active");
            Check(!(bool)suppressed.Invoke(set,new object[]{a,false}),"Nonstation cursor suppresses source");
            end.Invoke(set,new object[]{preview,false});
            Check(count()==1 && !(bool)suppressed.Invoke(set,new object[]{a,true}) && type.GetField("Preview").GetValue(set)==null,"Cancel/commit of existing owner retained preview or lost source");
        }
        register.Invoke(set,new[]{b});Check(count()==2,"Distinct physical sources collapsed");
        object construction=new object();begin.Invoke(set,new object[]{construction,null});register.Invoke(set,new[]{construction});
        Check(count()==2 && !(bool)suppressed.Invoke(set,new object[]{a,true}),"New-build preview changes built count or hides unrelated source");
        end.Invoke(set,new object[]{construction,true});Check(count()==3,"Native leave-on-scene did not promote exactly once");
        register.Invoke(set,new[]{construction});Check(count()==3,"Promotion plus native registration double-counted");
        var create=typeof(FloatingWorldGameObject).GetMethod("CreateFloatingObject",BindingFlags.Static|BindingFlags.Public,null,new[]{typeof(WorldGameObject)},null);
        var stop=typeof(FloatingWorldGameObject).GetMethod("StopCurrentFloating",BindingFlags.Static|BindingFlags.Public,null,new[]{typeof(bool)},null);
        Check(create!=null && create.ReturnType==typeof(FloatingWorldGameObject) && stop!=null && stop.ReturnType==typeof(void),"Native floating lifecycle shape changed");
        bool current=false,destroy=false;
        foreach(var i in PatchProcessor.GetOriginalInstructions(stop))
        {var f=i.operand as FieldInfo;if(f!=null && f.Name=="_cur_floating" && f.FieldType==typeof(FloatingWorldGameObject))current=true;var m=i.operand as MethodInfo;if(m!=null && m.DeclaringType==typeof(UnityEngine.Object) && m.Name=="Destroy")destroy=true;}
        Check(current && destroy,"Native stop no longer releases current preview through expected destroy path");
        Console.WriteLine("PASS compiled StationSet: eight repeated move/cancel cycles retain one source, preview registration excluded, original source suppressed only for station preview, unrelated sources retained, new construction promotes once. Actual native stop/current/destroy contract checked. No Unity lifecycle, external Move Objects field timing or overlay rendering acceptance implied.");
    }
}