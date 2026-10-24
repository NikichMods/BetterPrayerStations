// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    // GK1.407 placement boxes, not sprite bounds. Cached per current object definition.
    internal sealed class GridFootprint
    {
        public const float Cell=32f,HalfCheck=14.4f;
        sealed class Shape {public string Id;public WorldObjectPart Part;public Transform Transform;public Vector2 Offset,Size;}
        readonly Dictionary<WorldGameObject,Shape> shapes=new Dictionary<WorldGameObject,Shape>();
        readonly bool includeInactivePlacement;
        public GridFootprint():this(false){}
        public GridFootprint(bool includeInactivePlacement){this.includeInactivePlacement=includeInactivePlacement;}
        static readonly FieldInfo Cells=typeof(WorldGameObject).GetField("_cells",BindingFlags.Instance|BindingFlags.NonPublic);
        public void Clear(){shapes.Clear();}
        public void Forget(WorldGameObject w){if(!ReferenceEquals(w,null))shapes.Remove(w);}
        public static Rect Snap(float minX,float minY,float maxX,float maxY)
        {
            float x=Mathf.Ceil((minX-HalfCheck+.001f)/Cell)*Cell-Cell/2f;
            float y=Mathf.Ceil((minY-HalfCheck+.001f)/Cell)*Cell-Cell/2f;
            float right=Mathf.Floor((maxX+HalfCheck-.001f)/Cell)*Cell+Cell/2f;
            float top=Mathf.Floor((maxY+HalfCheck-.001f)/Cell)*Cell+Cell/2f;
            return Rect.MinMaxRect(x,y,right,top);
        }
        public bool TryRect(WorldGameObject w,out Rect rect)
        {
            rect=new Rect();if(w==null || w.is_removed)return false;
            var part=w.wop;if(part==null)return false;
            Shape shape;
            if(!shapes.TryGetValue(w,out shape) || shape.Id!=w.obj_id || !ReferenceEquals(shape.Part,part) || shape.Transform==null)
            {
                BoxCollider2D found=null;
                foreach(var c in part.GetComponentsInChildren<Collider2D>(true))
                {
                    // Global effects also need hidden chunks. Native SkipCollider consults
                    // an active-only, ID-cached discovery; do not seed that cache offscreen.
                    if(c.gameObject.layer!=23 || (!includeInactivePlacement && BuildGrid.SkipCollider(c,w)))continue;
                    var box=c as BoxCollider2D;
                    if(box==null || found!=null)throw new InvalidOperationException("Expected one native layer23 placement box: "+w.obj_id);
                    found=box;
                }
                if(found==null)return false;
                shape=new Shape{Id=w.obj_id,Part=part,Transform=found.transform,Offset=found.offset,Size=found.size};shapes[w]=shape;
            }
            var t=shape.Transform;var a=t.TransformPoint(shape.Offset-shape.Size/2f);var b=t.TransformPoint(shape.Offset+shape.Size/2f);
            if(Math.Abs(t.eulerAngles.z)>.01f && Math.Abs(t.eulerAngles.z-360f)>.01f)throw new InvalidOperationException("Rotated placement box is outside diagnostic geometry");
            rect=Snap(Math.Min(a.x,b.x),Math.Min(a.y,b.y),Math.Max(a.x,b.x),Math.Max(a.y,b.y));
            return rect.width>0 && rect.height>0;
        }
        public string NativeCheck(WorldGameObject w)
        {
            Rect rect;if(!TryRect(w,out rect))return "geometry=missing";
            if(w.GetComponent<FloatingWorldGameObject>()!=null)return "geometry=current-wop-box native_cells=not-compared-floating center="+rect.center+" size="+rect.size;
            var cells=Cells.GetValue(w) as List<DungeonGenerator.IntVector2>;
            if(cells==null || cells.Count==0)return "geometry=box native_cells=empty";
            int minX=int.MaxValue,minY=int.MaxValue,maxX=int.MinValue,maxY=int.MinValue;
            foreach(var c in cells){minX=Math.Min(minX,c.x);maxX=Math.Max(maxX,c.x);minY=Math.Min(minY,c.y);maxY=Math.Max(maxY,c.y);}
            bool same=Math.Abs(rect.xMin-(minX*Cell-16))<.01f && Math.Abs(rect.xMax-(maxX*Cell+16))<.01f && Math.Abs(rect.yMin-(minY*Cell-16))<.01f && Math.Abs(rect.yMax-(maxY*Cell+16))<.01f;
            return "geometry=box native_cells="+cells.Count+" native_bounds_match="+same+" center="+rect.center+" size="+rect.size;
        }
    }
}
