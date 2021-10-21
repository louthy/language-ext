using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace LanguageExt
{
    public readonly struct EitherLeft<L> :
        IEquatable<EitherLeft<L>>,
        IEquatable<L>,
        IComparable<EitherLeft<L>>,
        IComparable<L>,
        IEither,
        ISerializable
    {
        internal readonly L Value;

        internal EitherLeft(L value) => 
            Check.NullReturn(Value = value);

        EitherLeft(SerializationInfo info, StreamingContext context)
        {
            var state = (EitherStatus)info.GetValue("State", typeof(EitherStatus));
            switch (state)
            {
                case EitherStatus.IsLeft:
                    Value = (L)info.GetValue("Left", typeof(L));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("State", EitherStatus.IsLeft);
            if (IsRight) info.AddValue("Left", Value);
        }

        public EitherLeft<L> Select(Func<L, L> f) =>
            new EitherLeft<L>(f(Value));

        public EitherLeft<L> SelectMany(Func<L, EitherLeft<L>> f) =>
            f(Value);

        public Either<L, R> SelectMany<R, B>(Func<L, EitherLeft<R>> f) =>
            Left<L, R>(Value);

        public EitherLeft<C> SelectMany<C>(Func<L, EitherLeft<L>> bind, Func<L, Unit, C> project) =>
            new EitherLeft<C>(project(Value, unit));

        public Either<L, C> SelectMany<B, C>(Func<L, EitherLeft<B>> bind, Func<L, B, C> project) =>
            Left<L, C>(Value);

        public Either<L, B> SelectMany<B>(Func<L, Either<L, B>> f) =>
            f(Value);

        public Either<L, C> SelectMany<B, C>(Func<L, Either<L, B>> bind, Func<L, B, C> project)
        {
            var self = this;
            return bind(Value).Map(b => project(self.Value, b));
        }

        [Pure]
        public override string ToString() =>
            isnull(Value)
                ? "Left(null)"
                : $"Left({Value})";

        [Pure]
        public override int GetHashCode() =>
            Value?.GetHashCode() ?? 0;

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) &&
            obj is EitherLeft<L> left &&
            Equals(left);

        [Pure]
        public Lst<L> ToList() =>
            List(Value);

        [Pure]
        public Arr<L> ToArray() =>
            Array(Value);

        [Pure]
        public Seq<L> ToSeq() =>
            Seq1(Value);

        [Pure]
        public IEnumerable<L> AsEnumerable() =>
            Seq1(Value).AsEnumerable();

        [Pure]
        public EitherUnsafe<L, R> ToEitherUnsafe<R>() =>
            LeftUnsafe<L, R>(Value);

        [Pure]
        public static implicit operator L(EitherLeft<L> ma) =>
            ma.Value;

        [Pure]
        public static bool operator <(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public static bool operator >(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        public static bool operator ==(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static EitherLeft<L> operator |(EitherLeft<L> lhs, EitherLeft<L> rhs) =>
            lhs;

        [Pure]
        public static EitherRight<L> operator |(EitherRight<L> lhs, EitherLeft<L> rhs) =>
            lhs;

        [Pure]
        public static EitherRight<L> operator |(EitherLeft<L> lhs, EitherRight<L> rhs) =>
            rhs;

        [Pure]
        public static bool operator true(EitherLeft<L> value) =>
            false;

        [Pure]
        public static bool operator false(EitherLeft<L> value) =>
            true;

        [Pure]
        public int CompareTo(EitherLeft<L> other) =>
            default(OrdDefault<L>).Compare(Value, other.Value);

        [Pure]
        public int CompareTo(L other) =>
            default(OrdDefault<L>).Compare(Value, other);

        [Pure]
        public bool Equals(L other) =>
            default(EqDefault<L>).Equals(Value, other);

        [Pure]
        public bool Equals(EitherLeft<L> other) =>
            default(EqDefault<L>).Equals(Value, other.Value);

        [Pure]
        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            Left(Value);

        [Pure]
        public Type GetUnderlyingRightType() =>
            typeof(Unit);

        [Pure]
        public Type GetUnderlyingLeftType() =>
            typeof(L);

        [Pure]
        public bool IsRight =>
            false;

        [Pure]
        public bool IsLeft =>
            true;

        [Pure]
        public Either<L, R> Bind<R>() =>
            Either<L, R>.Left(Value);
    }
}
