using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;

namespace LanguageExt
{
    public struct EitherUnsafe<L, R> : IEnumerable<R>
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
            EitherUnsafe<L, R>.Right(value);

        public static implicit operator EitherUnsafe<L, R>(L value) =>
            EitherUnsafe<L, R>.Left(value);

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

        public R FailureUnsafe(Func<R> None) =>
            MatchUnsafe(identity, _ => None());

        public R FailureUnsafe(R noneValue) =>
            MatchUnsafe(identity, _ => noneValue);

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

        public IImmutableList<R> ToList() =>
            Prelude.toList(AsEnumerable());

        public ImmutableArray<R> ToArray() =>
            Prelude.toArray(AsEnumerable());

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

        public IEnumerator<R> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

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
}

public static class __EitherUnsafeExt
{
    public static EitherUnsafe<L, UR> Select<L, TR, UR>(this EitherUnsafe<L, TR> self, Func<TR, UR> map) =>
        matchUnsafe(self,
            Right: t => EitherUnsafe<L, UR>.Right(map(t)),
            Left: l => EitherUnsafe<L, UR>.Left(l)
            );

    public static EitherUnsafe<L, VR> SelectMany<L, TR, UR, VR>(this EitherUnsafe<L, TR> self,
        Func<TR, EitherUnsafe<L, UR>> bind,
        Func<TR, UR, VR> project
        ) =>
        matchUnsafe(self,
            Right: t =>
                matchUnsafe(bind(t),
                    Right: u => EitherUnsafe<L, VR>.Right(project(t, u)),
                    Left: l => EitherUnsafe<L, VR>.Left(l)
                ),
            Left: l => EitherUnsafe<L, VR>.Left(l)
            );
}
