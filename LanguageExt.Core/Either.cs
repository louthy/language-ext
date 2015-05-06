using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;

namespace LanguageExt
{
    public struct Either<L, R> : IEnumerable<R>
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

        internal static Either<L, R> Right(R value) => 
            new Either<L, R>(value);

        internal static Either<L, R> Left(L value) => 
            new Either<L, R>(value);

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

        public static implicit operator Either<L, R>(R value) =>
            value == null
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Right(value);

        public static implicit operator Either<L, R>(L value) =>
            value == null
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Left(value);


        private T CheckNullReturn<T>(T value, string location) =>
            value == null
                ? raise<T>(new ResultIsNullException("'"+location+"' result is null.  Not allowed."))
                : value;


        public Ret Match<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            IsRight
                ? CheckNullReturn(Right(RightValue),"Right")
                : CheckNullReturn(Left(LeftValue),"Left");

        public Unit Match(Action<R> Right, Action<L> Left)
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

        public R Failure(Func<R> None) => 
            Match(identity, _ => None());

        public R Failure(R noneValue) => 
            Match(identity, _ => noneValue);

        public EitherContext<R, L, Ret> Right<Ret>(Func<R, Ret> rightHandler) =>
            new EitherContext<R, L, Ret>(this, rightHandler);

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
                : value;

        public int Count =>
            IsRight ? 1 : 0;

        public bool ForAll(Func<R, bool> pred) =>
            IsRight
                ? pred(RightValue)
                : true;

        public S Fold<S>(S state, Func<S, R, S> folder) =>
            IsRight
                ? folder(state, RightValue)
                : state;

        public bool Exists(Func<R, bool> pred) =>
            IsRight
                ? pred(RightValue)
                : false;

        public Either<L, Ret> Map<Ret>(Func<R, Ret> mapper) =>
            IsRight
                ? CastRight<L, Ret>(mapper(RightValue))
                : Either<L, Ret>.Left(LeftValue);

        public bool Filter(Func<R, bool> pred) =>
            Exists(pred);

        public Either<L, Ret> Bind<Ret>(Func<R, Either<L, Ret>> binder) =>
            IsRight
                ? binder(RightValue)
                : Either<L, Ret>.Left(LeftValue);

        public IImmutableList<R> ToList() =>
            toList(AsEnumerable());

        public ImmutableArray<R> ToArray() =>
            toArray(AsEnumerable());

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

        public static bool operator ==(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Either<L, R> lhs, Either<L, R> rhs) =>
            !lhs.Equals(rhs);

        public static Either<L, R> operator |(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.IsRight
                ? lhs
                : rhs;

        public static bool operator true(Either<L, R> value) =>
            value.IsRight;

        public static bool operator false(Either<L, R> value) =>
            value.IsLeft;

        private static Either<NL, NR> CastRight<NL, NR>(NR right) =>
            right == null
                ? raise<Either<NL, NR>>(new ValueIsNullException())
                : Either<NL, NR>.Right(right);

        private static Either<NL, NR> CastLeft<NL, NR>(NL left) =>
            left == null
                ? raise<Either<NL, NR>>(new ValueIsNullException())
                : Either<NL, NR>.Left(left);
    }

    public struct EitherContext<R, L, Ret>
    {
        readonly Either<L, R> either;
        readonly Func<R, Ret> rightHandler;

        internal EitherContext(Either<L, R> either, Func<R, Ret> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Ret Left(Func<L, Ret> leftHandler)
        {
            return match(either, rightHandler, leftHandler);
        }
    }
}

public static class __EitherExt
{
    public static Either<L, UR> Select<L, TR, UR>(this Either<L, TR> self, Func<TR, UR> map) =>
        match(self,
            Right: t => Either<L, UR>.Right(map(t)),
            Left: l => Either<L, UR>.Left(l)
            );

    public static Either<L, VR> SelectMany<L, TR, UR, VR>(this Either<L, TR> self,
        Func<TR, Either<L, UR>> bind,
        Func<TR, UR, VR> project
        ) =>
        match(self,
            Right: t =>
                match(bind(t),
                    Right: u => Either<L, VR>.Right(project(t, u)),
                    Left: l => Either<L, VR>.Left(l)
                ),
            Left: l => Either<L, VR>.Left(l)
            );
}
