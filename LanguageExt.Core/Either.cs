using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;
using System.ComponentModel;

namespace LanguageExt
{
    /// <summary>
    /// Either L R
    /// Holds one of two values 'Left' or 'Right'.  Usually 'Left' is considered 'wrong' or 'in error', and
    /// 'Right' is, well, right.  So when the Either is in a Left state, it cancels computations like bind
    /// or map, etc.  So you can see Left as an 'early out, with a message'.  Unlike Option that has None
    /// as its alternative value (i.e. it has an 'early out, but no message').
    /// 
    /// NOTE: If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
    /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
    /// neither Right or Left.  And any usage could trigger a BottomException.  So be aware of the issue
    /// of filtering Either.
    /// 
    /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
    /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
    /// doesn't needlessly break expressions. 
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    public struct Either<L, R> :
        IEither,
        IComparable<Either<L,R>>, 
        IComparable<R>, 
        IEquatable<Either<L, R>>, 
        IEquatable<R>
    {
        enum EitherState : byte
        {
            IsUninitialised = 0,
            IsLeft = 1,
            IsRight = 2,
            IsBottom = 3
        }

        readonly EitherState state;
        readonly R right;
        readonly L left;

        private Either(R right)
        {
            this.state = EitherState.IsRight;
            this.right = right;
            this.left = default(L);
        }

        private Either(L left)
        {
            this.state = EitherState.IsLeft;
            this.right = default(R);
            this.left = left;
        }

        internal Either(bool bottom)
        {
            this.state = EitherState.IsBottom;
            this.right = default(R);
            this.left = default(L);
        }

        internal static Either<L, R> Right(R value) =>
            new Either<L, R>(value);

        internal static Either<L, R> Left(L value) =>
            new Either<L, R>(value);

        /// <summary>
        /// Is the Either in a Right state
        /// </summary>
        public bool IsRight =>
            CheckInitialised(state == EitherState.IsRight);

        /// <summary>
        /// Is the Either in a Left state
        /// </summary>
        public bool IsLeft =>
            CheckInitialised(state == EitherState.IsLeft);

        /// <summary>
        /// Is the Either in a Bottom state.  
        /// When the Either is filtered, both Right and Left are meaningless.
        /// </summary>
        public bool IsBottom =>
            state == EitherState.IsBottom;

        internal R RightValue =>
            CheckInitialised(
                IsRight
                    ? right
                    : raise<R>(new EitherIsNotRightException())
            );

        internal L LeftValue =>
            CheckInitialised(
                IsLeft
                    ? left
                    : raise<L>(new EitherIsNotLeftException())
            );

        /// <summary>
        /// Implicit conversion operator from R to Either R L
        /// </summary>
        /// <param name="value">Value</param>
        public static implicit operator Either<L, R>(R value) =>
            value == null
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Right(value);

        /// <summary>
        /// Implicit conversion operator from L to Either R L
        /// </summary>
        /// <param name="value">Value</param>
        public static implicit operator Either<L, R>(L value) =>
            value == null
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Left(value);


        private T CheckNullReturn<T>(T value, string location) =>
            value == null
                ? raise<T>(new ResultIsNullException("'" + location + "' result is null.  Not allowed."))
                : value;

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <returns>The return value of the invoked function</returns>
        public Ret Match<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            IsRight
                ? CheckNullReturn(Right(RightValue), "Right")
                : IsLeft
                    ? CheckNullReturn(Left(LeftValue), "Left")
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
        /// Deprecated
        /// </summary>
        [Obsolete("'Failure' has been deprecated.  Please use 'Left' instead")]
        public R Failure(Func<R> None) =>
            Match(identity, _ => None());

        /// <summary>
        /// Deprecated
        /// </summary>
        [Obsolete("'Failure' has been deprecated.  Please use 'Left' instead")]
        public R Failure(R noneValue) =>
            Match(identity, _ => noneValue);

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public R IfLeft(Func<R> Left) =>
            Match(identity, _ => Left());

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public R IfLeft(R rightValue) =>
            Match(identity, _ => rightValue);

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        public Unit IfRight(Action<R> Right)
        {
            if (IsRight)
            {
                Right(right);
            }
            return unit;
        }

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        public EitherUnitContext<L, R> Right(Action<R> right) =>
            new EitherUnitContext<L, R>(this, right);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        public EitherContext<L, R, Ret> Right<Ret>(Func<R, Ret> right) =>
            new EitherContext<L, R, Ret>(this, right);

        /// <summary>
        /// Return a string representation of the Either
        /// </summary>
        /// <returns>String representation of the Either</returns>
        public override string ToString() =>
            IsBottom
                ? "Bottom"
                : IsRight
                    ? RightValue == null
                        ? "Right(null)"
                        : String.Format("Right({0})", RightValue)
                    : LeftValue == null
                        ? "Left(null)"
                        : String.Format("Left({0})", LeftValue);

        /// <summary>
        /// Returns a hash code of the wrapped value of the Either
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() =>
            IsRight
                ? RightValue == null
                    ? 0
                    : RightValue.GetHashCode()
                : LeftValue == null
                    ? 0
                    : LeftValue.GetHashCode();

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj) =>
            obj is Either<L, R>
                ? map(this, (Either<L, R>)obj, (lhs, rhs) =>
                    lhs.IsLeft && rhs.IsLeft
                        ? lhs.LeftValue.Equals(rhs.LeftValue)
                        : lhs.IsLeft || rhs.IsLeft
                            ? false
                            : lhs.RightValue.Equals(rhs.RightValue))
                : false;

