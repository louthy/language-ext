#nullable enable
using System;

namespace LanguageExt.HKT;

/// <summary>
/// Monad bind trait
/// </summary>
public interface Monad<M> : Applicative<M> 
    where M : Monad<M>
{
    /// <summary>
    /// Monad bind
    /// </summary>
    KArr<M, Unit, B> Bind<A, B>(KArr<M, Unit, A> mx, Transducer<A, KArr<M, Unit, B>> f);
}

/// <summary>
/// Monad bind trait with fixed input type
/// </summary>
public interface MonadReader<M, Env> : Applicative<M, Env> 
    where M : MonadReader<M, Env>
{
    /// <summary>
    /// Monad bind
    /// </summary>
    KArr<M, Env, B> Bind<A, B>(KArr<M, Env, A> mx, Transducer<A, KArr<M, Env, B>> f);
}

public static class MonadExtensions
{
    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Unit, B> Bind<M, A, B>(this Monad<M> self, KArr<M, Unit, A> mx, Func<A, KArr<M, Unit, B>> f) 
        where M : Monad<M> =>
        default(M).Bind(mx, Transducer.lift(f));

    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Unit, A> Flatten<M, A>(this KArr<M, Unit, KArr<M, Unit, A>> mmx)
        where M : Monad<M> =>
        default(M).Bind(mmx, Transducer.identity<KArr<M, Unit, A>>());

    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Env, B> Bind<Env,M, A, B>(this KArr<M, Env, A> mx, Func<A, KArr<M, Env, B>> f)
        where M : MonadReader<M, Env> =>
        default(M).Bind(mx, Transducer.lift(f));

    /// <summary>
    /// Monad bind
    /// </summary>
    public static KArr<M, Env, A> Flatten<Env,M, A>(this KArr<M, Env, KArr<M, Env, A>> mmx) 
        where M : MonadReader<M, Env> =>
        default(M).Bind(mmx, Transducer.identity<KArr<M, Env, A>>());
}
