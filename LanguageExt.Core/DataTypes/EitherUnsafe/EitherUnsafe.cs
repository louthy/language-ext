using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.ChoiceUnsafe;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using LanguageExt.DataTypes.Serialisation;
using System.Collections;

namespace LanguageExt
{
    /// <summary>
    /// Either L R
    /// Holds one of two values 'Left' or 'Right'.  Usually 'Left' is considered 'wrong' or 'in error', and
    /// 'Right' is, well, right.  So when the Either is in a Left state, it cancels computations like bind
    /// or map, etc.  So you can see Left as an 'early out, with a message'.  Unlike Option that has None
    /// as its alternative value (i.e. it has an 'early out, but no message').
    /// </summary>
    /// <remarks>
    /// NOTE: If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
    /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
    /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
    /// of filtering Either.
    /// 
    /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
    /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
    /// doesn't needlessly break expressions. 
    /// </remarks>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    [Serializable]
    public struct EitherUnsafe<L, R> :
        IEnumerable<EitherData<L, R>>,
        IEither,
        IComparable<EitherUnsafe<L, R>>,
        IComparable<R>,
        IEquatable<EitherUnsafe<L, R>>,
        IEquatable<R>,
        ISerializable
    {
        public readonly static EitherUnsafe<L, R> Bottom = new EitherUnsafe<L, R>();

        internal readonly R right;
        internal readonly L left;

        private EitherUnsafe(R right)
        {
            this.State = EitherStatus.IsRight;
            this.right = right;
            this.left = default(L);
        }

        private EitherUnsafe(L left)
        {
            this.State = EitherStatus.IsLeft;
            this.right = default(R);
            this.left = left;
        }

        [Pure]
        EitherUnsafe(SerializationInfo info, StreamingContext context)
        {
            State = (EitherStatus)info.GetValue("State", typeof(EitherStatus));
            switch (State)
            {
                case EitherStatus.IsBottom:
                    right = default(R);
                    left = default(L);
                    break;
                case EitherStatus.IsRight:
                    right = (R)info.GetValue("Right", typeof(R));
                    left = default(L);
                    break;
                case EitherStatus.IsLeft:
                    left = (L)info.GetValue("Left", typeof(L));
                    right = default(R);
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
        /// Ctor that facilitates serialisation
        /// </summary>
        /// <param name="option">None or Some A.</param>
        [Pure]
        public EitherUnsafe(IEnumerable<EitherData<L, R>> either)
        {
            var first = either.Take(1).ToArray();
            if (first.Length == 0)
            {
                this.State = EitherStatus.IsBottom;
                this.right = default(R);
                this.left = default(L);
            }
            else
            {
                this.right = first[0].Right;
                this.left = first[0].Left;
                this.State =first[0].State;
            }
        }

        IEnumerable<EitherData<L, R>> EitherEnum()
        {
            yield return new EitherData<L, R>(State, right, left);
        }

        public IEnumerator<EitherData<L, R>> GetEnumerator() =>
            EitherEnum().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            EitherEnum().GetEnumerator();

        /// <summary>
        /// State of the EitherUnsafe
        /// You can also use:
        ///     IsRight
        ///     IsLeft
        ///     IsBottom
        /// </summary>
        public readonly EitherStatus State;

        /// <summary>
        /// Is the EitherUnsafe in a Right state?
        /// </summary>
        /// <exception cref="BottomException">EitherUnsafeT state is Bottom</exception>
        [Pure]
        public bool IsRight =>
            CheckInitialised(State == EitherStatus.IsRight);

        /// <summary>
        /// Is the EitherUnsafe in a Left state?
        /// </summary>
        /// <exception cref="BottomException">EitherUnsafeT state is Bottom</exception>
        [Pure]
        public bool IsLeft =>
            CheckInitialised(State == EitherStatus.IsLeft);

        /// <summary>
        /// Is the EitherUnsafe in a Bottom state?
        /// When the EitherUnsafe is filtered, both Right and Left are meaningless.
        /// 
        /// If you use Filter or Where (or 'where' in a LINQ expression) with EitherUnsafe, then the EitherUnsafe 
        /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
        /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
        /// of filtering Either.
        /// 
        /// Also note, when the EitherUnsafe is in a Bottom state, some operations on it will continue to give valid
        /// results or return another EitherUnsafe in the Bottom state and not throw.  This is so a filtered EitherUnsafe 
        /// doesn't needlessly break expressions. 
        /// </summary>
        [Pure]
        public bool IsBottom =>
            State == EitherStatus.IsBottom;

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public EitherCase<L, R> Case =>
            State switch
            {
                EitherStatus.IsRight => RightCase<L, R>.New(right),
                EitherStatus.IsLeft => LeftCase<L, R>.New(left),
                _ => null
            };

        /// <summary>
        /// Implicit conversion operator from R to EitherUnsafe R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        [Pure]
        public static implicit operator EitherUnsafe<L, R>(R value) =>
            Right(value);

        /// <summary>
        /// Implicit conversion operator from L to EitherUnsafe R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        [Pure]
        public static implicit operator EitherUnsafe<L, R>(L value) =>
            Left(value);

        /// <summary>
        /// Implicit conversion operator from EitherRight to Either
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        public static implicit operator EitherUnsafe<L, R>(EitherRight<R> right) =>
            Right(right.Value);

        /// <summary>
        /// Implicit conversion operator from EitherRight to Either
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        public static implicit operator EitherUnsafe<L, R>(EitherLeft<L> left) =>
            Left(left.Value);

        /// <summary>
        /// Explicit conversion operator from `EitherUnsafe` to `R`
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static explicit operator R(EitherUnsafe<L, R> ma) =>
            ma.IsRight
                ? ma.right
                : throw new InvalidCastException("EitherUnsafe is not in a Right state");

        /// <summary>
        /// Explicit conversion operator from `EitherUnsafe` to `L`
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static explicit operator L(EitherUnsafe<L, R> ma) =>
            ma.IsLeft
                ? ma.left
                : throw new InvalidCastException("EitherUnsafe is not in a Left state");

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the EitherUnsafe
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an EitherUnsafe in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret MatchUnsafe<Ret>(Func<R, Ret> Right, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            MEitherUnsafe<L, R>.Inst.MatchUnsafe(this, Left, Right, Bottom);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the EitherUnsafe
        /// </summary>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        /// <exception cref="BottomException">Thrown if matching on an EitherUnsafe in a bottom state</exception>
        public Unit MatchUnsafe(Action<R> Right, Action<L> Left, Action Bottom = null) =>
            MEitherUnsafe<L, R>.Inst.Match(this, Left, Right, Bottom);

        /// <summary>
        /// Executes the Left function if the EitherUnsafe is in a Left state.
        /// Returns the Right value if the EitherUnsafe is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeftUnsafe(Func<R> Left) =>
            ifLeftUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Left);

        /// <summary>
        /// Executes the leftMap function if the EitherUnsafe is in a Left state.
        /// Returns the Right value if the EitherUnsafe is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeftUnsafe(Func<L, R> leftMap) =>
            ifLeftUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, leftMap);

        /// <summary>
        /// Returns the rightValue if the EitherUnsafe is in a Left state.
        /// Returns the Right value if the EitherUnsafe is in a Right state.
        /// </summary>
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeftUnsafe(R rightValue) =>
            ifLeftUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, rightValue);

        /// <summary>
        /// Executes the Left action if the EitherUnsafe is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public Unit IfLeftUnsafe(Action<L> Left) =>
            ifLeftUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Left);