        private U CheckInitialised<U>(U value) =>
            state == EitherState.IsUninitialised
                ? raise<U>(new EitherNotInitialisedException())
                : state == EitherState.IsBottom
                    ? raise<U>(new BottomException("Either"))
                    : value;

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        public Lst<R> ToList() =>
            toList(AsEnumerable());

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        public ImmutableArray<R> ToArray() =>
            toArray(AsEnumerable());

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
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
        public Option<R> ToOption() =>
            IsRight
                ? Some(RightValue)
                : None;

        /// <summary>
        /// Convert the Either to an EitherUnsafe
        /// </summary>
        /// <returns>EitherUnsafe</returns>
        public EitherUnsafe<L, R> ToEitherUnsafe() =>
            IsRight
                ? RightUnsafe<L, R>(RightValue)
                : LeftUnsafe<L, R>(LeftValue);

        /// <summary>
        /// Convert the Either to an TryOption
        /// </summary>
        /// <returns>Some(Right) or None</returns>
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
        public static bool operator ==(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator override
        /// </summary>
        public static bool operator !=(Either<L, R> lhs, Either<L, R> rhs) =>
            !lhs.Equals(rhs);

        /// <summary>
        /// Override of the Or operator to be a Left coalescing operator
        /// </summary>
        public static Either<L, R> operator |(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.IsBottom || rhs.IsBottom
                ? lhs
                : lhs.IsRight
                    ? lhs
                    : rhs;

        /// <summary>
        /// Override of the True operator to return True if the Either is Right
        /// </summary>
        public static bool operator true(Either<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsRight;

        /// <summary>
        /// Override of the False operator to return True if the Either is Left
        /// </summary>
        public static bool operator false(Either<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsLeft;

        /// <summary>
        /// CompareTo override
        /// </summary>
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
        public int CompareTo(R other) =>
            IsRight
                ? Comparer<R>.Default.Compare(RightValue, other)
                : -1;

        /// <summary>
        /// CompareTo override
        /// </summary>
        public int CompareTo(L other) =>
            IsRight
                ? -1
                : Comparer<L>.Default.Compare(LeftValue, other);

        /// <summary>
        /// Equality override
        /// </summary>
        public bool Equals(R other) =>
            IsBottom
                ? false
                : IsRight
                    ? EqualityComparer<R>.Default.Equals(RightValue, other)
                    : false;

        /// <summary>
        /// Equality override
        /// </summary>
        public bool Equals(L other) =>
            IsBottom
                ? false
                : IsLeft
                    ? EqualityComparer<L>.Default.Equals(LeftValue, other)
                    : false;

        /// <summary>
        /// Equality override
        /// </summary>
        public bool Equals(Either<L, R> other) =>
            IsRight
                ? other.Equals(RightValue)
                : other.Equals(LeftValue);

        /// <summary>
        /// Match the Right and Left values but as objects.  This can be useful to avoid reflection.
        /// </summary>
        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

        /// <summary>
        /// Find out the underlying Right type
        /// </summary>
        public Type GetUnderlyingRightType() =>
            typeof(R);

        /// <summary>
        /// Find out the underlying Left type
        /// </summary>
        public Type GetUnderlyingLeftType() =>
            typeof(L);
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
    /// Counts the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
    public static int Count<L, R>(this Either<L, R> self) =>
        self.IsBottom || self.IsLeft
            ? 0
            : 1;

    /// <summary>
    /// Total of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
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
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Left state.  
    /// True if the Either is in a Right state and the predicate returns True.  
    /// False otherwise.</returns>
    public static bool ForAll<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? true
            : self.IsRight
                ? pred(self.RightValue)
                : true;

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
    public static S Fold<L,R,S>(this Either<L, R> self, S state, Func<S, R, S> folder) =>
        self.IsBottom
            ? state
            : self.IsRight
                ? folder(state, self.RightValue)
                : state;

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to check existence of</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
    public static bool Exists<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? false
            : self.IsRight
                ? pred(self.RightValue)
                : false;

    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="mapper">Map function</param>
    /// <returns>Mapped Either</returns>
    public static Either<L, Ret> Map<L, R, Ret>(this Either<L, R> self, Func<R, Ret> mapper) =>
        self.IsBottom
            ? new Either<L, Ret>(true)
            : self.IsRight
                ? Right<L, Ret>(mapper(self.RightValue))
                : Left<L, Ret>(self.LeftValue);

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
    public static Either<L, Ret> Bind<L, R, Ret>(this Either<L, R> self, Func<R, Either<L, Ret>> binder) =>
        self.IsBottom
            ? new Either<L, Ret>(true)
            : self.IsRight
                ? binder(self.RightValue)
                : Either<L, Ret>.Left(self.LeftValue);

    /// <summary>
    /// Filter the Either
    /// This may give unpredictable results for a filtered value.  The Either won't
    /// return true for IsLeft or IsRight.  IsBottom is True if the value is filterd and that
    /// should be checked.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to filter</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
    /// of Left = default(L)</returns>
    public static Either<L, R> Filter<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? self
            : match(self,
                Right: t => pred(t) ? Either<L, R>.Right(t) : new Either<L, R>(true),
                Left: l => Either<L, R>.Left(l));

    /// <summary>
    /// Filter the Either
    /// This may give unpredictable results for a filtered value.  The Either won't
    /// return true for IsLeft or IsRight.  IsBottom is True if the value is filterd and that
    /// should be checked.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to filter</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
    /// of Left = default(L)</returns>
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Either<L, UR> Select<L, TR, UR>(this Either<L, TR> self, Func<TR, UR> map) =>
        self.IsBottom
            ? new Either<L, UR>(true)
            : match(self,
                Right: t => Either<L, UR>.Right(map(t)),
                Left: l => Either<L, UR>.Left(l)
                );

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <returns>Bound Either</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Either<L, V> SelectMany<L, T, U, V>(this Either<L, T> self, Func<T, Either<L, U>> bind, Func<T, U, V> project)
    {
        if (self.IsBottom) return new Either<L, V>(true);
        if (self.IsLeft) return Either<L, V>.Left(self.LeftValue);
        var u = bind(self.RightValue);
        if (u.IsBottom) return new Either<L, V>(true);
        if (u.IsLeft) return Either<L, V>.Left(u.LeftValue);
        return project(self.RightValue, u.RightValue);
    }
}
