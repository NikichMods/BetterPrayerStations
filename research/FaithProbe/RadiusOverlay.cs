// SPDX-License-Identifier: MPL-2.0
using System;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    // Disposable screen overlay only: no world objects, textures, materials or input handlers.
    internal sealed class RadiusOverlay
    {
        const int Segments=64,Maximum=32;
        readonly Vector2[,] points=new Vector2[Maximum,Segments+1];
        readonly bool[] valid=new bool[Maximum];
        readonly Color[] colors=new Color[Maximum];
        int count;Camera camera;
        public void Clear(){count=0;camera=null;}
        void Add(WorldGameObject station,bool preview)
        {
            if(station==null || station.is_removed || count>=Maximum)return;
            Vector3 center=station.transform.position;valid[count]=true;colors[count]=preview?new Color(1f,.8f,.15f,.95f):new Color(.2f,.85f,1f,.8f);
            for(int i=0;i<=Segments;i++)
            {
                float a=i*(2f*Mathf.PI/Segments);var p=camera.WorldToScreenPoint(center+new Vector3(Mathf.Cos(a)*PrayerCoverage.Radius,Mathf.Sin(a)*PrayerCoverage.Radius,0));
                if(p.z<=0)valid[count]=false;points[count,i]=new Vector2(p.x,Screen.height-p.y);
            }
            count++;
        }
        public void Update(PrayerCoverage coverage,bool enabled)
        {
            count=0;if(!enabled)return;if(camera==null)camera=Camera.main;if(camera==null)return;
            bool preview=coverage.HasPreview;
            foreach(var s in coverage.Owners.Built)if(!coverage.Owners.Suppressed(s,preview))Add(s,false);
            if(preview)Add(coverage.Owners.Preview,true);
        }
        public void Draw()
        {
            if(Event.current.type!=EventType.Repaint)return;
            var matrix=GUI.matrix;var color=GUI.color;
            try
            {
                for(int k=0;k<count;k++)if(valid[k])for(int i=0;i<Segments;i++)
                {
                    var a=points[k,i];var d=points[k,i+1]-a;GUI.matrix=matrix;GUI.color=colors[k];
                    GUIUtility.RotateAroundPivot(Mathf.Atan2(d.y,d.x)*Mathf.Rad2Deg,a);
                    GUI.DrawTexture(new Rect(a.x,a.y-1f,d.magnitude,2f),Texture2D.whiteTexture);
                }
            }
            finally{GUI.matrix=matrix;GUI.color=color;}
        }
    }
}