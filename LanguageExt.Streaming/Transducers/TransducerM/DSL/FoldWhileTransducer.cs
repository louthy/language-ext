using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record FoldWhileTransducerM<M, A, S>(
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    TransducerM<M, A, S>
    where M : Applicative<M> 
{
    public override ReducerM<M, A, S1> Reduce<S1>(ReducerM<M, S, S1> reducer) 
    {
        var state = State;
        return (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return M.Pure(Reduced.Done(s1));
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

record FoldWhileTransducerM2<M, A, S>(
    Schedule Schedule, 
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    TransducerM<M, A, S>
    where M : Applicative<M> 
{
    public override ReducerM<M, A, S1> Reduce<S1>(ReducerM<M, S, S1> reducer)
    {
        var state = State;
        var sch   = Duration.Zero.Cons(Schedule.Run()).GetEnumerator();
        return (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return M.Pure(Reduced.Done(s1));
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
                           return M.Pure(Reduced.Done(s1));
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
