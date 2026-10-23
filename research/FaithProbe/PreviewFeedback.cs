// SPDX-License-Identifier: MPL-2.0
using System;
using System.Globalization;
namespace NikichMods.BPSFaithProbe
{
    internal static class PreviewFeedback
    {
        public static string Text(string committedText,float delta)
        {
            if(Math.Abs(delta)<.001f)return committedText;
            return committedText+" ["+(delta>0?"4DFF4D":"FF7366")+"]"+delta.ToString("+0.#;-0.#",CultureInfo.InvariantCulture)+"[-]";
        }
    }
}
