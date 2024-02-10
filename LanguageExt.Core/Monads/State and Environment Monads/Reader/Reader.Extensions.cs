using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Reader monad extensions
/// </summary>
public static class ReaderExt
{
    public static Reader<Env, A> As<Env, A>(this MonadT<ReaderT<Env, Identity>, Identity, A> ma) =>
        (Reader<Env, A>)ma;
    
    public static ReaderT<Env, M, A> As<Env, M, A>(this Monad<ReaderT<Env, M>, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
    
    public static ReaderT<Env, M, A> As<Env, M, A>(this Applicative<ReaderT<Env, M>, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
    
    public static ReaderT<Env, M, A> As<Env, M, A>(this Functor<ReaderT<Env, M>, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Reader<Env, A> Flatten<Env, A>(this Reader<Env, Reader<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Reader<Env, A> Do<Env, A>(this Reader<Env, A> ma, Action<A> f) =>
        ma.Map(a => { f(a); return a; });
}

