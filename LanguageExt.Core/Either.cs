using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public struct Either<R, L>
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

        public Either(R right)
        {
            this.state = EitherState.IsRight;
            this.right = right;
            this.left = default(L);
        }

        public Either(L left)
        {
            this.state = EitherState.IsLeft;
            this.right = default(R);
            this.left = left;
        }

        public static Either<R, L> Right(R value) => 
            new Either<R, L>(value);

        public static Either<R, L> Left(L value) => 
            new Either<R, L>(value);

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

        public static implicit operator Either<R, L>(R value) => 
            Either<R, L>.Right(value);

        public static implicit operator Either<R, L>(L value) => 
            Either<R, L>.Left(value);

        private static T CheckNullReturn<T>(T value, string location) =>
            value == null
                ? raise<T>(new ResultIsNullException("'\{location}' result is null.  Not allowed."))
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
            Match(identity<R>(), _ => None());

        public R Failure(R noneValue) => 
            Match(identity<R>(), _ => noneValue);

        public EitherContext<R, L, Ret> Right<Ret>(Func<R, Ret> rightHandler) =>
            new EitherContext<R, L, Ret>(this, rightHandler);

        public override string ToString() =>
            IsRight
                ? RightValue.ToString()
                : LeftValue.ToString();

        public override int GetHashCode() =>
            IsRight
                ? RightValue.GetHashCode()
                : LeftValue.GetHashCode();

        public override bool Equals(object obj) =>
            IsRight
                ? RightValue.Equals(obj)
                : LeftValue.Equals(obj);

        private U CheckInitialised<U>(U value) =>
            state == EitherState.IsUninitialised
                ? raise<U>(new EitherNotInitialisedException())
                : value;
    }

    public struct EitherContext<R, L, Ret>
    {
        Either<R, L> either;
        Func<R, Ret> rightHandler;

        internal EitherContext(Either<R,L> either, Func<R, Ret> rightHandler)
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
    public static Either<UR, L> Select<TR, UR, L>(this Either<TR, L> self, Func<TR, UR> map) =>
        match(self,
            Right: t => Either<UR, L>.Right(map(t)),
            Left: l => Either<UR, L>.Left(l)
            );

    public static Either<VR, L> SelectMany<TR, UR, VR, L>(this Either<TR, L> self,
        Func<TR, Either<UR, L>> bind,
        Func<TR, UR, VR> project
        ) =>
        match(self,
            Right: t =>
                match(bind(t),
                    Right: u => Either<VR, L>.Right(project(t, u)),
                    Left: l => Either<VR, L>.Left(l)
                ),
            Left: l => Either<VR, L>.Left(l)
            );

    public static IEnumerable<R> AsEnumerable<R, L>(this Either<R, L> self)
    {
        if (self.IsRight)
        {
            while (true)
            {
                yield return self.RightValue;
            }
        }
    }

    public static IEnumerable<R> AsEnumerableOne<R, L>(this Either<R, L> self)
    {
        if (self.IsRight)
        {
            yield return self.RightValue;
        }
    }
}
