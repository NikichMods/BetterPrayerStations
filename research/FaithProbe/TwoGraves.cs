// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSFaithProbe
{
    // Explicit two-owner research boundary; selection is not effect authorization.
    internal sealed class TwoGraves
    {
        public readonly List<WorldGameObject> Items=new List<WorldGameObject>();
        public bool Contains(WorldGameObject grave)
        {foreach(var item in Items)if(ReferenceEquals(item,grave))return true;return false;}
        public void Add(WorldGameObject grave)
        {
            if(ReferenceEquals(grave,null))throw new ArgumentNullException("grave");
            if(Contains(grave))return;
            if(Items.Count>=2)throw new InvalidOperationException("Two diagnostic graves already registered");
            foreach(var item in Items)if(item.unique_id==grave.unique_id)throw new InvalidOperationException("Duplicate live grave identity");
            Items.Add(grave);Items.Sort((a,b)=>a.unique_id.CompareTo(b.unique_id));
        }
        public static int SlotOf(WorldGameObject grave)
        {
            float slot=grave.data.GetParam(SavedFaithState.SlotKey,0f);
            if(slot!=0f && slot!=1f && slot!=2f)throw new InvalidOperationException("Unknown diagnostic grave slot");
            return (int)slot;
        }
        public WorldGameObject ForSlot(int slot)
        {
            WorldGameObject result=null;
            foreach(var g in Items)if(SlotOf(g)==slot)
            {if(!ReferenceEquals(result,null))throw new InvalidOperationException("Duplicate saved diagnostic slot");result=g;}
            return result;
        }
        public bool Assign(WorldGameObject grave,int slot)
        {
            if(slot!=1 && slot!=2)throw new ArgumentOutOfRangeException("slot");
            var existing=ForSlot(slot);int previous=SlotOf(grave);
            if((!ReferenceEquals(existing,null) && !ReferenceEquals(existing,grave)) || (previous!=0 && previous!=slot))return false;
            Add(grave);grave.data.SetParam(SavedFaithState.SlotKey,slot);return true;
        }
        public void Clear(){Items.Clear();}
    }
}
