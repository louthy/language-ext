using System;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Holds one of two values `Left` or `Right`.  Usually, `Left` is considered _wrong_ or _in-error_, and
/// `Right` is, well, right - as in correct.  When the `Either` is in a `Left` state, it cancels
/// computations like `bind` or `map`, etc.  So, you can see `Left` as an _'early out, with a message'_.
/// Unlike `Option` that has `None` as its alternative value (i.e. it has an _'early out, but no message'_).
/// </summary>
/// <remarks>
/// NOTE: If you use `Filter` or `Where` (or `where` in a LINQ expression) with `Either`, then the `Either` 
/// will be put into a `Bottom` state [if the predicate returns false].  When it's in this state it is 
/// neither `Right` nor `Left`.  And any usage could trigger a `BottomException`.  So be aware of the issue
/// of filtering `Either`.
/// 
/// Also note, when the `Either` is in a `Bottom` state, some operations on it will continue to give valid
/// results or return another `Either` in the `Bottom` state and not throw.  This is so a filtered `Either` 
/// doesn't needlessly break expressions and could be rescued into something useful via `Match`. 
/// </remarks>
/// <typeparam name="L">Left</typeparam>
/// <typeparam name="R">Right</typeparam>
public abstract partial record Either<L, R> :
    IEither,
    IComparable<R>,
    IComparable,
    IComparable<Pure<R>>,
    IEquatable<Pure<R>>,
    IEquatable<R>,
    Fallible<Either<L, R>, Either<L>, L, R>,
    K<Either<L>, R>,
    K<Either, L, R>
{
    /// <summary>
    /// Stop other types deriving from Either
    /// </summary>
    private Either() {}
    
    /// <summary>
    /// Is the Either in a Right state?
    /// </summary>
    public abstract bool IsRight { get; }

    /// <summary>
    /// Is the Either in a Left state?
    /// </summary>
    public abstract bool IsLeft { get; }
    
    /// <summary>
    /// Explicit conversion operator from `Either` to `R`
    /// </summary>
    /// <param name="value">Value</param>
    /// <exception cref="InvalidCastException">Value is not in a Right state</exception>
    [Pure]
    public static explicit operator R(Either<L, R> ma) =>
        ma.RightValue;

    /// <summary>
    /// Explicit conversion operator from `Either` to `L`
    /// </summary>
    /// <param name="value">Value</param>
    /// <exception cref="InvalidCastException">Value is not in a Left state</exception>
    [Pure]
    public static explicit operator L(Either<L, R> ma) =>
        ma.LeftValue;

    /// <summary>
    /// Implicit conversion operator from R to Either R L
    /// </summary>
    /// <param name="value">Value</param>
    [Pure]
    public static implicit operator Either<L, R>(R value) =>
        new Right(value);

    /// <summary>
    /// Implicit conversion operator from L to Either R L
    /// </summary>
    /// <param name="value">Value</param>
    [Pure]
    public static implicit operator Either<L, R>(L value) =>
        new Left(value);

    /// <summary>
    /// Invokes the Right or Left function depending on the state of the Either
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Left">Function to invoke if in a Left state</param>
    /// <param name="Right">Function to invoke if in a Right state</param>
    /// <returns>The return value of the invoked function</returns>
    [Pure]
    public abstract B Match<B>(Func<L, B> Left, Func<R, B> Right);

    /// <summary>
    /// Invokes the Right or Left action depending on the state of the Either
    /// </summary>
    /// <param name="Right">Action to invoke if in a Right state</param>
    /// <param name="Left">Action to invoke if in a Left state</param>
    /// <returns>Unit</returns>
    public Unit Match(Action<L> Left, Action<R> Right) =>
        Match(Left: l => fun(Left)(l), Right: r => fun(Right)(r));

    /// <summary>
    /// Executes the Left function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(Func<R> Left) =>
        IfLeft(_ => Left());

    /// <summary>
    /// Executes the leftMap function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(Func<L, R> leftMap) =>
        Match(Left: leftMap, Right: identity);

    /// <summary>
    /// Returns the rightValue if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="rightValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(R rightValue) =>
        IfLeft(_ => rightValue);

    /// <summary>
    /// Executes the Left action if the Either is in a Left state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    public Unit IfLeft(Action<L> Left) =>
        Match(Left: Left, Right: _ => {});

    /// <summary>
    /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
    /// </summary>
    /// <param name="Right">Action to invoke</param>
    /// <returns>Unit</returns>
    public Unit IfRight(Action<R> Right) =>
        Match(Left: _ => { }, Right: Right);

    /// <summary>
    /// Returns the leftValue if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="leftValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(L leftValue) =>
        Match(Left: identity, Right: _ => leftValue);

    /// <summary>
    /// Returns the result of Right() if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="Right">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(Func<L> Right) =>
        Match(Left: identity, Right: _ => Right());

    /// <summary>
    /// Returns the result of rightMap if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(Func<R, L> rightMap) =>
        Match(Left: identity, Right: rightMap);

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Either<L, R> t ? CompareTo(t) : 1;

    /// <summary>
    /// Span of left value
    /// </summary>
    [Pure]
    public abstract ReadOnlySpan<L> LeftSpan();

    /// <summary>
    /// Span of right value
    /// </summary>
    [Pure]
    public abstract ReadOnlySpan<R> RightSpan();

    /// <summary>
    /// Singleton enumerable if in a right state, otherwise empty.
    /// </summary>
    [Pure]
    public abstract IEnumerable<R> AsEnumerable();

    /// <summary>
    /// Singleton `Iterable` if in a right state, otherwise empty.
    /// </summary>
    [Pure]
    public Iterable<R> ToIterable() =>
        [..RightSpan()];

    /// <summary>
    /// Project the Either into a Lst R
    /// </summary>
    /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
    public Lst<R> ToList() =>
        new(RightSpan());

    /// <summary>
    /// Project the Either into an ImmutableArray R
    /// </summary>
    /// <returns>If the Either is in a Right state, an immutable-array of R with one item.  A zero-length array of R otherwise</returns>
    public Arr<R> ToArray() =>
        new(RightSpan());

    /// <summary>
    /// Convert either to sequence of 0 or 1 right values
    /// </summary>
    [Pure]
    public Seq<R> ToSeq() =>
        new(RightSpan());

    /// <summary>
    /// Convert the Either to an Option
    /// </summary>
    /// <returns>Some(Right) or None</returns>
    [Pure]
    public Option<R> ToOption() =>
        Match(Left: _ => None, Right: Some);

    /// <summary>
    /// Convert to an `Eff`
    /// </summary>
    /// <param name="Left">Map the `Left` value to the`Fail` state of the `Eff`</param>
    /// <returns>`Eff` monad</returns>
    [Pure]
    public Eff<R> ToEff(Func<L, Error> Left) =>
        Match(Left: e => Eff<R>.Fail(Left(e)), Right: Eff<R>.Pure);

    /// <summary>
    /// Convert to an Either transformer with embedded IO
    /// </summary>
    /// <returns></returns>
    [Pure]
    public EitherT<L, IO, R> ToIO() =>
        EitherT.lift<L, IO, R>(this);
    
    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs >= rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) >= 0;


    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Fail<L> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Fail<L>  lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Fail<L> lhs, Either<L, R>rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Fail<L> lhs, Either<L, R>  rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Pure<R> lhs, Either<L, R>  rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Pure<R> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Pure<R> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Pure<R> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Fail<L>  lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Pure<R> lhs, Either<L, R>  rhs) =>
        ((Either<L, R>)lhs).Equals(rhs);
        
    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Either<L, R> lhs, Fail<L> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Either<L, R> lhs, Pure<R> rhs) =>
        !(lhs == rhs);


    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Fail<L> lhs, Either<L, R> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Pure<R> lhs, Either<L, R> rhs) =>
        !(lhs == rhs);

    [Pure, MethodImpl(Opt.Default)]
    public static Either<L, R> operator |(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Either<L, R> operator |(K<Either<L>, R> lhs, Either<L, R> rhs) =>
        lhs.As().Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Either<L, R> operator |(Either<L, R> lhs, K<Either<L>, R> rhs) =>
        lhs.Choose(rhs.As()).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Either<L, R> operator |(Either<L, R> ma, Pure<R> mb) =>
        ma.Choose(pure<Either<L>, R>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Either<L, R> operator |(Either<L, R> ma, Fail<L> mb) =>
        ma.Choose(fail<L, Either<L>, R>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Either<L, R> operator |(Either<L, R> ma, CatchM<L, Either<L>, R> mb) =>
        (ma.Kind() | mb).As();
    
    /// <summary>
    /// Override of the True operator to return True if the Either is Right
    /// </summary>
    [Pure]
    public static bool operator true(Either<L, R> value) =>
        value is Right;

    /// <summary>
    /// Override of the False operator to return True if the Either is Left
    /// </summary>
    [Pure]
    public static bool operator false(Either<L, R> value) =>
        value is Left;

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Either<L, R> other) =>
        CompareTo<OrdDefault<L>, OrdDefault<R>>(other);

    /// <summary>
    /// CompareTo
    /// </summary>
    [Pure]
    public int CompareTo<OrdR>(Either<L, R> other)
        where OrdR : Ord<R> =>
        CompareTo<OrdDefault<L>, OrdR>(other);

    /// <summary>
    /// CompareTo
    /// </summary>
    [Pure]
    public abstract int CompareTo<OrdL, OrdR>(Either<L, R> other)
        where OrdL : Ord<L>
        where OrdR : Ord<R>;

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Fail<L> other) =>
        CompareTo((Either<L, R>)other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Pure<R> other) =>
        CompareTo((Either<L, R>)other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(R? other) =>
        other switch
        {
            null => 1,
            _    => CompareTo(Right<L, R>(other))
        };

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(L? other) =>
        other switch
        {
            null => 1,
            _    => CompareTo(Left<L, R>(other))
        };

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(R? other) =>
        other is not null && Equals(Right<L, R>(other));

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(L? other) =>
        other is not null && Equals(Left<L, R>(other));

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public virtual bool Equals(Either<L, R>? other) =>
        other is not null && Equals<EqDefault<L>, EqDefault<R>>(other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public virtual bool Equals<EqR>(Either<L, R> other) where EqR : Eq<R> =>
        Equals<EqDefault<L>, EqR>(other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public abstract bool Equals<EqL, EqR>(Either<L, R> other) 
        where EqL : Eq<L> 
        where EqR : Eq<R>;

    [Pure]
    public abstract override int GetHashCode();

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(Fail<L> other) =>
        Equals((Either<L, R>)other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(Pure<R> other) =>
        Equals((Either<L, R>)other);

    /// <summary>
    /// Match the Right and Left values but as objects.  This can be useful to avoid reflection.
    /// </summary>
    [Pure]
    public Res MatchUntyped<Res>(Func<object?, Res> Left, Func<object?, Res> Right) =>
        Match(Left: l => Left(l), Right: r => Right(r));

    /// <summary>
    /// Find out the underlying Right type
    /// </summary>
    [Pure]
    public Type GetUnderlyingRightType() =>
        typeof(R);

    /// <summary>
    /// Find out the underlying Left type
    /// </summary>
    [Pure]
    public Type GetUnderlyingLeftType() =>
        typeof(L);

    /// <summary>
    /// Unsafe access to the right-value 
    /// </summary>
    /// <exception cref="EitherIsNotRightException"></exception>
    internal abstract R RightValue { get; }
    
    /// <summary>
    /// Unsafe access to the left-value 
    /// </summary>
    /// <exception cref="EitherIsNotLeftException"></exception>
    internal abstract L LeftValue { get; }

    [Pure]
    public Type GetUnderlyingType() => 
        typeof(R);

    /// <summary>
    /// Flips the left and right tagged values
    /// </summary>
    /// <returns>Either with the types swapped</returns>
    [Pure]
    public Either<R, L> Swap() =>
        Match(Left: Either.Right<R, L>, Right: Either.Left<R, L>);

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="Right">Predicate</param>
    /// <returns>True if the Either is in a Left state.  
    /// True if the Either is in a Right state and the predicate returns True.  
    /// False otherwise.</returns>
    [Pure]
    public bool ForAll(Func<R, bool> Right) =>
        Match(Left: _ => true, Right: Right);

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="Right">Predicate</param>
    /// <param name="Left">Predicate</param>
    /// <returns>True if either Predicate returns true</returns>
    [Pure]
    public bool BiForAll(Func<L, bool> Left, Func<R, bool> Right) =>
        Match(Left: Left, Right: Right);

    /// <summary>
    /// <para>
    /// Either types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="Right">Folder function, applied if structure is in a Right state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    public S Fold<S>(S state, Func<S, R, S> Right) =>
        Match(Left: _ => state, Right: curry(Right)(state));

    /// <summary>
    /// <para>
    /// Either types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="Right">Folder function, applied if Either is in a Right state</param>
    /// <param name="Left">Folder function, applied if Either is in a Left state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    public S BiFold<S>(S state, Func<S, L, S> Left, Func<S, R, S> Right) =>
        Match(Left: curry(Left)(state), Right: curry(Right)(state));

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to check existence of</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
    [Pure]
    public bool Exists(Func<R, bool> pred) =>
        Match(Left: _ => false, Right: pred);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Either<L, R> Do(Action<R> f) =>
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
    public K<F, Either<L, B>> Traverse<F, B>(Func<R, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));

    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B">Mapped Either type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public abstract Either<L, B> Map<B>(Func<R, B> f);

    /// <summary>
    /// Maps the value in the Either if it's in a Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B">Mapped Either type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<B, R> MapLeft<B>(Func<L, B> f) =>
        BiMap(Left: f, Right: identity);

    /// <summary>
    /// Bi-maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="LRet">Left return</typeparam>
    /// <typeparam name="RRet">Right return</typeparam>
    /// <param name="Right">Right map function</param>
    /// <param name="Left">Left map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public abstract Either<L2, R2> BiMap<L2, R2>(Func<L, L2> Left, Func<R, R2> Right);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B">Resulting bound value</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Bound Either</returns>
    [Pure]
    public abstract Either<L, B> Bind<B>(Func<R, Either<L, B>> f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B"></typeparam>
    /// <param name="f"></param>
    /// <returns>Bound Either</returns>
    [Pure]
    public Either<L, B> Bind<B>(Func<R, K<Either<L>, B>> f) =>
        Bind(x => (Either<L, B>)f(x));

    /// <summary>
    /// Bi-bind.  Allows mapping of both monad states
    /// </summary>
    [Pure]
    public abstract Either<L2, R2> BiBind<L2, R2>(Func<L, Either<L2, R2>> Left, Func<R, Either<L2, R2>> Right);

    /// <summary>
    /// Bind left.  Binds the left path of the monad only
    /// </summary>
    [Pure]
    public Either<L2, R> BindLeft<L2>(Func<L, Either<L2, R>> f) =>
        BiBind(f, Right<L2, R>);

    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="TR">Right</typeparam>
    /// <typeparam name="UR">Mapped Either type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<L, U> Select<U>(Func<R, U> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public Either<L, T> SelectMany<S, T>(Func<R, Either<L, S>> bind, Func<R, S, T> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    /// <summary>
    /// Monadic bind function
    /// </summary>
    public Either<L, Unit> SelectMany(Func<R, Guard<L, Unit>> f) =>
        Bind(a => f(a).ToEither());

    /// <summary>
    /// Monadic bind function
    /// </summary>
    public Either<L, C> SelectMany<C>(Func<R, Guard<L, Unit>> bind, Func<R, Unit, C> project) =>
        Bind(a => bind(a).ToEither().Map(_ => project(a, default)));    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    // `Pure` and `Fail` support
    //

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Either<L, B> Bind<B>(Func<R, Pure<B>> f) =>
        Bind(x => (Either<L, B>)f(x));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Either<L, R> Bind(Func<R, Fail<L>> f) =>
        Bind(x => (Either<L, R>)f(x));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Either<L, C> SelectMany<B, C>(Func<R, Pure<B>> bind, Func<R, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Either<L, C> SelectMany<B, C>(Func<R, Fail<L>> bind, Func<R, B, C> _) =>
        Bind(x => bind(x).ToEither<C>());

    [Pure]
    public static implicit operator Either<L, R>(Pure<R> mr) =>
        new Right(mr.Value);

    [Pure]
    public static implicit operator Either<L, R>(Fail<L> mr) =>
        new Left(mr.Value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Obsolete
    //

    /// <summary>
    /// Project the Either into a Lst R
    /// </summary>
    /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
    [Pure]
    [Obsolete(Change.UseToListInstead)]
    public Lst<R> RightToList() =>
        ToList();

    /// <summary>
    /// Project the Either into an ImmutableArray R
    /// </summary>
    /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
    [Pure]
    [Obsolete(Change.UseToArrayInstead)]
    public Arr<R> RightToArray() =>
        ToArray();

    /// <summary>
    /// Convert either to sequence of 0 or 1 right values
    /// </summary>
    [Pure]
    [Obsolete(Change.UseToSeqInstead)]
    public Seq<R> RightToSeq() =>
        ToSeq();
}

/// <summary>
/// Context for the fluent Either matching
/// </summary>
public readonly struct EitherContext<L, R, Ret>
{
    readonly Either<L, R> either;
    readonly Func<R, Ret> rightHandler;

    internal EitherContext(Either<L, R> either, Func<R, Ret> rightHandler)
    {
        this.either = either;
        this.rightHandler = rightHandler;
    }

    /// <summary>
    /// Left match
    /// </summary>
    /// <param name="left"></param>
    /// <returns>Result of the match</returns>
    [Pure]
    public Ret Left(Func<L, Ret> left) =>
        either.Match(left, rightHandler);
}

/// <summary>
/// Context for the fluent Either matching
/// </summary>
public readonly struct EitherUnitContext<L, R>
{
    readonly Either<L, R> either;
    readonly Action<R> rightHandler;

    internal EitherUnitContext(Either<L, R> either, Action<R> rightHandler)
    {
        this.either = either;
        this.rightHandler = rightHandler;
    }

    public Unit Left(Action<L> leftHandler) =>
        either.Match(leftHandler, rightHandler);
}
