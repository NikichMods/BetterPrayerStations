// SPDX-License-Identifier: MPL-2.0
using System;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
namespace NikichMods.BPSFaithProbe
{
    // Read-only adapter for the exact installed Move Objects0.6.0, no dependency or writes.
    internal sealed class MovePreviewOwner
    {
        FieldInfo selected,active;bool initialized;
        public string Identity="absent; native construction preview only";
        public void Initialize()
        {
            if(initialized)return;initialized=true;
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly.GetName().Name!="GYKMoveMod")continue;
                string hash;using(var stream=File.OpenRead(assembly.Location))using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant();
                if(hash!="c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0")throw new InvalidOperationException("Unverified Move Objects preview contract "+hash);
                var type=assembly.GetType("MoveObjectsPlugin",true);var flags=BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly;
                selected=type.GetField("SelectedObjectToMove",flags);active=type.GetField("MoveModeActive",flags);
                if(selected==null || selected.FieldType!=typeof(WorldGameObject) || active==null || active.FieldType!=typeof(bool))throw new MissingFieldException("Move Objects preview source");
                Identity="Move Objects0.6.0 sha256="+hash;return;
            }
        }
        public WorldGameObject Read(){return active!=null && (bool)active.GetValue(null)?selected.GetValue(null) as WorldGameObject:null;}
    }
}