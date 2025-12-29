using System;
using System.Threading.Tasks;

namespace LanguageExt;

record FoldWhileTransducer<A, S>(
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    Transducer<A, S>
{
    public override ReducerIO<A, S1> Reduce<S1>(ReducerIO<S, S1> reducer)
    {
        var state = State;
        return (s1, x) =>
                   IO.liftVAsync(async e =>
                                 {
                                     if(e.Token.IsCancellationRequested) return Reduced.Done(s1);
                                     if (Pred(state, x))
                                     {
                                         state = Folder(state, x);
                                         return Reduced.Continue(s1);
                                     }
                                     else
                                     {
                                         switch (await reducer(s1, state).RunAsync(e))
                                         {
                                             case { Continue: true, Value: var nstate }:
                                                 state = Folder(State /* reset */, x);
                                                 return Reduced.Continue(nstate);

                                             case { Value: var nstate }:
                                                 return Reduced.Done(nstate);
                                         }
                                     }
                                 });
    }
}

record FoldWhileTransducer2<A, S>(
    Schedule Schedule, 
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    Transducer<A, S>
{
    public override ReducerIO<A, S1> Reduce<S1>(ReducerIO<S, S1> reducer)
    {
        var state = State;
        var sch = Duration.Zero.Cons(Schedule.Run()).GetEnumerator();
        return (s1, x) =>
                   IO.liftVAsync(async e =>
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
                                             if (!sch.Current.IsZero) await Task.Delay((TimeSpan)sch.Current);
                                         }
                                         else
                                         {
                                             return Reduced.Done(s1);
                                         }

                                         switch (await reducer(s1, state).RunAsync(e))
                                         {
                                             case { Continue: true, Value: var nstate }:
                                                 state = Folder(State /* reset */, x);
                                                 return Reduced.Continue(nstate);

                                             case { Value: var nstate }:
                                                 return Reduced.Done(nstate);
                                         }
                                     }
                                 });
    }
}
