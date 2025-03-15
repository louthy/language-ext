using System;
using System.Threading.Tasks;

namespace LanguageExt;

record FoldWhileTransducer<A, S>(
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    Transducer<A, S>
{
    public override Reducer<A, S1> Reduce<S1>(Reducer<S, S1> reducer)
    {
        var state = State;
        return async (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return Reduced.Continue(s1);
                   }
                   else
                   {
                       switch (await reducer(s1, state))
                       {
                           case { Continue: true, Value: var nstate }:
                               state = Folder(State /* reset */, x);
                               return Reduced.Continue(nstate);
                           
                           case { Value: var nstate }:
                               return Reduced.Done(nstate);
                       }
                   }
               };
    }

    public override ReducerM<M, A, S1> ReduceM<M, S1>(ReducerM<M, S, S1> reducer) 
    {
        var state = State;
        return (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return M.Pure(s1);
                   }
                   else
                   {
                       return reducer(s1, state)
                          .Map(ns =>
                               {
                                   state = Folder(State /* reset */, x);
                                   return ns;
                               });
                   }
               };
    }
}

record FoldWhileTransducer2<A, S>(
    Schedule Schedule, 
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    Transducer<A, S>
{
    public override Reducer<A, S1> Reduce<S1>(Reducer<S, S1> reducer)
    {
        var state = State;
        var sch = Duration.Zero.Cons(Schedule.Run()).GetEnumerator();
        return async (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return Reduced.Continue(s1);
                   }
                   else
                   {
                       // Schedule
                       if (sch.MoveNext())
                       {
                           if(!sch.Current.IsZero) await Task.Delay((TimeSpan)sch.Current);
                       }
                       else
                       {
                           return Reduced.Done(s1);
                       }

                       switch (await reducer(s1, state))
                       {
                           case { Continue: true, Value: var nstate }:
                               state = Folder(State /* reset */, x);
                               return Reduced.Continue(nstate);
                           
                           case { Value: var nstate }:
                               return Reduced.Done(nstate);
                       }
                   }
               };
    }

    public override ReducerM<M, A, S1> ReduceM<M, S1>(ReducerM<M, S, S1> reducer)
    {
        var state = State;
        var sch   = Duration.Zero.Cons(Schedule.Run()).GetEnumerator();
        return (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return M.Pure(s1);
                   }
                   else
                   {
                       // Schedule
                       if (sch.MoveNext())
                       {
                           if (!sch.Current.IsZero) Task.Delay((TimeSpan)sch.Current).GetAwaiter().GetResult();
                       }
                       else
                       {
                           return M.Pure(s1);
                       }

                       return reducer(s1, state)
                          .Map(ns =>
                               {
                                   state = Folder(State /* reset */, x);
                                   return ns;
                               });
                   }
               };
    }
}
