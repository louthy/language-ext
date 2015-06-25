using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;
using System.ComponentModel;

namespace LanguageExt
{
    public struct EitherUnsafe<L, R> :
        IEither,
        IComparable<EitherUnsafe<L, R>>,
        IComparable<R>,
        IEquatable<EitherUnsafe<L, R>>,
        IEquatable<R>
    {
        enum EitherState : byte
        {
            IsUninitialised= 0,
            IsLeft = 1,
            IsRight = 2
        }

        readonly EitherState state;
        readonly R right;
        readonly L left;

        private EitherUnsafe(R right)
        {
            this.state = EitherState.IsRight;
            this.right = right;
            this.left = default(L);
        }

        private EitherUnsafe(L left)
        {
            this.state = EitherState.IsLeft;
            this.right = default(R);
            this.left = left;
        }

        internal static EitherUnsafe<L, R> Right(R value) => 
            new EitherUnsafe<L, R>(value);

        internal static EitherUnsafe<L, R> Left(L value) => 
            new EitherUnsafe<L, R>(value);

        public bool IsRight =>
            CheckInitialised(state == EitherState.IsRight);

        public bool IsLeft =>
            CheckInitialised(state == EitherState.IsLeft);

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

        public static implicit operator EitherUnsafe<L, R>(R value) =>
            Right(value);

        public static implicit operator EitherUnsafe<L, R>(L value) =>
            Left(value);

        public Ret MatchUnsafe<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

        public Unit MatchUnsafe(Action<R> Right, Action<L> Left)
        {
            if (IsRight)
            {
                Right(RightValue);
            }
            else
            {
                Left(LeftValue);
            }
            return unit;
        }

        [Obsolete("'FailureUnsafe' has been deprecated.  Please use 'IfLeftUnsafe' instead")]
        public R FailureUnsafe(Func<R> None) =>
            MatchUnsafe(identity, _ => None());

        [Obsolete("'FailureUnsafe' has been deprecated.  Please use 'IfLeftUnsafe' instead")]
        public R FailureUnsafe(R noneValue) =>
            MatchUnsafe(identity, _ => noneValue);

        /// <summary>
        /// Matches on Left only.  Therefore Right is an identity function. 
        /// </summary>
        public R IfLeftUnsafe(Func<R> Left) =>
            MatchUnsafe(identity, _ => Left());

        /// <summary>
        /// Matches on Left only.  Therefore Right is an identity function. 
        /// </summary>
        public R IfLeftUnsafe(R leftValue) =>
            MatchUnsafe(identity, _ => leftValue);

        /// <summary>
        /// Invokes the rightHandler if EitherUnsafe is in the Right state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfRightUnsafe(Action<R> rightHandler)
        {
            if (IsRight)
            {
                rightHandler(right);
            }
            return unit;
        }

        public EitherUnsafeUnitContext<L, R> Right(Action<R> rightHandler) =>
            new EitherUnsafeUnitContext<L, R>(this, rightHandler);

        public EitherUnsafeContext<L, R, Ret> Right<Ret>(Func<R, Ret> rightHandler) =>
            new EitherUnsafeContext<L, R, Ret>(this, rightHandler);

        public override string ToString() =>
            IsRight
                ? RightValue == null
                    ? "Right(null)"
                    : String.Format("Right({0})", RightValue)
                : LeftValue == null
                    ? "Left(null)"
                    : String.Format("Left({0})", LeftValue);

        public override int GetHashCode() =>
            IsRight
                ? RightValue == null
                    ? 0
                    : RightValue.GetHashCode()
                : LeftValue == null
                    ? 0
                    : LeftValue.GetHashCode();


        public override bool Equals(object obj) =>
            obj is EitherUnsafe<L, R>
                ? map(this, (EitherUnsafe<L, R>)obj, (lhs, rhs) =>
                      lhs.IsLeft && rhs.IsLeft
                          ? lhs.LeftValue == null 
                                ? rhs.LeftValue == null
                                : lhs.LeftValue.Equals(rhs.LeftValue)
                          : lhs.IsLeft || rhs.IsLeft
                              ? false
                              : lhs.RightValue == null
                                    ? rhs.RightValue == null
                                    : lhs.RightValue.Equals(rhs.RightValue))
                : false;

        private U CheckInitialised<U>(U value) =>
            state == EitherState.IsUninitialised
                ? raise<U>(new EitherNotInitialisedException())
                : value;

        public int Count =>
            IsRight ? 1 : 0;

        public bool ForAllUnsafe(Func<R, bool> pred) =>
            IsRight
                ? pred(RightValue)
                : true;

        public S FoldUnsafe<S>(S state, Func<S, R, S> folder) =>
            IsRight
                ? folder(state, RightValue)
                : state;

        public bool ExistsUnsafe(Func<R, bool> pred) =>
            IsRight
                ? pred(RightValue)
                : false;

        public EitherUnsafe<L, Ret> MapUnsafe<Ret>(Func<R, Ret> mapper) =>
            IsRight
                ? mapper(RightValue)
                : EitherUnsafe<L, Ret>.Left(LeftValue);

        public bool FilterUnsafe(Func<R, bool> pred) =>
            ExistsUnsafe(pred);

        public EitherUnsafe<L, Ret> BindUnsafe<Ret>(Func<R, EitherUnsafe<L, Ret>> binder) =>
            IsRight
                ? binder(RightValue)
                : EitherUnsafe<L, Ret>.Left(LeftValue);

        public Lst<R> ToList() =>
            toList(AsEnumerable());

        public ImmutableArray<R> ToArray() =>
            toArray(AsEnumerable());

        public static bool operator ==(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            !lhs.Equals(rhs);

        public static EitherUnsafe<L, R> operator |(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            lhs.IsRight
                ? lhs
                : rhs;

        public static bool operator true(EitherUnsafe<L, R> value) =>
            value.IsRight;

        public static bool operator false(EitherUnsafe<L, R> value) =>
            value.IsLeft;

        public IEnumerable<R> AsEnumerable()
        {
            if (IsRight)
            {
                yield return RightValue;
            }
        }

        public int CompareTo(EitherUnsafe<L, R> other) =>
            IsLeft && other.IsLeft
                ? Comparer<L>.Default.Compare(LeftValue, other.LeftValue)
                : IsRight && other.IsRight
                    ? Comparer<R>.Default.Compare(RightValue, other.RightValue)
                    : IsLeft
                        ? -1
                        : 1;

        public int CompareTo(R other) =>
            IsRight
                ? Comparer<R>.Default.Compare(RightValue, other)
                : -1;

        public int CompareTo(L other) =>
            IsRight
                ? -1
                : Comparer<L>.Default.Compare(LeftValue, other);

        public bool Equals(R other) =>
            IsRight
                ? EqualityComparer<R>.Default.Equals(RightValue, other)
                : false;

        public bool Equals(L other) =>
            IsLeft
                ? EqualityComparer<L>.Default.Equals(LeftValue, other)
                : false;

        public bool Equals(EitherUnsafe<L, R> other) =>
            IsRight
                ? other.Equals(RightValue)
                : other.Equals(LeftValue);

        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

        public Type GetUnderlyingRightType() =>
            typeof(R);

        public Type GetUnderlyingLeftType() =>
            typeof(L);
    }

    public struct EitherUnsafeContext<L, R, Ret>
    {
        readonly EitherUnsafe<L, R> either;
        readonly Func<R, Ret> rightHandler;

        internal EitherUnsafeContext(EitherUnsafe<L, R> either, Func<R, Ret> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Ret Left(Func<L, Ret> leftHandler)
        {
            return matchUnsafe(either, rightHandler, leftHandler);
        }
    }

    public struct EitherUnsafeUnitContext<L, R>
    {
        readonly EitherUnsafe<L, R> either;
        readonly Action<R> rightHandler;

        internal EitherUnsafeUnitContext(EitherUnsafe<L, R> either, Action<R> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Unit Left(Action<L> leftHandler)
        {
            return matchUnsafe(either, rightHandler, leftHandler);
        }
    }
}

public static class __EitherUnsafeExt
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, UR> Select<L, TR, UR>(this EitherUnsafe<L, TR> self, Func<TR, UR> map) =>
        matchUnsafe(self,
            Right: t => EitherUnsafe<L, UR>.Right(map(t)),
            Left: l => EitherUnsafe<L, UR>.Left(l)
            );

    public static int Sum<L>(this EitherUnsafe<L, int> self) =>
        self.IsRight ? self.RightValue : 0;

    public static int Count<L, R>(this EitherUnsafe<L, R> self) =>
        self.IsRight ? 1 : 0;

    public static Unit Iter<L, R>(this EitherUnsafe<L, R> self, Action<R> action)
    {
        if (self.IsRight)
        {
            action(self.RightValue);
        }
        return unit;
    }

    public static bool ForAll<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(self.RightValue)
            : true;

    public static S Fold<L, R, S>(this EitherUnsafe<L, R> self, S state, Func<S, R, S> folder) =>
        self.IsRight
            ? folder(state, self.RightValue)
            : state;

    public static bool Exists<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        self.IsRight
            ? pred(self.RightValue)
            : false;

    public static EitherUnsafe<L, Ret> Map<L, R, Ret>(this EitherUnsafe<L, R> self, Func<R, Ret> mapper) =>
        self.IsRight
            ? RightUnsafe<L, Ret>(mapper(self.RightValue))
            : LeftUnsafe<L, Ret>(self.LeftValue);

    public static EitherUnsafe<L, Ret> Bind<L, R, Ret>(this EitherUnsafe<L, R> self, Func<R, EitherUnsafe<L, Ret>> binder) =>
        self.IsRight
            ? binder(self.RightValue)
            : EitherUnsafe<L, Ret>.Left(self.LeftValue);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, R> Where<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        Filter(self, pred);

    public static EitherUnsafe<L, R> Filter<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        matchUnsafe(self,
            Right: t => pred(t) ? EitherUnsafe<L, R>.Right(t) : EitherUnsafe<L, R>.Left(default(L)),
            Left: l => EitherUnsafe<L, R>.Left(l)
            );

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, V> SelectMany<L, T, U, V>(this EitherUnsafe<L, T> self, Func<T, EitherUnsafe<L, U>> bind, Func<T, U, V> project)
    {
        if (self.IsLeft) return EitherUnsafe<L, V>.Left(self.LeftValue);
        var u = bind(self.RightValue);
        if (u.IsLeft) return EitherUnsafe<L, V>.Left(u.LeftValue);
        return project(self.RightValue, u.RightValue);
    }
}
