using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;
using System.Collections.Immutable;

namespace LanguageExt
{
    public struct EitherUnsafe<R, L> : IEnumerable<R>
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

        internal static EitherUnsafe<R, L> Right(R value) => 
            new EitherUnsafe<R, L>(value);

        internal static EitherUnsafe<R, L> Left(L value) => 
            new EitherUnsafe<R, L>(value);

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

        public static implicit operator EitherUnsafe<R, L>(R value) =>
            EitherUnsafe<R, L>.Right(value);

        public static implicit operator EitherUnsafe<R, L>(L value) =>
            EitherUnsafe<R, L>.Left(value);

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
            MatchUnsafe(identity<R>(), _ => None());

        public R FailureUnsafe(R noneValue) =>
            MatchUnsafe(identity<R>(), _ => noneValue);

        public EitherUnsafeContext<R, L, Ret> Right<Ret>(Func<R, Ret> rightHandler) =>
            new EitherUnsafeContext<R, L, Ret>(this, rightHandler);

        public override string ToString() =>
            IsRight
                ? RightValue == null
                    ? "[R:null]"
                    : RightValue.ToString()
                : LeftValue == null
                    ? "[L:null]"
                    :  LeftValue.ToString();

        public override int GetHashCode() =>
            IsRight
                ? RightValue == null
                    ? 0
                    : RightValue.GetHashCode()
                : LeftValue == null
                    ? 0
                    : LeftValue.GetHashCode();

        public override bool Equals(object obj) =>
            IsRight
                ? RightValue == null
                    ? false
                    : RightValue.Equals(obj)
                : LeftValue == null
                    ? false
                    : LeftValue.Equals(obj);

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

        public EitherUnsafe<Ret, L> MapUnsafe<Ret>(Func<R, Ret> mapper) =>
            IsRight
                ? mapper(RightValue)
                : EitherUnsafe<Ret, L>.Left(LeftValue);

        public EitherUnsafe<Ret, L> BindUnsafe<Ret>(Func<R, EitherUnsafe<Ret, L>> binder) =>
            IsRight
                ? binder(RightValue)
                : EitherUnsafe<Ret, L>.Left(LeftValue);

        public IImmutableList<R> ToList() =>
            Prelude.toList(AsEnumerable());

        public ImmutableArray<R> ToArray() =>
            Prelude.toArray(AsEnumerable());

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

    public struct EitherUnsafeContext<R, L, Ret>
    {
        EitherUnsafe<R, L> either;
        Func<R, Ret> rightHandler;

        internal EitherUnsafeContext(EitherUnsafe<R,L> either, Func<R, Ret> rightHandler)
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
    public static EitherUnsafe<UR, L> Select<TR, UR, L>(this EitherUnsafe<TR, L> self, Func<TR, UR> map) =>
        matchUnsafe(self,
            Right: t => EitherUnsafe<UR, L>.Right(map(t)),
            Left: l => EitherUnsafe<UR, L>.Left(l)
            );

    public static EitherUnsafe<VR, L> SelectMany<TR, UR, VR, L>(this EitherUnsafe<TR, L> self,
        Func<TR, EitherUnsafe<UR, L>> bind,
        Func<TR, UR, VR> project
        ) =>
        matchUnsafe(self,
            Right: t =>
                matchUnsafe(bind(t),
                    Right: u => EitherUnsafe<VR, L>.Right(project(t, u)),
                    Left: l => EitherUnsafe<VR, L>.Left(l)
                ),
            Left: l => EitherUnsafe<VR, L>.Left(l)
            );

    public static bool Where<R, L>(this EitherUnsafe<R, L> self, Func<R, bool> pred) =>
        self.ExistsUnsafe(pred);
}
