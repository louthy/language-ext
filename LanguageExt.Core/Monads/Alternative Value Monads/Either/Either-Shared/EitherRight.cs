using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace LanguageExt
{
    [Serializable]
    public readonly struct EitherRight<R> : 
        IEquatable<EitherRight<R>>, 
        IEquatable<R>, 
        IComparable<EitherRight<R>>, 
        IComparable<R>, 
        IEither,
        ISerializable
    {
        public readonly R Value;

        internal EitherRight(R value) => Value = value;

        EitherRight(SerializationInfo info, StreamingContext context)
        {
            var state = (EitherStatus)info.GetValue("State", typeof(EitherStatus));
            switch (state)
            {
                case EitherStatus.IsRight:
                    Value = (R)info.GetValue("Right", typeof(R));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("State", EitherStatus.IsRight);
            if (IsRight) info.AddValue("Right", Value);
        }

        [Pure]
        public bool IsRight => true;

        [Pure]
        public bool IsLeft => false;

        [Pure]
        public EitherRight<B> Select<B>(Func<R, B> f) =>
            new EitherRight<B>(f(Value));

        [Pure]
        public EitherRight<B> SelectMany<B>(Func<R, EitherRight<B>> f) =>
            f(Value);

        [Pure]
        public Either<L, B> SelectMany<L, B>(Func<R, EitherLeft<L>> f) =>
            f(Value);

        [Pure]
        public EitherRight<C> SelectMany<B, C>(Func<R, EitherRight<B>> bind, Func<R, B, C> project) =>
            new EitherRight<C>(project(Value, bind(Value).Value));

        [Pure]
        public Either<L, C> SelectMany<L, C>(Func<R, EitherLeft<L>> bind, Func<R, Unit, C> project) =>
            new EitherLeft<L>(bind(Value).Value);

        [Pure]
        public Either<L, B> SelectMany<L, B>(Func<R, Either<L, B>> f) =>
            f(Value);

        [Pure]
        public Either<L, C> SelectMany<L, B, C>(Func<R, Either<L, B>> bind, Func<R, B, C> project)
        {
            var self = this;
            return bind(Value).Map(b => project(self.Value, b));
        }

        [Pure]
        public static implicit operator R(EitherRight<R> right) =>
            right.Value;

        [Pure]
        public static bool operator <(EitherRight<R> lhs, EitherRight<R> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(EitherRight<R> lhs, EitherRight<R> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public static bool operator >(EitherRight<R> lhs, EitherRight<R> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(EitherRight<R> lhs, EitherRight<R> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        public static bool operator ==(EitherRight<R> lhs, EitherRight<R> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(EitherRight<R> lhs, EitherRight<R> rhs) =>
            !(lhs == rhs);

        [Pure]
        public override string ToString() =>
            isnull(Value)
                ? "Right(null)"
                : $"Right({Value})";

        [Pure]
        public override int GetHashCode() =>
            Value?.GetHashCode() ?? 0;

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) &&
            obj is EitherRight<R> other &&
            Equals(other);

        [Pure]
        public Lst<R> ToList() =>
            List(Value);

        [Pure]
        public Arr<R> ToArray() =>
            Array(Value);

        [Pure]
        public Seq<R> ToSeq() =>
            Seq1(Value);

        [Pure]
        public IEnumerable<R> AsEnumerable() =>
            new[] { Value };

        [Pure]
        public Option<R> ToOption() =>
            Optional(Value);

        [Pure]
        public TryOption<R> ToTryOption() =>
            TryOption(Value);

        [Pure]
        public static EitherRight<R> operator |(EitherRight<R> lhs, EitherRight<R> rhs) =>
            lhs;

        [Pure]
        public static EitherRight<R> operator |(EitherRight<R> lhs, EitherLeft<R> rhs) =>
            lhs;

        [Pure]
        public static EitherRight<R> operator |(EitherLeft<R> lhs, EitherRight<R> rhs) =>
            rhs;

        [Pure]
        public static bool operator true(EitherRight<R> value) =>
            true;

        [Pure]
        public static bool operator false(EitherRight<R> value) =>
            false;

        [Pure]
        public int CompareTo(EitherRight<R> other) =>
            default(OrdDefault<R>).Compare(Value, other.Value);

        [Pure]
        public int CompareTo(R other) =>
            default(OrdDefault<R>).Compare(Value, other);

        [Pure]
        public bool Equals(R other) =>
            default(EqDefault<R>).Equals(Value, other);

        [Pure]
        public bool Equals(EitherRight<R> other) =>
            default(EqDefault<R>).Equals(Value, other.Value);

        [Pure]
        public Type GetUnderlyingType() =>
            typeof(R);

        [Pure]
        public int Count() =>
            1;

        public Unit Iter(Action<R> Right)
        {
            Right(Value);
            return unit;
        }

        [Pure]
        public bool ForAll(Func<R, bool> Right) =>
            Right(Value);

        [Pure]
        public S Fold<S>(S state, Func<S, R, S> Right) =>
            Right(state, Value);

        [Pure]
        public bool Exists(Func<R, bool> pred) =>
            pred(Value);

        [Pure]
        public EitherRight<Ret> Map<Ret>(Func<R, Ret> mapper) =>
            new EitherRight<Ret>(mapper(Value));

        [Pure]
        public EitherRight<Ret> Bind<Ret>(Func<R, EitherRight<Ret>> f) =>
            f(Value);

        [Pure]
        public Either<L, R> Bind<L>() =>
            Either<L, R>.Right(Value);

        [Pure]
        public Either<L, R> Bind<L>(Func<R, EitherLeft<L>> f) =>
            Either<L, R>.Left(f(Value).Value);

        [Pure]
        public Either<L, B> Bind<L, B>(Func<R, Either<L, B>> f) =>
            f(Value);

        [Pure]
        public EitherUnsafe<L, B> Bind<L, B>(Func<R, EitherUnsafe<L, B>> f) =>
            f(Value);

        [Pure]
        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            Right(Value);

        [Pure]
        public Type GetUnderlyingRightType() =>
            typeof(R);

        [Pure]
        public Type GetUnderlyingLeftType() =>
            typeof(Unit);
    }
}
