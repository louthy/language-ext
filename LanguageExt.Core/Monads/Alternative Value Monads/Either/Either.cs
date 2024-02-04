using System;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.Choice;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.HKT;

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
    KArr<HKT.Any, Unit, Sum<L, R>>
{
    public static readonly Either<L, R> Bottom = new ();

    internal readonly R? right;
    internal readonly L? left;
        
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
    /// </summary>
    public readonly EitherStatus State;

    /// <summary>
    /// Is the Either in a Right state?
    /// </summary>
    [Pure]
    public bool IsRight =>
        State == EitherStatus.IsRight;

    /// <summary>
    /// Is the Either in a Left state?
    /// </summary>
    [Pure]
    public bool IsLeft =>
        State == EitherStatus.IsLeft;

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
        State == EitherStatus.IsBottom;

    /// <summary>
    /// Explicit conversion operator from `Either` to `R`
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    /// <exception cref="InvalidCastException">Value is not in a Right state</exception>
    [Pure]
    public static explicit operator R(Either<L, R> ma) =>
        ma.IsRight
            ? ma.right!
            : throw new InvalidCastException("Either is not in a Right state");

    /// <summary>
    /// Explicit conversion operator from `Either` to `L`
    /// </summary>
    /// <param name="value">Value, must not be null.</param>
    /// <exception cref="InvalidCastException">Value is not in a Left state</exception>
    [Pure]
    public static explicit operator L(Either<L, R> ma) =>
        ma.IsLeft
            ? ma.left!
            : throw new InvalidCastException("Either is not in a Left state");

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
        if (State == EitherStatus.IsRight)
        {
            Right(right!);
        }
        else if (State == EitherStatus.IsLeft)
        {
            Left(left!);
        }
        else if (State == EitherStatus.IsBottom && Bottom != null)
        {
            Bottom();
        }
        else if (State == EitherStatus.IsBottom && Bottom == null)
        {
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
        ifLeft<MEither<L, R>, Either<L, R>, L, R>(this, Left);

    /// <summary>
    /// Executes the leftMap function if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(Func<L, R> leftMap) =>
        ifLeft<MEither<L, R>, Either<L, R>, L, R>(this, leftMap);

    /// <summary>
    /// Returns the rightValue if the Either is in a Left state.
    /// Returns the Right value if the Either is in a Right state.
    /// </summary>
    /// <param name="rightValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    [Pure]
    public R IfLeft(R rightValue) =>
        ifLeft<MEither<L, R>, Either<L, R>, L, R>(this, rightValue);

    /// <summary>
    /// Executes the Left action if the Either is in a Left state.
    /// </summary>
    /// <param name="Left">Function to generate a Right value if in the Left state</param>
    /// <returns>Returns an unwrapped Right value</returns>
    public Unit IfLeft(Action<L> Left) =>
        ifLeft<MEither<L, R>, Either<L, R>, L, R>(this, Left);

    /// <summary>
    /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
    /// </summary>
    /// <param name="Right">Action to invoke</param>
    /// <returns>Unit</returns>
    public Unit IfRight(Action<R> Right) =>
        ifRight<MEither<L, R>, Either<L, R>, L, R>(this, Right);

    /// <summary>
    /// Returns the leftValue if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="leftValue">Value to return if in the Left state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(L leftValue) =>
        ifRight<MEither<L, R>, Either<L, R>, L, R>(this, leftValue);

    /// <summary>
    /// Returns the result of Right() if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="Right">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(Func<L> Right) =>
        ifRight<MEither<L, R>, Either<L, R>, L, R>(this, Right);

    /// <summary>
    /// Returns the result of rightMap if the Either is in a Right state.
    /// Returns the Left value if the Either is in a Left state.
    /// </summary>
    /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
    /// <returns>Returns an unwrapped Left value</returns>
    [Pure]
    public L IfRight(Func<R, L> rightMap) =>
        ifRight<MEither<L, R>, Either<L, R>, L, R>(this, rightMap);

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
        IsBottom
            ? "Bottom"
            : IsRight
                ? RightValue is null
                      ? "Right(null)"
                      : $"Right({RightValue})"
                : LeftValue is null
                    ? "Left(null)"
                    : $"Left({LeftValue})";

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

    /// <summary>
    /// Returns a hash code of the wrapped value of the Either
    /// </summary>
    /// <returns>Hash code</returns>
    [Pure]
    public override int GetHashCode() =>
        hashCode<MEither<L, R>, Either<L, R>, L, R>(this);

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
        obj is Either<L, R> rs && 
        EqChoice<EqDefault<L>, EqDefault<R>, MEither<L, R>, Either<L,R>, L, R>.Equals(this, rs);

    /// <summary>
    /// Convert `Either` to a `Transducer`
    /// </summary>
    [Pure]
    public Transducer<Unit, Sum<L, R>> Morphism
    {
        get
        {
            var self = this;
            return IsRight
                       ? lift<Unit, Sum<L, R>>(_ => Sum<L, R>.Right(self.RightValue))
                       : IsLeft
                           ? lift<Unit, Sum<L, R>>(_ => Sum<L, R>.Left(self.LeftValue))
                           : Transducer.fail<Unit, Sum<L, R>>(Errors.Bottom);
        }
    }

    [Pure]
    public Reducer<Unit, S> Transform<S>(Reducer<Sum<L, R>, S> reduce) => 
        Morphism.Transform(reduce);

    /// <summary>
    /// Project the Either into a Lst L
    /// </summary>
    /// <returns>If the Either is in a Left state, a Lst of L with one item.  A zero length Lst L otherwise</returns>
    [Pure]
    public Lst<L> LeftToList() =>
        leftToList<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Project the Either into an ImmutableArray L
    /// </summary>
    /// <returns>If the Either is in a Left state, a ImmutableArray of L with one item.  A zero length ImmutableArray of L otherwise</returns>
    [Pure]
    public Arr<L> LeftToArray() =>
        leftToArray<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Project the Either into a Lst R
    /// </summary>
    /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
    public Lst<R> ToList() =>
        toList<MEither<L,R>,Either<L,R>,L,R>(this);

    /// <summary>
    /// Project the Either into an ImmutableArray R
    /// </summary>
    /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
    public Arr<R> ToArray() =>
        toArray<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Convert either to sequence of 0 or 1 right values
    /// </summary>
    [Pure]
    public Seq<R> ToSeq() =>
        RightAsEnumerable();

    /// <summary>
    /// Convert either to sequence of 0 or 1 left values
    /// </summary>
    [Pure]
    public Seq<L> LeftToSeq() =>
        LeftAsEnumerable();

    /// <summary>
    /// Project the Either into a IEnumerable R
    /// </summary>
    /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
    [Pure]
    public Seq<R> RightAsEnumerable() =>
        rightAsEnumerable<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Project the Either into a IEnumerable L
    /// </summary>
    /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
    [Pure]
    public Seq<L> LeftAsEnumerable() =>
        leftAsEnumerable<MEither<L, R>, Either<L, R>, L, R>(this);

    [Pure]
    public Validation<L, R> ToValidation() =>
        IsBottom
            ? throw new BottomException()
            : IsRight
                ? Success<L, R>(right!)
                : Fail<L, R>(left!);

    /// <summary>
    /// Convert the Either to an Option
    /// </summary>
    /// <returns>Some(Right) or None</returns>
    [Pure]
    public Option<R> ToOption() =>
        toOption<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Convert the Either to a Sum
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Sum<L, R> ToSum() =>
        IsBottom
            ? throw new BottomException()
            : IsRight
                ? Sum<L, R>.Right(right!)
                : Sum<L, R>.Left(left!);

    /// <summary>
    /// Convert to an `IO` monad
    /// </summary>
    /// <returns>`IO` monad</returns>
    [Pure]
    public IO<L, R> ToIO() =>
        State switch
        {
            EitherStatus.IsRight => Pure(RightValue),
            EitherStatus.IsLeft  => Fail(LeftValue),
            _                    => default // bottom
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
            EitherStatus.IsRight => Pure(RightValue),
            EitherStatus.IsLeft  => Fail(Left(LeftValue)),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Aff
    /// </summary>
    /// <param name="Left">Map the left value to the Eff Error</param>
    /// <returns>Aff monad</returns>
    [Pure]
    [Obsolete(Change.UseEffMonadInstead)]
    public Aff<R> ToAff(Func<L, Error> Left) =>
        State switch
        {
            EitherStatus.IsRight => SuccessAff(RightValue),
            EitherStatus.IsLeft  => FailAff<R>(Left(LeftValue)),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert the Either to an TryOption
    /// </summary>
    /// <returns>Some(Right) or None</returns>
    [Pure]
    public TryOption<R> ToTryOption() =>
        toTryOption<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Fail<L> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Fail<L> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Fail<L> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Fail<L> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Pure<R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Pure<R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Pure<R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Pure<R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) >= 0;


    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Fail<L> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Fail<L>  lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Fail<L> lhs, Either<L, R>rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Fail<L> lhs, Either<L, R>  rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) >= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Pure<R> lhs, Either<L, R>  rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Pure<R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Pure<R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Pure<R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) >= 0;




    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Either<L, R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs <= rhs</returns>
    [Pure]
    public static bool operator <=(Either<L, R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs > rhs</returns>
    [Pure]
    public static bool operator >(Either<L, R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs >= rhs</returns>
    [Pure]
    public static bool operator >=(Either<L, R> lhs, Either<L, R> rhs) =>
        compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) >= 0;

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
        MEither<L,R>.Plus(lhs,rhs);

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Either<L, R> operator |(Either<L, R> lhs, Pure<R> rhs) =>
        MEither<L, R>.Plus(lhs, rhs);

    /// <summary>
    /// Override of the Or operator to be a Left coalescing operator
    /// </summary>
    [Pure]
    public static Either<L, R> operator |(Either<L, R> lhs, Fail<L> rhs) =>
        MEither<L, R>.Plus(lhs, rhs);

    /// <summary>
    /// Override of the True operator to return True if the Either is Right
    /// </summary>
    [Pure]
    public static bool operator true(Either<L, R> value) =>
        value is { IsRight: true };

    /// <summary>
    /// Override of the False operator to return True if the Either is Left
    /// </summary>
    [Pure]
    public static bool operator false(Either<L, R> value) =>
        value is { IsLeft: true };

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Either<L, R> other) =>
        OrdChoice<OrdDefault<L>, OrdDefault<R>, MEither<L, R>, Either<L, R>, L, R>.Compare(this, other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Fail<L> other) =>
        OrdChoice<OrdDefault<L>, OrdDefault<R>, MEither<L, R>, Either<L, R>, L, R>.Compare(this, other);

    /// <summary>
    /// CompareTo override
    /// </summary>
    [Pure]
    public int CompareTo(Pure<R> other) =>
        OrdChoice<OrdDefault<L>, OrdDefault<R>, MEither<L, R>, Either<L, R>, L, R>.Compare(this, other);

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
        EqChoice<EqDefault<L>, EqDefault<R>, MEither<L, R>, Either<L, R>, L, R>.Equals(this, other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(Fail<L> other) =>
        EqChoice<EqDefault<L>, EqDefault<R>, MEither<L, R>, Either<L, R>, L, R>.Equals(this, other);

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public bool Equals(Pure<R> other) =>
        EqChoice<EqDefault<L>, EqDefault<R>, MEither<L, R>, Either<L, R>, L, R>.Equals(this, other);

    /// <summary>
    /// Match the Right and Left values but as objects.  This can be useful to avoid reflection.
    /// </summary>
    [Pure]
    public Res MatchUntyped<Res>(Func<object?, Res> Right, Func<object?, Res> Left) =>
        matchUntyped<MEither<L, R>, Either<L, R>, L, R, Res>(this, Left: Left, Right: Right);

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
        IsRight
            ? right!
            : raise<R>(new EitherIsNotRightException());

    [Pure]
    internal L LeftValue =>
        IsLeft
            ? left!
            : raise<L>(new EitherIsNotLeftException());

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
        IsBottom || IsLeft
            ? 0
            : 1;

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
    public Unit Iter(Action<R> Right) =>
        iter<MEither<L, R>, Either<L, R>, R>(this, Right);

    /// <summary>
    /// Iterate the Either
    /// action is invoked if in the Right state
    /// </summary>
    public Unit BiIter(Action<R> Right, Action<L> Left) =>
        biIter<MEither<L, R>, Either<L, R>, L, R>(this, Left, Right);

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
        forall<MEither<L, R>, Either<L, R>, R>(this, Right);

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
    public bool BiForAll(Func<R, bool> Right, Func<L, bool> Left) =>
        biForAll<MEither<L, R>, Either<L, R>, L, R>(this, Left, Right);

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
        MEither<L,R>.Fold(this, state, Right)(unit);

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
    public S BiFold<S>(S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
        MEither<L, R>.BiFold(this, state, Left, Right);

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
        exists<MEither<L, R>, Either<L, R>, R>(this, pred);

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
    public bool BiExists(Func<R, bool> Right, Func<L, bool> Left) =>
        biExists<MEither<L, R>, Either<L, R>,L,  R>(this, Left, Right);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Either<L, R> Do(Action<R> f)
    {
        Iter(f);
        return this;
    }

    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="mapper">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<L, Ret> Map<Ret>(Func<R, Ret> mapper) =>
        FEither<L, R, Ret>.Map(this, mapper);

    /// <summary>
    /// Maps the value in the Either if it's in a Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="mapper">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<Ret, R> MapLeft<Ret>(Func<L, Ret> mapper) =>
        FEitherBi<L, R, Ret, R>.BiMap(this, mapper, identity);

    /// <summary>
    /// Bi-maps the value in the Either into a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="LRet">Left return</typeparam>
    /// <typeparam name="RRet">Right return</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="Right">Right map function</param>
    /// <param name="Left">Left map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<L, Ret> BiMap<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
        FEither<L, R, Ret>.BiMap(this, Left, Right);

    /// <summary>
    /// Bi-maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="LRet">Left return</typeparam>
    /// <typeparam name="RRet">Right return</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="Right">Right map function</param>
    /// <param name="Left">Left map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<L2, R2> BiMap<L2, R2>(Func<R, R2> Right, Func<L, L2> Left) =>
        FEitherBi<L, R, L2, R2>.BiMap(this, Left, Right);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="B"></typeparam>
    /// <param name="self"></param>
    /// <param name="f"></param>
    /// <returns>Bound Either</returns>
    [Pure]
    public Either<L, B> Bind<B>(Func<R, Either<L, B>> f) =>
        MEither<L, R>.Bind<MEither<L, B>, Either<L, B>, B>(this, f);

    /// <summary>
    /// Bi-bind.  Allows mapping of both monad states
    /// </summary>
    [Pure]
    public Either<L, B> BiBind<B>(Func<R, Either<L, B>> Right, Func<L, Either<L, B>> Left) =>
        State switch
        {
            EitherStatus.IsRight => Right(RightValue),
            EitherStatus.IsLeft  => Left(LeftValue),
            _                    => Either<L, B>.Bottom
        };

    /// <summary>
    /// Bind left.  Binds the left path of the monad only
    /// </summary>
    [Pure]
    public Either<B, R> BindLeft<B>(Func<L, Either<B, R>> f) =>
        State switch
        {
            EitherStatus.IsRight => Right<B, R>(RightValue),
            EitherStatus.IsLeft  => f(LeftValue),
            _                    => Either<B, R>.Bottom
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
    /// <param name="self">Either to filter</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
    [Pure]
    public Either<L, R> Filter(Func<R, bool> pred) =>
        filter<MEither<L, R>, Either<L, R>, R>(this, pred);

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
    /// <param name="self">Either to filter</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
    /// of Left = default(L)</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Either<L, R> Where(Func<R, bool> pred) =>
        filter<MEither<L, R>, Either<L, R>, R>(this, pred);

    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="TR">Right</typeparam>
    /// <typeparam name="UR">Mapped Either type</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped Either</returns>
    [Pure]
    public Either<L, U> Select<U>(Func<R, U> map) =>
        FEither<L, R, U>.Map(this, map);

    /// <summary>
    /// Monadic bind function
    /// </summary>
    /// <returns>Bound Either</returns>
    [Pure]
    public Either<L, V> SelectMany<U, V>(Func<R, Either<L, U>> bind, Func<R, U, V> project) =>
        SelectMany<MEither<L, R>, MEither<L, U>, MEither<L, V>, Either<L, R>, Either<L, U>, Either<L, V>, R, U, V>(this, bind, project);

    [Pure]
    public Either<L, V> Join<U, K, V>(
        Either<L, U> inner,
        Func<R, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<R, U, V> project) =>
        join<EqDefault<K>, MEither<L, R>, MEither<L, U>, MEither<L, V>, Either<L, R>, Either<L, U>, Either<L, V>, R, U, K, V>(
            this, inner, outerKeyMap, innerKeyMap, project);
        
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
        IsRight  ? f(RightValue).ToEither<L>()
        : IsLeft ? Either<L, B>.Left(LeftValue)
                   : Either<L, B>.Bottom;      

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Either<L, B> Bind<B>(Func<R, Fail<L>> f) =>
        IsRight  ? f(RightValue).ToEither<B>()
        : IsLeft ? Either<L, B>.Left(LeftValue)
                   : Either<L, B>.Bottom;

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
    public Either<L, C> SelectMany<B, C>(Func<R, Fail<L>> bind, Func<R, B, C> project) =>
        Bind(x => bind(x).ToEither<C>());

    [Pure]
    public static implicit operator Either<L, R>(Pure<R> mr) =>
        Right(mr.Value);

    [Pure]
    public static implicit operator Either<L, R>(Fail<L> mr) =>
        Left(mr.Value);
    
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
        rightToList<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Project the Either into an ImmutableArray R
    /// </summary>
    /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
    [Pure]
    [Obsolete(Change.UseToArrayInstead)]
    public Arr<R> RightToArray() =>
        rightToArray<MEither<L, R>, Either<L, R>, L, R>(this);

    /// <summary>
    /// Convert either to sequence of 0 or 1 right values
    /// </summary>
    [Pure]
    [Obsolete(Change.UseToSeqInstead)]
    public Seq<R> RightToSeq() =>
        RightAsEnumerable();
    
    /// <summary>
    /// Convert the Either to an EitherAsync
    /// </summary>
    [Pure]
    [Obsolete(Change.UseEffMonadInstead)]
    public EitherAsync<L, R> ToAsync() =>
        Match(Left: EitherAsync<L, R>.Left,
              Right: EitherAsync<L, R>.Right,
              Bottom: () => EitherAsync<L, R>.Bottom);
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
