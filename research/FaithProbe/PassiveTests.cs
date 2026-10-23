// SPDX-License-Identifier: MPL-2.0
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
internal static class PassiveTests
{
    static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    public static void Run(Assembly assembly)
    {
        var math=assembly.GetType("NikichMods.BPSFaithProbe.ContributionMath",true);var emitter=math.GetNestedType("Emitter");
        var listType=typeof(List<>).MakeGenericType(emitter);var list=(IList)Activator.CreateInstance(listType);
        var choose=math.GetMethod("Choose");var positive=math.GetMethod("Positive");var bonus=math.GetMethod("Bonus");
        object zoneA=new object(),zoneB=new object();
        Action<long,double,double,float,object> add=(id,x,y,half,zone)=>{object v=Activator.CreateInstance(emitter);emitter.GetField("Id").SetValue(v,id);emitter.GetField("X").SetValue(v,x);emitter.GetField("Y").SetValue(v,y);emitter.GetField("Half").SetValue(v,half);emitter.GetField("Zone").SetValue(v,zone);list.Add(v);};
        Func<double,double,object,int> owner=(x,y,z)=>(int)choose.Invoke(null,new object[]{list,x,y,z});
        Func<double,int> count=s=>(int)bonus.Invoke(null,new object[]{s});
        foreach(var pair in new[]{new[]{0d,0d},new[]{9d,0d},new[]{9.9d,0d},new[]{10d,1d},new[]{19d,1d},new[]{20d,2d},new[]{21d,2d}})Check(count(pair[0])==(int)pair[1],"Passive threshold incorrect");
        Check((double)positive.Invoke(null,new object[]{-100f})==0 && (double)positive.Invoke(null,new object[]{0f})==0 && (double)positive.Invoke(null,new object[]{7f})==7,"Nonpositive graves subtract or positive quality lost");
        bool rejected=false;try{positive.Invoke(null,new object[]{float.NaN});}catch(TargetInvocationException e){rejected=e.InnerException is InvalidOperationException;}Check(rejected,"Nonfinite native quality accepted");
        add(20,0,0,3,zoneA);add(10,4,0,3,zoneA);add(1,2,0,100,zoneB);
        Check(owner(2,0,zoneA)==1,"Stable ID did not resolve exact distance tie");
        Check(owner(0,0,zoneA)==0 && owner(7,3,zoneA)==1,"Nearest or inclusive square corner failed");
        Check(owner(8,0,zoneA)==-1 && owner(2,0,new object())==-1,"Outside or cross-zone assignment leaked");
        object first=list[0];list[0]=list[1];list[1]=first;Check(owner(2,0,zoneA)==0,"Registry order changes ownership");
        list.Clear();add(20,0,0,3,zoneA);add(10,4,0,3,zoneA);
        double[] sums={0,0};int[] numbers={0,0};
        var graves=new[]{new[]{0d,1d,8d},new[]{2d,0d,7d},new[]{3d,0d,-6d},new[]{6d,0d,6d}};
        foreach(var g in graves){double q=(double)positive.Invoke(null,new object[]{(float)g[2]});int k=q>0?owner(g[0],g[1],zoneA):-1;if(k>=0){sums[k]+=q;numbers[k]++;}}
        Check(sums[0]==8 && sums[1]==13 && numbers[0]+numbers[1]==3 && count(sums[0])+count(sums[1])==1,"Overlap duplicated/lost positive contributions or floor applied globally");
        list.Clear();add(20,100,0,3,zoneA);Check(owner(0,0,zoneA)==-1,"Relocation retains old coverage");list.Clear();add(20,0,0,3,zoneA);Check(owner(0,0,zoneA)==0,"Return cannot reassign");
        var planner=assembly.GetType("NikichMods.BPSFaithProbe.ThresholdPlan",true);
        var find=planner.GetMethod("Find");
        Func<int,int,int,float,float,float,double,int,object> plan=(r,w,a,d,de,q,sum,f)=>find.Invoke(null,new object[]{r,w,a,d,de,q,sum,f});
        object prepared=plan(3,3,3,1f,6f,3f,38d,20);
        Check(prepared!=null && (int)planner.GetField("C").GetValue(prepared)==1 && (int)planner.GetField("L").GetValue(prepared)==0 && (bool)planner.GetField("Conversion").GetValue(prepared) && (double)planner.GetField("Sum").GetValue(prepared)==39 && (int)planner.GetField("Cost").GetValue(prepared)==2,"Exact threshold preparation/paid conversion failed");
        Check(plan(0,3,3,1f,3f,3f,39d,20)==null,"Capped body manufactured a quality increase");
        Check(plan(3,3,3,1f,6f,3f,38d,1)==null,"Planner selected unaffordable paid step");
        Check(plan(3,3,3,1f,6f,2f,38d,20)==null,"Prediction/native baseline mismatch accepted");
        prepared=plan(0,10,10,1f,5f,5f,39d,20);
        Check(prepared!=null && !(bool)planner.GetField("Conversion").GetValue(prepared) && (int)planner.GetField("Cost").GetValue(prepared)==1,"Existing threshold-minus-one wreath step failed");
        Check(plan(0,10,10,1f,5f,5f,30d,20)==null,"Impossible next threshold invented");
        prepared=plan(1,6,4,.8f,6f,4f,39d,20);
        Check(prepared!=null,"Decay-bounded possible step rejected");
        var predict=planner.GetMethod("Predict");
        Check((float)predict.Invoke(null,new object[]{3,3,2,.8f,6f,3,0})==4,"Planner lost native decay cap");
        Console.WriteLine("PASS compiled threshold planner: exact free preparation then paid conversion/wreath, shared quote, native cap/decay, unaffordable/no-plan and inconsistent baseline refusals. Actual live getter/zone integration remains unaccepted.");
        var plugin=assembly.GetType("NikichMods.BPSFaithProbe.Plugin",true);
        int actualBuy=0,actualTotals=0;
        foreach(var i in PatchProcessor.GetOriginalInstructions(plugin.GetMethod("BuyThreshold",BindingFlags.Instance|BindingFlags.NonPublic)))
        {var m=i.operand as MethodInfo;if(m==null)continue;if(m.DeclaringType==plugin && m.Name=="Buy")actualBuy++;if(m.DeclaringType==typeof(WorldZone) && m.Name=="GetTotalQuality")actualTotals++;Check(m.Name!="Write" && m.Name!="SetParam","Paid threshold observer writes a manufactured result");}
        Check(actualBuy==1 && actualTotals==2,"Threshold observer bypasses existing purchase or before/after native zone calls");
        var getter=PatchProcessor.GetOriginalInstructions(plugin.GetMethod("StationQuality",BindingFlags.Static|BindingFlags.NonPublic));int lookups=0;
        foreach(var i in getter){var m=i.operand as MethodInfo;if(m==null)continue;if(m.Name=="BonusFor")lookups++;Check(m.Name!="GetTotalQuality" && m.Name!="get_quality","Station postfix recursively queries quality");}
        Check(lookups==1,"Actual station consumer bypasses snapshot");
        var controller=assembly.GetType("NikichMods.BPSFaithProbe.PassiveQuality",true);int allocation=0;
        foreach(var i in PatchProcessor.GetOriginalInstructions(controller.GetMethod("Rebuild")))
        {var m=i.operand as MethodInfo;if(m==null)continue;if(m.DeclaringType==math && m.Name=="Choose")allocation++;Check(m.Name!="GetTotalQuality" && m.Name!="GetMyWorldZone" && !m.Name.StartsWith("FindObjects"),"Collector scans native world/physics or queries total");}
        Check(allocation==1,"Real collector bypasses tested allocation helper");
        Console.WriteLine("PASS compiled passive positive-only thresholds, nonfinite refusal, nearest/exact-ID tie independent of registry order, inclusive square corner, zone isolation, exclusive overlap and per-station floor, relocation/return. Actual collector calls tested chooser; station postfix reads snapshot without recursive quality/zone queries. Unity allocation/zone/HUD rendering remain live tests.");
    }
}
