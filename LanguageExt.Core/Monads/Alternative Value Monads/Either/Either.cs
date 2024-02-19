using System;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

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
[Serializable]
public readonly struct Either<L, R> :
    IEither,
    IEnumerable<R>,
    IComparable<Either<L, R>>,
    IComparable<R>,
    IComparable,
    IComparable<Pure<R>>,
    IEquatable<Either<L, R>>,
    IEquatable<Pure<R>>,
    IEquatable<R>, 
    ISerializable,
    K<Either<L>, R>
{
    public static readonly Either<L, R> Bottom = new ();

    readonly R? right;
    readonly L? left;

    internal Either(Transducer<Unit, Sum<L, R>> morphism)
    {
        right = default;
        left = default;
    }

    internal Either(Transducer<Unit, R> morphism) : this(morphism.Map(Sum<L, R>.Right))
    { }

    private Either(R right)
    {
        State = EitherStatus.IsRight;
        this.right = right;
        left = default;
    }

    private Either(L left)
    {
        State = EitherStatus.IsLeft;
        right = default;
        this.left = left;
    }

    Either(SerializationInfo info, StreamingContext context)
    {
        State = (EitherStatus?)info.GetValue("State", typeof(EitherStatus)) ?? throw new SerializationException();
        switch(State)
        {
            case EitherStatus.IsBottom:
                right = default;
                left = default;
                break;
            case EitherStatus.IsRight:
                right = (R?)info.GetValue("Right", typeof(R)) ?? throw new SerializationException();
                left = default;
                break;
            case EitherStatus.IsLeft:
                left = (L?)info.GetValue("Left", typeof(L)) ?? throw new SerializationException();
                right = default;
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("State", State);
        if (IsRight) info.AddValue("Right", right);
        if (IsLeft) info.AddValue("Left", left);
    }

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    /// <remarks>
    ///
    ///    Left   = result is L
    ///    Right  = result is R
    ///    Bottom = result is null 
    /// 
    /// </remarks>
    [Pure]
    public object? Case =>
        State switch
        {
            EitherStatus.IsRight => right,
            EitherStatus.IsLeft  => left,
            _                    => null
        };

    /// <summary>
    /// State of the Either
    /// You can also use:
    ///     IsRight
    ///     IsLeft
    ///     IsBottom
    ///     IsLazy
    /// </summary>
    public readonly EitherStatus State;

    /// <summary>
    /// Is the Either in a Right state?
    /// </summary>
    [Pure]
    public bool IsRight =>
        State switch
        {
            EitherStatus.IsRight => true,
            _                    => false
        };

    /// <summary>
    /// Is the Either in a Left state?
    /// </summary>
    [Pure]
    public bool IsLeft =>
        State switch
        {
            EitherStatus.IsLeft => true,
            _                    => false
        };
    
    /// <summary>
    /// Is the Either in a Bottom state?
    /// When the Either is filtered, both Right and Left are meaningless.
    /// 
    /// If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
    /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
    /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
    /// of filtering Either.
    /// 
    /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
    /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
    /// doesn't needlessly break expressions. 
    /// </summary>
    [Pure]
    public bool IsBottom =>
        State switch
        {
            EitherStatus.IsBottom => true,
            _                    => false
        };

    /// <summary>
    /// Explicit conversion operator from `Either` to `R`
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    /// <exception cref="InvalidCastException">Value is not in a Right state</exception>
    [Pure]
    public static explicit operator R(Either<L, R> ma) =>
        ma.RightValue;

    /// <summary>
    /// Explicit conversion operator from `Either` to `L`
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    /// <exception cref="InvalidCastException">Value is not in a Left state</exception>
    [Pure]
    public static explicit operator L(Either<L, R> ma) =>
        ma.LeftValue;

    /// <summary>
    /// Implicit conversion operator from R to Either R L
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    [Pure]
    public static implicit operator Either<L, R>(R value) =>
        Right(value);

    /// <summary>
    /// Implicit conversion operator from L to Either R L
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    [Pure]
    public static implicit operator Either<L, R>(L value) =>
        Left(value);

    /// <summary>
    /// Invokes the Right or Left function depending on the state of the Either
    /// </summary>
    /// <typeparam name="Ret">Return type</typeparam>
    /// <param name="Right">Function to invoke if in a Right state</param>
    /// <param name="Left">Function to invoke if in a Left state</param>
    /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
    /// <returns>The return value of the invoked function</returns>
    [Pure]
    public Ret Match<Ret>(Func<R, Ret> Right, Func<L, Ret> Left, Func<Ret>? Bottom = null) =>
        State switch
        {
            EitherStatus.IsRight => Right(right!),
            EitherStatus.IsLeft  => Left(left!),
            _                    => Bottom is null ? throw new BottomException() : Bottom()
        };

    /// <summary>
    /// Invokes the Right or Left action depending on the state of the Either
    /// </summary>
    /// <param name="Right">Action to invoke if in a Right state</param>
    /// <param name="Left">Action to invoke if in a Left state</param>
    /// <returns>Unit</returns>
    /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
    public Unit Match(Action<R> Right, Action<L> Left, Action? Bottom = null)
    {
        switch (State)
        {
            case EitherStatus.IsRight:
                Right(right!);
                break;

            case EitherStatus.IsLeft:
                Left(left!);
                break;
            
            case EitherStatus.IsBottom when Bottom != null:
                Bottom();
                break;

            case EitherStatus.IsBottom:
                throw new BottomException();
        }
        return unit;
    }

    /// <summary>
    /// Match the two states of the Either and return a promise for a non-null R2.
    /// </summary>
    /// <returns>A promise to return a non-null R2</returns>
    [Obsolete(Change.UseIOMonadInstead)]
    public Task<R2> MatchAsync<R2>(Func<R, R2> Right, Func<L, Task<R2>> LeftAsync) =>
        Match(x => Right(x).AsTask(), LeftAsync);

    /// <summary>
    /// Match the two states of the Either and return a promise for a non-null R2.
    /// </summary>
    /// <returns>A promise to return a non-null R2</returns>
    [Obsolete(Change.UseIOMonadInstead)]
    public Task<R2> MatchAsync<R2>(Func<R, Task<R2>> RightAsync, Func<L, R2> Left) =>
        Match(RightAsync, x => Left(x).AsTask());

    /// <summary>
    /// Match the two states of the Either and return a promise for a non-null R2.
    /// </summary>
    /// <returns>A promise to return a non-null R2</returns>
    [Obsolete(Change.UseIOMonadInstead)]
    public Task<R2> MatchAsync<R2>(Func<R, Task<R2>> RightAsync, Func<L, Task<R2>> LeftAsync) =>
        Match(RightAsync, LeftAsync);

    /// <summary>
    /// Executes the Left function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(Func<R> Left) =>
        State switch
        {
            EitherStatus.IsRight => right!,
            EitherStatus.IsLeft  => Left(),
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Executes the leftMap function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(Func<L, R> leftMap) =>
        State switch
        {
            EitherStatus.IsRight => right!,
            EitherStatus.IsLeft  => leftMap(left!),
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Returns the rightValue if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="rightValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(R rightValue) =>
        State switch
        {
            EitherStatus.IsRight => right!,
            EitherStatus.IsLeft  => rightValue,
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Executes the Left action if the Either is in a Left state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    public Unit IfLeft(Action<L> Left)
    {
        switch(State)
        {
            case EitherStatus.IsLeft:
                Left(left!);
                break;
        }
        return default;
    }

    /// <summary>
    /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
    /// </summary>
    /// <param name="Right">Action to invoke</param>
    /// <returns>Unit</returns>
    public Unit IfRight(Action<R> Right)
    {
        switch(State)
        {
            case EitherStatus.IsRight:
                Right(right!);
                break;
        }
        return default;
    }

    /// <summary>
    /// Returns the leftValue if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="leftValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(L leftValue) =>
        State switch
        {
            EitherStatus.IsRight => leftValue,
            EitherStatus.IsLeft  => left!,
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Returns the result of Right() if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="Right">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(Func<L> Right) =>
        State switch
        {
            EitherStatus.IsRight => Right(),
            EitherStatus.IsLeft  => left!,
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Returns the result of rightMap if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(Func<R, L> rightMap) =>
        State switch
        {
            EitherStatus.IsRight => rightMap(right!),
            EitherStatus.IsLeft  => left!,
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
    /// </summary>
    /// <param name="right">Action to invoke if the Either is in a Right state</param>
    /// <returns>Context that must have Left() called upon it.</returns>
    [Pure]
    public EitherUnitContext<L, R> Right(Action<R> right) =>
        new (this, right);

    /// <summary>
    /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
    /// </summary>
    /// <param name="right">Action to invoke if the Either is in a Right state</param>
    /// <returns>Context that must have Left() called upon it.</returns>
    [Pure]
    public EitherContext<L, R, Ret> Right<Ret>(Func<R, Ret> right) =>
        new (this, right);

    /// <summary>
    /// Return a string representation of the Either
    /// </summary>
    /// <returns>String representation of the Either</returns>
    [Pure]
    public override string ToString() =>
        State switch
        {
            EitherStatus.IsRight => right is null ? "Right(null)" : $"Right({right})",
            EitherStatus.IsLeft  => left is null ? "Left(null)" : $"Left({left})",
            _                    => "Bottom"
        };

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

    /// <summary>
    /// Returns a hash code of the wrapped value of the Either
    /// </summary>
    /// <returns>Hash code</returns>
    [Pure]
    public override int GetHashCode() =>
        State switch
        {
            EitherStatus.IsRight => right is null ? 0 : HashableDefault<R>.GetHashCode(right),
            EitherStatus.IsLeft  => left is null ? 0 : HashableDefault<L>.GetHashCode(left),
            _                    => 0
        };

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Either<L, R> t ? CompareTo(t) : 1;

    [Pure]
    public IEnumerator<R> GetEnumerator()
    {
        if (IsRight)
        {
            yield return RightValue;
        }
    }

    /// <summary>
    /// Equality check
    /// </summary>
    /// <param name="obj">Object to test for equality</param>
    /// <returns>True if equal</returns>
    [Pure]
    public override bool Equals(object? obj) =>
        obj is Either<L, R> rhs && Equals(rhs);  

    /// <summary>
    /// Project the Either into a Lst L
    /// </summary>
    /// <returns>If the Either is in a Left state, a Lst of L with one item.  A zero length Lst L otherwise</returns>
    [Pure]
    public Lst<L> LeftToList() =>
        State switch
        {
            EitherStatus.IsRight => [],
            EitherStatus.IsLeft  => [left!],
            _                    => []
        };

    /// <summary>
    /// Project the Either into an ImmutableArray L
    /// </summary>
    /// <returns>If the Either is in a Left state, a ImmutableArray of L with one item.  A zero length ImmutableArray of L otherwise</returns>
    [Pure]
    public Arr<L> LeftToArray() =>
        State switch
        {
            EitherStatus.IsRight => [],
            EitherStatus.IsLeft  => [left!],
            _                    => []
        };

    /// <summary>
    /// Project the Either into a Lst R
    /// </summary>
    /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
    public Lst<R> ToList() =>
        State switch
        {
            EitherStatus.IsRight => [right!],
            EitherStatus.IsLeft  => [],
            _                    => []
        };

    /// <summary>
    /// Project the Either into an ImmutableArray R
    /// </summary>
    /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
    public Arr<R> ToArray() =>
        State switch
        {
            EitherStatus.IsRight => [right!],
            EitherStatus.IsLeft  => [],
            _                    => []
        };

    /// <summary>
    /// Convert either to sequence of 0 or 1 right values
    /// </summary>
    [Pure]
    public Seq<R> ToSeq() =>
        State switch
        {
            EitherStatus.IsRight => [right!],
            EitherStatus.IsLeft  => [],
            _                    => []
        };

    /// <summary>
    /// Convert either to sequence of 0 or 1 left values
    /// </summary>
    [Pure]
    public Seq<L> LeftToSeq() =>
        State switch
        {
            EitherStatus.IsRight => [],
            EitherStatus.IsLeft  => [left!],
            _                    => []
        };

    /// <summary>
    /// Project the Either into a IEnumerable R
    /// </summary>
    /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
    [Pure]
    public Seq<R> RightAsEnumerable() =>
        State switch
        {
            EitherStatus.IsRight => [right!],
            EitherStatus.IsLeft  => [],
            _                    => []
        };

    /// <summary>
    /// Project the Either into a IEnumerable L
    /// </summary>
    /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
    [Pure]
    public Seq<L> LeftAsEnumerable() =>
        State switch
        {
            EitherStatus.IsRight => [],
            EitherStatus.IsLeft  => [left!],
            _                    => []
        };

    /// <summary>
    /// Convert the Either to an Option
    /// </summary>
    /// <returns>Some(Right) or None</returns>
    [Pure]
    public Option<R> ToOption() =>
        State switch
        {
            EitherStatus.IsRight => Pure(right!),
            EitherStatus.IsLeft  => None,
            _                    => None
        };

    /// <summary>
    /// Convert the Either to a Sum
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Sum<L, R> ToSum() =>
        State switch
        {
            EitherStatus.IsRight => Pure(right!),
            EitherStatus.IsLeft  => Fail(left!),
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Convert to an `Eff`
    /// </summary>
    /// <param name="Left">Map the `Left` value to the`Fail` state of the `Eff`</param>
    /// <returns>`Eff` monad</returns>
    [Pure]
    public Eff<R> ToEff(Func<L, Error> Left) =>
        State switch
        {
            EitherStatus.IsRight => Pure(right!),
            EitherStatus.IsLeft  => Fail(Left(left!)),
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Fail<L> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Pure<R> rhs) =>
        lhs.CompareTo(rhs) >= 0;


    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Fail<L> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Fail<L>  lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Fail<L> lhs, Either<L, R>rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Fail<L> lhs, Either<L, R>  rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Pure<R> lhs, Either<L, R>  rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Pure<R> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Pure<R> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Pure<R> lhs, Either<L, R> rhs) =>
        ((Either<L, R>)lhs).CompareTo(rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs <= rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs > rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs >= rhs</returns>
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
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Either<L, R> lhs, Either<L, R> rhs) =>
        lhs.Equals(rhs);
        
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

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Either<L, R> lhs, Either<L, R> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Either<L, R> operator |(Either<L, R> lhs, Either<L, R> rhs) =>
        (lhs.State, rhs.State) switch
        {
            (EitherStatus.IsRight, _) =>
                lhs,
            
            (_, EitherStatus.IsBottom) =>
                lhs,
            
            _ => rhs
        };

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Either<L, R> operator |(Either<L, R> lhs, Pure<R> rhs) =>
        lhs | (Either<L, R>)rhs;

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Either<L, R> operator |(Either<L, R> lhs, Fail<L> rhs) =>
        lhs | (Either<L, R>)rhs;

    /// <summary>
    /// Override of the True operator to return True if the Either is Right
    /// </summary>
    [Pure]
    public static bool operator true(Either<L, R> value) =>
        value.State switch
        {
            EitherStatus.IsRight => true,
            EitherStatus.IsLeft  => false,
            _                    => false
        };

    /// <summary>
    /// Override of the False operator to return True if the Either is Left
    /// </summary>
    [Pure]
    public static bool operator false(Either<L, R> value) =>
        value.State switch
        {
            EitherStatus.IsRight => false,
            EitherStatus.IsLeft  => true,
            _                    => false
        };

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Either<L, R> other) =>
        CompareTo<OrdDefault<L>, OrdDefault<R>>(other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo<OrdR>(Either<L, R> other)
        where OrdR : Ord<R> =>
        CompareTo<OrdDefault<L>, OrdR>(other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo<OrdL, OrdR>(Either<L, R> other)
        where OrdL : Ord<L>
        where OrdR : Ord<R> =>
        (State, other.State) switch
        {
            (EitherStatus.IsRight, EitherStatus.IsRight)   => OrdR.Compare(right!, other.RightValue),
            (EitherStatus.IsLeft, EitherStatus.IsLeft)     => OrdL.Compare(left!, other.LeftValue),
            (EitherStatus.IsBottom, EitherStatus.IsBottom) => 0,
            _                                              => State.CompareTo(other.State) 
        };

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
    public bool Equals(Either<L, R> other) =>
        Equals<EqDefault<L>, EqDefault<R>>(other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals<EqR>(Either<L, R> other) where EqR : Eq<R> =>
        Equals<EqDefault<L>, EqR>(other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals<EqL, EqR>(Either<L, R> other) 
        where EqL : Eq<L> 
        where EqR : Eq<R> =>
        (State, other.State) switch
        {
            (EitherStatus.IsRight, EitherStatus.IsRight)   => EqR.Equals(right!, other.RightValue),
            (EitherStatus.IsLeft, EitherStatus.IsLeft)     => EqL.Equals(left!, other.LeftValue),
            (EitherStatus.IsBottom, EitherStatus.IsBottom) => true,
            _                                              => false
        };

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
    public Res MatchUntyped<Res>(Func<object?, Res> Right, Func<object?, Res> Left) =>
        State switch
        {
            EitherStatus.IsRight => Right(right),
            EitherStatus.IsLeft  => Left(left),
            _                    => throw new BottomException()
        };

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

    [Pure]
    public static Either<L, R> Right(R value) =>
        new (value);

    [Pure]
    public static Either<L, R> Left(L value) =>
        new (value);

    [Pure]
    internal R RightValue =>
        State switch
        {
            EitherStatus.IsRight => right!,
            _                    => throw new EitherIsNotRightException()
        };

    [Pure]
    internal L LeftValue =>
        State switch
        {
            EitherStatus.IsLeft => left!,
            _                   => throw new EitherIsNotLeftException()
        };

    [Pure]
    public Type GetUnderlyingType() => 
        typeof(R);

    /// <summary>
    /// Counts the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
    [Pure]
    public int Count() =>
        State switch
        {
            EitherStatus.IsRight => 1,
            EitherStatus.IsLeft  => 0,
            _                    => throw new BottomException()
        };

    /// <summary>
    /// Flips the left and right tagged values
    /// </summary>
    /// <returns>Either with the types swapped</returns>
    [Pure]
    public Either<R, L> Swap() =>
        State switch
        {
            EitherStatus.IsRight => Either<R, L>.Left(RightValue),
            EitherStatus.IsLeft  => Either<R, L>.Right(LeftValue),
            _                    => Either<R, L>.Bottom
        };

    /// <summary>
    /// Iterate the Either
    /// action is invoked if in the Right state
    /// </summary>
    public Unit Iter(Action<R> Right)
    {
        switch (State)
        {
            case EitherStatus.IsRight:
                Right(right!);
                break;
        }
        return default;
    }

    /// <summary>
    /// Iterate the Either
    /// action is invoked if in the Right state
    /// </summary>
    public Unit BiIter(Action<L> Left, Action<R> Right)
    {
        switch (State)
        {
            case EitherStatus.IsRight:
                Right(right!);
                break;

            case EitherStatus.IsLeft:
                Left(left!);
                break;
        }
        return default;
    }

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
        State switch
        {
            EitherStatus.IsRight => Right(right!),
            EitherStatus.IsLeft  => true,
            _                    => true
        };

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
        State switch
        {
            EitherStatus.IsRight => Right(right!),
            EitherStatus.IsLeft  => Left(left!),
            _                    => true
        };

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
        State switch
        {
            EitherStatus.IsRight => Right(state, right!),
            EitherStatus.IsLeft  => state,
            _                    => state
        };

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
        State switch
        {
            EitherStatus.IsRight => Right(state, right!),
            EitherStatus.IsLeft  => Left(state, left!),
            _                    => state
        };

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
        State switch
        {
            EitherStatus.IsRight => pred(right!),
            EitherStatus.IsLeft  => false,
            _                    => false
        };

    /// <summary>
    /// Invokes a predicate on the value of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to check existence of</param>
    /// <param name="Right">Right predicate</param>
    /// <param name="Left">Left predicate</param>
    /// <returns>True if the predicate returns True.  False otherwise or if the Either is in a bottom state.</returns>
    [Pure]
    public bool BiExists(Func<L, bool> Left, Func<R, bool> Right) =>
        State switch
        {
            EitherStatus.IsRight => Right(right!),
            EitherStatus.IsLeft  => Left(left!),
            _                    => false
        };

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
    /// </remarks>
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
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<L, Ret> Map<Ret>(Func<R, Ret> f) =>
        State switch
        {
            EitherStatus.IsRight => f(right!),
            EitherStatus.IsLeft  => left!,
            _                    => Either<L, Ret>.Bottom
        };

    /// <summary>
    /// Maps the value in the Either if it's in a Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="f">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<Ret, R> MapLeft<Ret>(Func<L, Ret> f) =>
        State switch
        {
            EitherStatus.IsRight => right!,
            EitherStatus.IsLeft  => f(left!),
            _                    => Either<Ret, R>.Bottom
        };

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
    public Either<L2, R2> BiMap<L2, R2>(Func<L, L2> Left, Func<R, R2> Right) =>
        State switch
        {
            EitherStatus.IsRight => Right(right!),
            EitherStatus.IsLeft  => Left(left!),
            _                    => Either<L2, R2>.Bottom
        };

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B"></typeparam>
    /// <param name="f"></param>
    /// <returns>Bound Either</returns>
    [Pure]
    public Either<L, B> Bind<B>(Func<R, Either<L, B>> f) =>
        State switch
        {
            EitherStatus.IsRight => f(right!),
            EitherStatus.IsLeft  => left!,
            _                    => Either<L, B>.Bottom
        };

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
    public Either<L2, R2> BiBind<L2, R2>(Func<L, Either<L2, R2>> Left, Func<R, Either<L2, R2>> Right) =>
        State switch
        {
            EitherStatus.IsRight => Right(right!),
            EitherStatus.IsLeft  => Left(left!),
            _                    => Either<L2, R2>.Bottom
        };

    /// <summary>
    /// Bind left.  Binds the left path of the monad only
    /// </summary>
    [Pure]
    public Either<B, R> BindLeft<B>(Func<L, Either<B, R>> f) =>
        BiBind(f, x => x);

    /// <summary>
    /// Filter the Either
    /// </summary>
    /// <remarks>
    /// This may give unpredictable results for a filtered value.  The Either won't
    /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
    /// should be checked for.
    /// </remarks>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
    [Pure]
    public Either<L, R> Filter(Func<R, bool> pred) =>
        State switch
        {
            EitherStatus.IsRight => pred(right!) ? this : Bottom,
            EitherStatus.IsLeft  => left!,
            _                    => this
        };

    /// <summary>
    /// Filter the Either
    /// </summary>
    /// <remarks>
    /// This may give unpredictable results for a filtered value.  The Either won't
    /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
    /// should be checked for.
    /// </remarks>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
    /// of Left = default(L)</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Either<L, R> Where(Func<R, bool> pred) =>
        Filter(pred);

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
    /// <returns>Bound Either</returns>
    [Pure]
    public Either<L, T> SelectMany<S, T>(Func<R, Either<L, S>> bind, Func<R, S, T> project) =>
        State switch
        {
            EitherStatus.IsRight => Bind(x => bind(x).Map(y => project(x, y))),
            EitherStatus.IsLeft  => left!,
            _                    => Either<L, T>.Bottom
        };
        
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
        Right(mr.Value);

    [Pure]
    public static implicit operator Either<L, R>(Fail<L> mr) =>
        Left(mr.Value);

    [Pure]
    public static implicit operator Either<L, R>(Transducer<Unit, Sum<L, R>> t) =>
        new (t);
    
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
        match(either, rightHandler, left);
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
        match(either, rightHandler, leftHandler);
}
