// SPDX-License-Identifier: MPL-2.0
using System.Collections.Generic;
using UnityEngine;
namespace NikichMods.BPSMenuProbe
{
    internal sealed class FieldGeometry
    {
        readonly NikichMods.BPSFaithProbe.GridFootprint geometry=new NikichMods.BPSFaithProbe.GridFootprint();
        readonly Dictionary<WorldGameObject,WorldObjectPart> initialized=new Dictionary<WorldGameObject,WorldObjectPart>();
        internal void Clear(){initialized.Clear();geometry.Clear();}
        internal void Forget(WorldGameObject grave){initialized.Remove(grave);geometry.Forget(grave);}
        internal static bool Contains(Rect footprint,int x,int y){return footprint.Contains(new Vector2(x*32f,y*32f));}
        internal bool TryRect(WorldGameObject grave,out Rect rect)
        {
            rect=new Rect();if(grave==null || grave.wop==null)return false;
            WorldObjectPart part;
            if(!initialized.TryGetValue(grave,out part) || !ReferenceEquals(part,grave.wop))
            {
                foreach(var c in grave.wop.GetComponentsInChildren<OptimizedCollider2D>(true))c.Init();
                initialized[grave]=grave.wop;
            }
            return geometry.TryRect(grave,out rect);
        }
        internal int Matches(WorldGameObject grave,BeamState state,int support)
        {
            if(state.Active(support)==0)return 0;
            Rect rect;if(!TryRect(grave,out rect))return 0;
            int matches=0;
            for(int i=0;i<state.Active(support);i++){var p=state.Points[i];if(Contains(rect,p.X,p.Y))matches++;}
            return matches;
        }
        internal int NativeMatches(WorldGameObject grave,BeamState state,int support)
        {
            // Independent, bounded action-time physics oracle; never used by the rating getter.
            Rect ignored;TryRect(grave,out ignored);
            if(grave.wop==null)return 0;
            var colliders=new List<Collider2D>();
            foreach(var c in grave.wop.GetComponentsInChildren<Collider2D>(true))if(c.gameObject.layer==23 && !BuildGrid.SkipCollider(c,grave))colliders.Add(c);
            if(colliders.Count==0)return 0;
            int matches=0;
            for(int i=0;i<state.Active(support);i++){var p=state.Points[i];if(BuildGrid.IsCellBusy(new Vector2(p.X*32f,p.Y*32f),colliders))matches++;}
            return matches;
        }
    }
}
