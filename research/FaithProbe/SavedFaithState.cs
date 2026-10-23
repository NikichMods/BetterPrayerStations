// SPDX-License-Identifier: MPL-2.0
using System;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    // Diagnostic schema only. No production saved-format promise or economy is defined.
    internal static class SavedFaithState
    {
        public const string SchemaKey = "nikichmods.bps.faith_probe.schema";
        public const string ConversionKey = "nikichmods.bps.faith_probe.conversions";
        public const string SlotKey = "nikichmods.bps.faith_probe.slot";
        public const string WreathKey = "nikichmods.bps.faith_probe.wreaths";
        public const int Limit = 12;
        internal struct Value
        {
            public readonly int Conversions, Wreaths;
            public int Actions { get { return Conversions + Wreaths; } }
            public Value(int conversions, int wreaths) { Conversions = conversions; Wreaths = wreaths; }
        }
        static int Count(float value)
        {
            if (float.IsNaN(value) || value < 0 || value > Limit || value != Mathf.Floor(value))
                throw new InvalidOperationException("Invalid diagnostic investment count");
            return (int)value;
        }
        public static bool IsOwned(string name) { return name == SchemaKey || name == ConversionKey || name == WreathKey || name == SlotKey; }
        public static Value Read(Item owner)
        {
            float schema = owner.GetParam(SchemaKey, 0f);
            if (schema == 0f) return new Value(0,0);
            if (schema != 1f) throw new InvalidOperationException("Unknown diagnostic investment schema");
            return new Value(Count(owner.GetParam(ConversionKey,0f)), Count(owner.GetParam(WreathKey,0f)));
        }
        public static void Write(Item owner, int conversions, int wreaths)
        {
            Count(conversions); Count(wreaths);
            owner.SetParam(SchemaKey,1f);
            owner.SetParam(ConversionKey,conversions);
            owner.SetParam(WreathKey,wreaths);
        }
        public static void Clear(Item owner)
        {
            owner.SetParam(ConversionKey,0f); owner.SetParam(WreathKey,0f); owner.SetParam(SchemaKey,0f);owner.SetParam(SlotKey,0f);
        }
        public static void Convert(ref int red, ref int white, ref int available, float durability, int count)
        {
            int changed = Math.Min(count,red);
            if (changed <= 0) return;
            red -= changed; white += changed;
            int percent = Mathf.CeilToInt(durability*100f);
            if (percent>90) percent=100;
            available=Mathf.FloorToInt((float)(white*percent)/100f);
        }
        // Caller owns live target/save/activation authorization; these methods own only
        // grave-backed data projection. They never write a body, organ or decoration.
        public static void ApplySkulls(Item owner, Item body, bool active, ref int red, ref int white, ref int available)
        {
            if (!active) return;
            var state=Read(owner);
            Convert(ref red,ref white,ref available,body.durability,state.Conversions);
        }
        public static float ApplyDecoration(Item owner, float nativeValue, bool active)
        {
            return nativeValue+(active?Read(owner).Wreaths:0);
        }
    }
}
