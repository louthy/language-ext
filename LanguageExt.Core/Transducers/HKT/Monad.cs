using System;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface Monad<M> : Functor<M> 
    where M : Monad<M>
{
    public static abstract KStar<M, A> Pure<A>(A value);

    public static virtual KStar<M, B> Bind<A, B>(KStar<M, A> ma, Func<A, KStar<M, B>> f) =>
        M.Bind(ma, lift(f));

    public static abstract KStar<M, B> Bind<A, B>(KStar<M, A> ma, Transducer<A, KStar<M, B>> f);
    
    public static virtual KStar<M, A> Flatten<A>(KStar<M, KStar<M, A>> mma) =>
        M.Bind(mma, identity);


    public static virtual TResult<S> Run<S, A>(
        KStar<M, A> ma,
        S initialState,
        Reducer<A, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        ma.Morphism.Run(default, initialState, reducer, token, syncContext);
}
