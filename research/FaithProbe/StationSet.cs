// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSFaithProbe
{
    // Reference identity: one committed source can be projected to one preview position.
    internal sealed class StationSet<T> where T:class
    {
        public readonly List<T> Built=new List<T>();
        public T Preview, Source;
        public void Register(T item)
        {if(item==null || ReferenceEquals(item,Preview))return;foreach(var x in Built)if(ReferenceEquals(x,item))return;Built.Add(item);}
        public void Forget(T item){for(int i=Built.Count-1;i>=0;i--)if(ReferenceEquals(Built[i],item))Built.RemoveAt(i);}
        public void Begin(T preview,T source){Forget(preview);Preview=preview;Source=source;}
        public void End(T preview,bool leave)
        {Forget(preview);if(ReferenceEquals(preview,Preview)){Preview=null;Source=null;}if(leave)Register(preview);}
        public bool Suppressed(T item,bool stationPreview){return stationPreview && ReferenceEquals(Source,item);}
        public void Clear(){Built.Clear();Preview=Source=null;}
    }
}