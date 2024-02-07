using System;
using System.Threading;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public static class Monad
{
    public static KStar<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        M.Pure(value);

    public static KStar<M, A> flatten<M, A>(KStar<M, KStar<M, A>> mma)
        where M : Monad<M> =>
        M.Flatten(mma);

    public static KStar<M, B> bind<M, A, B>(KStar<M, A> ma, Transducer<A, KStar<M, B>> f)
        where M  : Monad<M> =>
        M.Bind(ma, f);

    public static KStar<M, B> bind<M, A, B>(KStar<M, A> ma, Func<A, KStar<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    public static MB bind<M, MB, A, B>(KStar<M, A> ma, Transducer<A, MB> f)
        where MB : KStar<M, B>
        where M  : Monad<M> =>
        (MB)M.Bind(ma, f.Map(mb => mb.AsKind()));

    public static MB bind<M, MB, A, B>(KStar<M, A> ma, Func<A, MB> f)
        where MB : KStar<M, B>
        where M : Monad<M> =>
        bind<M, MB, A, B>(ma, lift(f));
    
    public static TResult<S> run<M, S, A>(
        KStar<M, A> ma,
        S initialState,
        Reducer<A, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null)
        where M  : Monad<M> =>
        M.Run(ma, initialState, reducer, token, syncContext);
}
