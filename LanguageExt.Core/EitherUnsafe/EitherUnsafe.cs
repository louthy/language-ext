using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

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
    public struct EitherUnsafe<L, R> :
        IEither,
        Optional<R>,
        Choice<L, R>, 
        MonadPlus<R>,
        IOptional,
        IComparable<EitherUnsafe<L, R>>,
        IComparable<R>,
        IEquatable<EitherUnsafe<L, R>>,
        IEquatable<R>
    {
        public readonly static EitherUnsafe<L, R> Bottom = new EitherUnsafe<L, R>();

        readonly R right;
        readonly L left;

        private EitherUnsafe(R right)
        {
            if (isnull(right))
                throw new ValueIsNullException();

            this.State = EitherState.IsRight;
            this.right = right;
            this.left = default(L);
        }

        private EitherUnsafe(L left)
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
        public static implicit operator EitherUnsafe<L, R>(R value) =>
            isnull(value)
                ? raise<EitherUnsafe<L, R>>(new ValueIsNullException())
                : EitherUnsafe<L, R>.Right(value);

        /// <summary>
        /// Implicit conversion operator from L to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator EitherUnsafe<L, R>(L value) =>
            isnull(value)
                ? raise<EitherUnsafe<L, R>>(new ValueIsNullException())
                : EitherUnsafe<L, R>.Left(value);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret MatchUnsafe<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            IsRight
                ? Right(RightValue)
                : IsLeft
                    ? Left(LeftValue)
                    : raise<Ret>(new BottomException("Either"));

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        public Unit MatchUnsafe(Action<R> Right, Action<L> Left)
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
        public async Task<R2> MatchAsyncUnsafe<R2>(Func<R, Task<R2>> Right, Func<L, R2> Left) =>
            IsRight
                ? await Right(RightValue)
                : Left(LeftValue);

        /// <summary>
        /// Match the two states of the Either and return a promise for a non-null R2.
        /// </summary>
        /// <returns>A promise to return a non-null R2</returns>
        public async Task<R2> MatchAsyncUnsafe<R2>(Func<R, Task<R2>> Right, Func<L, Task<R2>> Left) =>
            IsRight
                ? await Right(RightValue)
                : await Left(LeftValue);

        /// <summary>
        /// Match the two states of the Either and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public IObservable<R2> MatchObservableUnsafe<R2>(Func<R, IObservable<R2>> Right, Func<L, R2> Left) =>
            IsRight
                ? Right(RightValue)
                : Observable.Return(Left(LeftValue));

        /// <summary>
        /// Match the two states of the Either and return an observable stream of non-null R2s.
        /// </summary>
        [Pure]
        public IObservable<R2> MatchObservableUnsafe<R2>(Func<R, IObservable<R2>> Right, Func<L, IObservable<R2>> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeftUnsafe(Func<R> Left) =>
            MatchUnsafe(identity, _ => Left());

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeftUnsafe(Func<L, R> leftMap) =>
            MatchUnsafe(identity, leftMap);

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public R IfLeftUnsafe(R rightValue) =>
            MatchUnsafe(identity, _ => rightValue);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public Unit IfLeftUnsafe(Action<L> Left)
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
        public Unit IfRightUnsafe(Action<R> Right)
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
        public L IfRightUnsafe(L leftValue) =>
            MatchUnsafe(_ => leftValue, identity);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRightUnsafe(Func<L> Right) =>
            MatchUnsafe(_ => Right(), identity);

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public L IfRightUnsafe(Func<R, L> rightMap) =>
            MatchUnsafe(r => rightMap(r), identity);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherUnsafeUnitContext<L, R> Right(Action<R> right) =>
            new EitherUnsafeUnitContext<L, R>(this, right);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherUnsafeContext<L, R, Ret> Right<Ret>(Func<R, Ret> right) =>
            new EitherUnsafeContext<L, R, Ret>(this, right);

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
        /// Convert the EitherUnsafe to an Either
        /// </summary>
        /// <returns>EitherUnsafe</returns>
        [Pure]
        public Either<L, R> ToEither() =>
            IsRight
                ? Either<L, R>.Right(RightValue)
                : Either<L, R>.Left(LeftValue);

        /// <summary>
        /// Convert the Either to an TryOption
        /// </summary>
        /// <returns>Some(Right) or None</returns>
        [Pure]
        public TryOption<R> ToTryOption()
        {
            var self = this;
            return TryOption(() =>
                self.IsRight
                    ? Some(self.RightValue)
                    : None);
        }

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
            !lhs.Equals(rhs);

        /// <summary>
        /// Override of the Or operator to be a Left coalescing operator
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, R> operator |(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            lhs.IsBottom || rhs.IsBottom
                ? lhs
                : lhs.IsRight
                    ? lhs
                    : rhs;

        /// <summary>
        /// Override of the True operator to return True if the Either is Right
        /// </summary>
        [Pure]
        public static bool operator true(EitherUnsafe<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsRight;

        /// <summary>
        /// Override of the False operator to return True if the Either is Left
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
        public bool Equals(EitherUnsafe<L, R> other) =>
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

        public bool IsSome => 
            IsRight;

        public bool IsNone =>
            IsLeft;

        [Pure]
        public bool IsUnsafe(Optional<R> a) => 
            true;

        [Pure]
        public bool IsSomeA(Optional<R> a) =>
            AsEither(a).IsRight;

        [Pure]
        public bool IsNoneA(Optional<R> a) =>
            AsEither(a).IsLeft;

        [Pure]
        public B Match<B>(Optional<R> a, Func<R, B> Some, Func<B> None)
        {
            var ma = AsEither(a);
            return ma.IsRight
                ? Some(ma.right)
                : None();
        }

        [Pure]
        public B MatchUnsafe<B>(Optional<R> a, Func<R, B> Some, Func<B> None)
        {
            var ma = AsEither(a);
            return ma.IsRight
                ? Some(ma.right)
                : None();
        }

        [Pure]
        public MonadPlus<R> Plus(MonadPlus<R> a, MonadPlus<R> b)
        {
            var ma = AsEither(a);
            return ma.IsRight
                ? a
                : b;
        }

        [Pure]
        public MonadPlus<R> Zero() => 
            Bottom;

        [Pure]
        public Monad<R> Return(R x, params R[] xs) =>
            Right(x);

        [Pure]
        public Monad<R> Return(IEnumerable<R> vs) =>
            vs.Match(
                ()      => Bottom,
                x       => Right(x),
                (x, xs) => Right(x));

        [Pure]
        public MB Bind<MB, B>(Monad<R> ma, Func<R, MB> f) where MB : struct, Monad<B>
        {
            var either = AsEither(ma);
            return either.IsRight
                ? f(either.right)
                : (MB)default(MB).Fail(either.left);
        }

        [Pure]
        public Monad<B> Bind<B>(Monad<R> ma, Func<R, Monad<B>> f)
        {
            var either = AsEither(ma);
            return either.IsRight
                ? f(either.right)
                : either.Map(_ => default(B));
        }

        [Pure]
        public Monad<R> Fail(Exception err = null) =>
            Bottom;

        [Pure]
        public Monad<R> Fail<F>(F err = default(F)) =>
            new EitherUnsafe<F, R>(err);

        [Pure]
        public Functor<B> Map<B>(Functor<R> fa, Func<R, B> f) =>
            AsEither(fa).Map(f);

        [Pure]
        public S Fold<S>(Foldable<R> fa, S state, Func<S, R, S> f) =>
            AsEither(fa).Fold(state, f);

        [Pure]
        public S FoldBack<S>(Foldable<R> fa, S state, Func<S, R, S> f) =>
            AsEither(fa).Fold(state, f);

        [Pure]
        public R1 MatchUntyped<R1>(Func<object, R1> Some, Func<R1> None) =>
            IsRight
                ? Some(right)
                : None();

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
        /// Iterate the Either
        /// action is invoked if in the Right state
        /// </summary>
        public Unit Iter(Action<R> action)
        {
            if (IsBottom)
            {
                return unit;
            }
            if (IsRight)
            {
                action(RightValue);
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
        [Pure]
        public bool ForAll(Func<R, bool> pred) =>
            IsBottom
                ? true
                : IsRight
                    ? pred(RightValue)
                    : true;

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="Right">Right predicate</param>
        /// <param name="Left">Left predicate</param>
        [Pure]
        public bool BiForAll(Func<R, bool> Right, Func<L, bool> Left) =>
            IsBottom
                ? true
                : IsRight
                    ? Right(RightValue)
                    : Left(LeftValue);

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
        public S Fold<S>(S state, Func<S, R, S> folder) =>
            IsBottom
                ? state
                : IsRight
                    ? folder(state, RightValue)
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
        public S BiFold<S>(S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            IsBottom
                ? state
                : IsRight
                    ? Right(state, RightValue)
                    : Left(state, LeftValue);

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
            IsBottom
                ? false
                : IsRight
                    ? pred(RightValue)
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
        public bool BiExists(Func<R, bool> Right, Func<L, bool> Left) =>
            IsBottom
                ? false
                : IsLeft
                    ? Left(LeftValue)
                    : Right(RightValue);

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
        public EitherUnsafe<L, Ret> Map<Ret>(Func<R, Ret> mapper) =>
            IsBottom
                ? EitherUnsafe<L, Ret>.Bottom
                : IsRight
                    ? EitherUnsafe<L, Ret>.Right(mapper(RightValue))
                    : EitherUnsafe<L, Ret>.Left(LeftValue);

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
        public EitherUnsafe<Ret, R> MapLeft<Ret>(Func<L, Ret> mapper) =>
            IsBottom
                ? EitherUnsafe<Ret, R>.Bottom
                : IsLeft
                    ? EitherUnsafe<Ret, R>.Left(mapper(LeftValue))
                    : EitherUnsafe<Ret, R>.Right(RightValue);

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
        public EitherUnsafe<LRet, RRet> BiMap<LRet, RRet>(Func<R, RRet> Right, Func<L, LRet> Left) =>
            IsBottom
                ? EitherUnsafe<LRet, RRet>.Bottom
                : IsRight
                    ? EitherUnsafe<LRet, RRet>.Right(Right(RightValue))
                    : EitherUnsafe<LRet, RRet>.Left(Left(LeftValue));

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
        public EitherUnsafe<L, Ret> Bind<Ret>(Func<R, EitherUnsafe<L, Ret>> binder) =>
            IsBottom
                ? EitherUnsafe<L, Ret>.Bottom
                : IsRight
                    ? binder(RightValue)
                    : EitherUnsafe<L, Ret>.Left(LeftValue);

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
        public EitherUnsafe<LRet, RRet> BiBind<LRet, RRet>(Func<R, EitherUnsafe<LRet, RRet>> Right, Func<L, EitherUnsafe<LRet, RRet>> Left) =>
            IsBottom
                ? EitherUnsafe<LRet, RRet>.Bottom
                : IsLeft
                    ? Left(LeftValue)
                    : Right(RightValue);

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
        public EitherUnsafe<L, R> Filter(Func<R, bool> pred) =>
            IsBottom
                ? this
                : MatchUnsafe(
                    Right: t => pred(t) ? Right(t) : Bottom,
                    Left:  l => Left(l));

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
        public EitherUnsafe<L, R> BiFilter(Func<R, bool> Right, Func<L, bool> Left) =>
            IsBottom
                ? this
                : MatchUnsafe(
                    Right: r => Right(r) ? EitherUnsafe<L, R>.Right(r) : Bottom,
                    Left: l => Left(l)   ? EitherUnsafe<L, R>.Left(l) : Bottom);


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
        public EitherUnsafe<L, R> Where(Func<R, bool> pred) =>
            Filter(pred);

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
        public EitherUnsafe<L, U> Select<U>(Func<R, U> map) =>
            IsBottom
                ? EitherUnsafe<L, U>.Bottom
                : MatchUnsafe(
                    Right: t => EitherUnsafe<L, U>.Right(map(t)),
                    Left: l  => EitherUnsafe<L, U>.Left(l));

        /// <summary>
        /// Monadic bind function
        /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
        /// </summary>
        /// <returns>Bound Either</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EitherUnsafe<L, V> SelectMany<U, V>(Func<R, EitherUnsafe<L, U>> bind, Func<R, U, V> project)
        {
            if (IsBottom) return EitherUnsafe<L, V>.Bottom;
            if (IsLeft) return EitherUnsafe<L, V>.Left(LeftValue);
            var u = bind(RightValue);
            if (u.IsBottom) return EitherUnsafe<L, V>.Bottom;
            if (u.IsLeft) return EitherUnsafe<L, V>.Left(u.LeftValue);
            return project(RightValue, u.RightValue);
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<V> SelectMany<U, V>(
            Func<R, IEnumerable<U>> bind,
            Func<R, U, V> project
            )
        {
            if (IsBottom) return new V[0];
            if (IsLeft) return new V[0];
            var r = RightValue;
            return bind(r).Map(u => project(r, u));
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EitherUnsafe<L, V> Join<U, K, V>(
            EitherUnsafe<L, U> inner,
            Func<R, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<R, U, V> project)
        {
            if (IsLeft) return EitherUnsafe<L, V>.Left(LeftValue);
            if (inner.IsLeft) return EitherUnsafe<L, V>.Left(inner.LeftValue);
            if (IsBottom || inner.IsBottom) return EitherUnsafe<L, V>.Bottom;
            return EqualityComparer<K>.Default.Equals(outerKeyMap(RightValue), innerKeyMap(inner.RightValue))
                ? EitherUnsafe<L, V>.Right(project(RightValue, inner.RightValue))
                : EitherUnsafe<L, V>.Bottom;
        }

        EitherUnsafe<L, R> AsEither(Optional<R> x) => (EitherUnsafe<L, R>)x;
        EitherUnsafe<L, R> AsEither(Foldable<R> x) => (EitherUnsafe<L, R>)x;
        EitherUnsafe<L, R> AsEither(Functor<R> x) => (EitherUnsafe<L, R>)x;
        EitherUnsafe<L, R> AsEither(Monad<R> x) => (EitherUnsafe<L, R>)x;
        EitherUnsafe<L, R> AsEither(MonadPlus<R> x) => (EitherUnsafe<L, R>)x;
        EitherUnsafe<L, R> AsEither(Choice<L, R> x) => (EitherUnsafe<L, R>)x;

        public bool IsUnsafe(Choice<L, R> a) => 
            false;

        public bool IsChoice1(Choice<L, R> a) =>
            AsEither(a).IsLeft;

        public bool IsChoice2(Choice<L, R> a) =>
            AsEither(a).IsRight;

        public C Match<C>(Choice<L, R> a, Func<L, C> Choice1, Func<R, C> Choice2) =>
            AsEither(a).MatchUnsafe(Choice2, Choice1);

        public C MatchUnsafe<C>(Choice<L, R> a, Func<L, C> Choice1, Func<R, C> Choice2) =>
            AsEither(a).MatchUnsafe(Choice2, Choice1);
    }

    /// <summary>
    /// Context for the fluent Either matching
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
