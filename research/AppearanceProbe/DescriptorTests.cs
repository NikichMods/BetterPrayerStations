// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

static class DescriptorTests
{
    static void Assert(bool condition, string label) { if (!condition) throw new Exception(label); }
    static int Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
        {
            string name = new AssemblyName(e.Name).Name + ".dll";
            foreach (string dir in new[] { args[0], args[1], Path.GetDirectoryName(args[2]) })
            {
                string path = Path.Combine(dir, name);
                if (File.Exists(path)) return Assembly.LoadFrom(path);
            }
            return null;
        };
        try { return Run(args[2]); }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    static int Run(string path)
    {
        var assembly = Assembly.LoadFrom(path);
        var type = assembly.GetType("NikichMods.BPSProbe.Plugin", true);
        string version = (string)type.GetField("Version").GetRawConstantValue();
        Assert(assembly.GetName().Version.ToString() == version + ".0", "Version owner differs from assembly metadata");
        var method = type.GetMethod("ProjectParts", BindingFlags.NonPublic | BindingFlags.Static);
        Action<IList<WOPPrefabName>> project = parts => method.Invoke(null, new object[] { parts, "actual_stone", "desired_wood" });
        var source = new WOPPrefabName { part_1 = "actual_stone_stg_", stage = 3, part_2 = "_suffix", need_herb = true };
        var fence = new WOPPrefabName { part_1 = "real_fence_stg_", stage = 2 };
        var list = new List<WOPPrefabName> { fence, source };
        project(list);
        Assert(list.Count == 2 && ReferenceEquals(list[0], fence), "Unrelated fence altered");
        Assert(source.GetName() == "actual_stone_stg_3_suffix", "Original descriptor/cache object mutated");
        Assert(list[1].GetName() == "desired_wood_stg_3_suffix" && list[1].need_herb, "Native stage/suffix/herb composition lost");
        Assert(!source.EqualsTo(list[1]), "Native cache comparison cannot detect projected appearance");
        var again = new List<WOPPrefabName> { new WOPPrefabName { part_1 = "actual_stone_stg_", stage = 3, part_2 = "_suffix", need_herb = true } };
        project(again);
        Assert(again[0].EqualsTo(list[1]), "Same projected input creates false cache changes");
        var placeholder = new WOPPrefabName { part_1 = "grave_top_building_1_stg_1" };
        var construction = new List<WOPPrefabName> { placeholder };
        project(construction);
        Assert(ReferenceEquals(construction[0], placeholder), "Construction placeholder overwritten");
        var empty = new List<WOPPrefabName>();
        project(empty);
        Assert(empty.Count == 0, "Empty slot conjures a visual part");
        var unsupported = new List<WOPPrefabName> { null, fence };
        project(unsupported);
        Assert(unsupported.Count == 2 && unsupported[0] == null && ReferenceEquals(unsupported[1], fence), "Unsupported entries changed");
        Console.WriteLine("PASS descriptor projection: native names/cache comparison, stages/herbs, original descriptor preservation, unrelated fence, construction and empty slots; version identity.");
        Console.WriteLine("LIMIT no Unity scene, native resource instantiation, GUI/load lifecycle or in-game acceptance.");
        return 0;
    }
}
