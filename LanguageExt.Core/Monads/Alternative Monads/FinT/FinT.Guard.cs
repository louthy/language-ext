using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static class FinTGuardExtensions
{
    /// <summary>
    /// Natural transformation to `FinT`
    /// </summary>
    public static FinT<M, Unit> ToFinT<M>(this Guard<Error, Unit> guard)
        where M : Monad<M> =>
        guard.Flag
            ? FinT<M, Unit>.Succ(default)
            : FinT<M, Unit>.Fail(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `FinT`
    /// </summary>
    public static FinT<M, B> Bind<M, B>(
        this Guard<Error, Unit> guard,
        Func<Unit, FinT<M, B>> f) 
        where M : Monad<M> =>
        guard.Flag
            ? f(default).As()
            : FinT<M, B>.Fail(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `FinT`
    /// </summary>
    public static FinT<M, C> SelectMany<M, B, C>(
        this Guard<Error, Unit> guard,
        Func<Unit, FinT<M, B>> bind, 
        Func<Unit, B, C> project) 
        where M : Monad<M> =>
        guard.Flag
            ? bind(default).As().Map(b => project(default, b))
            : FinT<M, C>.Fail(guard.OnFalse());

    /// <summary>
    /// Monadic binding support for `FinT`
    /// </summary>
    public static FinT<M, Unit> SelectMany<M, A>(
        this FinT<M, A> ma, 
        Func<A, Guard<Error, Unit>> f) 
        where M : Monad<M> =>
        ma.Bind(a => f(a).ToFinT<M>());

    /// <summary>
    /// Monadic binding support for `FinT`
    /// </summary>
    public static FinT<M, C> SelectMany<M, A, C>(
        this FinT<M, A> ma, 
        Func<A, Guard<Error, Unit>> bind, 
        Func<A, Unit, C> project) 
        where M : Monad<M> =>
        ma.Bind(a => bind(a).ToFinT<M>().Map(_ => project(a, default)));    
}
