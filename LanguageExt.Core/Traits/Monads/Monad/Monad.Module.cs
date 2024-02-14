using System;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static class Monad
{
    public static K<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        M.Pure(value);

    public static K<M, A> flatten<M, A>(K<M, K<M, A>> mma)
        where M : Monad<M> =>
        M.Flatten(mma);

    public static K<M, B> bind<M, A, B>(K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    public static MB bind<M, MB, A, B>(K<M, A> ma, Func<A, MB> f)
        where MB : K<M, B>
        where M : Monad<M> =>
        (MB)bind(ma, x => f(x));
    
    /// <summary>
    /// Embeds the `IO` monad into the `M<A>` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its inner-most monad.
    /// </summary>
    public static K<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        M.LiftIO(ma);
}
