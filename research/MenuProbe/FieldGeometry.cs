// SPDX-License-Identifier: MPL-2.0
using System.Collections.Generic;
using UnityEngine;
namespace NikichMods.BPSMenuProbe
{
    internal sealed class FieldGeometry
    {
        readonly NikichMods.BPSFaithProbe.GridFootprint geometry=new NikichMods.BPSFaithProbe.GridFootprint(true);
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
        internal bool TryNativeMatches(WorldGameObject grave,BeamState state,int support,out int matches,out string unavailable)
        {
            // Independent, bounded action-time physics oracle; never used by the rating getter.
            matches=0;unavailable=null;
            Rect ignored;TryRect(grave,out ignored);
            if(grave.wop==null){unavailable="missing-part";return false;}
            var colliders=new List<Collider2D>();
            foreach(var c in grave.wop.GetComponentsInChildren<Collider2D>(true))if(c.gameObject.layer==23)
            {
                // ChunkManager legitimately removes hidden objects from the physics world.
                if(!c.gameObject.activeInHierarchy){unavailable="inactive-chunk-or-part";return false;}
                if(!c.enabled){unavailable="disabled-collider";return false;}
                var box=c as BoxCollider2D;
                if(box==null){unavailable="unsupported-physics-shape";return false;}
                var a=box.transform.TransformPoint(box.offset-box.size/2f);
                var b=box.transform.TransformPoint(box.offset+box.size/2f);
                var bounds=box.bounds;
                if(Mathf.Abs(bounds.min.x-Mathf.Min(a.x,b.x))>.01f || Mathf.Abs(bounds.min.y-Mathf.Min(a.y,b.y))>.01f || Mathf.Abs(bounds.max.x-Mathf.Max(a.x,b.x))>.01f || Mathf.Abs(bounds.max.y-Mathf.Max(a.y,b.y))>.01f)
                {unavailable="physics-bounds-not-current";return false;}
                if(BuildGrid.SkipCollider(c,grave)){unavailable="native-filter-excludes-placement";return false;}
                colliders.Add(c);
            }
            if(colliders.Count==0){unavailable="missing-placement-collider";return false;}
            for(int i=0;i<state.Active(support);i++){var p=state.Points[i];if(BuildGrid.IsCellBusy(new Vector2(p.X*32f,p.Y*32f),colliders))matches++;}
            return true;
        }
    }
}
