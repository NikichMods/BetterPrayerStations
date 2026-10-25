// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSMenuProbe
{
    internal static class TestResources
    {
        internal const int FaithAmount=50;
        internal static bool GrantFaith(Item payer,Action<string> log)
        {
            if(GameBalance.me.GetDataOrNull<ItemDefinition>("faith")==null)throw new InvalidOperationException("Missing faith definition");
            int before=CapacityPurchase.Count(payer);float money=payer.money;
            var bundle=new List<Item>{new Item("faith",FaithAmount)};
            if(!payer.CanAddItems(bundle,false))
            {
                bool unchanged=before==CapacityPurchase.Count(payer) && money==payer.money;
                log("TEST_FAITH result=no_capacity unchanged="+unchanged);
                if(!unchanged)throw new InvalidOperationException("Faith capacity check mutated payer");return false;
            }
            log("TEST_FAITH_BEGIN synthetic=True amount="+FaithAmount);
            bool success=payer.AddItems(bundle,true);int after=CapacityPurchase.Count(payer);
            bool exact=success && after==before+FaithAmount && payer.money==money;
            log("TEST_FAITH_END native_success="+success+" before="+before+" after="+after+" exact="+exact);
            if(!exact)throw new InvalidOperationException("Faith grant mismatch; do not retry setup");return true;
        }
    }
}
