using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    [Pure]
    public static Validation<F, A> As<F, A>(this K<Validation<F>, A> ma) =>
        (Validation<F, A>)ma;
    
    [Pure]
    public static Validation<F, A> As2<F, A>(this K<Validation, F, A> ma) =>
        (Validation<F, A>)ma;

    [Pure]
    public static Validation<F, A> Combine<F, A>(this K<Validation<F>, A> lhs, K<Validation<F>, A> rhs)
        where F : Semigroup<F> =>
        lhs.As().CombineFirst(rhs.As(), F.Instance);        

    [Pure]
    public static Validation<F, A> Choose<F, A>(this K<Validation<F>, A> lhs, K<Validation<F>, A> rhs) =>
        Choice.choose(lhs, rhs).As();

    /// <summary>
    /// Match Success and return a context.  You must follow this with `.Fail(...)` to complete the match
    /// </summary>
    /// <param name="success">Action to invoke if in a Success state</param>
    /// <returns>Context that must have `Fail()` called upon it.</returns>
    [Pure]
    public static ValidationUnitContext<F, A> Success<F, A>(this K<Validation<F>, A> ma, Action<A> success) =>
        new (ma.As(), success);

    /// <summary>
    /// Match Success and return a context.  You must follow this with `.Fail(...)` to complete the match
    /// </summary>
    /// <param name="success">Action to invoke if in a Success state</param>
    /// <returns>Context that must have `Fail()` called upon it.</returns>
    [Pure]
    public static ValidationContext<F, A, B> Success<F, A, B>(this K<Validation<F>, A> ma, Func<A, B> success) =>
        new (ma.As(), success);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Validation<F, A> Flatten<F, A>(this Validation<F, Validation<F, A>> mma) =>
        mma.Bind(x => x);

    /// <summary>
    /// Filter the Validation
    /// </summary>
    /// <remarks>
    /// If the predicate returns `false`, then the `Validation` goes into a failed state using `Monoid.Empty` of `F` as
    /// its failure value.
    /// </remarks>
    [Pure]
    public static Validation<F, A> Filter<F, A>(this K<Validation<F>, A> ma, Func<A, bool> pred) 
        where F : Monoid<F> =>
        ma.As().Bind(x => pred(x)
                              ? Validation.Success<F, A>(x)
                              : Validation.Fail<F, A>(F.Empty));

    /// <summary>
    /// Filter the Validation
    /// </summary>
    /// <remarks>
    /// If the predicate returns `false`, then the `Validation` goes into a failed state using `Monoid.Empty` of `F` as
    /// its failure value.
    /// </remarks>
    [Pure]
    public static Validation<F, A> Where<F, A>(this K<Validation<F>, A> ma, Func<A, bool> pred) 
        where F : Monoid<F> =>
        ma.Filter(pred);
    
    /// <summary>
    /// Extract only the successes 
    /// </summary>
    /// <param name="vs">Enumerable of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of successes</returns>
    [Pure]
    public static IEnumerable<S> Successes<F, S>(this IEnumerable<Validation<F, S>> vs)
    {
        foreach (var v in vs)
        {
            if (v.IsSuccess) yield return (S)v;
        }
    }

    /// <summary>
    /// Extract only the failures 
    /// </summary>
    /// <param name="vs">Enumerable of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of failures</returns>
    [Pure]
    public static IEnumerable<F> Fails<F, S>(this IEnumerable<Validation<F, S>> vs)
    {
        foreach (var v in vs)
        {
            if (v.IsFail) yield return (F)v;
        }
    }
    
    /// <summary>
    /// Extract only the successes 
    /// </summary>
    /// <param name="vs">Seq of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of successes</returns>
    [Pure]
    public static Seq<S> Successes<F, S>(this Seq<Validation<F, S>> vs) =>
        toSeq(Successes(vs.AsEnumerable()));
    
    /// <summary>
    /// Extract only the failures 
    /// </summary>
    /// <param name="vs">Seq of validations</param>
    /// <typeparam name="F">Fail type</typeparam>
    /// <typeparam name="S">Success type</typeparam>
    /// <returns>Enumerable of failures</returns>
    [Pure]
    public static Seq<F> Fails<F, S>(this Seq<Validation<F, S>> vs) =>
        toSeq(Fails(vs.AsEnumerable()));

    /// <summary>
    /// Convert `Validation` type to `Fin` type.
    /// </summary>
    [Pure]
    public static Fin<A> ToFin<A>(this Validation<Error, A> ma) =>
        ma switch
        {
            Validation<Error, A>.Success (var x) => new Fin<A>.Succ(x),
            Validation<Error, A>.Fail (var x)    => new Fin<A>.Fail(x),
            _                                    => throw new NotSupportedException()
        };


    /// <summary>
    /// If any items are `Fail`, then the errors are collected and returned.  If they all pass, then the Success values
    /// are collected into a `Seq`.  
    /// </summary>
    /// <exception cref="TypeLoadException">
    /// <para>
    /// Any `TypeLoadException` thrown is because the `F` type used does not derive from `Semigroup〈F〉`.
    /// This is a runtime error rather than a compile-time constraint error because we're resolving the `Semigroup〈F〉`
    /// trait ad hoc.
    /// </para>
    /// <para>
    /// That means we delay finding out that the provided `F` type isn't compatible for `Validation〈F, A〉`.  That is
    /// annoying, we would prefer compile-time constraints, of course, but it enables much more freedom to implement the
    /// `Coproduct`, `Bifunctor`, and `Bimonad` traits which, in turn, give additional functionality for free (like
    /// `Partition`).
    /// </para>
    /// <para>
    /// Implementation of those traits would not be possible if we were to add compile-time constraints to `F`.  So, the
    /// resolution of any type-exception thrown is to only use `Semigroup〈F〉` deriving types for `F`.
    /// </para> 
    /// </exception>
    [Pure]
    internal static Validation<F, A> CombineFirst<F, A>(
        this Validation<F, A> lhs,
        Validation<F, A> rhs,
        Option<SemigroupInstance<F>> trait) =>
        trait switch
        {
            { IsSome: true, Case: SemigroupInstance<F> semi } =>
                (lhs, rhs) switch
                {
                    ({ IsSuccess: true }, { IsSuccess: true }) =>
                        lhs,

                    ({ IsFail: true }, { IsFail: true }) =>
                        semi.Combine(lhs.FailValue, rhs.FailValue),

                    ({ IsFail: true }, _) =>
                        lhs.FailValue,

                    _ =>
                        rhs.FailValue
                },

            _ => (lhs, rhs) switch
                 {
                     ({ IsSuccess: true }, _) => lhs,
                     _                        => rhs
                 }
        };

    /// <summary>
    /// If any items are `Fail`, then the errors are collected and returned.  If they all pass, then the Success values
    /// are collected into a `Seq`.  
    /// </summary>
    /// <exception cref="TypeLoadException">
    /// <para>
    /// Any `TypeLoadException` thrown is because the `F` type used does not derive from `Semigroup〈F〉`.
    /// This is a runtime error rather than a compile-time constraint error because we're resolving the `Semigroup〈F〉`
    /// trait ad hoc.
    /// </para>
    /// <para>
    /// That means we delay finding out that the provided `F` type isn't compatible for `Validation〈F, A〉`.  That is
    /// annoying, we would prefer compile-time constraints, of course, but it enables much more freedom to implement the
    /// `Coproduct`, `Bifunctor`, and `Bimonad` traits which, in turn, give additional functionality for free (like
    /// `Partition`).
    /// </para>
    /// <para>
    /// Implementation of those traits would not be possible if we were to add compile-time constraints to `F`.  So, the
    /// resolution of any type-exception thrown is to only use `Semigroup〈F〉` deriving types for `F`.
    /// </para> 
    /// </exception>
    [Pure]
    internal static Validation<F, Seq<A>> CombineI<F, A>(
        this Validation<F, A> lhs,
        Validation<F, A> rhs,
        Option<SemigroupInstance<F>> trait) =>
        trait switch
        {
            { IsSome: true, Case: SemigroupInstance<F> semi } =>
                (lhs, rhs) switch
                {
                    ({ IsSuccess: true }, { IsSuccess: true }) =>
                        Validation.SuccessI<F, Seq<A>>([lhs.SuccessValue, rhs.SuccessValue]),

                    ({ IsFail: true }, { IsFail: true }) =>
                        semi.Combine(lhs.FailValue, rhs.FailValue),

                    ({ IsFail: true }, _) =>
                        lhs.FailValue,

                    _ =>
                        rhs.FailValue
                },
            _ => (lhs, rhs) switch
                 {
                     ({ IsSuccess: true, SuccessValue: var x }, { IsSuccess: true, SuccessValue: var y }) => Seq(x, y),
                     ({ IsSuccess: false, FailValue : var lf }, _) => lf,
                     (_, { IsSuccess: false, FailValue : var rf }) => rf
                 }
        };

    /// <summary>
    /// If any items are `Fail`, then the errors are collected and returned.  If they all pass, then the Success values
    /// are collected into a `Seq`.  
    /// </summary>
    /// <exception cref="TypeLoadException">
    /// <para>
    /// Any `TypeLoadException` thrown is because the `F` type used does not derive from `Semigroup〈F〉`.
    /// This is a runtime error rather than a compile-time constraint error because we're resolving the `Semigroup〈F〉`
    /// trait ad hoc.
    /// </para>
    /// <para>
    /// That means we delay finding out that the provided `F` type isn't compatible for `Validation〈F, A〉`.  That is
    /// annoying, we would prefer compile-time constraints, of course, but it enables much more freedom to implement the
    /// `Coproduct`, `Bifunctor`, and `Bimonad` traits which, in turn, give additional functionality for free (like
    /// `Partition`).
    /// </para>
    /// <para>
    /// Implementation of those traits would not be possible if we were to add compile-time constraints to `F`.  So, the
    /// resolution of any type-exception thrown is to only use `Semigroup〈F〉` deriving types for `F`.
    /// </para> 
    /// </exception>
    [Pure]
    public static Validation<F, Seq<A>> CombineI<F, A>(
        this Validation<F, Seq<A>> lhs,
        Validation<F, A> rhs,
        Option<SemigroupInstance<F>> trait) =>
        trait switch
        {
            { IsSome: true, Case: SemigroupInstance<F> semi } =>
                (lhs, rhs) switch
                {
                    ({ IsSuccess: true }, { IsSuccess: true }) =>
                        Validation.SuccessI<F, Seq<A>>(lhs.SuccessValue.Add(rhs.SuccessValue)),

                    ({ IsFail: true }, { IsFail: true }) =>
                        semi.Combine(lhs.FailValue, rhs.FailValue),

                    ({ IsFail: true }, _) =>
                        lhs.FailValue,

                    _ =>
                        rhs.FailValue
                },
            _ => (lhs, rhs) switch
                 {
                     ({ IsSuccess: true, SuccessValue: var xs }, { IsSuccess: true, SuccessValue: var y }) => xs.Add(y),
                     ({ IsSuccess: false, FailValue : var lf }, _) => lf,
                     (_, { IsSuccess: false, FailValue : var rf }) => rf
                 }
        };

    /// <summary>
    /// If any items are `Fail`, then the errors are collected and returned.  If they all pass, then the Success values
    /// are collected into a `Seq`.  
    /// </summary>
    /// <exception cref="TypeLoadException">
    /// <para>
    /// Any `TypeLoadException` thrown is because the `F` type used does not derive from `Semigroup〈F〉`.
    /// This is a runtime error rather than a compile-time constraint error because we're resolving the `Semigroup〈F〉`
    /// trait ad hoc.
    /// </para>
    /// <para>
    /// That means we delay finding out that the provided `F` type isn't compatible for `Validation〈F, A〉`.  That is
    /// annoying, we would prefer compile-time constraints, of course, but it enables much more freedom to implement the
    /// `Coproduct`, `Bifunctor`, and `Bimonad` traits which, in turn, give additional functionality for free (like
    /// `Partition`).
    /// </para>
    /// <para>
    /// Implementation of those traits would not be possible if we were to add compile-time constraints to `F`.  So, the
    /// resolution of any type-exception thrown is to only use `Semigroup〈F〉` deriving types for `F`.
    /// </para> 
    /// </exception>
    [Pure]
    internal static Validation<F, Seq<A>> CombineI<F, A>(
        this Validation<F, A> lhs,
        Validation<F, Seq<A>> rhs,
        Option<SemigroupInstance<F>> trait) =>
        trait switch
        {
            { IsSome: true, Case: SemigroupInstance<F> semi } =>
                (lhs, rhs) switch
                {
                    ({ IsSuccess: true }, { IsSuccess: true }) =>
                        Validation.SuccessI<F, Seq<A>>(lhs.SuccessValue.Cons(rhs.SuccessValue)),

                    ({ IsFail: true }, { IsFail: true }) =>
                        semi.Combine(lhs.FailValue, rhs.FailValue),

                    ({ IsFail: true }, _) =>
                        lhs.FailValue,

                    _ =>
                        rhs.FailValue
                },
            _ => (lhs, rhs) switch
                 {
                     ({ IsSuccess: true, SuccessValue: var x }, { IsSuccess: true, SuccessValue: var ys }) => x.Cons(ys),
                     ({ IsSuccess: false, FailValue : var lf }, _) => lf,
                     (_, { IsSuccess: false, FailValue : var rf }) => rf
                 }
        };
}
