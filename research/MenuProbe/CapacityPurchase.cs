// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSMenuProbe
{
    // Adapted from the accepted research FaithPurchase transaction; no GUI/yield/save inside.
    internal sealed class CapacityPurchase
    {
        readonly Func<Item,int,bool> debit;
        readonly Action<string> checkpoint;
        bool running;
        internal CapacityPurchase(Func<Item,int,bool> debit=null,Action<string> checkpoint=null)
        {this.debit=debit??NativeDebit;this.checkpoint=checkpoint;}
        static bool NativeDebit(Item payer,int cost){return payer.RemoveItem("faith",cost,null);}
        internal static int Count(Item payer){return payer.GetTotalCount("faith",true);}
        internal sealed class Result{internal string Status;internal int Cost,Before,After,Purchased;}
        sealed class ListState
        {
            readonly List<Item> list;readonly Item[] items;readonly int[] values;
            internal ListState(List<Item> list)
            {
                if(list==null)throw new InvalidOperationException("Missing payer inventory");
                this.list=list;items=list.ToArray();values=new int[items.Length];
                for(int i=0;i<items.Length;i++){if(items[i]==null)throw new InvalidOperationException("Null payer entry");values[i]=items[i].value;}
            }
            internal void Restore(){list.Clear();list.AddRange(items);for(int i=0;i<items.Length;i++)items[i].value=values[i];}
            internal bool Matches(){if(list.Count!=items.Length)return false;for(int i=0;i<items.Length;i++)if(!ReferenceEquals(list[i],items[i]) || items[i].value!=values[i])return false;return true;}
        }
        static bool Same(BeamState a,BeamState b)
        {
            if(a.Purchased!=b.Purchased || a.Points.Count!=b.Points.Count)return false;
            for(int i=0;i<a.Points.Count;i++)if(a.Points[i].X!=b.Points[i].X || a.Points[i].Y!=b.Points[i].Y)return false;
            return true;
        }
        void Mark(string phase){if(checkpoint!=null)checkpoint(phase);}
        internal Result Buy(Item payer,BeamState state,int support,int quotedPurchased,int quotedCost)
        {
            if(running)return new Result{Status="busy"};running=true;
            try
            {
                var before=SavedBeamState.Read(payer);int faith=Count(payer);float money=payer.money;
                var result=new Result{Status="stale-quote",Cost=quotedCost,Before=faith,After=faith,Purchased=state.Purchased};
                if(!Same(before,state) || state.Purchased!=quotedPurchased || state.NextPrice!=quotedCost)return result;
                if(!state.CanBuy(support)){result.Status="limit";return result;}
                if(quotedCost<=0)throw new InvalidOperationException("Invalid faith price");
                if(faith<quotedCost){result.Status="insufficient";return result;}
                var proposed=new BeamState();proposed.Restore(before.Purchased,before.Points);
                if(!proposed.Buy(support))throw new InvalidOperationException("Capacity proposal rejected");
                var lists=new List<ListState>{new ListState(payer.inventory)};
                foreach(var item in payer.inventory)if(item.is_bag)lists.Add(new ListState(item.inventory));
                try
                {
                    Mark("before-debit");
                    if(!debit(payer,quotedCost) || Count(payer)!=faith-quotedCost || payer.money!=money)throw new InvalidOperationException("Native faith debit mismatch");
                    Mark("after-debit");SavedBeamState.Write(payer,proposed);Mark("after-write");
                    state.Restore(proposed.Purchased,proposed.Points);Mark("after-state");
                    if(!Same(SavedBeamState.Read(payer),state) || !Same(proposed,state) || Count(payer)!=faith-quotedCost || payer.money!=money)throw new InvalidOperationException("Capacity commit mismatch");
                    result.Status="committed";result.After=Count(payer);result.Purchased=state.Purchased;return result;
                }
                catch(Exception failure)
                {
                    bool restored=false;
                    try
                    {
                        foreach(var list in lists)list.Restore();SavedBeamState.Write(payer,before);state.Restore(before.Purchased,before.Points);
                        restored=Count(payer)==faith && payer.money==money && Same(SavedBeamState.Read(payer),before) && Same(state,before);
                        foreach(var list in lists)restored &= list.Matches();
                    }
                    catch(Exception rollback){throw new AggregateException("Purchase rollback FAILED",failure,rollback);}
                    throw new InvalidOperationException("Purchase failed; rollback="+(restored?"verified":"FAILED"),failure);
                }
            }
            finally{running=false;}
        }
    }
}
