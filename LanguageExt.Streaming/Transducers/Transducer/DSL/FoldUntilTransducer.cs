using System;
using System.Threading.Tasks;

namespace LanguageExt;

record FoldUntilTransducer<A, S>(
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    Transducer<A, S>
{
    public override ReducerAsync<A, S1> Reduce<S1>(ReducerAsync<S, S1> reducer)
    {
        var state = State;
        return async (s1, x) =>
               {
                   state = Folder(state, x);
                   if (Pred(state, x))
                   {
                       switch (await reducer(s1, state))
                       {
                           case { Continue: true, Value: var nstate }:
                               state = State; // reset
                               return Reduced.Continue(nstate);
                           
                           case { Value: var nstate }:
                               return Reduced.Done(nstate);
                       }
                   }
                   else
                   {
                       return Reduced.Continue(s1);
                   }
               };
    }
}

record FoldUntilTransducer2<A, S>(
    Schedule Schedule,
    Func<S, A, S> Folder,
    Func<S, A, bool> Pred,
    S State) :
    Transducer<A, S>
{
    public override ReducerAsync<A, S1> Reduce<S1>(ReducerAsync<S, S1> reducer)
    {
        var state = State;
        var sch   = Duration.Zero.Cons(Schedule.Run()).GetEnumerator();
        return async (s1, x) =>
               {
                   state = Folder(state, x);
                   if (Pred(state, x))
                   {
                       // Schedule
                       if (sch.MoveNext())
                       {
                           if (!sch.Current.IsZero) await Task.Delay((TimeSpan)sch.Current);
                       }
                       else
                       {
                           return Reduced.Done(s1);
                       }

                       switch (await reducer(s1, state))
                       {
                           case { Continue: true, Value: var nstate }:
                               state = State; // reset
                               return Reduced.Continue(nstate);

                           case { Value: var nstate }:
                               return Reduced.Done(nstate);
                       }
                   }
                   else
                   {
                       return Reduced.Continue(s1);
                   }
               };
    }
}
