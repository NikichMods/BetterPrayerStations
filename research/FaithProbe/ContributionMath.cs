// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSFaithProbe
{
    internal static class ContributionMath
    {
        public struct Emitter {public long Id;public double X,Y;public float Half;public object Zone;}
        public static int Choose(IList<Emitter> stations,double x,double y,object zone)
        {
            int winner=-1;double best=double.PositiveInfinity;long bestId=long.MaxValue;
            for(int i=0;i<stations.Count;i++)
            {
                var s=stations[i];if(!ReferenceEquals(s.Zone,zone))continue;
                double dx=x-s.X,dy=y-s.Y;if(!Better(dx,dy,s.Half,s.Id,best,bestId))continue;
                winner=i;best=dx*dx+dy*dy;bestId=s.Id;
            }
            return winner;
        }
        public static bool Better(double dx,double dy,float half,long id,double best,long bestId)
        {
            if(Math.Abs(dx)>half || Math.Abs(dy)>half)return false;
            double d=dx*dx+dy*dy;return d<best || (d==best && id<bestId);
        }
        public static double Positive(float quality)
        {
            if(float.IsNaN(quality) || float.IsInfinity(quality))throw new InvalidOperationException("Nonfinite native grave quality");
            return quality>0?quality:0;
        }
        public static int Bonus(double sum)
        {if(double.IsNaN(sum) || double.IsInfinity(sum) || sum<0 || sum>int.MaxValue)throw new InvalidOperationException("Invalid grave quality sum");return (int)Math.Floor(sum/10d);}
    }
}