        /// <summary>
        /// Invokes the Right action if the EitherUnsafe is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        public Unit IfRightUnsafe(Action<R> Right) =>
            ifRightUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Right);

        /// <summary>
        /// Returns the leftValue if the EitherUnsafe is in a Right state.
        /// Returns the Left value if the EitherUnsafe is in a Left state.
        /// </summary>
        /// <param name="leftValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRightUnsafe(L leftValue) =>
            ifRightUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, leftValue);

        /// <summary>
        /// Returns the result of Right() if the EitherUnsafe is in a Right state.
        /// Returns the Left value if the EitherUnsafe is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRightUnsafe(Func<L> Right) =>
            ifRightUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Right);

        /// <summary>
        /// Returns the result of rightMap if the EitherUnsafe is in a Right state.
        /// Returns the Left value if the EitherUnsafe is in a Left state.
        /// </summary>
        /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRightUnsafe(Func<R, L> rightMap) =>
            ifRightUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, rightMap);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the EitherUnsafe is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherUnsafeUnitContext<L, R> Right(Action<R> right) =>
            new EitherUnsafeUnitContext<L, R>(this, right);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the EitherUnsafe is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherUnsafeContext<L, R, Ret> Right<Ret>(Func<R, Ret> right) =>
            new EitherUnsafeContext<L, R, Ret>(this, right);

        /// <summary>
        /// Return a string representation of the EitherUnsafe
        /// </summary>
        /// <returns>String representation of the EitherUnsafe</returns>
        [Pure]
        public override string ToString() =>
            IsBottom
                ? "Bottom"
                : IsRight
                    ? isnull(RightValue)
                        ? "Right(null)"
                        : $"Right({RightValue})"
                    : isnull(LeftValue)
                        ? "Left(null)"
                        : $"Left({LeftValue})";

        /// <summary>
        /// Returns a hash code of the wrapped value of the EitherUnsafe
        /// </summary>
        /// <returns>Hash code</returns>
        [Pure]
        public override int GetHashCode() =>
            hashCode<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns>True if equal</returns>
        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) &&
            obj is EitherUnsafe<L, R> &&
            EqChoiceUnsafe<EqDefault<L>, EqDefault<R>, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Inst.Equals(this, (EitherUnsafe<L, R>)obj);

        /// <summary>
        /// Project the EitherUnsafe into a Lst R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("ToList has been deprecated.  Please use RightToList.")]
        public Lst<R> ToList() =>
            toList<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Project the EitherUnsafe into an ImmutableArray R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("ToArray has been deprecated.  Please use RightToArray.")]
        public Arr<R> ToArray() =>
            toArray<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Project the EitherUnsafe into a Lst R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public Lst<R> RightToList() =>
            rightToList<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Project the EitherUnsafe into an ImmutableArray R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public Arr<R> RightToArray() =>
            rightToArray<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Project the EitherUnsafe into a Lst R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public Lst<L> LeftToList() =>
            leftToList<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Project the EitherUnsafe into an ImmutableArray R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public Arr<L> LeftToArray() =>
            leftToArray<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Convert either to sequence of 0 or 1 right values
        /// </summary>
        [Pure]
        public Seq<R> ToSeq() =>
            RightAsEnumerable();

        /// <summary>
        /// Convert either to sequence of 0 or 1 right values
        /// </summary>
        [Pure]
        public Seq<R> RightToSeq() =>
            RightAsEnumerable();

        /// <summary>
        /// Convert either to sequence of 0 or 1 left values
        /// </summary>
        [Pure]
        public Seq<L> LeftToSeq() =>
            LeftAsEnumerable();

        /// <summary>
        /// Project the EitherUnsafe into a IEnumerable R
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [Pure]
        public Seq<R> RightAsEnumerable() =>
            rightAsEnumerable<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Project the EitherUnsafe into a IEnumerable L
        /// </summary>
        /// <returns>If the EitherUnsafe is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
        [Pure]
        public Seq<L> LeftAsEnumerable() =>
            leftAsEnumerable<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        [Pure]
        public Validation<L, R> ToValidation() =>
            IsBottom
                ? throw new BottomException()
                : IsRight
                    ? Success<L, R>(right)
                    : Fail<L, R>(left);

        /// <summary>
        /// Convert the EitherUnsafe to an OptionUnsafe
        /// </summary>
        /// <returns>Some(Right) or None</returns>
        [Pure]
        public OptionUnsafe<R> ToOption() =>
            toOptionUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Convert the EitherUnsafe to an EitherUnsafeUnsafe
        /// </summary>
        /// <returns>EitherUnsafeUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, R> ToEither() =>
            toEitherUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this);

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs < rhs</returns>
        [Pure]
        public static bool operator <(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) < 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs <= rhs</returns>
        [Pure]
        public static bool operator <=(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) <= 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs > rhs</returns>
        [Pure]
        public static bool operator >(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) > 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs >= rhs</returns>
        [Pure]
        public static bool operator >=(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            compare<OrdDefault<L>, OrdDefault<R>, L, R>(lhs, rhs) >= 0;

        /// <summary>
        /// Equality operator override
        /// </summary>
        [Pure]
        public static bool operator ==(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator override
        /// </summary>
        [Pure]
        public static bool operator !=(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            !(lhs == rhs);

        /// <summary>
        /// Override of the Or operator to be a Left coalescing operator
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, R> operator |(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            MEitherUnsafe<L, R>.Inst.Plus(lhs, rhs);

        /// <summary>
        /// Override of the True operator to return True if the EitherUnsafe is Right
        /// </summary>
        [Pure]
        public static bool operator true(EitherUnsafe<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsRight;

        /// <summary>
        /// Override of the False operator to return True if the EitherUnsafe is Left
        /// </summary>
        [Pure]
        public static bool operator false(EitherUnsafe<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsLeft;

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(EitherUnsafe<L, R> other) =>
            OrdChoiceUnsafe<OrdDefault<L>, OrdDefault<R>, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Inst.Compare(this, other);

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(R other) =>
            CompareTo(RightUnsafe<L, R>(other));

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(L other) =>
            CompareTo(LeftUnsafe<L, R>(other));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(R other) =>
            Equals(Right<L, R>(other));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(L other) =>
            Equals(Left<L, R>(other));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(EitherUnsafe<L, R> other) =>
            EqChoiceUnsafe<EqDefault<L>, EqDefault<R>, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Inst.Equals(this, other);

        /// <summary>
        /// Match the Right and Left values but as objects.  This can be useful to avoid reflection.
        /// </summary>
        [Pure]
        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            matchUntypedUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R, TResult>(this, Left: Left, Right: Right);

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
        private U CheckInitialised<U>(U value) =>
            State == EitherStatus.IsBottom
                ? raise<U>(new BottomException("EitherUnsafe"))
                : value;

        [Pure]
        internal static EitherUnsafe<L, R> Right(R value) =>
            new EitherUnsafe<L, R>(value);

        [Pure]
        internal static EitherUnsafe<L, R> Left(L value) =>
            new EitherUnsafe<L, R>(value);

        [Pure]
        internal R RightValue =>
            CheckInitialised(
                IsRight
                    ? right
                    : raise<R>(new EitherIsNotRightException())
            );

        [Pure]
        internal L LeftValue =>
            CheckInitialised(
                IsLeft
                    ? left
                    : raise<L>(new EitherIsNotLeftException())
            );

        [Pure]
        public R1 MatchUntyped<R1>(Func<object, R1> Some, Func<R1> None) =>
            matchUntypedUnsafe<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R, R1>(this, Some, _ => None());

        [Pure]
        public Type GetUnderlyingType() =>
            typeof(R);

        /// <summary>
        /// Counts the EitherUnsafe
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to count</param>
        /// <returns>1 if the EitherUnsafe is in a Right state, 0 otherwise.</returns>
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
        public EitherUnsafe<R, L> Swap() =>
            State switch
            {
                EitherStatus.IsRight => EitherUnsafe<R, L>.Left(RightValue),
                EitherStatus.IsLeft  => EitherUnsafe<R, L>.Right(LeftValue),
                _                    => EitherUnsafe<R, L>.Bottom
            };        

        /// <summary>
        /// Iterate the EitherUnsafe
        /// action is invoked if in the Right state
        /// </summary>
        public Unit Iter(Action<R> Right) =>
            iter<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(this, Right);

        /// <summary>
        /// Iterate the EitherUnsafe
        /// action is invoked if in the Right state
        /// </summary>
        public Unit BiIter(Action<R> Right, Action<L> Left) =>
            biIter<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Left, Right);

        /// <summary>
        /// Invokes a predicate on the value of the EitherUnsafe if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to forall</param>
        /// <param name="Right">Predicate</param>
        /// <returns>True if the EitherUnsafe is in a Left state.  
        /// True if the EitherUnsafe is in a Right state and the predicate returns True.  
        /// False otherwise.</returns>
        [Pure]
        public bool ForAll(Func<R, bool> Right) =>
            forall<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(this, Right);

        /// <summary>
        /// Invokes a predicate on the value of the EitherUnsafe if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to forall</param>
        /// <param name="Right">Predicate</param>
        /// <param name="Left">Predicate</param>
        /// <returns>True if EitherUnsafe Predicate returns true</returns>
        [Pure]
        public bool BiForAll(Func<R, bool> Right, Func<L, bool> Left) =>
            biForAll<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Left, Right);

        /// <summary>
        /// <para>
        /// EitherUnsafe types are like lists of 0 or 1 items, and therefore follow the 
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
            MEitherUnsafe<L, R>.Inst.Fold(this, state, Right)(unit);

        /// <summary>
        /// <para>
        /// EitherUnsafe types are like lists of 0 or 1 items, and therefore follow the 
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
        /// <param name="Right">Folder function, applied if EitherUnsafe is in a Right state</param>
        /// <param name="Left">Folder function, applied if EitherUnsafe is in a Left state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S BiFold<S>(S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            MEitherUnsafe<L, R>.Inst.BiFold(this, state, Left, Right);

        /// <summary>
        /// Invokes a predicate on the value of the EitherUnsafe if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to check existence of</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the EitherUnsafe is in a Right state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public bool Exists(Func<R, bool> pred) =>
            exists<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(this, pred);

        /// <summary>
        /// Invokes a predicate on the value of the EitherUnsafe
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to check existence of</param>
        /// <param name="Right">Right predicate</param>
        /// <param name="Left">Left predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise or if the EitherUnsafe is in a bottom state.</returns>
        [Pure]
        public bool BiExists(Func<R, bool> Right, Func<L, bool> Left) =>
            biExists<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(this, Left, Right);

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public EitherUnsafe<L, R> Do(Action<R> f)
        {
            Iter(f);
            return this;
        }

        /// <summary>
        /// Maps the value in the EitherUnsafe if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped EitherUnsafe type</typeparam>
        /// <param name="self">EitherUnsafe to map</param>
        /// <param name="mapper">Map function</param>
        /// <returns>Mapped EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, Ret> Map<Ret>(Func<R, Ret> mapper) =>
            FEitherUnsafe<L, R, Ret>.Inst.Map(this, mapper);

        /// <summary>
        /// Maps the value in the EitherUnsafe if it's in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped EitherUnsafe type</typeparam>
        /// <param name="self">EitherUnsafe to map</param>
        /// <param name="mapper">Map function</param>
        /// <returns>Mapped EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<Ret, R> MapLeft<Ret>(Func<L, Ret> mapper) =>
            FEitherUnsafeBi<L, R, Ret, R>.Inst.BiMap(this, mapper, identity);

        /// <summary>
        /// Bi-maps the value in the EitherUnsafe into a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">EitherUnsafe to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, Ret> BiMap<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            FEitherUnsafe<L, R, Ret>.Inst.BiMap(this, Left, Right);

        /// <summary>
        /// Bi-maps the value in the EitherUnsafe if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">EitherUnsafe to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L2, R2> BiMap<L2, R2>(Func<R, R2> Right, Func<L, L2> Left) =>
            FEitherUnsafeBi<L, R, L2, R2>.Inst.BiMap(this, Left, Right);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="self"></param>
        /// <param name="binder"></param>
        /// <returns>Bound EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, Ret> Bind<Ret>(Func<R, EitherUnsafe<L, Ret>> binder) =>
            MEitherUnsafe<L, R>.Inst.Bind<MEitherUnsafe<L, Ret>, EitherUnsafe<L, Ret>, Ret>(this, binder);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public EitherUnsafe<L, B> BiBind<B>(Func<R, EitherUnsafe<L, B>> Right, Func<L, EitherUnsafe<L, B>> Left) =>
            IsRight ? Right(RightValue)
          : IsLeft  ? Left(LeftValue)
          : EitherUnsafe<L, B>.Bottom;

        /// <summary>
        /// Bind left.  Binds the left path of the monad only
        /// </summary>
        [Pure]
        public EitherUnsafe<B, R> BindLeft<B>(Func<L, EitherUnsafe<B, R>> f) =>
            IsLeft  ? f(LeftValue)
          : IsRight ? RightUnsafe<B, R>(RightValue)
          : EitherUnsafe<B, R>.Bottom;

        /// <summary>
        /// Filter the EitherUnsafe
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The EitherUnsafe won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>If the EitherUnsafe is in the Left state it is returned as-is.  
        /// If in the Right state the predicate is applied to the Right value.
        /// If the predicate returns True the EitherUnsafe is returned as-is.
        /// If the predicate returns False the EitherUnsafe is returned in a 'Bottom' state.</returns>
        [Pure]
        public EitherUnsafe<L, R> Filter(Func<R, bool> pred) =>
            filter<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(this, pred);

        /// <summary>
        /// Filter the EitherUnsafe
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The EitherUnsafe won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">EitherUnsafe to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>If the EitherUnsafe is in the Left state it is returned as-is.  
        /// If in the Right state the predicate is applied to the Right value.
        /// If the predicate returns True the EitherUnsafe is returned as-is.
        /// If the predicate returns False the EitherUnsafe is returned in a 'Bottom' state.  IsLeft will return True, but the value 
        /// of Left = default(L)</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EitherUnsafe<L, R> Where(Func<R, bool> pred) =>
            filter<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, R>(this, pred);

        /// <summary>
        /// Maps the value in the EitherUnsafe if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="TR">Right</typeparam>
        /// <typeparam name="UR">Mapped EitherUnsafe type</typeparam>
        /// <param name="self">EitherUnsafe to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, U> Select<U>(Func<R, U> map) =>
            FEitherUnsafe<L, R, U>.Inst.Map(this, map);

        /// <summary>
        /// Monadic bind function
        /// </summary>
        /// <returns>Bound EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, V> SelectMany<U, V>(Func<R, EitherUnsafe<L, U>> bind, Func<R, U, V> project) =>
            SelectMany<MEitherUnsafe<L, R>, MEitherUnsafe<L, U>, MEitherUnsafe<L, V>, EitherUnsafe<L, R>, EitherUnsafe<L, U>, EitherUnsafe<L, V>, R, U, V>(this, bind, project);

        [Pure]
        public EitherUnsafe<L, V> Join<U, K, V>(
            EitherUnsafe<L, U> inner,
            Func<R, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<R, U, V> project) =>
            join<EqDefault<K>, MEitherUnsafe<L, R>, MEitherUnsafe<L, U>, MEitherUnsafe<L, V>, EitherUnsafe<L, R>, EitherUnsafe<L, U>, EitherUnsafe<L, V>, R, U, K, V>(
                this, inner, outerKeyMap, innerKeyMap, project
                );
    }
    /// <summary>
    /// Context for the fluent EitherUnsafe matching
    /// </summary>
    public struct EitherUnsafeContext<L, R, Ret>
    {
        readonly EitherUnsafe<L, R> either;
        readonly Func<R, Ret> rightHandler;

        internal EitherUnsafeContext(EitherUnsafe<L, R> either, Func<R, Ret> rightHandler)
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
            either.MatchUnsafe(rightHandler, left);
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct EitherUnsafeUnitContext<L, R>
    {
        readonly EitherUnsafe<L, R> either;
        readonly Action<R> rightHandler;

        internal EitherUnsafeUnitContext(EitherUnsafe<L, R> either, Action<R> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Unit Left(Action<L> leftHandler) =>
            either.MatchUnsafe(rightHandler, leftHandler);
    }
}
