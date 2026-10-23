// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    internal sealed class PrayerCoverage
    {
        public const float UnitsPerCell=96f, RadiusCells=3f, Radius=UnitsPerCell*RadiusCells;
        public readonly StationSet<WorldGameObject> Owners=new StationSet<WorldGameObject>();
        public int Count {get{return Owners.Built.Count;}}
        public bool HasPreview {get{return Owners.Preview!=null && Tier(Owners.Preview.obj_id)>0;}}
        public static int Tier(string id){return id=="pray_stand_stn"?2:id=="pray_stand_wd"?1:0;}
        public static bool Inside(float dx,float dy){return dx*dx+dy*dy<=Radius*Radius;}
        public static int EligibleTier(string id,bool removed,bool busy,float dx,float dy){return removed || busy || !Inside(dx,dy)?0:Tier(id);}
        public void Clear(){Owners.Clear();}
        public void Register(WorldGameObject station)
        {
            if(station==null || station.is_removed || Tier(station.obj_id)==0 || station.GetComponent<FloatingWorldGameObject>()!=null)return;
            Owners.Register(station);
        }
        public void Refresh()
        {for(int i=Owners.Built.Count-1;i>=0;i--){var s=Owners.Built[i];if(s==null || s.is_removed || Tier(s.obj_id)==0)Owners.Built.RemoveAt(i);}}
        public void Begin(WorldGameObject preview,WorldGameObject source){Owners.Begin(preview,source);}
        public void End(WorldGameObject preview,bool leave){Owners.End(preview,leave && preview!=null && Tier(preview.obj_id)>0);Refresh();}
        public static string Position(WorldGameObject w)
        {if(w==null)return "none";var p=w.transform.position;return p.x.ToString("R",CultureInfo.InvariantCulture)+","+p.y.ToString("R",CultureInfo.InvariantCulture);}
        public string Describe(WorldGameObject grave)
        {
            var b=new StringBuilder("grave_xy="+Position(grave));
            foreach(var s in Owners.Built)if(s!=null && !s.is_removed)b.Append(" built=").Append(s.unique_id).Append(':').Append(s.obj_id).Append(" xy=").Append(Position(s));
            if(HasPreview)b.Append(" preview=").Append(Owners.Preview.unique_id).Append(" xy=").Append(Position(Owners.Preview)).Append(" replaces=").Append(Owners.Source==null?0:Owners.Source.unique_id);
            return b.ToString();
        }
        void Accumulate(WorldGameObject s,Vector2 g,ref long nearestId,ref float nearestDistance,ref int covering,ref int tier)
        {
            if(s==null || s.is_removed || Tier(s.obj_id)==0)return;
            Vector2 p=s.pos;float dx=p.x-g.x,dy=p.y-g.y;float distance=Mathf.Sqrt(dx*dx+dy*dy)/UnitsPerCell;
            if(distance<nearestDistance){nearestDistance=distance;nearestId=s.unique_id;}
            int candidate=EligibleTier(s.obj_id,false,s.components.craft.is_crafting,dx,dy);
            if(candidate>0){covering++;tier=Math.Max(tier,candidate);}
        }
        public int Evaluate(WorldGameObject grave,out long nearestId,out float nearestDistance,out int covering)
        {
            nearestId=0;nearestDistance=float.PositiveInfinity;covering=0;
            if(grave==null || grave.is_removed)return 0;
            Refresh();Vector2 g=grave.pos;int tier=0;bool preview=HasPreview;
            foreach(var s in Owners.Built)if(!Owners.Suppressed(s,preview))Accumulate(s,g,ref nearestId,ref nearestDistance,ref covering,ref tier);
            if(preview)Accumulate(Owners.Preview,g,ref nearestId,ref nearestDistance,ref covering,ref tier);
            return tier;
        }
    }
}