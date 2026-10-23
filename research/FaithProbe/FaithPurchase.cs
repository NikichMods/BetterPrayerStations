// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSFaithProbe
{
    // Research transaction. Synchronous main-thread operation; no GUI/save/yield inside.
    internal sealed class FaithPurchase
    {
        readonly Func<Item,int,bool> debit;
        readonly Action<string> checkpoint;
        bool running;
        public FaithPurchase(Func<Item,int,bool> debit=null,Action<string> checkpoint=null)
        {this.debit=debit??NativeDebit;this.checkpoint=checkpoint;}
        static bool NativeDebit(Item payer,int amount){return payer.RemoveItem("faith",amount,null);}
        public static int Count(Item payer){return payer.GetTotalCount("faith",true);}
        public static int Price(int actions)
        {if(actions<0 || actions>SavedFaithState.Limit*2)throw new InvalidOperationException("Invalid action quote");return actions+1;}
        public struct Result
        {
            public string Status;
            public int Cost,FaithBefore,FaithAfter,ActionsBefore,ActionsAfter;
        }
        sealed class ListState
        {
            readonly List<Item> list;
            readonly Item[] items;
            readonly int[] values;
            public ListState(List<Item> list)
            {
                if(list==null)throw new InvalidOperationException("Missing native inventory list");
                this.list=list;items=list.ToArray();values=new int[items.Length];
                for(int i=0;i<items.Length;i++){if(items[i]==null)throw new InvalidOperationException("Null native inventory entry");values[i]=items[i].value;}
            }
            public void Restore(){list.Clear();list.AddRange(items);for(int i=0;i<items.Length;i++)items[i].value=values[i];}
            public bool Matches()
            {if(list.Count!=items.Length)return false;for(int i=0;i<items.Length;i++)if(!ReferenceEquals(list[i],items[i]) || items[i].value!=values[i])return false;return true;}
        }
        void Mark(string name){if(checkpoint!=null)checkpoint(name);}
        public Result Buy(Item grave,Item payer,bool conversion,int quotedActions,int quotedCost,bool shortageTest=false)
        {
            if(running)return new Result{Status="busy"};
            running=true;
            try
            {
                var before=SavedFaithState.Read(grave);int count=Count(payer);
                var result=new Result{Status="stale-quote",Cost=quotedCost,FaithBefore=count,FaithAfter=count,ActionsBefore=before.Actions,ActionsAfter=before.Actions};
                if(before.Actions!=quotedActions || (!shortageTest && quotedCost!=Price(before.Actions)))return result;
                if(quotedCost<=0)throw new InvalidOperationException("Nonpositive diagnostic quote");
                // This special quote proves the actual shortage branch without moving inventory.
                if(shortageTest && quotedCost!=checked(count+1))return result;
                if((conversion?before.Conversions:before.Wreaths)>=SavedFaithState.Limit){result.Status="limit";return result;}
                if(count<quotedCost){result.Status="insufficient";return result;}
                if(shortageTest)throw new InvalidOperationException("Shortage-test quote unexpectedly affordable");
                float schema=grave.GetParam(SavedFaithState.SchemaKey,0f),c=grave.GetParam(SavedFaithState.ConversionKey,0f),l=grave.GetParam(SavedFaithState.WreathKey,0f);
                var lists=new List<ListState>{new ListState(payer.inventory)};
                foreach(var item in payer.inventory)if(item.is_bag)lists.Add(new ListState(item.inventory));
                try
                {
                    Mark("before-debit");
                    if(!debit(payer,quotedCost) || Count(payer)!=count-quotedCost)throw new InvalidOperationException("Native faith debit did not match quote");
                    Mark("after-debit");
                    SavedFaithState.Write(grave,before.Conversions+(conversion?1:0),before.Wreaths+(conversion?0:1));
                    Mark("after-state");
                    var after=SavedFaithState.Read(grave);
                    if(after.Actions!=before.Actions+1 || after.Conversions!=before.Conversions+(conversion?1:0) || after.Wreaths!=before.Wreaths+(conversion?0:1) || Count(payer)!=count-quotedCost)
                        throw new InvalidOperationException("Purchase postcondition mismatch");
                    result.Status="committed";result.FaithAfter=Count(payer);result.ActionsAfter=after.Actions;return result;
                }
                catch(Exception ex)
                {
                    foreach(var list in lists)list.Restore();
                    grave.SetParam(SavedFaithState.SchemaKey,schema);grave.SetParam(SavedFaithState.ConversionKey,c);grave.SetParam(SavedFaithState.WreathKey,l);
                    bool restored;
                    // Compare owned values and original list order/references/quantities explicitly.
                    restored=Count(payer)==count && grave.GetParam(SavedFaithState.SchemaKey,0)==schema && grave.GetParam(SavedFaithState.ConversionKey,0)==c && grave.GetParam(SavedFaithState.WreathKey,0)==l;
                    foreach(var list in lists)restored &= list.Matches();
                    throw new InvalidOperationException("Purchase failed; rollback="+(restored?"verified":"FAILED"),ex);
                }
            }
            finally{running=false;}
        }
    }
}
