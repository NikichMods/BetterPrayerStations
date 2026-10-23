// SPDX-License-Identifier: MPL-2.0
using System;
namespace NikichMods.BPSFaithProbe
{
    internal sealed class PreviewBaseline
    {
        public readonly float Quality;
        public string Text {get;private set;}
        public PreviewBaseline(float quality,string text){Quality=quality;Text=text;}
        public bool AcceptText(float committedQuality,string text)
        {
            if(Text!=null)return true;
            if(text==null)return false;
            if(Math.Abs(committedQuality-Quality)>.001f)throw new InvalidOperationException("Committed quality changed before native preview text became ready");
            Text=text;return true;
        }
    }
}
