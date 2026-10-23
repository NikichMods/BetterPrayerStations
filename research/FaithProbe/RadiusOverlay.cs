// SPDX-License-Identifier: MPL-2.0
using System;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    // Screen-only diagnostic; no world resources or input interception.
    internal sealed class RadiusOverlay
    {
        const int Maximum=512;
        readonly Vector2[,] points=new Vector2[Maximum,5];
        readonly bool[] valid=new bool[Maximum],fill=new bool[Maximum];
        readonly Color[] colors=new Color[Maximum];
        int count;Camera camera;
        public void Clear(){count=0;camera=null;}
        void Add(Rect rect,float z,Color color,bool filled)
        {
            if(count>=Maximum)return;valid[count]=true;colors[count]=color;fill[count]=filled;
            for(int i=0;i<5;i++)
            {
                var p=camera.WorldToScreenPoint(new Vector3(i==0 || i==3 || i==4?rect.xMin:rect.xMax,i<2 || i==4?rect.yMin:rect.yMax,z));
                if(p.z<=0)valid[count]=false;points[count,i]=new Vector2(p.x,Screen.height-p.y);
            }
            count++;
        }
        public void Update(PrayerCoverage coverage,bool enabled,Vector2 player)
        {
            count=0;if(!enabled)return;if(camera==null)camera=Camera.main;if(camera==null)return;
            WorldGameObject station=coverage.HasPreview?coverage.Owners.Preview:null;
            if(station==null)
            {
                float nearest=128f*128f;
                foreach(var s in coverage.Owners.Built)
                {
                    Rect r;if(s==null || s.is_removed || s.components.craft.is_crafting || !coverage.Geometry.TryRect(s,out r))continue;
                    float d=(r.center-player).sqrMagnitude;if(d<nearest){nearest=d;station=s;}
                }
            }
            if(station==null)return;
            Rect footprint;if(!coverage.Geometry.TryRect(station,out footprint))return;
            float half=PrayerCoverage.HalfSide(station.obj_id);var center=footprint.center;
            var area=Rect.MinMaxRect(center.x-half,center.y-half,center.x+half,center.y+half);
            Add(area,station.transform.position.z,coverage.HasPreview?new Color(1f,.8f,.15f,.95f):new Color(.2f,.85f,1f,.8f),false);
            foreach(var g in coverage.GraveOwners)
            {
                Rect r;if(!coverage.Geometry.TryRect(g,out r))continue;
                if(PrayerCoverage.Inside(r.center.x-center.x,r.center.y-center.y,half))Add(r,g.transform.position.z,new Color(.15f,1f,.35f,.22f),true);
            }
        }
        public void Draw()
        {
            if(Event.current.type!=EventType.Repaint)return;
            var matrix=GUI.matrix;var color=GUI.color;
            try
            {
                for(int k=0;k<count;k++)if(valid[k])
                {
                    if(fill[k])
                    {
                        var a=points[k,0];var b=points[k,2];GUI.matrix=matrix;GUI.color=colors[k];
                        GUI.DrawTexture(Rect.MinMaxRect(Math.Min(a.x,b.x),Math.Min(a.y,b.y),Math.Max(a.x,b.x),Math.Max(a.y,b.y)),Texture2D.whiteTexture);
                    }
                    else for(int i=0;i<4;i++)
                    {
                        var a=points[k,i];var d=points[k,i+1]-a;GUI.matrix=matrix;GUI.color=colors[k];
                        GUIUtility.RotateAroundPivot(Mathf.Atan2(d.y,d.x)*Mathf.Rad2Deg,a);
                        GUI.DrawTexture(new Rect(a.x,a.y-1f,d.magnitude,2f),Texture2D.whiteTexture);
                    }
                }
            }
            finally{GUI.matrix=matrix;GUI.color=color;}
        }
    }
}
