using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

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
#if !COREFX
    [Serializable]
#endif
    public struct Either<L, R> :
        IEither,
        IComparable<Either<L,R>>,
        IComparable<R>,
        IEquatable<Either<L, R>>,
        IEquatable<R>,
        IAppendable<Either<L, R>>,
        ISubtractable<Either<L, R>>,
        IMultiplicable<Either<L, R>>,
        IDivisible<Either<L, R>>
    {
        public readonly static Either<L, R> Bottom = new Either<L, R>();

        readonly R right; 
        readonly L left;

        private Either(R right)
        {
            if (isnull(right))
                throw new ValueIsNullException();

            this.State = EitherState.IsRight;
            this.right = right;
            this.left = default(L);
        }

        private Either(L left)
        {
            if (isnull(left))
                throw new ValueIsNullException();

            this.State = EitherState.IsLeft;
            this.right = default(R);
            this.left = left;
        }

        /// <summary>
        /// State of the Either
        /// You can also use:
        ///     IsRight
        ///     IsLeft
        ///     IsBottom
        /// </summary>
        public readonly EitherState State;

        /// <summary>
        /// Is the Either in a Right state?
        /// </summary>
        /// <exception cref="BottomException">EitherT state is Bottom</exception>
        [Pure]
        public bool IsRight =>
            CheckInitialised(State == EitherState.IsRight);

        /// <summary>
        /// Is the Either in a Left state?
        /// </summary>
        /// <exception cref="BottomException">EitherT state is Bottom</exception>
        [Pure]
        public bool IsLeft =>
            CheckInitialised(State == EitherState.IsLeft);

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
            State == EitherState.IsBottom;

        /// <summary>
        /// Implicit conversion operator from R to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator Either<L, R>(R value) =>
            isnull(value)
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Right(value);

        /// <summary>
        /// Implicit conversion operator from L to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator Either<L, R>(L value) =>
            isnull(value)
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Left(value);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret Match<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            IsRight
                ? CheckNullRightReturn(Right(RightValue))
                : IsLeft
                    ? CheckNullLeftReturn(Left(LeftValue))
                    : raise<Ret>(new BottomException("Either"));

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public Unit Match(Action<R> Right, Action<L> Left)
        {
            if (IsRight)
            {
                Right(RightValue);
            }
            else if (IsLeft)
            {
                Left(LeftValue);
            }
            else
            {
                raise<Unit>(new BottomException("Either"));
            }
            return unit;
        }

        /// <summary>
        /// Match the two states of the Either and return a promise for a non-null R2.
        /// </summary>
        /// <returns>A promise to return a non-null R2</returns>
        public async Task<R2> MatchAsync<R2>(Func<R, Task<R2>> Right, Func<L, R2> Left) =>
            IsRight
                ? CheckNullRightReturn(await Right(RightValue))
                : CheckNullLeftReturn(Left(LeftValue));

        /// <summary>
        /// Match the two states of the Either and return a promise for a non-null R2.
        /// </summary>
        /// <returns>A promise to return a non-null R2</returns>
        public async Task<R2> MatchAsync<R2>(Func<R, Task<R2>> Right, Func<L, Task<R2>> Left) =>
            IsRight
                ? CheckNullRightReturn(await Right(RightValue))
                : CheckNullLeftReturn(await Left(LeftValue));

        /// <summary>
        /// Match the two states of the Either and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public IObservable<R2> MatchObservable<R2>(Func<R, IObservable<R2>> Right, Func<L, R2> Left) =>
            IsRight
                ? Right(RightValue).Select(CheckNullRightReturn)
                : Observable.Return(CheckNullLeftReturn(Left(LeftValue)));

        /// <summary>
        /// Match the two states of the Either and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public IObservable<R2> MatchObservable<R2>(Func<R, IObservable<R2>> Right, Func<L, IObservable<R2>> Left) =>
            IsRight
                ? Right(RightValue).Select(CheckNullRightReturn)
                : Left(LeftValue).Select(CheckNullLeftReturn);

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeft(Func<R> Left) =>
            Match(identity, _ => CheckNullReturn(Left(), nameof(Left)));

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeft(Func<L, R> leftMap) =>
            Match(identity, l => CheckNullReturn(leftMap(l), nameof(leftMap)));

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeft(R rightValue) =>
            Match(identity, _ => rightValue);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public Unit IfLeft(Action<L> Left)
        {
            if (!IsBottom && IsLeft)
            {
                Left(LeftValue);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        public Unit IfRight(Action<R> Right)
        {
            if (!IsBottom && IsRight)
            {
                Right(right);
            }
            return unit;
        }

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRight(L leftValue) =>
            Match(_ => leftValue, identity);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRight(Func<L> Right) =>
            Match(_ => CheckNullReturn(Right(), nameof(Right)), identity);

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRight(Func<R, L> rightMap) =>
            Match(r => CheckNullReturn(rightMap(r), nameof(rightMap)), identity);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherUnitContext<L, R> Right(Action<R> right) =>
            new EitherUnitContext<L, R>(this, right);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherContext<L, R, Ret> Right<Ret>(Func<R, Ret> right) =>
            new EitherContext<L, R, Ret>(this, right);

        /// <summary>
        /// Return a string representation of the Either
        /// </summary>
        /// <returns>String representation of the Either</returns>
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
        /// Returns a hash code of the wrapped value of the Either
        /// </summary>
        /// <returns>Hash code</returns>
        [Pure]
        public override int GetHashCode() =>
            IsBottom
                ? -1
                : IsRight
                    ? isnull(RightValue)
                        ? 0
                        : RightValue.GetHashCode()
                    : isnull(LeftValue)
                        ? 0
                        : LeftValue.GetHashCode();

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns>True if equal</returns>
        [Pure]
        public override bool Equals(object obj) =>
            obj is Either<L, R>
                ? map(this, (Either<L, R>)obj, (lhs, rhs) =>
                    lhs.IsLeft && rhs.IsLeft
                        ? lhs.LeftValue.Equals(rhs.LeftValue)
                        : lhs.IsLeft || rhs.IsLeft
                            ? false
                            : lhs.RightValue.Equals(rhs.RightValue))
                : false;

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("ToList has been deprecated.  Please use RightToList.")]
        public Lst<R> ToList() =>
            toList(AsEnumerable());

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("ToArray has been deprecated.  Please use RightToArray.")]
        public R[] ToArray() =>
            toArray<R>(AsEnumerable());

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public Lst<R> RightToList() =>
            toList(RightAsEnumerable());

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public R[] RightToArray() =>
            toArray(RightAsEnumerable());

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public Lst<L> LeftToList() =>
            toList(LeftAsEnumerable());

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public L[] LeftToArray() =>
            toArray(LeftAsEnumerable());

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [Pure]
        public IEnumerable<R> RightAsEnumerable()
        {
            if (IsRight)
            {
                yield return RightValue;
            }
        }

        /// <summary>
        /// Project the Either into a IEnumerable L
        /// </summary>
        /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
        [Pure]
        public IEnumerable<L> LeftAsEnumerable()
        {
            if (IsLeft)
            {
                yield return LeftValue;
            }
        }

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("AsEnumerable has been deprecated.  Please use RightAsEnumerable.")]
        public IEnumerable<R> AsEnumerable()
        {
            if (IsRight)
            {
                yield return RightValue;
            }
        }
        /// <summary>
        /// Convert the Either to an Option
        /// </summary>
        /// <returns>Some(Right) or None</returns>
        [Pure]
        public Option<R> ToOption() =>
            IsRight
                ? Some(RightValue)
                : None;

        /// <summary>
        /// Convert the Either to an EitherUnsafe
        /// </summary>
        /// <returns>EitherUnsafe</returns>
        [Pure]
        public EitherUnsafe<L, R> ToEitherUnsafe() =>
            IsRight
                ? RightUnsafe<L, R>(RightValue)
                : LeftUnsafe<L, R>(LeftValue);

        /// <summary>
        /// Convert the Either to an TryOption
        /// </summary>
        /// <returns>Some(Right) or None</returns>
        [Pure]
        public TryOption<R> ToTryOption()
        {
            var self = this;
            return () =>
                self.IsRight
                    ? Some(self.RightValue)
                    : None;
        }

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
        public static bool operator !=(Either<L, R> lhs, Either<L, R> rhs) =>
            !lhs.Equals(rhs);

        /// <summary>
        /// Override of the Or operator to be a Left coalescing operator
        /// </summary>
        [Pure]
        public static Either<L, R> operator |(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.IsBottom || rhs.IsBottom
                ? lhs
                : lhs.IsRight
                    ? lhs
                    : rhs;

        /// <summary>
        /// Override of the True operator to return True if the Either is Right
        /// </summary>
        [Pure]
        public static bool operator true(Either<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsRight;

        /// <summary>
        /// Override of the False operator to return True if the Either is Left
        /// </summary>
        [Pure]
        public static bool operator false(Either<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsLeft;

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(Either<L, R> other) =>
            IsLeft && other.IsLeft
                ? Comparer<L>.Default.Compare(LeftValue, other.LeftValue)
                : IsRight && other.IsRight
                    ? Comparer<R>.Default.Compare(RightValue, other.RightValue)
                    : IsLeft
                        ? -1
                        : 1;

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(R other) =>
            IsRight
                ? Comparer<R>.Default.Compare(RightValue, other)
                : -1;

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(L other) =>
            IsRight
                ? -1
                : Comparer<L>.Default.Compare(LeftValue, other);

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(R other) =>
            IsBottom
                ? false
                : IsRight
                    ? EqualityComparer<R>.Default.Equals(RightValue, other)
                    : false;

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(L other) =>
            IsBottom
                ? false
                : IsLeft
                    ? EqualityComparer<L>.Default.Equals(LeftValue, other)
                    : false;

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(Either<L, R> other) =>
            IsBottom && other.IsBottom
                ? true
                : IsBottom || other.IsBottom
                    ? false
                    : IsRight
                        ? other.Equals(RightValue)
                        : other.Equals(LeftValue);

        /// <summary>
        /// Match the Right and Left values but as objects.  This can be useful to avoid reflection.
        /// </summary>
        [Pure]
        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

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
            State == EitherState.IsBottom
                ? raise<U>(new BottomException("Either"))
                : value;

        [Pure]
        internal static Either<L, R> Right(R value) =>
            new Either<L, R>(value);

        [Pure]
        internal static Either<L, R> Left(L value) =>
            new Either<L, R>(value);

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
        internal static T CheckNullReturn<T>(T value, string location) =>
            isnull(value)
                ? raise<T>(new ResultIsNullException($"'{location}' result is null.  Not allowed."))
                : value;

        internal static T CheckNullRightReturn<T>(T value) =>
            CheckNullReturn(value, "Right");

        internal static T CheckNullLeftReturn<T>(T value) =>
            CheckNullReturn(value, "Left");


        /// <summary>
        /// Append the Right of one either to the Right of another
        /// For numeric values the behaviour is to sum the Rights (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the R type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Either<L,R> operator +(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Append the Right of one either to the Right of another
        /// For numeric values the behaviour is to sum the Rights (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the R type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public Either<L, R> Append(Either<L, R> rhs)
        {
            if (IsLeft) return this;    // The rules here are different to Option because
            if (rhs.IsLeft) return rhs; // dropping the 'Left' value would also lose information
            return TypeDesc.Append<R>(RightValue, rhs.RightValue, TypeDesc<R>.Default);
        }

        /// <summary>
        /// Subtract the Right of one either from the Right of another
        /// For numeric values the behaviour is to find the difference between the Rights (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static Either<L, R> operator -(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Subtract the Right of one either from the Right of another
        /// For numeric values the behaviour is to find the difference between the Rights (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public Either<L, R> Subtract(Either<L, R> rhs)
        {
            if (IsLeft) return this;    // The rules here are different to Option because
            if (rhs.IsLeft) return rhs; // dropping the 'Left' value would also lose information
            return TypeDesc.Subtract<R>(RightValue, rhs.RightValue, TypeDesc<R>.Default);
        }

        /// <summary>
        /// Find the product of the Rights 
        /// For numeric values the behaviour is to multiply the Rights (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static Either<L, R> operator *(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Multiply(rhs);

        /// <summary>
        /// Find the product of the Rights 
        /// For numeric values the behaviour is to multiply the Rights (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public Either<L, R> Multiply(Either<L, R> rhs)
        {
            if (IsLeft) return this;    // The rules here are different to Option because
            if (rhs.IsLeft) return rhs; // dropping the 'Left' value would also lose information
            return TypeDesc.Multiply<R>(RightValue, rhs.RightValue, TypeDesc<R>.Default);
        }

        /// <summary>
        /// Divide the Rights 
        /// For numeric values the behaviour is to divide the Rights (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static Either<L, R> operator /(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Divide(rhs);

        /// <summary>
        /// Divide the Rights 
        /// For numeric values the behaviour is to divide the Rights (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public Either<L, R> Divide(Either<L, R> rhs)
        {
            if (IsLeft) return this;    // The rules here are different to Option because
            if (rhs.IsLeft) return rhs; // dropping the 'Left' value would also lose information
            return TypeDesc.Divide<R>(RightValue, rhs.RightValue, TypeDesc<R>.Default);
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'Failure' has been deprecated.  Please use 'Left' instead")]
        public R Failure(Func<R> None) =>
            Match(identity, _ => None());

        /// <summary>
        /// Deprecated
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'Failure' has been deprecated.  Please use 'Left' instead")]
        public R Failure(R noneValue) =>
            Match(identity, _ => noneValue);
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct EitherContext<L, R, Ret>
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
        public Ret Left(Func<L, Ret> left)
        {
            return match(either, rightHandler, left);
        }
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct EitherUnitContext<L, R>
    {
        readonly Either<L, R> either;
        readonly Action<R> rightHandler;

        internal EitherUnitContext(Either<L, R> either, Action<R> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Unit Left(Action<L> leftHandler)
        {
            return match(either, rightHandler, leftHandler);
        }
    }
}

/// <summary>
/// Extension methods for Either
/// </summary>
public static class __EitherExt
{
    /// <summary>
    /// Apply an Either value to an Either function
    /// </summary>
    /// <param name="self">Either function</param>
    /// <param name="arg">Either argument</param>
    /// <returns>Returns the result of applying the Either argument to the Either function</returns>
    [Pure]
    public static Either<L, Res> Apply<L, R, Res>(this Either<L, Func<R, Res>> self, Either<L, R> arg) =>
        arg.IsBottom || self.IsBottom
            ? Either<L, Res>.Bottom
            : self.IsLeft
                ? Either<L, Res>.Left(self.LeftValue)
                : arg.IsLeft
                    ? Either<L, Res>.Left(arg.LeftValue)
                    : self.Select(f => f(arg.RightValue));

    /// <summary>
    /// Apply an Either value to an Either function of arity 2
    /// </summary>
    /// <param name="self">Either function</param>
    /// <param name="arg">Either argument</param>
    /// <returns>Returns the result of applying the Either argument to the Either function:
    /// an Either function of arity 1</returns>
    [Pure]
    public static Either<L, Func<T2, R>> Apply<L, T1, T2, R>(this Either<L, Func<T1, T2, R>> self, Either<L, T1> arg) =>
        arg.IsBottom || self.IsBottom
            ? Either<L, Func<T2, R>>.Bottom
            : self.IsLeft
                ? Either<L, Func<T2, R>>.Left(self.LeftValue)
                : arg.IsLeft
                    ? Either<L, Func<T2, R>>.Left(arg.LeftValue)
                    : self.Select(f => par(f, arg.RightValue));

    /// <summary>
    /// Apply Either values to an Either function of arity 2
    /// </summary>
    /// <param name="self">Either function</param>
    /// <param name="arg1">Either argument</param>
    /// <param name="arg2">Either argument</param>
    /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
    [Pure]
    public static Either<L, R> Apply<L, T1, T2, R>(this Either<L, Func<T1, T2, R>> self, Either<L, T1> arg1, Either<L, T2> arg2) =>
        arg1.IsBottom || arg2.IsBottom || self.IsBottom
            ? Either<L, R>.Bottom
            : self.IsLeft
                ? Either<L, R>.Left(self.LeftValue)
                : arg1.IsLeft
                    ? Either<L, R>.Left(arg1.LeftValue)
                    : arg2.IsLeft
                        ? Either<L, R>.Left(arg2.LeftValue)
                        : self.Select(f => f(arg1.RightValue, arg2.RightValue));

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<L> Lefts<L, R>(this IEnumerable<Either<L, R>> self)
    {
        foreach (var item in self)
        {
            if (item.IsLeft)
            {
                yield return item.LeftValue;
            }
        }
    }

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<R> Rights<L, R>(this IEnumerable<Either<L, R>> self)
    {
        foreach (var item in self)
        {
            if (item.IsRight)
            {
                yield return item.RightValue;
            }
        }
    }

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static Tuple<IEnumerable<L>, IEnumerable<R>> Partition<L, R>(this IEnumerable<Either<L, R>> self) =>
        Tuple(lefts(self), rights(self));

    /// <summary>
    /// Counts the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
    [Pure]
    public static int Count<L, R>(this Either<L, R> self) =>
        self.IsBottom || self.IsLeft
            ? 0
            : 1;

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    [Pure]
    public static int Sum<L>(this Either<L, int> self) =>
        self.IsBottom || self.IsLeft
            ? 0
            : self.RightValue;

    /// <summary>
    /// Iterate the Either
    /// action is invoked if in the Right state
    /// </summary>
    public static Unit Iter<L, R>(this Either<L, R> self, Action<R> action)
    {
        if (self.IsBottom)
        {
            return unit;
        }
        if (self.IsRight)
        {
            action(self.RightValue);
        }
        return unit;
    }

    /// <summary>
    /// Iterate the Either
    /// action is invoked if in the Left state
    /// </summary>
    public static Unit Iter<L, R>(this Either<L, R> self, Action<L> action)
    {
        if (self.IsBottom)
        {
            return unit;
        }
        if (self.IsLeft)
        {
            action(self.LeftValue);
        }
        return unit;
    }

    /// <summary>
    /// Iterate the Either
    /// Appropriate action is invoked depending on the state of the Either
    /// </summary>
    public static Unit Iter<L, R>(this Either<L, R> self, Action<R> Right, Action<L> Left)
    {
        self.Iter(Right);
        self.Iter(Left);
        return unit;
    }

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Left state.  
    /// True if the Either is in a Right state and the predicate returns True.  
    /// False otherwise.</returns>
    [Pure]
    public static bool ForAll<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? true
            : self.IsRight
                ? pred(self.RightValue)
                : true;

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Right state.  
    /// True if the Either is in a Left state and the predicate returns True.  
    /// False otherwise.</returns>
    [Pure]
    public static bool ForAll<L, R>(this Either<L, R> self, Func<L, bool> pred) =>
        self.IsBottom
            ? true
            : self.IsLeft
                ? pred(self.LeftValue)
                : true;

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="Right">Right predicate</param>
    /// <param name="Left">Left predicate</param>
    /// <returns>True if the predicate returns True.  True if the Either is in a bottom state.</returns>
    [Pure]
    public static bool ForAll<L, R>(this Either<L, R> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.ForAll(Right) && self.ForAll(Left);

    /// <summary>
    /// Folds the either into an S
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<L, R, S>(this Either<L, R> self, S state, Func<S, R, S> folder) =>
        self.IsBottom
            ? state
            : self.IsRight
                ? folder(state, self.RightValue)
                : state;

    /// <summary>
    /// Folds the either into an S
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<L, R, S>(this Either<L, R> self, S state, Func<S, L, S> folder) =>
        self.IsBottom
            ? state
            : self.IsLeft
                ? folder(state, self.LeftValue)
                : state;

    /// <summary>
    /// Folds the either into an S
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Right">Right fold function</param>
    /// <param name="Left">Left fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<L, R, S>(this Either<L, R> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
        self.IsBottom
            ? state
            : self.IsRight
                ? self.Fold(state, Right)
                : self.Fold(state, Left);

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to check existence of</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
    [Pure]
    public static bool Exists<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? false
            : self.IsRight
                ? pred(self.RightValue)
                : false;

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Left state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to check existence of</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Left state and the predicate returns True.  False otherwise.</returns>
    [Pure]
    public static bool Exists<L, R>(this Either<L, R> self, Func<L, bool> pred) =>
        self.IsBottom
            ? false
            : self.IsLeft
                ? pred(self.LeftValue)
                : false;

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
    public static bool Exists<L, R>(this Either<L, R> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.IsBottom
            ? false
            : self.IsLeft
                ? Left(self.LeftValue)
                : Right(self.RightValue);

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
    public static Either<L, Ret> Map<L, R, Ret>(this Either<L, R> self, Func<R, Ret> mapper) =>
        self.IsBottom
            ? Either<L, Ret>.Bottom
            : self.IsRight
                ? Right<L, Ret>(mapper(self.RightValue))
                : Left<L, Ret>(self.LeftValue);

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
    public static Either<Ret, R> MapLeft<L, R, Ret>(this Either<L, R> self, Func<L, Ret> mapper) =>
        self.IsBottom
            ? Either<Ret, R>.Bottom
            : self.IsLeft
                ? Left<Ret, R>(mapper(self.LeftValue))
                : Right<Ret, R>(self.RightValue);

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
    public static Either<LRet, RRet> BiMap<L, R, LRet, RRet>(this Either<L, R> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
        self.IsBottom
            ? Either<LRet, RRet>.Bottom
            : self.IsRight
                ? Right<LRet, RRet>(Right(self.RightValue))
                : Left<LRet, RRet>(Left(self.LeftValue));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Either<L, Func<T2, R>> ParMap<L, T1, T2, R>(this Either<L, T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Either<L, Func<T2, Func<T3, R>>> ParMap<L, T1, T2, T3, R>(this Either<L, T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret"></typeparam>
    /// <param name="self"></param>
    /// <param name="binder"></param>
    /// <returns>Bound Either</returns>
    [Pure]
    public static Either<L, Ret> Bind<L, R, Ret>(this Either<L, R> self, Func<R, Either<L, Ret>> binder) =>
        self.IsBottom
            ? Either<L, Ret>.Bottom
            : self.IsRight
                ? binder(self.RightValue)
                : Either<L, Ret>.Left(self.LeftValue);

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret"></typeparam>
    /// <param name="self"></param>
    /// <param name="binder"></param>
    /// <returns>Bound Either</returns>
    [Pure]
    public static Either<Ret, R> Bind<L, R, Ret>(this Either<L, R> self, Func<L, Either<Ret, R>> binder) =>
        self.IsBottom
            ? Either<Ret, R>.Bottom
            : self.IsLeft
                ? binder(self.LeftValue)
                : Either<Ret, R>.Right(self.RightValue);

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret"></typeparam>
    /// <param name="self">this</param>
    /// <param name="Right">Right bind function</param>
    /// <param name="Left">Left bind function</param>
    /// <returns>Bound Either</returns>
    [Pure]
    public static Either<LRet, RRet> Bind<L, R, LRet, RRet>(this Either<L, R> self, Func<R, Either<LRet, RRet>> Right, Func<L, Either<LRet, RRet>> Left) =>
        self.IsBottom
            ? Either<LRet, RRet>.Bottom
            : self.IsLeft
                ? Left(self.LeftValue)
                : Right(self.RightValue);

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
    public static Either<L, R> Filter<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? self
            : match(self,
                Right: t => pred(t) ? Either<L, R>.Right(t) : Either<L, R>.Bottom,
                Left: l => Either<L, R>.Left(l));

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
    /// <returns>If the Either is in the Right state it is returned as-is.  
    /// If in the Left state the predicate is applied to the Left value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
    [Pure]
    public static Either<L, R> Filter<L, R>(this Either<L, R> self, Func<L, bool> pred) =>
        self.IsBottom
            ? self
            : match(self,
                Right: r => Either<L, R>.Right(r),
                Left:  t => pred(t) ? Either<L, R>.Left(t) : Either<L, R>.Bottom);

    /// <summary>
    /// Bi-filter the Either
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
    /// <returns>
    /// If the Either is in the Left state then the Left predicate is run against it.
    /// If the Either is in the Right state then the Right predicate is run against it.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
    [Pure]
    public static Either<L, R> Filter<L, R>(this Either<L, R> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.IsBottom
            ? self
            : match(self,
                Right: r => Right(r) ? Either<L, R>.Right(r) : Either<L, R>.Bottom,
                Left:  l => Left(l)  ? Either<L, R>.Left(l)  : Either<L, R>.Bottom);

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
    public static Either<L, R> Where<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        Filter(self, pred);

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Either<L, UR> Select<L, TR, UR>(this Either<L, TR> self, Func<TR, UR> map) =>
        self.IsBottom
            ? Either<L, UR>.Bottom
            : match(self,
                Right: t => Either<L, UR>.Right(map(t)),
                Left: l => Either<L, UR>.Left(l)
                );

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <returns>Bound Either</returns>
    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Either<L, V> SelectMany<L, T, U, V>(this Either<L, T> self, Func<T, Either<L, U>> bind, Func<T, U, V> project)
    {
        if (self.IsBottom) return Either<L, V>.Bottom;
        if (self.IsLeft) return Either<L, V>.Left(self.LeftValue);
        var u = bind(self.RightValue);
        if (u.IsBottom) return Either<L, V>.Bottom;
        if (u.IsLeft) return Either<L, V>.Left(u.LeftValue);
        return project(self.RightValue, u.RightValue);
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<V> SelectMany<L, T, U, V>(this Either<L, T> self,
        Func<T, IEnumerable<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsBottom) return new V[0];
        if (self.IsLeft) return new V[0];
        return bind(self.RightValue).Map(u => project(self.RightValue, u));
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Either<L, V> SelectMany<L, T, U, V>(this IEnumerable<T> self,
        Func<T, Either<L, U>> bind,
        Func<T, U, V> project
        )
    {
        var ta = self.Take(1).ToArray();
        if (ta.Length == 0) return Either<L,V>.Bottom;
        var u = bind(ta[0]);
        if (u.IsBottom) return Either<L, V>.Bottom;
        if (u.IsLeft) return Either<L, V>.Left(u.LeftValue);
        return project(ta[0], u.RightValue);
    }

    /// <summary>
    /// Match the two states of the Either and return a promise of a non-null R2.
    /// </summary>
    public static async Task<R2> MatchAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.IsRight
            ? Either<L, R>.CheckNullRightReturn(Right(await self.RightValue))
            : Either<L, R>.CheckNullLeftReturn(Left(self.LeftValue));

    /// <summary>
    /// Match the two states of the Either and return a stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this Either<L, IObservable<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.IsRight
            ? self.RightValue.Select(Right).Select(Either<L, R>.CheckNullRightReturn)
            : Observable.Return(Either<L, R>.CheckNullLeftReturn(Left(self.LeftValue)));

    /// <summary>
    /// Match the two states of the IObservable Either and return a stream of non-null R2s.
    /// </summary>
    [Pure]
    public static IObservable<R2> MatchObservable<L, R, R2>(this IObservable<Either<L, R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        self.Select(either => match(either, Right, Left));

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, R> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Task<R2>> map)
    {
        var val = await self;
        return val.IsRight
            ? await map(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, R2> map)
    {
        var val = await self;
        return val.IsRight
            ? map(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, R2> map) =>
        self.IsRight
            ? map(await self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(await self.RightValue)
            : Left<L, R2>(self.LeftValue);


    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, R> self, Func<R, Task<Either<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Task<Either<L, R2>>> bind)
    {
        var val = await self;
        return val.IsRight
            ? await bind(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Either<L, R2>> bind)
    {
        var val = await self;
        return val.IsRight
            ? bind(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Either<L, R2>> bind) =>
        self.IsRight
            ? bind(await self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Task<Either<L, R2>>> bind) =>
        self.IsRight
            ? await bind(await self.RightValue)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Unit> IterAsync<L, R>(this Task<Either<L, R>> self, Action<R> action)
    {
        var val = await self;
        if (val.IsRight) action(val.RightValue);
        return unit;
    }

    public static async Task<Unit> IterAsync<L, R>(this Either<L, Task<R>> self, Action<R> action)
    {
        if (self.IsRight) action(await self.RightValue);
        return unit;
    }

    public static async Task<int> CountAsync<L, R>(this Task<Either<L, R>> self) =>
        (await self).Count();

    public static async Task<int> SumAsync<L>(this Task<Either<L, int>> self) =>
        (await self).Sum();

    public static async Task<int> SumAsync<L>(this Either<L, Task<int>> self) =>
        self.IsRight
            ? await self.RightValue
            : 0;

    public static async Task<S> FoldAsync<L, R, S>(this Task<Either<L, R>> self, S state, Func<S, R, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<L, R, S>(this Either<L, Task<R>> self, S state, Func<S, R, S> folder) =>
        self.IsRight
            ? folder(state, await self.RightValue)
            : state;

    public static async Task<bool> ForAllAsync<L, R>(this Task<Either<L, R>> self, Func<R, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<L, R>(this Either<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(await self.RightValue)
            : true;

    public static async Task<bool> ExistsAsync<L, R>(this Task<Either<L, R>> self, Func<R, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<L, R>(this Either<L, Task<R>> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(await self.RightValue)
            : false;

}
