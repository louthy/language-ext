using System;
using System.Threading;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface Monad<M> : Functor<M> 
    where M : Monad<M>
{
    public static virtual KStar<M, A> Pure<A>(A value) => 
        M.Lift(constant<Unit, A>(value));

    public static virtual KStar<M, A> Flatten<A>(KStar<M, KStar<M, A>> mma) => 
        mma.Invoke();

    public static virtual KStar<M, B> Bind<A, B>(KStar<M, A> ma, Transducer<A, KStar<M, B>> f) =>
        M.Flatten(M.Map(ma, f));

    public static virtual KStar<M, B> Bind<A, B>(KStar<M, A> ma, Func<A, KStar<M, B>> f) =>
        M.Bind(ma, lift(f));

    public static virtual TResult<S> Run<S, A>(
        KStar<M, A> ma,
        S initialState,
        Reducer<A, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        ma.Morphism.Run(default, initialState, reducer, token, syncContext);
}
