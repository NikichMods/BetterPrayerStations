// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
namespace NikichMods.BPSMenuProbe
{
    internal static class SavedBeamState
    {
        internal const string Prefix="nikichmods.bps.menu_probe.beams.";
        static readonly string[] Keys=MakeKeys();
        static string[] MakeKeys()
        {
            var keys=new List<string>{Prefix+"schema",Prefix+"purchased",Prefix+"count"};
            for(int i=0;i<BeamState.Prices.Length;i++){keys.Add(Prefix+"x"+i);keys.Add(Prefix+"y"+i);}return keys.ToArray();
        }
        internal static bool IsOwned(string key){return Array.IndexOf(Keys,key)>=0;}
        static int Integer(float value,int min,int max)
        {
            if(float.IsNaN(value) || float.IsInfinity(value) || value<min || value>max || value!=Math.Floor(value))throw new InvalidOperationException("Invalid saved beam number");
            return (int)value;
        }
        internal static BeamState Read(Item owner){if(owner==null)throw new ArgumentNullException("owner");return Decode(key=>owner.GetParam(key,0));}
        internal static BeamState Decode(Func<string,float> read)
        {
            float schema=read(Keys[0]);var state=new BeamState();
            if(schema==0)
            {
                foreach(var key in Keys)if(read(key)!=0)throw new InvalidOperationException("Incomplete saved beam schema");
                return state;
            }
            if(schema!=1)throw new InvalidOperationException("Unknown saved beam schema");
            int p=Integer(read(Keys[1]),0,BeamState.Prices.Length),n=Integer(read(Keys[2]),0,BeamState.Prices.Length);
            var points=new List<BeamState.Point>();
            for(int i=0;i<n;i++)points.Add(new BeamState.Point{X=Integer(read(Prefix+"x"+i),-1000000,1000000),Y=Integer(read(Prefix+"y"+i),-1000000,1000000)});
            for(int i=n;i<BeamState.Prices.Length;i++)if(read(Prefix+"x"+i)!=0 || read(Prefix+"y"+i)!=0)throw new InvalidOperationException("Orphaned saved beam coordinates");
            state.Restore(p,points);return state;
        }
        static void SetOwned(Item owner,string key,float value)
        {if(!IsOwned(key))throw new InvalidOperationException("Foreign saved beam key");owner.SetParam(key,value);}
        internal static void Write(Item owner,BeamState state)
        {Commit(key=>owner.GetParam(key,0),(key,value)=>SetOwned(owner,key,value),state);}
        internal static void Commit(Func<string,float> read,Action<string,float> set,BeamState state)
        {
            Decode(read);var validated=new BeamState();validated.Restore(state.Purchased,state.Points);
            var before=new float[Keys.Length];var after=new float[Keys.Length];
            for(int i=0;i<Keys.Length;i++)before[i]=read(Keys[i]);
            after[0]=validated.Purchased==0?0:1;after[1]=validated.Purchased;after[2]=validated.Points.Count;
            for(int i=0;i<validated.Points.Count;i++){after[3+i*2]=validated.Points[i].X;after[4+i*2]=validated.Points[i].Y;}
            try
            {
                set(Keys[0],0);
                for(int i=1;i<Keys.Length;i++)set(Keys[i],after[i]);
                set(Keys[0],after[0]);
            }
            catch(Exception writeFailure)
            {
                try{for(int i=1;i<Keys.Length;i++)set(Keys[i],before[i]);set(Keys[0],before[0]);}
                catch(Exception rollbackFailure){throw new AggregateException("Saved beam write and rollback failed; data preserved for diagnosis",writeFailure,rollbackFailure);}
                throw;
            }
        }
    }
}
