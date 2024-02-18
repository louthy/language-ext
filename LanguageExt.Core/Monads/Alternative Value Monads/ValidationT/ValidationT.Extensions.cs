using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Validation monad extensions
/// </summary>
public static class ValidationTExt
{
    public static ValidationT<L, M, A> As<L, M, A>(this K<ValidationT<L, M>, A> ma)
        where M : Monad<M>
        where L : Monoid<L> =>
        (ValidationT<L, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ValidationT<L, M, A> Flatten<L, M, A>(this ValidationT<L, M, ValidationT<L, M, A>> mma)
        where M : Monad<M> 
        where L : Monoid<L> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    [Pure]
    public static ValidationT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<ValidationT<L, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>
        where L : Monoid<L> =>
        ValidationT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    [Pure]
    public static ValidationT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ValidationT<L, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>
        where L : Monoid<L> =>
        ValidationT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static ValidationT<L, M, B> Apply<L, M, A, B>(this ValidationT<L, M, Func<A, B>> mf, ValidationT<L, M, A> ma)
        where M : Monad<M>
        where L : Monoid<L> =>
        new(M.Bind(mf.As().runValidation,
                   ef => ef.State switch
                         {
                             EitherStatus.IsRight => 
                                 M.Bind(ma.As().runValidation,
                                        ea => ea.State switch
                                              {
                                                  EitherStatus.IsRight => 
                                                      M.Pure(Either<L, B>.Right(ef.RightValue(ea.RightValue))),
                                                 
                                                  EitherStatus.IsLeft => 
                                                      M.Pure(Either<L, B>.Left(ef.LeftValue.Append(ea.LeftValue))),
                                                 
                                                  _ => 
                                                      M.Pure(Either<L, B>.Left(ef.LeftValue))

                                              }),
                             
                             EitherStatus.IsLeft =>
                                 M.Bind(ma.As().runValidation,
                                        ea => ea.State switch
                                              {
                                                  EitherStatus.IsLeft => 
                                                      M.Pure(Either<L, B>.Left(ef.LeftValue.Append(ea.LeftValue))),
                                                 
                                                  _ => 
                                                      M.Pure(Either<L, B>.Left(ef.LeftValue))

                                              }),
                             _ => M.Pure(L.Empty)
                         }));

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static ValidationT<L, M, B> Action<L, M, A, B>(this ValidationT<L, M, A> ma, ValidationT<L, M, B> mb)
        where M : Monad<M>
        where L : Monoid<L> =>
        fun((A _, B b) => b).Map(ma).Apply(mb).As();
}
