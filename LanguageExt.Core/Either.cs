using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public struct Either<R, L> : IEnumerable<R>
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

        internal static Either<R, L> Right(R value) => 
            new Either<R, L>(value);

        internal static Either<R, L> Left(L value) => 
            new Either<R, L>(value);

        internal static Either<R, L> RightUnsafe(R value) =>
            new Either<R, L>(value);

        internal static Either<R, L> LeftUnsafe(L value) =>
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
            value == null
                ? raise<Either<R, L>>(new ValueIsNullException())
                : Either<R, L>.Right(value);

        public static implicit operator Either<R, L>(L value) =>
            value == null
                ? raise<Either<R, L>>(new ValueIsNullException())
                : Either<R, L>.Left(value);


        private T CheckNullReturn<T>(T value, string location) =>
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

        public bool ForAll(Func<R,bool> pred) =>
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

        public Either<Ret,L> Map<Ret>(Func<R, Ret> mapper) =>
            IsRight
                ? CastRight<Ret, L>(mapper(RightValue))
                : Either<Ret, L>.Left(LeftValue);

        public Either<Ret,L> Bind<Ret>(Func<R, Either<Ret,L>> binder) =>
            IsRight
                ? binder(RightValue)
                : Either<Ret, L>.Left(LeftValue);

        public List<R> ToList() =>
            AsEnumerable().ToList();

        public R[] ToArray() =>
            AsEnumerable().ToArray();

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

        private static Either<NR, NL> CastRight<NR, NL>(NR right) =>
            right == null
                ? raise<Either<NR, NL>>(new ValueIsNullException())
                : Either<NR, NL>.Right(right);

        private static Either<NR, NL> CastLeft<NR, NL>(NL left) =>
            left == null
                ? raise<Either<NR, NL>>(new ValueIsNullException())
                : Either<NR, NL>.Left(left);
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

    public static bool Where<R, L>(this Either<R, L> self, Func<R, bool> pred) =>
        self.Exists(pred);
}
