// SPDX-License-Identifier: MPL-2.0
using System;
using System.Reflection;
using HarmonyLib;
internal static class PreviewTests
{
    static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    public static void Run(Assembly probe)
    {
        var baselineType=probe.GetType("NikichMods.BPSFaithProbe.PreviewBaseline",true);
        var accept=baselineType.GetMethod("AcceptText");var textProperty=baselineType.GetProperty("Text");
        var snapshot=Activator.CreateInstance(baselineType,new object[]{2f,null});
        Check(!(bool)accept.Invoke(snapshot,new object[]{2f,null}),"Absent native text was treated as ready");
        Check((bool)accept.Invoke(snapshot,new object[]{2f,"(wskull)\n2"}),"Late native text cannot resolve frozen baseline");
        accept.Invoke(snapshot,new object[]{3f,"(wskull)\n3"});
        Check((string)textProperty.GetValue(snapshot,null)=="(wskull)\n2" && (float)baselineType.GetField("Quality").GetValue(snapshot)==2f,"Cursor movement overwrote original snapshot");
        var changed=Activator.CreateInstance(baselineType,new object[]{2f,null});bool rejected=false;
        try{accept.Invoke(changed,new object[]{3f,"(wskull)\n3"});}catch(TargetInvocationException e){rejected=e.InnerException is InvalidOperationException;}
        Check(rejected && textProperty.GetValue(changed,null)==null,"Changed committed quality silently rebased deferred snapshot");
        var geometry=probe.GetType("NikichMods.BPSFaithProbe.GridFootprint",true);
        bool ownerRead=false,partWalk=false;
        foreach(var i in PatchProcessor.GetOriginalInstructions(geometry.GetMethod("TryRect")))
        {
            var fieldInfo=i.operand as FieldInfo;if(fieldInfo!=null && fieldInfo.DeclaringType==typeof(WorldGameObject) && fieldInfo.Name=="wop")ownerRead=true;
            var methodInfo=i.operand as MethodInfo;if(methodInfo!=null && methodInfo.Name=="GetComponentsInChildren")partWalk=true;
        }
        Check(ownerRead && partWalk,"Geometry no longer selects the native current WOP owner");
        Console.WriteLine("PASS compiled deferred native-text readiness: null then ready, immutable numeric/text baseline, changed committed value rejected. Geometry reads current native WOP before subtree discovery; Unity old/new hierarchy transition still requires live observation.");
        var feedback=probe.GetType("NikichMods.BPSFaithProbe.PreviewFeedback",true).GetMethod("Text");
        Func<float,string> text=d=>(string)feedback.Invoke(null,new object[]{"(wskull)\n3",d});
        Check(text(0)=="(wskull)\n3","Zero preview delta changes committed number");
        Check(text(2)=="(wskull)\n3 [4DFF4D]+2[-]","Positive preview shows projected total or changes baseline");
        Check(text(-1)=="(wskull)\n3 [FF7366]-1[-]","Negative preview loses baseline/sign");
        Check(text(2)==text(2) && text(0)=="(wskull)\n3","Cursor return accumulates delta");
        var snap=probe.GetType("NikichMods.BPSFaithProbe.GridFootprint",true).GetMethod("Snap");
        var rect=(UnityEngine.Rect)snap.Invoke(null,new object[]{-46f,-86f,46f,6f});
        Check(rect.xMin==-48 && rect.xMax==48 && rect.yMin==-112 && rect.yMax==16,"Placement grid center/footprint is shifted by half a cell");
        var shifted=(UnityEngine.Rect)snap.Invoke(null,new object[]{18f,-54f,110f,38f});
        Check(shifted.center==rect.center+new UnityEngine.Vector2(64,32),"Grid translation changes local footprint center");
        Console.WriteLine("PASS compiled preview text retains frozen native number/icon with zero/positive/negative/returned deltas; native32-unit placement footprint fixture and translation. Actual box versus native occupied-cell comparison is logged live, not asserted by this fixture.");
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
        var adapterType=probe.GetType("NikichMods.BPSFaithProbe.MovePreviewOwner",true);
        var adapter=Activator.CreateInstance(adapterType);
        string gameRoot=System.IO.Directory.GetParent(System.IO.Directory.GetParent(typeof(WorldGameObject).Assembly.Location).FullName).Parent.FullName;
        string movePath=System.IO.Path.Combine(gameRoot,"BepInEx","plugins","MoveObjects","GYKMoveMod.dll");
        Check(System.IO.File.Exists(movePath),"Required installed Move Objects test payload missing");
        Assembly.LoadFrom(movePath);adapterType.GetMethod("Initialize").Invoke(adapter,null);
        Check(((string)adapterType.GetField("Identity").GetValue(adapter)).Contains("c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0"),"Adapter failed after delayed real assembly load");
        var plugin=probe.GetType("NikichMods.BPSFaithProbe.Plugin",true);
        var recover=PatchProcessor.GetOriginalInstructions(plugin.GetMethod("Recover",BindingFlags.Instance|BindingFlags.NonPublic));
        var awake=PatchProcessor.GetOriginalInstructions(plugin.GetMethod("Awake",BindingFlags.Instance|BindingFlags.NonPublic));
        int readyInit=0,earlyInit=0;
        foreach(var i in recover){var m=i.operand as MethodInfo;if(m!=null && m.DeclaringType==adapterType && m.Name=="Initialize")readyInit++;}
        foreach(var i in awake){var m=i.operand as MethodInfo;if(m!=null && m.DeclaringType==adapterType && m.Name=="Initialize")earlyInit++;}
        Check(readyInit==1 && earlyInit==0,"Adapter resolution still depends on plugin Awake order");
        var badge=PatchProcessor.GetOriginalInstructions(plugin.GetMethod("RefreshQualityBadge",BindingFlags.Instance|BindingFlags.NonPublic));
        int nativeProducer=0,nativeSetter=0;
        foreach(var i in badge)
        {
            var m=i.operand as MethodInfo;if(m==null)continue;
            if(m.DeclaringType==typeof(WorldGameObject) && m.Name=="GetQualityWidgetData")nativeProducer++;
            if(m.DeclaringType==typeof(WorldGameObject) && m.Name=="SetBubbleWidgetData")nativeSetter++;
            Check(m.Name!="SetQualityHint" && m.Name!="SetObject" && m.Name!="RefreshBubblesData","Quality-only refresh changes native visibility/other widgets or recreates world parts");
        }
        Check(nativeProducer==1 && nativeSetter==1,"Badge no longer uses native quality text producer/owner setter");
        Console.WriteLine("PASS delayed real Move Objects assembly load resolves exact hash/typed source fields; compiled Recover owns initialization, Awake does not. Badge refresh calls actual native quality-text producer/setter without rewriting visibility, interaction data or world object. Rendering remains a live test.");
        Console.WriteLine("PASS compiled StationSet: eight repeated move/cancel cycles retain one source, preview registration excluded, original source suppressed only for station preview, unrelated sources retained, new construction promotes once. Actual native stop/current/destroy contract checked. No Unity lifecycle, external Move Objects field timing or overlay rendering acceptance implied.");
    }
}