// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
namespace NikichMods.BPSMenuProbe
{
    // Bounded read-only diagnostic, called around changed-owner observations, never in a getter patch.
    internal static class PhysicalSnapshot
    {
        static readonly FieldInfo Params=typeof(Item).GetField("_params",BindingFlags.Instance|BindingFlags.NonPublic);
        static readonly FieldInfo[] Fields=typeof(GameRes).GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
        internal static string Read(Item item){var b=new StringBuilder();Append(b,item,0);return b.ToString();}
        static void Append(StringBuilder b,Item item,int depth)
        {
            if(item==null){b.Append("null;");return;}
            if(depth>8)throw new InvalidOperationException("Unexpected diagnostic inventory depth");
            b.Append(item.id).Append(':').Append(item.value).Append('{');
            var p=Params.GetValue(item);
            foreach(var field in Fields)
            {
                var value=field.GetValue(p);b.Append(field.Name).Append('=');var list=value as IEnumerable;
                if(list!=null && !(value is string))foreach(var x in list)b.Append(System.Convert.ToString(x,CultureInfo.InvariantCulture)).Append(',');
                else b.Append(System.Convert.ToString(value,CultureInfo.InvariantCulture));
                b.Append(';');
            }
            b.Append("}[");if(item.inventory!=null)foreach(var child in item.inventory)Append(b,child,depth+1);
            b.Append("][");if(item.secondary_inventory!=null)foreach(var child in item.secondary_inventory)Append(b,child,depth+1);b.Append(']');
        }
    }
}
