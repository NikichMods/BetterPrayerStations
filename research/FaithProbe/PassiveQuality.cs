// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    internal sealed class PassiveQuality
    {
        internal sealed class Row
        {public WorldGameObject Station;public WorldZone Zone;public Vector2 Center;public float Half;public double Sum;public int Count,Bonus;}
        struct Input {public long Id,Owner;public float Quality;}
        public readonly List<Row> Rows=new List<Row>();
        readonly List<Input> inputs=new List<Input>();
        readonly List<ContributionMath.Emitter> emitters=new List<ContributionMath.Emitter>();
        readonly Dictionary<WorldGameObject,int> bonuses=new Dictionary<WorldGameObject,int>();
        readonly List<Row> pool=new List<Row>();
        public string Summary="";string lastDescription="";
        public bool Collecting;
        static readonly CultureInfo Inv=CultureInfo.InvariantCulture;
        public void Clear(){Rows.Clear();inputs.Clear();emitters.Clear();bonuses.Clear();pool.Clear();Summary=lastDescription="";}
        public int BonusFor(WorldGameObject station){int value;return !ReferenceEquals(station,null) && bonuses.TryGetValue(station,out value)?value:0;}
        public void Rebuild(PrayerCoverage coverage,FieldInfo zoneField,Action<string> log,bool describe)
        {
            if(Collecting)return;Collecting=true;bool old=coverage.CommittedOnly;coverage.CommittedOnly=true;
            try
            {
                Rows.Clear();inputs.Clear();emitters.Clear();bonuses.Clear();
                foreach(var station in coverage.Owners.Built)
                {
                    Rect rect;if(station==null || station.is_removed || PrayerCoverage.Tier(station.obj_id)==0 || station.components.craft.is_crafting || station.GetComponent<FloatingWorldGameObject>()!=null || !coverage.Geometry.TryRect(station,out rect))continue;
                    var zone=zoneField.GetValue(station) as WorldZone;if(zone==null)continue;
                    if(Rows.Count==pool.Count)pool.Add(new Row());var row=pool[Rows.Count];
                    row.Station=station;row.Zone=zone;row.Center=rect.center;row.Half=PrayerCoverage.HalfSide(station.obj_id);row.Sum=0;row.Count=row.Bonus=0;Rows.Add(row);
                    emitters.Add(new ContributionMath.Emitter{Id=station.unique_id,X=rect.center.x,Y=rect.center.y,Half=row.Half,Zone=zone});
                }
                if(Rows.Count>0)foreach(var grave in coverage.GraveOwners)
                {
                    Rect rect;if(grave==null || grave.is_removed || !coverage.Geometry.TryRect(grave,out rect))continue;
                    var zone=zoneField.GetValue(grave) as WorldZone;float q=grave.quality;double positive=ContributionMath.Positive(q);
                    int winner=positive>0?ContributionMath.Choose(emitters,rect.center.x,rect.center.y,zone):-1;
                    long owner=0;if(winner>=0){var row=Rows[winner];row.Sum+=positive;row.Count++;owner=row.Station.unique_id;}
                    inputs.Add(new Input{Id=grave.unique_id,Quality=q,Owner=owner});
                }
                bool changed;
                if(!describe){foreach(var row in Rows){row.Bonus=ContributionMath.Bonus(row.Sum);bonuses[row.Station]=row.Bonus;}return;}
                // Aggregate strings/logs are throttled separately from numeric collection.
                var text=new System.Text.StringBuilder();
                foreach(var row in Rows)
                {
                    row.Bonus=ContributionMath.Bonus(row.Sum);bonuses[row.Station]=row.Bonus;
                    float actual=row.Station.quality;
                    text.Append("№").Append(row.Station.unique_id).Append(": могил ").Append(row.Count).Append(", сумма ").Append(row.Sum.ToString("0.#",Inv)).Append(", бонус +").Append(row.Bonus).Append("; база ").Append((actual-row.Bonus).ToString("0.#",Inv)).Append(", итог ").Append(actual.ToString("0.#",Inv)).AppendLine();
                }
                string description=text.ToString();changed=description!=lastDescription;
                if(changed){lastDescription=description;Summary=description;foreach(var row in Rows)log("PASSIVE station="+row.Station.unique_id+" zone="+row.Zone.id+" graves="+row.Count+" positive_sum="+row.Sum.ToString("R",Inv)+" bonus="+row.Bonus+" scope=committed");}
            }
            finally{coverage.CommittedOnly=old;Collecting=false;}
        }
        public void Trace(Action<string> log)
        {foreach(var input in inputs)log("PASSIVE_GRAVE grave="+input.Id+" current_quality="+input.Quality.ToString("R",Inv)+" owner="+input.Owner);}
    }
}
