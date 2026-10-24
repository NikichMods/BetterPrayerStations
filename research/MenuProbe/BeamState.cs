// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSMenuProbe
{
    internal sealed class BeamState
    {
        internal sealed class Point { public int X,Y; public long Order; }
        internal static readonly int[] Prices={1,2,3,4,6,9,12,15,18,21};
        internal readonly List<Point> Points=new List<Point>();
        internal int Purchased {get;private set;}
        long serial;
        internal int Limit(int support){return Math.Min(Purchased,Math.Max(0,support));}
        internal int Active(int support){return Math.Min(Points.Count,Limit(support));}
        internal int Free(int support){return Math.Max(0,Limit(support)-Points.Count);}
        internal int NextPrice {get{return Purchased<Prices.Length?Prices[Purchased]:0;}}
        internal bool CanBuy(int support){return Purchased<support && Purchased<Prices.Length;}
        internal bool Buy(int support){if(!CanBuy(support))return false;Purchased++;return true;}
        internal int Index(int x,int y){for(int i=0;i<Points.Count;i++)if(Points[i].X==x && Points[i].Y==y)return i;return -1;}
        internal bool Place(int x,int y,int support)
        {if(Free(support)==0 || Index(x,y)>=0)return false;Points.Add(new Point{X=x,Y=y,Order=++serial});return true;}
        internal bool Remove(int x,int y){int i=Index(x,y);if(i<0)return false;Points.RemoveAt(i);return true;}
        internal void Restore(int purchased,IList<Point> points)
        {
            if(purchased<0 || purchased>Prices.Length || points==null || points.Count>purchased)throw new InvalidOperationException("Invalid saved beam capacity/count");
            var copy=new List<Point>();
            for(int i=0;i<points.Count;i++)
            {
                var p=points[i];if(p==null || p.X < -1000000 || p.X>1000000 || p.Y < -1000000 || p.Y>1000000)throw new InvalidOperationException("Invalid saved beam coordinates");
                foreach(var q in copy)if(q.X==p.X && q.Y==p.Y)throw new InvalidOperationException("Duplicate saved beam cell");
                copy.Add(new Point{X=p.X,Y=p.Y,Order=i+1});
            }
            Points.Clear();Points.AddRange(copy);Purchased=purchased;serial=copy.Count;
        }
        internal void Clear(){Points.Clear();Purchased=0;serial=0;}
    }
}
