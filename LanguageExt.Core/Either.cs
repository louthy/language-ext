using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;
using System.Collections.Immutable;

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
            obj is Either<R, L>
                ? map(this, (Either<R, L>)obj, (lhs, rhs) =>
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

        public bool Filter(Func<R, bool> pred) =>
            Exists(pred);

        public Either<Ret,L> Bind<Ret>(Func<R, Either<Ret,L>> binder) =>
            IsRight
                ? binder(RightValue)
                : Either<Ret, L>.Left(LeftValue);

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

        public static bool operator ==(Either<R, L> lhs, Either<R, L> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Either<R, L> lhs, Either<R, L> rhs) =>
            !lhs.Equals(rhs);

        public static Either<R, L> operator |(Either<R, L> lhs, Either<R, L> rhs) =>
            lhs.IsRight
                ? lhs
                : rhs;

        public static bool operator true(Either<R, L> value) =>
            value.IsRight;

        public static bool operator false(Either<R, L> value) =>
            value.IsLeft;

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
        readonly Either<R, L> either;
        readonly Func<R, Ret> rightHandler;

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
}
