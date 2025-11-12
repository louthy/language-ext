using System;
using System.Collections;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Like `Either` but collects multiple failed values
/// </summary>
/// <exception cref="TypeLoadException">
/// <para>
/// Any `TypeLoadException` thrown is because the `F` type used does not derive from `Monoid〈F〉` or `Semigroup〈F〉`.
/// This is a runtime error rather than a compile-time constraint error because we're resolving the `Monoid〈F〉` and
/// `Semigroup〈F〉` traits ad-hoc.
/// </para>
/// <para>
/// That means we delay finding out that the provided `F` type isn't compatible for `Validation〈F, A〉`.  That is
/// annoying, we would prefer compile-time constraints, of course, but it enables much more freedom to implement the
/// `Coproduct`, `Bifunctor`, and `Bimonad` traits which, in turn, give additional functionality for free (like
/// `Partition`).
/// </para>
/// <para>
/// Implementation of those traits would not be possible if we were to add compile-time constraints to `F`.  So, the
/// resolution of any type-exception thrown is to only use `Monoid〈F〉` or `Semigroup〈F〉` deriving types for `F`,
/// depending on the functionality required.
/// </para> 
/// </exception>
/// <typeparam name="F">Failure value type: it is important that this implements `Monoid〈F〉`</typeparam>
/// <typeparam name="A">Success value type</typeparam>
[Serializable]
public abstract partial record Validation<F, A> :
    IComparable<Validation<F, A>>,
    IComparable<A>,
    IComparable,
    IEquatable<Pure<A>>,
    IComparable<Pure<A>>,
    IEquatable<A>,
    Fallible<Validation<F, A>, Validation<F>, F, A>,
    K<Validation, F, A>
{
    /// <summary>
    /// Is the Validation in a Success state?
    /// </summary>
    [Pure]
    public abstract bool IsSuccess { get; }

    /// <summary>
    /// Is the Validation in a Fail state?
    /// </summary>
    [Pure]
    public abstract bool IsFail { get; }

    /// <summary>
    /// Invokes the Success or Fail function depending on the state of the Validation
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Fail">Function to invoke if in a Fail state</param>
    /// <param name="Succ">Function to invoke if in a Success state</param>
    /// <returns>The return value of the invoked function</returns>
    [Pure]
    public abstract B Match<B>(Func<F, B> Fail, Func<A, B> Succ);

    /// <summary>
    /// Empty span
    /// </summary>
    [Pure]
    public abstract ReadOnlySpan<F> FailSpan();

    /// <summary>
    /// Span of right value
    /// </summary>
    [Pure]
    public abstract ReadOnlySpan<A> SuccessSpan();

    /// <summary>
    /// Compare this structure to another to find its relative ordering
    /// </summary>
    [Pure]
    public abstract int CompareTo<OrdF, OrdA>(Validation<F, A> other)
        where OrdF : Ord<F>
        where OrdA : Ord<A>;

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public abstract bool Equals<EqF, EqA>(Validation<F, A> other)
        where EqF : Eq<F>
        where EqA : Eq<A>;

    /// <summary>
    /// Unsafe access to the right-value 
    /// </summary>
    /// <exception cref="InvalidCastException"></exception>
    internal abstract A SuccessValue { get; }

    /// <summary>
    /// Unsafe access to the left-value 
    /// </summary>
    /// <exception cref="InvalidCastException"></exception>
    internal abstract F FailValue { get; }

    /// <summary>
    /// Maps the value in the Validation if it's in a Success state
    /// </summary>
    /// <typeparam name="F">Fail</typeparam>
    /// <typeparam name="A">Success</typeparam>
    /// <typeparam name="B">Mapped Validation type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Validation</returns>
    [Pure]
    public abstract Validation<F, B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Bi-maps the value in the Validation if it's in a Success state
    /// </summary>
    /// <typeparam name="F">Fail</typeparam>
    /// <typeparam name="A">Success</typeparam>
    /// <typeparam name="F1">Fail return</typeparam>
    /// <typeparam name="A1">Success return</typeparam>
    /// <param name="Succ">Success map function</param>
    /// <param name="Fail">Fail map function</param>
    /// <returns>Mapped Validation</returns>
    [Pure]
    public abstract Validation<F1, A1> BiMap<F1, A1>(Func<F, F1> Fail, Func<A, A1> Succ);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="F">Fail</typeparam>
    /// <typeparam name="A">Success</typeparam>
    /// <typeparam name="B">Resulting bound value</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Bound Validation</returns>
    [Pure]
    public abstract Validation<F, B> Bind<B>(Func<A, Validation<F, B>> f);

    /// <summary>
    /// Bi-bind.  Allows mapping of both monad states
    /// </summary>
    [Pure]
    public abstract Validation<F1, A1> BiBind<F1, A1>(
        Func<F, Validation<F1, A1>> Fail,
        Func<A, Validation<F1, A1>> Succ);

    /// <summary>
    /// Bind the failure
    /// </summary>
    [Pure]
    public Validation<F1, A> BindFail<F1>(
        Func<F, Validation<F1, A>> Fail) 
        where F1 : Monoid<F1> =>
        BiBind(Fail, Validation.SuccessI<F1, A>);

    /// <summary>
    /// Explicit conversion operator from `` to `R`
    /// </summary>
    /// <param name="value">Value</param>
    /// <exception cref="InvalidCastException">Value is not in a Right state</exception>
    [Pure]
    public static explicit operator A(Validation<F, A> ma) =>
        ma.SuccessValue;

    /// <summary>
    /// Explicit conversion operator from `Validation` to `L`
    /// </summary>
    /// <param name="value">Value</param>
    /// <exception cref="InvalidCastException">Value is not in a Fail state</exception>
    [Pure]
    public static explicit operator F(Validation<F, A> ma) =>
        ma.FailValue;

    /// <summary>
    /// Implicit conversion operator from `A` to `Validation〈F, A〉`
    /// </summary>
    [Pure]
    public static implicit operator Validation<F, A>(A value) =>
        new Success(value);

    /// <summary>
    /// Implicit conversion operator from `F` to `Validation〈F, A〉`
    /// </summary>
    [Pure]
    public static implicit operator Validation<F, A>(F value) =>
        new Fail(value);

    /// <summary>
    /// Invokes the `Succ` or `Fail` action depending on the state of the value
    /// </summary>
    /// <param name="Fail">Action to invoke if in a Fail state</param>
    /// <param name="Succ">Action to invoke if in a Success state</param>
    /// <returns>Unit</returns>
    public Unit Match(Action<F> Fail, Action<A> Succ) =>
        Match(fun(Fail), fun(Succ));

    /// <summary>
    /// Executes the `Fail` function if the value is in a Fail state.
    /// Returns the Success value if the value is in a Success state.
    /// </summary>
    /// <param name="Fail">Function to generate a value if in the Fail state</param>
    /// <returns>Returns an unwrapped value</returns>
    [Pure]
    public A IfFail(Func<A> Fail) =>
        Match(_ => Fail(), identity);

    /// <summary>
    /// Executes the `failMap` function if the value is in a Fail state.
    /// Returns the Success value if in a Success state.
    /// </summary>
    /// <param name="failMap">Function to generate a value if in the Fail state</param>
    /// <returns>Returns an unwrapped value</returns>
    [Pure]
    public A IfFail(Func<F, A> failMap) =>
        Match(failMap, identity);

    /// <summary>
    /// Returns the `successValue` if in a Fail state.
    /// Returns the Success value if in a Success state.
    /// </summary>
    /// <param name="successValue">Value to return if in the Fail state</param>
    /// <returns>Returns an unwrapped value</returns>
    [Pure]
    public A IfFail(A successValue) =>
        Match(_ => successValue, identity);

    /// <summary>
    /// Executes the Fail action if in a Fail state.
    /// </summary>
    /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
    /// <returns>Unit</returns>
    public Unit IfFail(Action<F> Fail) =>
        Match(Fail, _ => { });

    /// <summary>
    /// Invokes the `Success` action if in a Success state, otherwise does nothing
    /// </summary>
    /// <param name="Success">Action to invoke</param>
    /// <returns>Unit</returns>
    public Unit IfRight(Action<A> Success) =>
        Match(_ => { }, Success);

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Validation<F, A> t ? CompareTo(t) : 1;

    /// <summary>
    /// Project into a `Lst〈A〉`
    /// </summary>
    /// <returns>If in a Success state, a `Lst` of `R` with one item.  A zero length `Lst` of `R` otherwise</returns>
    public Lst<A> ToList() =>
        new(SuccessSpan());

    /// <summary>
    /// Project into an `Arr〈A〉`
    /// </summary>
    /// <returns>If in a Success state, an `Arr` of `R` with one item.  A zero length `Arr` of `R` otherwise</returns>
    public Arr<A> ToArray() =>
        new(SuccessSpan());

    /// <summary>
    /// Convert to sequence of 0 or 1 success values
    /// </summary>
    [Pure]
    public Seq<A> ToSeq() =>
        new(SuccessSpan());

    /// <summary>
    /// Convert to sequence of 0 or 1 success values
    /// </summary>
    [Pure]
    public Iterable<A> ToIterable() =>
        [..SuccessSpan()];
    
    /// <summary>
    /// Convert to sequence of 0 or 1 success values
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable() =>
        [..SuccessSpan()];

    [Pure]
    public Either<F, A> ToEither() =>
        this switch
        {
            Success (var x) => new Either<F, A>.Right(x),
            Fail (var x)    => new Either<F, A>.Left(x),
            _               => throw new NotSupportedException()
        };

    /// <summary>
    /// Convert to an Option
    /// </summary>
    /// <returns>Some(Right) or None</returns>
    [Pure]
    public Option<A> ToOption() =>
        this switch
        {
            Success (var x) => Option.Some(x),
            Fail            => Option<A>.None,
            _               => throw new NotSupportedException()
        };

    /// <summary>
    /// Action operator
    /// </summary>
    [Pure]
    public static Validation<F, A> operator >>(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.Action(rhs).As();
    
    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Fail<F> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Fail<F>  lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Fail<F> lhs, Validation<F, A>rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Fail<F> lhs, Validation<F, A>  rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Pure<A> lhs, Validation<F, A>  rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Pure<A> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Pure<A> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Pure<A> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left-hand side of the operation</param>
    /// <param name="rhs">The right-hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Fail<F>  lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Pure<A> lhs, Validation<F, A>  rhs) =>
        ((Validation<F, A>)lhs).Equals(rhs);
        
    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Validation<F, A> lhs, Fail<F> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Validation<F, A> lhs, Pure<A> rhs) =>
        !(lhs == rhs);


    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Fail<F> lhs, Validation<F, A> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Pure<A> lhs, Validation<F, A> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator +(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.Combine(rhs).As();

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator +(K<Validation<F>, A> lhs, Validation<F, A> rhs) =>
        lhs.Combine(rhs).As();

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator +(Validation<F, A> lhs, K<Validation<F>, A> rhs) =>
        lhs.Combine(rhs.As()).As();

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    public static Validation<F, A> operator +(Validation<F, A> lhs, A rhs) => 
        lhs.Combine(Validation.SuccessI<F, A>(rhs)).As();

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator +(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.Combine(Validation.SuccessI<F, A>(rhs.Value)).As();

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator +(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.Combine(Validation.FailI<F, A>(rhs.Value)).As();

    /// <summary>
    /// Combine operator: uses the underlying `F.Combine` to collect failures
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator +(Validation<F, A> lhs, F rhs) =>
        lhs.Combine(Validation.FailI<F, A>(rhs)).As();    
    
    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator |(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.Choose(rhs).As();

    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator |(K<Validation<F>, A> lhs, Validation<F, A> rhs) =>
        lhs.Choose(rhs).As();

    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator |(Validation<F, A> lhs, K<Validation<F>, A> rhs) =>
        lhs.Choose(rhs.As()).As();

    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    public static Validation<F, A> operator |(Validation<F, A> lhs, A rhs) => 
        lhs.Choose(Validation.SuccessI<F, A>(rhs)).As();

    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator |(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.Choose(Validation.SuccessI<F, A>(rhs.Value)).As();

    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator |(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.Choose(Validation.FailI<F, A>(rhs.Value)).As();

    /// <summary>
    /// Choice operator: returns the first argument to succeed.  If both fail, then the last failure is returned.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Validation<F, A> operator |(Validation<F, A> lhs, F rhs) =>
        lhs.Choose(Validation.FailI<F, A>(rhs)).As();

    /// <summary>
    /// Catch operator: returns the first argument if it to succeeds. Otherwise, the `F` failure is mapped.
    /// </summary>
    public static Validation<F, A> operator |(Validation<F, A> lhs, CatchM<F, Validation<F>, A> rhs) =>
        lhs.Catch(rhs).As();

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
    public static Validation<F, Seq<A>> operator &(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CombineI(rhs, SemigroupInstance<F>.Instance);

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
    public static Validation<F, Seq<A>> operator &(Validation<F, Seq<A>> lhs, Validation<F, A> rhs) =>
        lhs.CombineI(rhs, SemigroupInstance<F>.Instance);

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
    public static Validation<F, Seq<A>> operator &(Validation<F, A> lhs, Validation<F, Seq<A>> rhs) =>
        lhs.CombineI(rhs, SemigroupInstance<F>.Instance);

    /// <summary>
    /// Override of the True operator to return True if the Either is Right
    /// </summary>
    [Pure]
    public static bool operator true(Validation<F, A> value) =>
        value.IsSuccess;

    /// <summary>
    /// Override of the False operator to return True if the Either is Left
    /// </summary>
    [Pure]
    public static bool operator false(Validation<F, A> value) =>
        value.IsFail;

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Validation<F, A>? other) =>
        other is null
            ? 1
            : CompareTo<OrdDefault<F>, OrdDefault<A>>(other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo<OrdR>(Validation<F, A> other)
        where OrdR : Ord<A> =>
        CompareTo<OrdDefault<F>, OrdR>(other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Fail<F> other) =>
        CompareTo((Validation<F, A>)other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Pure<A> other) =>
        CompareTo((Validation<F, A>)other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(A? other) =>
        other switch
        {
            null => 1,
            _    => CompareTo(Validation.SuccessI<F, A>(other))
        };

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(F? other) =>
        other switch
        {
            null => 1,
            _    => CompareTo(Fail(other))
        };

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(A? other) =>
        other is not null && Equals(Validation.SuccessI<F, A>(other));

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(F? other) =>
        other is not null && Equals(Fail(other));

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public virtual bool Equals(Validation<F, A>? other) =>
        other is not null && Equals<EqDefault<F>, EqDefault<A>>(other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public virtual bool Equals<EqR>(Validation<F, A> other) where EqR : Eq<A> =>
        Equals<EqDefault<F>, EqR>(other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(Fail<F> other) =>
        Equals((Validation<F, A>)other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(Pure<A> other) =>
        Equals((Validation<F, A>)other);

    /// <summary>
    /// Match the Success and Fail values but as untyped objects.
    /// </summary>
    [Pure]
    public B MatchUntyped<B>(Func<object?, B> Succ, Func<object?, B> Fail) =>
        Match(x => Succ(x), x => Fail(x));

    /// <summary>
    /// Iterate the value
    /// action is invoked if in the Success state
    /// </summary>
    public Unit Iter(Action<A> Succ) =>
        Match(_ => { }, Succ);

    /// <summary>
    /// Invokes a predicate on the success value if it's in the Success state
    /// </summary>
    /// <returns>
    /// True if in a `Left` state.  
    /// `True` if the in a `Right` state and the predicate returns `True`.  
    /// `False` otherwise.</returns>
    [Pure]
    public bool ForAll(Func<A, bool> Succ) =>
        Match(_ => true, Succ);

    /// <summary>
    /// Invokes a predicate on the values 
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="Succ">Predicate</param>
    /// <param name="Fail">Predicate</param>
    /// <returns>True if either Predicate returns true</returns>
    [Pure]
    public bool BiForAll(Func<F, bool> Fail, Func<A, bool> Succ) =>
        Match(Fail, Succ);

    /// <summary>
    /// Validation types are like lists of 0 or 1 items and therefore follow the 
    /// same rules when folding.
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Folder function, applied if structure is in a Success state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    public S Fold<S>(S state, Func<S, A, S> Succ) =>
        Match(_ => state, curry(Succ)(state));

    /// <summary>
    /// Either types are like lists of 0 or 1 items, and therefore follow the 
    /// Validation types are like lists of 0 or 1 items and therefore follow the 
    /// same rules when folding.
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Folder function, applied if in a Success state</param>
    /// <param name="Fail">Folder function, applied if in a Fail state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    public S BiFold<S>(S state, Func<S, F, S> Fail, Func<S, A, S> Succ) =>
        Match(curry(Fail)(state), curry(Succ)(state));

    /// <summary>
    /// Invokes a predicate on the value if it's in the Success state
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if in a Success state and the predicate returns `True`.  `False` otherwise.</returns>
    [Pure]
    public bool Exists(Func<A, bool> pred) =>
        Match(_ => false, pred);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Validation<F, A> Do(Action<A> f) =>
        Map(r => { f(r); return r; });
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<AF, Validation<F, B>> Traverse<AF, B>(Func<A, K<AF, B>> f) 
        where AF : Applicative<AF> =>
        AF.Map(x => x.As(), Traversable.traverse(f, this));

    /// <summary>
    /// Maps the value in the Either if it's in a Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="F1">Mapped Either type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Validation<F1, A> MapFail<F1>(Func<F, F1> f) 
        where F1 : Monoid<F1> =>
        Match(e => Validation.FailI<F1, A>(f(e)), Validation.SuccessI<F1, A>);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B"></typeparam>
    /// <param name="f"></param>
    /// <returns>Bound Either</returns>
    [Pure]
    public Validation<F, B> Bind<B>(Func<A, K<Validation<F>, B>> f) =>
        Bind(x => (Validation<F, B>)f(x));

    /// <summary>
    /// Maps the bound value 
    /// </summary>
    [Pure]
    public Validation<F, B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public Validation<F, B> SelectMany<S, B>(Func<A, Validation<F, S>> bind, Func<A, S, B> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
        
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    // `Pure` and `Fail` support
    //

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Validation<F, B> Bind<B>(Func<A, Pure<B>> f) =>
        Bind(x => (Validation<F, B>)f(x));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Validation<F, A> Bind(Func<A, Fail<F>> f) =>
        Bind(x => (Validation<F, A>)f(x));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Validation<F, Unit> Bind(Func<A, Guard<F, Unit>> f)=>
        Bind(a => f(a).ToValidationI());

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Validation<F, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Validation<F, C> SelectMany<B, C>(Func<A, Fail<F>> bind, Func<A, B, C> _) =>
        Bind(x => Validation.FailI<F, C>(bind(x).Value));
    
    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Validation<F, C> SelectMany<C>(
        Func<A, Guard<F, Unit>> f,
        Func<A, Unit, C> project) =>
        SelectMany(a => f(a).ToValidationI(), project);
    
    [Pure]
    public static implicit operator Validation<F, A>(Pure<A> mr) =>
        Validation.SuccessI<F, A>(mr.Value);

    [Pure]
    public static implicit operator Validation<F, A>(Fail<F> mr) =>
        Validation.FailI<F, A>(mr.Value);

    public override int GetHashCode() => 
        HashCode.Combine(IsSuccess, IsFail, SuccessValue, FailValue);
}

/// <summary>
/// Context for the fluent Either matching
/// </summary>
public readonly struct ValidationContext<F, A, B>
{
    readonly Validation<F, A> validation;
    readonly Func<A, B> success;

    internal ValidationContext(Validation<F, A> validation, Func<A, B> success)
    {
        this.validation = validation;
        this.success = success;
    }

    /// <summary>
    /// Fail match
    /// </summary>
    /// <param name="Fail"></param>
    /// <returns>Result of the match</returns>
    [Pure]
    public B Fail(Func<F, B> fail) =>
        validation.Match(fail, success);
}

/// <summary>
/// Context for the fluent Validation matching
/// </summary>
public readonly struct ValidationUnitContext<F, A>
{
    readonly Validation<F, A> validation;
    readonly Action<A> success;

    internal ValidationUnitContext(Validation<F, A> validation, Action<A> success)
    {
        this.validation = validation;
        this.success = success;
    }

    public Unit Left(Action<F> fail) =>
        validation.Match(fail, success);
}
