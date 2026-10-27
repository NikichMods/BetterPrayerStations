// SPDX-License-Identifier: MPL-2.0
using System;
using System.Globalization;
using UnityEngine;

namespace NikichMods.BPSMenuProbe
{
    // Read-only, event-bound evidence for the shared native construction window.
    // No widget/anchor/scroll setters: this candidate must reproduce 0.5.0 geometry.
    internal static class MenuLayoutTrace
    {
        static string Number(float value){return value.ToString("0.###",CultureInfo.InvariantCulture);}
        static string Position(Vector3 value){return Number(value.x)+","+Number(value.y);}
        static string Node(string role,Transform value,Transform root)
        {
            if(value==null)return " "+role+"=none";
            var widget=value.GetComponent<UIWidget>();
            return " "+role+"="+value.GetInstanceID()+":local("+Position(value.localPosition)+")/root("+Position(root.InverseTransformPoint(value.position))+")"+
                (widget==null?"":"/size("+widget.width+","+widget.height+")");
        }
        internal static string Snapshot(CraftGUI menu,string phase,bool owned)
        {
            var root=menu.transform;
            var scroll=menu.scroll_view;
            var panel=scroll==null?null:scroll.GetComponent<UIPanel>();
            var headerTable=menu.object_info==null?null:menu.object_info.table;
            var row=menu.list_items.Count==0?null:menu.list_items[0];
            string result="LAYOUT_TRACE phase="+phase+" frame="+Time.frameCount+" owned="+owned+" gamepad="+LazyInput.gamepad_active+" shown="+menu.is_shown+" rows="+menu.list_items.Count+
                Node("window",root,root)+
                Node("list",menu.craft_list_grid==null?null:menu.craft_list_grid.transform,root)+
                Node("scroll",scroll==null?null:scroll.transform,root)+
                Node("template",menu.craft_item_prefab==null?null:menu.craft_item_prefab.transform,root)+
                Node("first",row==null?null:row.transform,root)+
                Node("header",headerTable==null?null:headerTable.transform,root)+
                Node("header_back",headerTable==null?null:headerTable.transform.parent,root)+
                Node("header_reserve",menu.header_height_widget==null?null:menu.header_height_widget.transform,root);
            if(panel!=null)
            {
                var clip=panel.baseClipRegion;var offset=panel.clipOffset;
                result+=" clip="+Number(clip.x)+","+Number(clip.y)+","+Number(clip.z)+","+Number(clip.w)+" clip_offset="+Number(offset.x)+","+Number(offset.y);
            }
            return result;
        }
    }
}
