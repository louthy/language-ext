using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record TakeForSourceT<M, A>(SourceT<M, A> Source, TimeSpan Duration) : SourceT<M, A>
    where M : MonadIO<M>, Fallible<Error, M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        from sofar in atomIO(state)
        from value in M.TimeoutIOMaybe(reduce(state, reducer, sofar), Duration)
                        | @catch(ErrorCodes.TimedOut, M.Pure(Reduced.Done(state)))
                        | @catch(ErrorCodes.Cancelled, M.Pure(Reduced.Done(state)))
        select value;

    K<M, Reduced<S>> reduce<S>(S state, ReducerM<M, K<M, A>, S> reducer, Atom<S> sofar) =>
        Source.ReduceInternalM(
            state,
            (s, ma) => from ns in reducer(s, ma)
                       from _ in sofar.SwapIO(_ => ns.Value)
                       select ns);
}
