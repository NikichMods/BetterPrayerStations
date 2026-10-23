// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSFaithProbe
{
    // Diagnostic resource setup only, not a production feature or crafting shortcut.
    internal static class StationMaterials
    {
        public static string Recipe(bool stone){return stone?"graveyard_builddesk:p:pray_stand_stn_place":"graveyard_builddesk:p:pray_stand_wd_place";}
        public static bool Grant(Item inventory,bool stone,Action<string> log)
        {
            var id=Recipe(stone);var definition=GameBalance.me.GetDataOrNull<ObjectCraftDefinition>(id);
            if(definition==null || definition.needs==null || definition.needs.Count!=3 || (definition.needs_from_wgo!=null && definition.needs_from_wgo.Count>0))throw new InvalidOperationException("Unverified station build requirements");
            var expected=new HashSet<string>(stone?new[]{"stone_plate_2","stone_plate_3","book:book_hard:2"}:new[]{"flitch","nails","book:book_hard:1"});
            var bundle=new List<Item>();var amounts=new List<int>();var ids=new List<string>();var before=new List<int>();
            foreach(var need in definition.needs)
            {
                if(need==null || !expected.Remove(need.id) || need.value<1 || need.value>100 || (need.multiquality_items!=null && need.multiquality_items.Count>0) || GameBalance.me.GetDataOrNull<ItemDefinition>(need.id)==null)throw new InvalidOperationException("Unsupported station material requirement");
                ids.Add(need.id);amounts.Add(need.value);before.Add(inventory.GetTotalCount(need.id,true));bundle.Add(new Item(need.id,need.value));
            }
            if(expected.Count!=0)throw new InvalidOperationException("Missing expected station materials");
            float money=inventory.money;int faith=inventory.GetTotalCount("faith",true);
            if(!inventory.CanAddItems(bundle,false))
            {
                bool unchanged=money==inventory.money && faith==inventory.GetTotalCount("faith",true);
                for(int i=0;i<ids.Count;i++)unchanged &= inventory.GetTotalCount(ids[i],true)==before[i];
                log("MATERIAL_GRANT recipe="+id+" result=no_capacity unchanged="+unchanged);
                if(!unchanged)throw new InvalidOperationException("Native capacity check changed material/payment state");
                return false;
            }
            log("MATERIAL_GRANT_BEGIN recipe="+id+" synthetic=True full_bundle=True");
            bool success=inventory.AddItems(bundle,true),exact=success;
            for(int i=0;i<ids.Count;i++)
            {
                int actual=inventory.GetTotalCount(ids[i],true);exact &= actual==before[i]+amounts[i];
                log("MATERIAL_GRANT_ITEM recipe="+id+" item="+ids[i]+" amount="+amounts[i]+" before="+before[i]+" after="+actual);
            }
            bool other=money==inventory.money && faith==inventory.GetTotalCount("faith",true);
            log("MATERIAL_GRANT_END recipe="+id+" native_success="+success+" exact="+exact+" money_faith_unchanged="+other);
            if(!exact || !other)throw new InvalidOperationException("Native material grant result mismatch; disable setup without retry");
            return true;
        }
    }
}