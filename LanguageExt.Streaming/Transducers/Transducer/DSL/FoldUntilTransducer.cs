using System;
using System.Threading.Tasks;


namespace LanguageExt;

record FoldUntilTransducer<A, S>(
    Func<S, A, S> Folder, 
    Func<S, A, bool> Pred, 
    S State) : 
    Transducer<A, S>
{
    public override ReducerIO<A, S1> Reduce<S1>(ReducerIO<S, S1> reducer)
    {
        var state = State;
        return (s1, x) => IO.liftVAsync(async e =>
                                        {
                                            if(e.Token.IsCancellationRequested) return Reduced.Done(s1);
                                            state = Folder(state, x);
                                            if (Pred(state, x))
                                            {
                                                switch (await reducer(s1, state).RunAsync(e))
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
                                        });
    }
    
    public override TransducerM<M, A, S> Lift<M>() =>
        new FoldUntilTransducerM2<M, A, S>(Schedule.Forever, Folder, Pred, State);
}

record FoldUntilTransducer2<A, S>(
    Schedule Schedule,
    Func<S, A, S> Folder,
    Func<S, A, bool> Pred,
    S State) :
    Transducer<A, S>
{
    public override ReducerIO<A, S1> Reduce<S1>(ReducerIO<S, S1> reducer)
    {
        var state = State;
        var sch   = Duration.Zero.Cons(Schedule.Run()).GetEnumerator();
        return (s1, x) =>
                   IO.liftVAsync(async e =>
                                 {
                                     if(e.Token.IsCancellationRequested) return Reduced.Done(s1);
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

                                         switch (await reducer(s1, state).RunAsync(e))
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
                                 });
    }
    
    public override TransducerM<M, A, S> Lift<M>() =>
        new FoldUntilTransducerM2<M, A, S>(Schedule, Folder, Pred, State);
}
