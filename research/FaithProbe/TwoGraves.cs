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
        public void Clear(){Items.Clear();}
    }
}
