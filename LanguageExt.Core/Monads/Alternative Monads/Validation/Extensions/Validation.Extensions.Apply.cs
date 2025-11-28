using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Validation<F, B> Action<F, A, B>(this K<Validation<F>, A> ma, K<Validation<F>, B> mb) 
        where F : Semigroup<F> =>
        ActionI(ma, mb, F.Instance).As();

    /// <summary>
    /// Applicative-functor apply-operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
    /// then takes the resulting value and wraps it back up into a new applicative-functor.
    /// </remarks>
    /// <param name="ma">Value(s) applicative functor</param>
    /// <param name="mf">Mapping function(s)</param>
    /// <returns>Mapped applicative functor</returns>
    public static Validation<F, B> Apply<F, A, B>(this K<Validation<F>, Func<A, B>> mf, K<Validation<F>, A> ma)
        where F : Semigroup<F> =>
        ApplyI(mf, ma, F.Instance).As();

    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    internal static Validation<F, B> ActionI<F, A, B>(
        this K<Validation<F>, A> ma,
        K<Validation<F>, B> mb,
        Option<SemigroupInstance<F>> trait) =>
        trait switch
        {
            { IsSome: true, Value: { } semigroup } =>
                ma switch
                {
                    Validation<F, A>.Success =>
                        mb.As(),

                    Validation<F, A>.Fail (var e1) =>
                        mb switch
                        {
                            Validation<F, B>.Fail (var e2) =>
                                Validation.FailI<F, B>(semigroup.Combine(e1, e2)),

                            _ =>
                                Validation.FailI<F, B>(e1)
                        },
                    _ => throw new NotSupportedException()
                },

            _ =>
                ma switch
                {
                    Validation<F, A>.Success       => mb.As(),
                    Validation<F, A>.Fail (var e1) => Validation.FailI<F, B>(e1),
                    _                              => throw new NotSupportedException()
                }
        };

    /// <summary>
    /// Applicative-functor apply-operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
    /// then takes the resulting value and wraps it back up into a new applicative-functor.
    /// </remarks>
    /// <param name="ma">Value(s) applicative functor</param>
    /// <param name="mf">Mapping function(s)</param>
    /// <returns>Mapped applicative functor</returns>
    internal static Validation<F, B> ApplyI<F, A, B>(
        this K<Validation<F>, Func<A, B>> mf, 
        K<Validation<F>, A> ma,
        Option<SemigroupInstance<F>> trait) =>
        trait switch
        {
            { IsSome: true, Value: { } semigroup } =>
                mf switch
                {
                    Validation<F, Func<A, B>>.Success (var f) =>
                        ma switch
                        {
                            Validation<F, A>.Success (var a) =>
                                Validation.SuccessI<F, B>(f(a)),

                            Validation<F, A>.Fail (var e) =>
                                Validation.FailI<F, B>(e),

                            _ => throw new NotSupportedException()
                        },

                    Validation<F, Func<A, B>>.Fail (var e1) =>
                        ma switch
                        {
                            Validation<F, A>.Fail (var e2) =>
                                Validation.FailI<F, B>(semigroup.Combine(e1, e2)),

                            _ =>
                                Validation.FailI<F, B>(e1)

                        },

                    _ => throw new NotSupportedException()
                },

            _ => mf switch
                 {
                     Validation<F, Func<A, B>>.Success (var f) =>
                         ma switch
                         {
                             Validation<F, A>.Success (var a) =>
                                 Validation.SuccessI<F, B>(f(a)),

                             Validation<F, A>.Fail (var e) =>
                                 Validation.FailI<F, B>(e),

                             _ => throw new NotSupportedException()
                         },

                     Validation<F, Func<A, B>>.Fail (var e1) =>
                         Validation.FailI<F, B>(e1),

                     _ => throw new NotSupportedException()
                 }
        };
}    
