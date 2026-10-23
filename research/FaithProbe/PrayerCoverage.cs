// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    // Diagnostic geometry only, not final product radius, tier cap or placement policy.
    internal sealed class PrayerCoverage
    {
        public const float UnitsPerCell=96f, RadiusCells=3f, Radius=UnitsPerCell*RadiusCells;
        readonly List<WorldGameObject> stations=new List<WorldGameObject>();
        public int Count { get { return stations.Count; } }
        public static int Tier(string id) { return id=="pray_stand_stn"?2:id=="pray_stand_wd"?1:0; }
        public static bool Inside(float dx,float dy) { return dx*dx+dy*dy<=Radius*Radius; }
        public static int EligibleTier(string id,bool removed,bool busy,float dx,float dy)
        { return removed || busy || !Inside(dx,dy)?0:Tier(id); }
        public void Clear(){stations.Clear();}
        public void Register(WorldGameObject station)
        {
            if(station==null || station.is_removed || Tier(station.obj_id)==0 || stations.Contains(station))return;
            stations.Add(station);
        }
        static string Position(WorldGameObject w)
        {
            var p=w.transform.position;
            return p.x.ToString("R",CultureInfo.InvariantCulture)+","+p.y.ToString("R",CultureInfo.InvariantCulture);
        }
        public string Describe(WorldGameObject grave)
        {
            var b=new StringBuilder("grave_xy="+Position(grave));
            foreach(var s in stations)
                if(s!=null && !s.is_removed)b.Append(" station=").Append(s.unique_id).Append(':').Append(s.obj_id).Append(" xy=").Append(Position(s)).Append(" busy=").Append(s.components.craft.is_crafting);
            return b.ToString();
        }
        public int Evaluate(WorldGameObject grave,out long nearestId,out float nearestDistance,out int covering)
        {
            nearestId=0;nearestDistance=float.PositiveInfinity;covering=0;
            if(grave==null || grave.is_removed)return 0;
            Vector2 g=grave.pos;int tier=0;
            for(int i=stations.Count-1;i>=0;i--)
            {
                var s=stations[i];
                if(s==null || s.is_removed || Tier(s.obj_id)==0){stations.RemoveAt(i);continue;}
                Vector2 p=s.pos;float dx=p.x-g.x,dy=p.y-g.y;
                float distance=Mathf.Sqrt(dx*dx+dy*dy)/UnitsPerCell;
                if(distance<nearestDistance){nearestDistance=distance;nearestId=s.unique_id;}
                int candidate=EligibleTier(s.obj_id,false,s.components.craft.is_crafting,dx,dy);
                if(candidate>0){covering++;tier=Math.Max(tier,candidate);}
            }
            return tier;
        }
    }
}
