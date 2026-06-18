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
        // TODO: This needs checking since it's changed to an IteratorIO
        var state = State;
        var sch   = Duration.Zero.Cons(Schedule.Run()).ForwardIteratorIO();
        return (s1, x) =>
               {
                   if (Pred(state, x))
                   {
                       state = Folder(state, x);
                       return M.Pure(Reduced.Done(s1));
                   }
                   else
                   {
                       var nxt = sch.NextIO().Run();
                       
                       // Schedule
                       if (nxt is (Exist<Duration> (var d), var tail))
                       {
                           sch = tail;
                           if (!d.IsZero) Task.Delay((TimeSpan)d).GetAwaiter().GetResult();
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
