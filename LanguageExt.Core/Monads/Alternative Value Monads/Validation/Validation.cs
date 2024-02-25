using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Like `Either` but collects the failed values
/// </summary>
/// <typeparam name="MonoidFail"></typeparam>
/// <typeparam name="F"></typeparam>
/// <typeparam name="A"></typeparam>
[Serializable]
public abstract record Validation<F, A> : 
    IEnumerable<A>,
    IComparable<Validation<F, A>>,
    IComparable<A>,
    IComparable,
    IEquatable<Pure<A>>,
    IComparable<Pure<A>>,
    IEquatable<A>, 
    Monoid<Validation<F, A>>,
    K<Validation<F>, A>
    where F : Monoid<F>
{
    [Pure]
    public static Validation<F, A> Success(A value) =>
        new Validation.Success<F, A>(value);

    [Pure]
    public static Validation<F, A> Fail(F value) =>
        new Validation.Fail<F, A>(value);

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
    public abstract B Match<B>(Func<A, B> Succ, Func<F, B> Fail);

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
    /// <typeparam name="L2">Fail return</typeparam>
    /// <typeparam name="R2">Success return</typeparam>
    /// <param name="Succ">Success map function</param>
    /// <param name="Fail">Fail map function</param>
    /// <returns>Mapped Validation</returns>
    [Pure]
    public abstract Validation<L2, R2> BiMap<L2, R2>(Func<A, R2> Succ, Func<F, L2> Fail)
        where L2 : Monoid<L2>;

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
    public abstract Validation<L2, R2> BiBind<L2, R2>(
        Func<A, Validation<L2, R2>> Succ,
        Func<F, Validation<L2, R2>> Fail)
        where L2 : Monoid<L2>;

    /// <summary>
    /// Semigroup append operator
    /// </summary>
    [Pure]
    public abstract Validation<F, A> Append(Validation<F, A> rhs);

    /// <summary>
    /// Monoid empty
    /// </summary>
    [Pure]
    public static Validation<F, A> Empty { get; } = 
        new Validation.Fail<F, A>(F.Empty);

    /// <summary>
    /// Explicit conversion operator from `` to `R`
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    /// <exception cref="InvalidCastException">Value is not in a Right state</exception>
    [Pure]
    public static explicit operator A(Validation<F, A> ma) =>
        ma.SuccessValue;

    /// <summary>
    /// Explicit conversion operator from `Validation` to `L`
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    /// <exception cref="InvalidCastException">Value is not in a Fail state</exception>
    [Pure]
    public static explicit operator F(Validation<F, A> ma) =>
        ma.FailValue;

    /// <summary>
    /// Implicit conversion operator from `A` to `Validation<F, A>`
    /// </summary>
    [Pure]
    public static implicit operator Validation<F, A>(A value) =>
        new Validation.Success<F, A>(value);

    /// <summary>
    /// Implicit conversion operator from `F` to `Validation<F, A>`
    /// </summary>
    [Pure]
    public static implicit operator Validation<F, A>(F value) =>
        new Validation.Fail<F, A>(value);

    /// <summary>
    /// Invokes the `Succ` or `Fail` action depending on the state of the value
    /// </summary>
    /// <param name="Succ">Action to invoke if in a Success state</param>
    /// <param name="Fail">Action to invoke if in a Fail state</param>
    /// <returns>Unit</returns>
    public Unit Match(Action<A> Succ, Action<F> Fail) =>
        Match(fun(Succ), fun(Fail));

    /// <summary>
    /// Executes the `Fail` function if the value is in a Fail state.
    /// Returns the Success value if the value is in a Success state.
    /// </summary>
    /// <param name="Fail">Function to generate a value if in the Fail state</param>
    /// <returns>Returns an unwrapped value</returns>
    [Pure]
    public A IfFail(Func<A> Fail) =>
        Match(identity, _ => Fail());

    /// <summary>
    /// Executes the `failMap` function if the value is in a Fail state.
    /// Returns the Success value if in a Success state.
    /// </summary>
    /// <param name="failMap">Function to generate a value if in the Fail state</param>
    /// <returns>Returns an unwrapped value</returns>
    [Pure]
    public A IfFail(Func<F, A> failMap) =>
        Match(identity, failMap);

    /// <summary>
    /// Returns the `successValue` if in a Fail state.
    /// Returns the Success value if in a Success state.
    /// </summary>
    /// <param name="successValue">Value to return if in the Fail state</param>
    /// <returns>Returns an unwrapped value</returns>
    [Pure]
    public A IfFail(A successValue) =>
        Match(identity, _ => successValue);

    /// <summary>
    /// Executes the Fail action if in a Fail state.
    /// </summary>
    /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
    /// <returns>Returns unit</returns>
    public Unit IfFail(Action<F> Fail) =>
        Match(_ => { }, Fail);

    /// <summary>
    /// Invokes the `Success` action if in a Success state, otherwise does nothing
    /// </summary>
    /// <param name="Success">Action to invoke</param>
    /// <returns>Unit</returns>
    public Unit IfRight(Action<A> Success) =>
        Match(Success, _ => { });

    /// <summary>
    /// Match Success and return a context.  You must follow this with `.Fail(...)` to complete the match
    /// </summary>
    /// <param name="success">Action to invoke if in a Success state</param>
    /// <returns>Context that must have `Fail()` called upon it.</returns>
    [Pure]
    public ValidationUnitContext<F, A> Success(Action<A> success) =>
        new (this, success);

    /// <summary>
    /// Match Success and return a context.  You must follow this with `.Fail(...)` to complete the match
    /// </summary>
    /// <param name="success">Action to invoke if in a Success state</param>
    /// <returns>Context that must have `Fail()` called upon it.</returns>
    [Pure]
    public ValidationContext<F, A, B> Success<B>(Func<A, B> success) =>
        new (this, success);

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Validation<F, A> t ? CompareTo(t) : 1;

    [Pure]
    public IEnumerator<A> GetEnumerator()
    {
        foreach (var x in SuccessSpan().ToArray()) 
            yield return x;
    }

    /// <summary>
    /// Project the value into a `Lst<F>`
    /// </summary>
    /// <returns>If in a Fail state, a `Lst` of `L` with one item.  A zero length `Lst` of `L` otherwise</returns>
    [Pure]
    public Lst<F> FailToList() =>
        new(FailSpan());

    /// <summary>
    /// Project into an `Arr<F>`
    /// </summary>
    /// <returns>If in a Fail state, a `Arr` of `L` with one item.  A zero length `Arr` of `L` otherwise</returns>
    [Pure]
    public Arr<F> FailToArray() =>
        new(FailSpan());

    /// <summary>
    /// Project into a `Lst<A>`
    /// </summary>
    /// <returns>If in a Success state, a `Lst` of `R` with one item.  A zero length `Lst` of `R` otherwise</returns>
    public Lst<A> ToList() =>
        new(SuccessSpan());

    /// <summary>
    /// Project into an `Arr<A>`
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
    /// Convert either to sequence of 0 or 1 left values
    /// </summary>
    [Pure]
    public Seq<F> FailToSeq() =>
        new(FailSpan());

    [Pure]
    public Either<F, A> ToEither() =>
        this switch
        {
            Validation.Success<F, A> (var x) => new Either.Right<F, A>(x),
            Validation.Fail<F, A> (var x)    => new Either.Left<F, A>(x),
            _ => throw new NotSupportedException()
        };

    /// <summary>
    /// Convert to an Option
    /// </summary>
    /// <returns>Some(Right) or None</returns>
    [Pure]
    public Option<A> ToOption() =>
        this switch
        {
            Validation.Success<F, A> (var x) => Option<A>.Some(x),
            Validation.Fail<F, A>            => Option<A>.None,
            _                                => throw new NotSupportedException()
        };

    [Pure, MethodImpl(Opt.Default)]
    public Sum<F, A> ToSum() =>
        this switch
        {
            Validation.Success<F, A> (var x) => Sum<F, A>.Right(x),
            Validation.Fail<F, A> (var x)    => Sum<F, A>.Left(x),
            _                                => throw new NotSupportedException()
        };

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;


    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Fail<F> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Fail<F>  lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Fail<F> lhs, Validation<F, A>rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Fail<F> lhs, Validation<F, A>  rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Pure<A> lhs, Validation<F, A>  rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Pure<A> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Pure<A> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Pure<A> lhs, Validation<F, A> rhs) =>
        ((Validation<F, A>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs <= rhs</returns>
    [Pure]
    public static bool operator <=(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs > rhs</returns>
    [Pure]
    public static bool operator >(Validation<F, A> lhs, Validation<F, A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs >= rhs</returns>
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
    /// If any items are Fail then the errors are collected and returned.  If they
    /// all pass then the first successful item is returned.
    /// </summary>
    [Pure]
    public static Validation<F, A> operator |(Validation<F, A> lhs, Validation<F, A> rhs) =>
        (lhs, rhs) switch
        {
            ({ IsSuccess: true } , { IsSuccess: true }) => 
                lhs,
            
            ({ IsFail: true } , {IsFail: true}) => 
                lhs.FailValue.Append(rhs.FailValue),
            
            ({ IsFail: true } , _) => 
                lhs,
            
            _ => 
                rhs
        };
    
    /// <summary>
    /// If any items are Fail then the errors are collected and returned.  If they
    /// all pass then the Success values are collected into a `Seq`.  
    /// </summary>
    [Pure]
    public static Validation<F, Seq<A>> operator &(Validation<F, A> lhs, Validation<F, A> rhs) =>
        (lhs, rhs) switch
        {
            ({ IsSuccess: true } , { IsSuccess: true }) => 
                Validation<F, Seq<A>>.Success([lhs.SuccessValue, rhs.SuccessValue]),
            
            ({ IsFail: true } , {IsFail: true}) => 
                lhs.FailValue.Append(rhs.FailValue),
            
            ({ IsFail: true } , _) => 
                lhs.FailValue,
            
            _ => 
                rhs.FailValue
        };
    
    /// <summary>
    /// If any items are Fail then the errors are collected and returned.  If they
    /// all pass then the Success values are collected into a `Seq`.  
    /// </summary>
    [Pure]
    public static Validation<F, Seq<A>> operator &(Validation<F, Seq<A>> lhs, Validation<F, A> rhs) =>
        (lhs, rhs) switch
        {
            ({ IsSuccess: true } , { IsSuccess: true }) => 
                Validation<F, Seq<A>>.Success(lhs.SuccessValue.Add(rhs.SuccessValue)),
            
            ({ IsFail: true } , {IsFail: true}) => 
                lhs.FailValue.Append(rhs.FailValue),
            
            ({ IsFail: true } , _) => 
                lhs.FailValue,
            
            _ => 
                rhs.FailValue
        };
    
    /// <summary>
    /// If any items are Fail then the errors are collected and returned.  If they
    /// all pass then the Success values are collected into a `Seq`.  
    /// </summary>
    [Pure]
    public static Validation<F, Seq<A>> operator &(Validation<F, A> lhs, Validation<F, Seq<A>> rhs) =>
        (lhs, rhs) switch
        {
            ({ IsSuccess: true } , { IsSuccess: true }) => 
                Validation<F, Seq<A>>.Success(lhs.SuccessValue.Cons(rhs.SuccessValue)),
            
            ({ IsFail: true } , {IsFail: true}) => 
                lhs.FailValue.Append(rhs.FailValue),
            
            ({ IsFail: true } , _) => 
                lhs.FailValue,
            
            _ => 
                rhs.FailValue
        };

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Validation<F, Seq<A>> operator &(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs & (Validation<F, A>)rhs;

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Validation<F, A> operator |(Validation<F, A> lhs, Pure<A> rhs) =>
        lhs | (Validation<F, A>)rhs;

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Validation<F, Seq<A>> operator &(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs & (Validation<F, A>)rhs;

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Validation<F, A> operator |(Validation<F, A> lhs, Fail<F> rhs) =>
        lhs | (Validation<F, A>)rhs;

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
            _    => CompareTo(Success(other))
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
        other is not null && Equals(Success(other));

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
        Match(Succ, _ => { });

    /// <summary>
    /// Invokes a predicate on the success value if it's in the Success state
    /// </summary>
    /// <returns>
    /// True if in a `Left` state.  
    /// `True` if the in a `Right` state and the predicate returns `True`.  
    /// `False` otherwise.</returns>
    [Pure]
    public bool ForAll(Func<A, bool> Succ) =>
        Match(Succ, _ => true);

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
    public bool BiForAll(Func<A, bool> Succ, Func<F, bool> Fail) =>
        Match(Succ, Fail);

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
        Match(curry(Succ)(state), _ => state);

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
    public S BiFold<S>(S state, Func<S, A, S> Succ, Func<S, F, S> Fail) =>
        Match(curry(Succ)(state), curry(Fail)(state));

    /// <summary>
    /// Invokes a predicate on the value if it's in the Success state
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>True if in a Success state and the predicate returns `True`.  `False` otherwise.</returns>
    [Pure]
    public bool Exists(Func<A, bool> pred) =>
        Match(pred, _ => false);

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
    /// </remarks>
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
        Match(Validation<F1, A>.Success, e => Validation<F1, A>.Fail(f(e)));

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
    /// Filter the Validation
    /// </summary>
    /// <remarks>
    /// If the predicate returns `false` then the `Validation` goes into a failed state
    /// using `Monoid.Empty` of `F` as its failure value.
    /// </remarks>
    [Pure]
    public Validation<F, A> Filter(Func<A, bool> pred) =>
        Bind(x => pred(x) ? Success(x) : Fail(F.Empty));

    /// <summary>
    /// Filter the Validation
    /// </summary>
    /// <remarks>
    /// If the predicate returns `false` then the `Validation` goes into a failed state
    /// using `Monoid.Empty` of `F` as its failure value.
    /// </remarks>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Validation<F, A> Where(Func<A, bool> pred) =>
        Filter(pred);

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
        Bind(a => f(a).ToValidation());

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
        Bind(x => Validation<F, C>.Fail(bind(x).Value));
    
    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Validation<F, C> SelectMany<C>(
        Func<A, Guard<F, Unit>> f,
        Func<A, Unit, C> project) =>
        SelectMany(a => f(a).ToValidation(), project);
    
    [Pure]
    public static implicit operator Validation<F, A>(Pure<A> mr) =>
        Success(mr.Value);

    [Pure]
    public static implicit operator Validation<F, A>(Fail<F> mr) =>
        Fail(mr.Value);

    public override int GetHashCode()
    {
        return HashCode.Combine(IsSuccess, IsFail, SuccessValue, FailValue);
    }
}

/// <summary>
/// Context for the fluent Either matching
/// </summary>
public struct ValidationContext<F, A, B>
    where F : Monoid<F>
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
        validation.Match(success, fail);
}

/// <summary>
/// Context for the fluent Validation matching
/// </summary>
public struct ValidationUnitContext<F, A>
    where F : Monoid<F>
{
    readonly Validation<F, A> validation;
    readonly Action<A> success;

    internal ValidationUnitContext(Validation<F, A> validation, Action<A> success)
    {
        this.validation = validation;
        this.success = success;
    }

    public Unit Left(Action<F> fail) =>
        validation.Match(success, fail);
}


//     readonly FAIL fail;
    //     readonly SUCCESS success;
    //     readonly Validation.StateType state;
    //
    //     Validation(SUCCESS success)
    //     {
    //         if (isnull(success)) throw new ValueIsNullException();
    //         this.success = success;
    //         this.fail = default(FAIL);
    //         this.state = Validation.StateType.Success;
    //     }
    //
    //     Validation(FAIL fail)
    //     {
    //         if (isnull(fail)) throw new ValueIsNullException();
    //         this.success = default(SUCCESS);
    //         this.fail = fail;
    //         this.state = Validation.StateType.Fail;
    //     }
    //
    //     /// <summary>
    //     /// Ctor that facilitates serialisation
    //     /// </summary>
    //     public Validation(IEnumerable<ValidationData<MonoidFail, FAIL, SUCCESS>> validationData)
    //     {
    //         var seq = toSeq(validationData);
    //         if (seq.IsEmpty)
    //         {
    //             this.state = Validation.StateType.Fail;
    //             this.fail = default(MonoidFail).Empty();
    //             this.success = default(SUCCESS);
    //         }
    //         else
    //         {
    //             this.fail = seq.Head.Fail;
    //             this.success = seq.Head.Success;
    //             this.state = seq.Head.State;
    //         }
    //     }
    //
    //     /// <summary>
    //     /// Ctor that facilitates serialisation
    //     /// </summary>
    //     Validation(SerializationInfo info, StreamingContext context)
    //     {
    //         state = (Validation.StateType)info.GetValue("State", typeof(Validation.StateType));
    //         switch (state)
    //         {
    //             case Validation.StateType.Success:
    //                 success = (SUCCESS)info.GetValue("Success", typeof(SUCCESS));
    //                 fail = default(FAIL);
    //                 break;
    //             case Validation.StateType.Fail:
    //                 fail = (FAIL)info.GetValue("Fail", typeof(FAIL));
    //                 success = default(SUCCESS);
    //                 break;
    //
    //             default:
    //                 throw new NotSupportedException();
    //         }
    //     }
    //
    //     public void GetObjectData(SerializationInfo info, StreamingContext context)
    //     {
    //         info.AddValue("State", state);
    //         if (IsSuccess) info.AddValue("Success", SuccessValue);
    //         if (IsFail) info.AddValue("Fail", FailValue);
    //     }
    //
    //     internal FAIL FailValue => isnull(fail) ? default(MonoidFail).Empty() : fail;
    //     internal SUCCESS SuccessValue => success;
    //
    //     [Pure]
    //     public bool IsFail =>
    //         state == Validation.StateType.Fail;
    //
    //     [Pure]
    //     public bool IsSuccess =>
    //         state == Validation.StateType.Success;
    //
    //     IEnumerable<ValidationData<MonoidFail, FAIL, SUCCESS>> Enum()
    //     {
    //         yield return new ValidationData<MonoidFail, FAIL, SUCCESS>(state, success, FailValue);
    //     }
    //
    //     public IEnumerator<ValidationData<MonoidFail, FAIL, SUCCESS>> GetEnumerator() =>
    //         Enum().GetEnumerator();
    //
    //     IEnumerator IEnumerable.GetEnumerator() =>
    //         Enum().GetEnumerator();
    //
    //     /// <summary>
    //     /// Implicit conversion operator from `SUCCESS` to `Validation<MonoidFail, FAIL, SUCCESS>`
    //     /// </summary>
    //     /// <param name="value">`value`, must not be `null`.</param>
    //     /// <exception cref="ValueIsNullException">`value` is `null`</exception>
    //     [Pure]
    //     public static implicit operator Validation<MonoidFail, FAIL, SUCCESS>(SUCCESS value) =>
    //         isnull(value)
    //             ? throw new ValueIsNullException()
    //             : Success(value);
    //
    //     /// <summary>
    //     /// Implicit conversion operator from `FAIL` to `Validation<MonoidFail, FAIL, SUCCESS>`
    //     /// </summary>
    //     /// <param name="value">`value`, must not be `null`.</param>
    //     /// <exception cref="ValueIsNullException">`value` is `null`</exception>
    //     [Pure]
    //     public static implicit operator Validation<MonoidFail, FAIL, SUCCESS>(FAIL value) =>
    //         isnull(value)
    //             ? throw new ValueIsNullException()
    //             : Fail(value);
    //
    //     /// <summary>
    //     /// Explicit conversion operator from `Validation` to `SUCCESS`
    //     /// </summary>
    //     /// <param name="value">Value, must not be null.</param>
    //     /// <exception cref="ValueIsNullException">Value is null</exception>
    //     [Pure]
    //     public static explicit operator SUCCESS(Validation<MonoidFail, FAIL, SUCCESS> ma) =>
    //         ma.IsSuccess
    //             ? ma.SuccessValue
    //             : throw new InvalidCastException("Validation is not in a Success state");
    //
    //     /// <summary>
    //     /// Explicit conversion operator from `Validation` to `FAIL`
    //     /// </summary>
    //     /// <param name="value">Value, must not be null.</param>
    //     /// <exception cref="ValueIsNullException">Value is null</exception>
    //     [Pure]
    //     public static explicit operator FAIL(Validation<MonoidFail, FAIL, SUCCESS> ma) =>
    //         ma.IsFail
    //             ? ma.FailValue
    //             : throw new InvalidCastException("Validation is not in a Fail state");
    //
    //     /// <summary>
    //     /// Reference version for use in pattern-matching
    //     /// </summary>
    //     /// <remarks>
    //     ///     Validation Succ   = result is SUCCESS
    //     ///     Validation Fail   = result is FAIL
    //     ///     Validation Bottom = result is mempty FAIL
    //     /// </remarks>
    //     [Pure]
    //     public object Case =>
    //         state switch
    //         {
    //             Validation.StateType.Success => success,
    //             Validation.StateType.Fail    => fail,
    //             _                            => default(MonoidFail).Empty()
    //         };
    //
    //     /// <summary>
    //     /// Success constructor
    //     /// </summary>
    //     [Pure]
    //     public static Validation<MonoidFail, FAIL, SUCCESS> Success(SUCCESS success) =>
    //         new Validation<MonoidFail, FAIL, SUCCESS>(success);
    //
    //     /// <summary>
    //     /// Fail constructor
    //     /// </summary>
    //     [Pure]
    //     public static Validation<MonoidFail, FAIL, SUCCESS> Fail(FAIL fail) =>
    //         new Validation<MonoidFail, FAIL, SUCCESS>(fail);
    //
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, SUCCESS> Disjunction<SUCCESSB>(Validation<MonoidFail, FAIL, SUCCESSB> other)
    //     {
    //         if (IsSuccess && other.IsSuccess) return this;
    //         if (IsSuccess) return new Validation<MonoidFail, FAIL, SUCCESS>(other.FailValue);
    //         if (other.IsSuccess) return this;
    //         return new Validation<MonoidFail, FAIL, SUCCESS>(default(MonoidFail).Append(FailValue, other.FailValue));
    //     }
    //
    //     /// <summary>
    //     /// Fluent matching
    //     /// </summary>
    //     public ValidationContext<MonoidFail, FAIL, SUCCESS, Ret> Succ<Ret>(Func<SUCCESS, Ret> f) =>
    //         new ValidationContext<MonoidFail, FAIL, SUCCESS, Ret>(this, f);
    //
    //     /// <summary>
    //     /// Fluent matching
    //     /// </summary>
    //     public ValidationUnitContext<MonoidFail, FAIL, SUCCESS> Succ<Ret>(Action<SUCCESS> f) =>
    //         new ValidationUnitContext<MonoidFail, FAIL, SUCCESS>(this, f);
    //
    //     /// <summary>
    //     /// Invokes the `Succ` or `Fail` function depending on the state of the `Validation`
    //     /// </summary>
    //     /// <typeparam name="Ret">Return type</typeparam>
    //     /// <param name="Succ">Function to invoke if in a `Success` state</param>
    //     /// <param name="Fail">Function to invoke if in a `Fail` state</param>
    //     /// <returns>The return value of the invoked function</returns>
    //     [Pure]
    //     public Ret Match<Ret>(Func<SUCCESS, Ret> Succ, Func<FAIL, Ret> Fail) =>
    //         Check.NullReturn(MatchUnsafe(Succ, Fail));
    //
    //     /// <summary>
    //     /// Returns `Succ` value or invokes `Fail` function depending on the state of the `Validation`
    //     /// </summary>
    //     /// <typeparam name="Ret">Return type</typeparam>
    //     /// <param name="Succ">Value to return if in a `Success` state</param>
    //     /// <param name="Fail">Function to invoke if in a `Fail` state</param>
    //     /// <returns>The return value of the invoked function</returns>
    //     [Pure]
    //     public Ret Match<Ret>(Ret Succ, Func<FAIL, Ret> Fail) =>
    //         Check.NullReturn(MatchUnsafe(_ => Succ, Fail));
    //
    //     /// <summary>
    //     /// Invokes the `Succ` or `Fail` function depending on the state of the `Validation`
    //     /// </summary>
    //     /// <typeparam name="Ret">Return type</typeparam>
    //     /// <param name="Succ">Function to invoke if in a `Success` state</param>
    //     /// <param name="Fail">Function to invoke if in a `Fail` state</param>
    //     /// <returns>The return value of the invoked function</returns>
    //     [Pure]
    //     public Ret MatchUnsafe<Ret>(Func<SUCCESS, Ret> Succ, Func<FAIL, Ret> Fail) =>
    //         IsFail
    //             ? Fail == null
    //                 ? throw new ArgumentNullException(nameof(Fail))
    //                 : Fail(FailValue)
    //             : Succ == null
    //                 ? throw new ArgumentNullException(nameof(Succ))
    //                 : Succ(success);
    //
    //     /// <summary>
    //     /// Invokes the `Succ` or `Fail` action depending on the state of the `Validation`
    //     /// </summary>
    //     /// <param name="Succ">Action to invoke if in a `Success` state</param>
    //     /// <param name="Fail">Action to invoke if in a `Fail` state</param>
    //     /// <returns>Unit</returns>
    //     public Unit Match(Action<SUCCESS> Succ, Action<FAIL> Fail)
    //     {
    //         if (IsFail)
    //         {
    //             Fail(FailValue);
    //         }
    //         else 
    //         {
    //             Succ(success);
    //         }
    //         return unit;
    //     }
    //
    //     /// <summary>
    //     /// Match the two states of the Validation and return a promise for a non-null R2.
    //     /// </summary>
    //     /// <returns>A promise to return a non-null R2</returns>
    //     public async Task<R2> MatchAsync<R2>(Func<SUCCESS, Task<R2>> SuccAsync, Func<FAIL, R2> Fail) =>
    //         await Match(SuccAsync, f => Fail(f).AsTask());
    //
    //     /// <summary>
    //     /// Match the two states of the Validation and return a promise for a non-null R2.
    //     /// </summary>
    //     /// <returns>A promise to return a non-null R2</returns>
    //     public async Task<R2> MatchAsync<R2>(Func<SUCCESS, Task<R2>> SuccAsync, Func<FAIL, Task<R2>> FailAsync) =>
    //         await Match(SuccAsync, FailAsync);
    //
    //     /// <summary>
    //     /// Executes the Fail function if the Validation is in a Fail state.
    //     /// Returns the Success value if the Validation is in a Success state.
    //     /// </summary>
    //     /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
    //     /// <returns>Returns an unwrapped Success value</returns>
    //     [Pure]
    //     public SUCCESS IfFail(Func<SUCCESS> Fail) =>
    //         ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail);
    //
    //     /// <summary>
    //     /// Executes the FailMap function if the Validation is in a Fail state.
    //     /// Returns the Success value if the Validation is in a Success state.
    //     /// </summary>
    //     /// <param name="FailMap">Function to generate a Success value if in the Fail state</param>
    //     /// <returns>Returns an unwrapped Success value</returns>
    //     [Pure]
    //     public SUCCESS IfFail(Func<FAIL, SUCCESS> FailMap) =>
    //         ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, FailMap);
    //
    //     /// <summary>
    //     /// Returns the SuccessValue if the Validation is in a Fail state.
    //     /// Returns the Success value if the Validation is in a Success state.
    //     /// </summary>
    //     /// <param name="SuccessValue">Value to return if in the Fail state</param>
    //     /// <returns>Returns an unwrapped Success value</returns>
    //     [Pure]
    //     public SUCCESS IfFail(SUCCESS SuccessValue) =>
    //         ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, SuccessValue);
    //
    //     /// <summary>
    //     /// Executes the Fail action if the Validation is in a Fail state.
    //     /// </summary>
    //     /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
    //     /// <returns>Returns an unwrapped Success value</returns>
    //     public Unit IfFail(Action<FAIL> Fail) =>
    //         ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail);
    //
    //     /// <summary>
    //     /// Invokes the Success action if the Validation is in a Success state, otherwise does nothing
    //     /// </summary>
    //     /// <param name="Success">Action to invoke</param>
    //     /// <returns>Unit</returns>
    //     public Unit IfSuccess(Action<SUCCESS> Success) =>
    //         ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Success);
    //
    //     /// <summary>
    //     /// Returns the FailValue if the Validation is in a Success state.
    //     /// Returns the Fail value if the Validation is in a Fail state.
    //     /// </summary>
    //     /// <param name="FailValue">Value to return if in the Fail state</param>
    //     /// <returns>Returns an unwrapped Fail value</returns>
    //     [Pure]
    //     public FAIL IfSuccess(FAIL FailValue) =>
    //         ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, FailValue);
    //
    //     /// <summary>
    //     /// Returns the result of Success() if the Validation is in a Success state.
    //     /// Returns the Fail value if the Validation is in a Fail state.
    //     /// </summary>
    //     /// <param name="Success">Function to generate a Fail value if in the Success state</param>
    //     /// <returns>Returns an unwrapped Fail value</returns>
    //     [Pure]
    //     public FAIL IfSuccess(Func<FAIL> Success) =>
    //         ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Success);
    //
    //     /// <summary>
    //     /// Returns the result of SuccessMap if the Validation is in a Success state.
    //     /// Returns the Fail value if the Validation is in a Fail state.
    //     /// </summary>
    //     /// <param name="SuccessMap">Function to generate a Fail value if in the Success state</param>
    //     /// <returns>Returns an unwrapped Fail value</returns>
    //     [Pure]
    //     public FAIL IfSuccess(Func<SUCCESS, FAIL> SuccessMap) =>
    //         ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, SuccessMap);
    //
    //     /// <summary>
    //     /// Return a string representation of the Validation
    //     /// </summary>
    //     /// <returns>String representation of the Validation</returns>
    //     [Pure]
    //     public override string ToString() =>
    //         IsSuccess
    //             ? isnull(success)
    //                 ? "Success(null)"
    //                 : $"Success({success})"
    //             : $"Fail({FailValue})";
    //
    //     /// <summary>
    //     /// Returns a hash code of the wrapped value of the Validation
    //     /// </summary>
    //     /// <returns>Hash code</returns>
    //     [Pure]
    //     public override int GetHashCode() =>
    //         hashCode<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     [Pure]
    //     public int CompareTo(object obj) =>
    //         obj is Validation<MonoidFail, FAIL, SUCCESS> t ? CompareTo(t) : 1;
    //
    //     /// <summary>
    //     /// Equality check
    //     /// </summary>
    //     /// <param name="obj">Object to test for equality</param>
    //     /// <returns>True if equal</returns>
    //     [Pure]
    //     public override bool Equals(object obj) =>
    //         !ReferenceEquals(obj, null) &&
    //         obj is Validation<MonoidFail, FAIL, SUCCESS> &&
    //         EqChoice<
    //             MonoidFail, 
    //             EqDefault<SUCCESS>, 
    //             FoldValidation<MonoidFail, FAIL, SUCCESS>, 
    //             Validation<MonoidFail, FAIL, SUCCESS>, 
    //             FAIL, SUCCESS>
    //            .Inst.Equals(this, (Validation<MonoidFail, FAIL, SUCCESS>)obj);
    //
    //
    //     /// <summary>
    //     /// Project the Validation into a Lst
    //     /// </summary>
    //     [Pure]
    //     public Lst<SUCCESS> SuccessToList() =>
    //         rightToList<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Project the Validation into an immutable array
    //     /// </summary>
    //     [Pure]
    //     public Arr<SUCCESS> SuccessToArray() =>
    //         rightToArray<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Project the Validation into a Lst
    //     /// </summary>
    //     [Pure]
    //     public Lst<FAIL> FailToList() =>
    //         leftToList<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Project the Validation into an immutable array R
    //     /// </summary>
    //     [Pure]
    //     public Arr<FAIL> FailToArray() =>
    //         leftToArray<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Convert Validation to sequence of 0 or 1 right values
    //     /// </summary>
    //     [Pure]
    //     public Seq<SUCCESS> ToSeq() =>
    //         SuccessAsEnumerable();
    //
    //     /// <summary>
    //     /// Convert Validation to sequence of 0 or 1 success values
    //     /// </summary>
    //     [Pure]
    //     public Seq<SUCCESS> SuccessToSeq() =>
    //         SuccessAsEnumerable();
    //
    //     /// <summary>
    //     /// Convert Validation to sequence of 0 or 1 success values
    //     /// </summary>
    //     [Pure]
    //     public Seq<FAIL> FailToSeq() =>
    //         FailAsEnumerable();
    //
    //     /// <summary>
    //     /// Project the Validation success into a Seq
    //     /// </summary>
    //     [Pure]
    //     public Seq<SUCCESS> SuccessAsEnumerable() =>
    //         rightAsEnumerable<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Project the Validation fail into a Seq
    //     /// </summary>
    //     [Pure]
    //     public Seq<FAIL> FailAsEnumerable() =>
    //         leftAsEnumerable<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     [Pure]
    //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //     public Eff<SUCCESS> ToEff(Func<FAIL, Error> Fail) =>
    //         state switch
    //         {
    //             Validation.StateType.Success => SuccessEff<SUCCESS>(SuccessValue),
    //             _                            => FailEff<SUCCESS>(Fail(FailValue))
    //         };
    //
    //     [Pure]
    //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //     public Aff<SUCCESS> ToAff(Func<FAIL, Error> Fail) =>
    //         state switch
    //         {
    //             Validation.StateType.Success => SuccessAff<SUCCESS>(SuccessValue),
    //             _                            => FailAff<SUCCESS>(Fail(FailValue))
    //         };
    //     
    //     /// <summary>
    //     /// Convert the Validation to an Option
    //     /// </summary>
    //     [Pure]
    //     public Option<SUCCESS> ToOption() =>
    //         toOption<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Convert the Validation to an EitherUnsafe
    //     /// </summary>
    //     [Pure]
    //     public Either<FAIL, SUCCESS> ToEither() =>
    //         toEither<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Convert the Validation to an EitherUnsafe
    //     /// </summary>
    //     [Pure]
    //     public EitherUnsafe<FAIL, SUCCESS> ToEitherUnsafe() =>
    //         toEitherUnsafe<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //     /// <summary>
    //     /// Convert the Validation to an TryOption
    //     /// </summary>
    //     [Pure]
    //     public TryOption<SUCCESS> ToTryOption() =>
    //         toTryOption<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);
    //
    //
    //     /// <summary>
    //     /// Comparison operator
    //     /// </summary>
    //     /// <param name="lhs">The left hand side of the operation</param>
    //     /// <param name="rhs">The right hand side of the operation</param>
    //     /// <returns>True if lhs < rhs</returns>
    //     [Pure]
    //     public static bool operator <(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         OrdChoice<
    //             OrdDefault<FAIL>, 
    //             OrdDefault<SUCCESS>, 
    //             FoldValidation<MonoidFail, FAIL, SUCCESS>, 
    //             Validation<MonoidFail, FAIL, SUCCESS>, 
    //             FAIL, SUCCESS>
    //            .Inst.Compare(lhs, rhs) < 0;
    //
    //     /// <summary>
    //     /// Comparison operator
    //     /// </summary>
    //     /// <param name="lhs">The left hand side of the operation</param>
    //     /// <param name="rhs">The right hand side of the operation</param>
    //     /// <returns>True if lhs <= rhs</returns>
    //     [Pure]
    //     public static bool operator <=(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         OrdChoice<
    //             OrdDefault<FAIL>,
    //             OrdDefault<SUCCESS>,
    //             FoldValidation<MonoidFail, FAIL, SUCCESS>,
    //             Validation<MonoidFail, FAIL, SUCCESS>,
    //             FAIL, SUCCESS>
    //            .Inst.Compare(lhs, rhs) <= 0;
    //
    //     /// <summary>
    //     /// Comparison operator
    //     /// </summary>
    //     /// <param name="lhs">The left hand side of the operation</param>
    //     /// <param name="rhs">The right hand side of the operation</param>
    //     /// <returns>True if lhs > rhs</returns>
    //     [Pure]
    //     public static bool operator >(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         OrdChoice<
    //             OrdDefault<FAIL>,
    //             OrdDefault<SUCCESS>,
    //             FoldValidation<MonoidFail, FAIL, SUCCESS>,
    //             Validation<MonoidFail, FAIL, SUCCESS>,
    //             FAIL, SUCCESS>
    //            .Inst.Compare(lhs, rhs) > 0;
    //
    //     /// <summary>
    //     /// Comparison operator
    //     /// </summary>
    //     /// <param name="lhs">The left hand side of the operation</param>
    //     /// <param name="rhs">The right hand side of the operation</param>
    //     /// <returns>True if lhs >= rhs</returns>
    //     [Pure]
    //     public static bool operator >=(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         OrdChoice<
    //             OrdDefault<FAIL>,
    //             OrdDefault<SUCCESS>,
    //             FoldValidation<MonoidFail, FAIL, SUCCESS>,
    //             Validation<MonoidFail, FAIL, SUCCESS>,
    //             FAIL, SUCCESS>
    //            .Inst.Compare(lhs, rhs) >= 0;
    //
    //     /// <summary>
    //     /// Equality operator override
    //     /// </summary>
    //     [Pure]
    //     public static bool operator ==(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         lhs.Equals(rhs);
    //
    //     /// <summary>
    //     /// Non-equality operator override
    //     /// </summary>
    //     [Pure]
    //     public static bool operator !=(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         !(lhs == rhs);
    //
    //     /// <summary>
    //     /// Coalescing operator
    //     /// </summary>
    //     [Pure]
    //     public static Validation<MonoidFail, FAIL, SUCCESS> operator |(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
    //         default(FoldValidation<MonoidFail, FAIL, SUCCESS>).Append(lhs, rhs);
    //
    //     /// <summary>
    //     /// Override of the True operator to return True if the Validation is Success
    //     /// </summary>
    //     [Pure]
    //     public static bool operator true(Validation<MonoidFail, FAIL, SUCCESS> value) =>
    //         value.IsSuccess;
    //
    //     /// <summary>
    //     /// Override of the False operator to return True if the Validation is Fail
    //     /// </summary>
    //     [Pure]
    //     public static bool operator false(Validation<MonoidFail, FAIL, SUCCESS> value) =>
    //         value.IsFail;
    //
    //     /// <summary>
    //     /// CompareTo override
    //     /// </summary>
    //     [Pure]
    //     public int CompareTo(Validation<MonoidFail, FAIL, SUCCESS> other) =>
    //         CompareTo<OrdDefault<FAIL>, OrdDefault<SUCCESS>>(other);
    //
    //     /// <summary>
    //     /// CompareTo override
    //     /// </summary>
    //     [Pure]
    //     public int CompareTo<OrdSuccess>(Validation<MonoidFail, FAIL, SUCCESS> other) 
    //         where OrdSuccess : Ord<SUCCESS> =>
    //         CompareTo<OrdDefault<FAIL>, OrdSuccess>(other);
    //
    //     /// <summary>
    //     /// CompareTo override
    //     /// </summary>
    //     [Pure]
    //     public int CompareTo<OrdFail, OrdSuccess>(Validation<MonoidFail, FAIL, SUCCESS> other) 
    //         where OrdFail : Ord<FAIL>
    //         where OrdSuccess : Ord<SUCCESS> =>
    //         OrdChoice<
    //                 OrdFail,
    //                 OrdSuccess,
    //                 FoldValidation<MonoidFail, FAIL, SUCCESS>,
    //                 Validation<MonoidFail, FAIL, SUCCESS>,
    //                 FAIL, SUCCESS>
    //            .Inst.Compare(this, other);
    //
    //     /// <summary>
    //     /// CompareTo override
    //     /// </summary>
    //     [Pure]
    //     public int CompareTo(SUCCESS success) =>
    //         CompareTo(Success(success));
    //
    //     /// <summary>
    //     /// CompareTo override
    //     /// </summary>
    //     [Pure]
    //     public int CompareTo(FAIL fail) =>
    //         CompareTo(Fail(fail));
    //
    //     /// <summary>
    //     /// Equality override
    //     /// </summary>
    //     [Pure]
    //     public bool Equals(SUCCESS success) =>
    //         Equals(Success(success));
    //
    //     /// <summary>
    //     /// Equality override
    //     /// </summary>
    //     [Pure]
    //     public bool Equals(FAIL fail) =>
    //         Equals(Fail(fail));
    //
    //     /// <summary>
    //     /// Equality override
    //     /// </summary>
    //     [Pure]
    //     public bool Equals(Validation<MonoidFail, FAIL, SUCCESS> other) =>
    //         EqChoice<
    //             MonoidFail, 
    //             EqDefault<SUCCESS>, 
    //             FoldValidation<MonoidFail, FAIL, SUCCESS>, 
    //             Validation<MonoidFail, FAIL, SUCCESS>, 
    //             FAIL, SUCCESS>
    //            .Inst.Equals(this, other);
    //
    //
    //
    //     /// <summary>
    //     /// Counts the Validation
    //     /// </summary>
    //     /// <param name="self">Validation to count</param>
    //     /// <returns>1 if the Validation is in a Success state, 0 otherwise.</returns>
    //     [Pure]
    //     public int Count() =>
    //         IsFail
    //             ? 0
    //             : 1;
    //
    //     /// <summary>
    //     /// Iterate the Validation
    //     /// action is invoked if in the Success state
    //     /// </summary>
    //     public Unit Iter(Action<SUCCESS> Success) =>
    //         iter<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>(this, Success);
    //
    //     /// <summary>
    //     /// Iterate the Validation
    //     /// action is invoked if in the Success state
    //     /// </summary>
    //     public Unit BiIter(Action<SUCCESS> Success, Action<FAIL> Fail) =>
    //         biIter<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);
    //
    //     /// <summary>
    //     /// Invokes a predicate on the value of the Validation if it's in the Success state
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="R">Success</typeparam>
    //     /// <param name="self">Validation to forall</param>
    //     /// <param name="Success">Predicate</param>
    //     /// <returns>True if the Validation is in a Fail state.  
    //     /// True if the Validation is in a Success state and the predicate returns True.  
    //     /// False otherwise.</returns>
    //     [Pure]
    //     public bool ForAll(Func<SUCCESS, bool> Success) =>
    //         forall<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>(this, Success);
    //
    //     /// <summary>
    //     /// Invokes a predicate on the value of the Validation if it's in the Success state
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="R">Success</typeparam>
    //     /// <param name="self">Validation to forall</param>
    //     /// <param name="Success">Predicate</param>
    //     /// <param name="Fail">Predicate</param>
    //     /// <returns>True if Validation Predicate returns true</returns>
    //     [Pure]
    //     public bool BiForAll(Func<SUCCESS, bool> Success, Func<FAIL, bool> Fail) =>
    //         biForAll<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);
    //
    //     /// <summary>
    //     /// <para>
    //     /// Validation types are like lists of 0 or 1 items, and therefore follow the 
    //     /// same rules when folding.
    //     /// </para><para>
    //     /// In the case of lists, 'Fold', when applied to a binary
    //     /// operator, a starting value(typically the Fail-identity of the operator),
    //     /// and a list, reduces the list using the binary operator, from Fail to
    //     /// Success:
    //     /// </para>
    //     /// </summary>
    //     /// <typeparam name="S">Aggregate state type</typeparam>
    //     /// <param name="state">Initial state</param>
    //     /// <param name="Success">Folder function, applied if structure is in a Success state</param>
    //     /// <returns>The aggregate state</returns>
    //     [Pure]
    //     public S Fold<S>(S state, Func<S, SUCCESS, S> Success) =>
    //         default(FoldValidation<MonoidFail, FAIL, SUCCESS>).Fold(this, state, Success)(unit);
    //
    //     /// <summary>
    //     /// <para>
    //     /// Validation types are like lists of 0 or 1 items, and therefore follow the 
    //     /// same rules when folding.
    //     /// </para><para>
    //     /// In the case of lists, 'Fold', when applied to a binary
    //     /// operator, a starting value(typically the Fail-identity of the operator),
    //     /// and a list, reduces the list using the binary operator, from Fail to
    //     /// Success:
    //     /// </para>
    //     /// </summary>
    //     /// <typeparam name="S">Aggregate state type</typeparam>
    //     /// <param name="state">Initial state</param>
    //     /// <param name="Success">Folder function, applied if Validation is in a Success state</param>
    //     /// <param name="Fail">Folder function, applied if Validation is in a Fail state</param>
    //     /// <returns>The aggregate state</returns>
    //     [Pure]
    //     public S BiFold<S>(S state, Func<S, SUCCESS, S> Success, Func<S, FAIL, S> Fail) =>
    //         default(FoldValidation<MonoidFail, FAIL, SUCCESS>).BiFold(this, state, Fail, Success);
    //
    //     /// <summary>
    //     /// Invokes a predicate on the value of the Validation if it's in the Success state
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="R">Success</typeparam>
    //     /// <param name="self">Validation to check existence of</param>
    //     /// <param name="pred">Predicate</param>
    //     /// <returns>True if the Validation is in a Success state and the predicate returns True.  False otherwise.</returns>
    //     [Pure]
    //     public bool Exists(Func<SUCCESS, bool> pred) =>
    //         exists<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>(this, pred);
    //
    //     /// <summary>
    //     /// Invokes a predicate on the value of the Validation
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="R">Success</typeparam>
    //     /// <param name="self">Validation to check existence of</param>
    //     /// <param name="Success">Success predicate</param>
    //     /// <param name="Fail">Fail predicate</param>
    //     [Pure]
    //     public bool BiExists(Func<SUCCESS, bool> Success, Func<FAIL, bool> Fail) =>
    //         biExists<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);
    //
    //     /// <summary>
    //     /// Impure iteration of the bound value in the structure
    //     /// </summary>
    //     /// <returns>
    //     /// Returns the original unmodified structure
    //     /// </returns>
    //     public Validation<MonoidFail, FAIL, SUCCESS> Do(Action<SUCCESS> f)
    //     {
    //         Iter(f);
    //         return this;
    //     }
    //
    //     /// <summary>
    //     /// Maps the value in the Validation if it's in a Success state
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="R">Success</typeparam>
    //     /// <typeparam name="Ret">Mapped Validation type</typeparam>
    //     /// <param name="self">Validation to map</param>
    //     /// <param name="mapper">Map function</param>
    //     /// <returns>Mapped Validation</returns>
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, Ret> Map<Ret>(Func<SUCCESS, Ret> mapper) =>
    //         FValidation<MonoidFail, FAIL, SUCCESS, Ret>.Inst.Map(this, mapper);
    //
    //     /// <summary>
    //     /// Bi-maps the value in the Validation if it's in a Success state
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="R">Success</typeparam>
    //     /// <typeparam name="RRet">Success return</typeparam>
    //     /// <param name="self">Validation to map</param>
    //     /// <param name="Success">Success map function</param>
    //     /// <param name="Fail">Fail map function</param>
    //     /// <returns>Mapped Validation</returns>
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, Ret> BiMap<Ret>(Func<SUCCESS, Ret> Success, Func<FAIL, Ret> Fail) =>
    //         FValidation<MonoidFail, FAIL, SUCCESS, Ret>.Inst.BiMap(this, Fail, Success);
    //
    //     /// <summary>
    //     /// Maps the value in the Validation if it's in a Fail state
    //     /// </summary>
    //     /// <typeparam name="MonoidRet">Monad of Fail</typeparam>
    //     /// <typeparam name="Ret">Fail return</typeparam>
    //     /// <param name="Fail">Fail map function</param>
    //     /// <returns>Mapped Validation</returns>
    //     [Pure]
    //     public Validation<MonoidRet, Ret, SUCCESS> MapFail<MonoidRet, Ret>(Func<FAIL, Ret> Fail) where MonoidRet : Monoid<Ret>, Eq<Ret> =>
    //         FValidationBi<MonoidFail, FAIL, SUCCESS, MonoidRet, Ret, SUCCESS>.Inst.BiMap(this, Fail, identity);
    //
    //     /// <summary>
    //     /// Bi-maps the value in the Validation
    //     /// </summary>
    //     /// <typeparam name="MonoidFail2">Monad of Fail</typeparam>
    //     /// <typeparam name="FAIL2">Fail return</typeparam>
    //     /// <typeparam name="SUCCESS2">Success return</typeparam>
    //     /// <param name="Success">Success map function</param>
    //     /// <param name="Fail">Fail map function</param>
    //     /// <returns>Mapped Validation</returns>
    //     [Pure]
    //     public Validation<MonoidFail2, FAIL2, SUCCESS2> BiMap<MonoidFail2, FAIL2, SUCCESS2>(Func<SUCCESS, SUCCESS2> Success, Func<FAIL, FAIL2> Fail) where MonoidFail2 : Monoid<FAIL2>, Eq<FAIL2> =>
    //         FValidationBi<MonoidFail, FAIL, SUCCESS, MonoidFail2, FAIL2, SUCCESS2>.Inst.BiMap(this, Fail, Success);
    //
    //     /// <summary>
    //     /// Maps the value in the Validation if it's in a Success state
    //     /// </summary>
    //     /// <typeparam name="L">Fail</typeparam>
    //     /// <typeparam name="TR">Success</typeparam>
    //     /// <typeparam name="UR">Mapped Validation type</typeparam>
    //     /// <param name="self">Validation to map</param>
    //     /// <param name="map">Map function</param>
    //     /// <returns>Mapped Validation</returns>
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, U> Select<U>(Func<SUCCESS, U> map) =>
    //         FValidation<MonoidFail, FAIL, SUCCESS, U>.Inst.Map(this, map);
    //
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, U> Bind<U>(Func<SUCCESS, Validation<MonoidFail, FAIL, U>> f) =>
    //         IsSuccess
    //             ? f(success)
    //             : Validation<MonoidFail, FAIL, U>.Fail(FailValue);
    //
    //     /// <summary>
    //     /// Bi-bind.  Allows mapping of both monad states
    //     /// </summary>
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, B> BiBind<B>(Func<SUCCESS, Validation<MonoidFail, FAIL, B>> Succ, Func<FAIL, Validation<MonoidFail, FAIL, B>> Fail) =>
    //         IsSuccess
    //             ? Succ(SuccessValue)
    //             : Fail(FailValue);
    //
    //     [Pure]
    //     public Validation<MonoidFail, FAIL, V> SelectMany<U, V>(Func<SUCCESS, Validation<MonoidFail, FAIL, U>> bind, Func<SUCCESS, U, V> project)
    //     {
    //         var t = success;
    //         return IsSuccess
    //             ? bind(t).Map(u => project(t, u))
    //             : Validation<MonoidFail, FAIL, V>.Fail(FailValue);
    //     }
    // }
    //
    // /// <summary>
    // /// Context for the fluent Either matching
    // /// </summary>
    // public struct ValidationContext<MonoidFail, FAIL, SUCCESS, Ret>
    //     where MonoidFail : Monoid<FAIL>, Eq<FAIL>
    // {
    //     readonly Validation<MonoidFail, FAIL, SUCCESS> validation;
    //     readonly Func<SUCCESS, Ret> success;
    //
    //     internal ValidationContext(Validation<MonoidFail, FAIL, SUCCESS> validation, Func<SUCCESS, Ret> success)
    //     {
    //         this.validation = validation;
    //         this.success = success;
    //     }
    //
    //     /// <summary>
    //     /// Fail match
    //     /// </summary>
    //     /// <param name="Fail"></param>
    //     /// <returns>Result of the match</returns>
    //     [Pure]
    //     public Ret Fail(Func<FAIL, Ret> fail) =>
    //         validation.Match(success, fail);
    // }
    //
    // /// <summary>
    // /// Context for the fluent Validation matching
    // /// </summary>
    // public struct ValidationUnitContext<MonoidFail, FAIL, SUCCESS>
    //     where MonoidFail : Monoid<FAIL>, Eq<FAIL>
    // {
    //     readonly Validation<MonoidFail, FAIL, SUCCESS> validation;
    //     readonly Action<SUCCESS> success;
    //
    //     internal ValidationUnitContext(Validation<MonoidFail, FAIL, SUCCESS> validation, Action<SUCCESS> success)
    //     {
    //         this.validation = validation;
    //         this.success = success;
    //     }
    //
    //     public Unit Left(Action<FAIL> fail) =>
    //         validation.Match(success, fail);
    // }
    // }
