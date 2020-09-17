using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.DataTypes.Serialisation;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Equivalent of `Either<Error, A>`
    /// Called `Fin` because it is expected to be used as the concrete result of a computation
    /// </summary>
    [Serializable]
    public struct Fin<A> : 
        IComparable<Fin<A>>, 
        IComparable, 
        IEquatable<Fin<A>>,
        IEnumerable<Fin<A>>,
        ISerializable
    {
        internal readonly EitherData<Error, A> data;

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal Fin(EitherData<Error, A> data) =>
            this.data = data;

        [Pure, MethodImpl(AffOpt.mops)]
        public static Fin<A> Succ(A value) => 
            value is null
                ? throw new ValueIsNullException(nameof(value))
                : new Fin<A>(EitherData.Right<Error, A>(value));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Fin<A> Fail(Error error) => 
            new Fin<A>(EitherData.Left<Error, A>(error));

        [Pure]
        public bool IsSucc
        {
            [MethodImpl(AffOpt.mops)]
            get => data.State == EitherStatus.IsRight;
        }

        [Pure]
        public bool IsFail
        {
            [MethodImpl(AffOpt.mops)]
            get => data.State == EitherStatus.IsLeft || data.State == EitherStatus.IsBottom;
        }

        [Pure]
        public bool IsBottom
        {
            [MethodImpl(AffOpt.mops)]
            get => data.State == EitherStatus.IsBottom;
        }
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static implicit operator Fin<A>(A value) =>
            Fin<A>.Succ(value);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static implicit operator Fin<A>(Error error) =>
            Fin<A>.Fail(error);

        [Pure, MethodImpl(AffOpt.mops)]
        public static explicit operator A(Fin<A> ma) =>
            ma.IsSucc
                ? ma.Value
                : throw new EitherIsNotRightException();

        [Pure, MethodImpl(AffOpt.mops)]
        public static explicit operator Error(Fin<A> ma) =>
            ma.IsFail
                ? ma.Error
                : throw new EitherIsNotLeftException();

        internal A Value 
        { 
            [MethodImpl(AffOpt.mops)]
            get => data.State == EitherStatus.IsBottom
                ? throw new BottomException()
                : data.Right;  
        }

        internal Error Error 
        { 
            [MethodImpl(AffOpt.mops)]
            get => data.State == EitherStatus.IsBottom 
                ? Error.Bottom 
                : data.Left;  
        }
        
        [Pure, MethodImpl(AffOpt.mops)]
        static Option<T> convert<T>(object value)
        {
            if (value == null)
            {
                return None;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return None;
            }
        }

        [Pure, MethodImpl(AffOpt.mops)]
        internal Fin<B> Cast<B>() =>
            new Fin<B>(
                IsSucc
                    ? convert<B>(Value)
                        .Map(EitherData.Right<Error, B>)
                        .IfNone(() => EitherData.Left<Error, B>(Error.New($"Can't cast success value of `Err<{nameof(A)}>` to `Err<{nameof(B)}>` ")))
                    : EitherData.Left<Error, B>(Error));

        public int CompareTo(Fin<A> other)
        {
            var cmp = data.State.CompareTo(other.data.State);
            if (cmp != 0) return 0;
            return data.State switch
            {
                EitherStatus.IsRight  => default(OrdDefault<A>).Compare(data.Right, other.data.Right),
                EitherStatus.IsLeft   => 0,
                EitherStatus.IsBottom => 0,
                _                     => throw new NotSupportedException()
            };
        }

        public bool Equals(Fin<A> other) =>
            data.State == other.data.State && data.State switch
            {
                EitherStatus.IsRight  => default(EqDefault<A>).Equals(data.Right, other.data.Right),
                EitherStatus.IsLeft   => true,
                EitherStatus.IsBottom => true,
                _                     => throw new NotSupportedException()
            };

        public IEnumerator<Fin<A>> GetEnumerator()
        {
            if (IsSucc)
            {
                yield return Value;
            }
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public override string ToString() =>
            IsSucc
                ? $"Succ({Value})"
                : $"Fail({Error})";

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("State", data.State);
            if (data.State == EitherStatus.IsRight) info.AddValue("Succ", data.Right);
            if (data.State == EitherStatus.IsLeft) info.AddValue("Fail", data.Left);
        }
        
        Fin(SerializationInfo info, StreamingContext context)
        {
            var state = (EitherStatus)info.GetValue("State", typeof(EitherStatus));
            switch(state)
            {
                case EitherStatus.IsBottom:
                    data = default;
                    break;
                case EitherStatus.IsRight:
                    data = EitherData.Right<Error, A>((A)info.GetValue("Succ", typeof(A)));
                    break;
                case EitherStatus.IsLeft:
                    data = EitherData.Left<Error, A>((Error)info.GetValue("Fail", typeof(Error)));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        [Pure, MethodImpl(AffOpt.mops)]
        public int CompareTo(object obj) =>
            obj is Fin<A> t ? CompareTo(t) : 1;

        [Pure, MethodImpl(AffOpt.mops)]
        public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
            IsSucc
                ? Succ(Value)
                : Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public Unit Match(Action<A> Succ, Action<Error> Fail)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            else
            {
                Fail(Error);
            }

            return default;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public A IfFail(Func<Error, A> Fail) =>
            IsSucc
                ? Value
                : Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public A IfFail(A alternative) =>
            IsSucc
                ? Value
                : alternative;

        [Pure, MethodImpl(AffOpt.mops)]
        public Unit IfFail(Action<Error> Fail)
        {
            if (IsFail)
            {
                Fail(Error);
            }
            return default;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public Unit IfSucc(Action<A> Succ)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            return default;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public Unit Iter(Action<A> Succ)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            return default;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<A> Do(Action<A> Succ)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            return this;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public S Fold<S>(S state, Func<S, A, S> f) =>
            IsSucc
                ? f(state, Value)
                : state;

        [Pure, MethodImpl(AffOpt.mops)]
        public S BiFold<S>(S state, Func<S, A, S> Succ, Func<S, Error, S> Fail) =>
            IsSucc
                ? Succ(state, Value)
                : Fail(state, Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public bool Exists(Func<A, bool> f) =>
            IsSucc && f(Value);

        [Pure, MethodImpl(AffOpt.mops)]
        public bool ForAll(Func<A, bool> f) =>
            IsFail || (IsSucc && f(Value));

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<B> Map<B>(Func<A, B> f) =>
            IsSucc
                ? Fin<B>.Succ(f(Value))
                : Fin<B>.Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
            IsSucc
                ? Fin<B>.Succ(Succ(Value))
                : Fin<B>.Fail(Fail(Error));

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<B> Select<B>(Func<A, B> f) =>
            IsSucc
                ? Fin<B>.Succ(f(Value))
                : Fin<B>.Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<B> Bind<B>(Func<A, Fin<B>> f) =>
            IsSucc
                ? f(Value)
                : Fin<B>.Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<B> BiBind<B>(Func<A, Fin<B>> Succ, Func<Error, Fin<B>> Fail) =>
            IsSucc
                ? Succ(Value)
                : Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<B> SelectMany<B>(Func<A, Fin<B>> f) =>
            IsSucc
                ? f(Value)
                : Fin<B>.Fail(Error);

        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project)
        {
            var a = data.Right;
            if(IsSucc)
            {
                var mb = bind(Value);
                return mb.IsSucc
                    ? project(a, mb.Value)
                    : Fin<C>.Fail(mb.Error);
            }
            else
            {
                return Fin<C>.Fail(Error);
            }
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public System.Collections.Generic.List<A> ToList() =>
            IsSucc
                ? new System.Collections.Generic.List<A>() { Value }
                : new System.Collections.Generic.List<A>();

        [Pure, MethodImpl(AffOpt.mops)]
        public Lst<A> ToLst() =>
            IsSucc
                ? List(Value)
                : Empty;

        [Pure, MethodImpl(AffOpt.mops)]
        public Seq<A> ToSeq() =>
            IsSucc
                ? Seq1(Value)
                : Empty;

        [Pure, MethodImpl(AffOpt.mops)]
        public Arr<A> ToArr() =>
            IsSucc
                ? Array(Value)
                : Empty;

        [Pure, MethodImpl(AffOpt.mops)]
        public A[] ToArray() =>
            IsSucc
                ? new A[] {Value}
                : new A[0];

        [Pure, MethodImpl(AffOpt.mops)]
        public Option<A> ToOption() =>
            IsSucc
                ? Some(Value)
                : None;

        [Pure, MethodImpl(AffOpt.mops)]
        public OptionAsync<A> ToOptionAsync() =>
            IsSucc
                ? SomeAsync(Value)
                : None;

        [Pure, MethodImpl(AffOpt.mops)]
        public OptionUnsafe<A> ToOptionUnsafe() =>
            IsSucc
                ? SomeUnsafe(Value)
                : None;

        [Pure, MethodImpl(AffOpt.mops)]
        public Either<Error, A> ToEither() =>
            data.State switch
            {
                EitherStatus.IsRight => Right<Error, A>(Value),
                EitherStatus.IsLeft  => Left<Error, A>(Error),
                _                    => default
            };

        [Pure, MethodImpl(AffOpt.mops)]
        public EitherUnsafe<Error, A> ToEitherUnsafe() =>
            data.State switch
            {
                EitherStatus.IsRight => RightUnsafe<Error, A>(Value),
                EitherStatus.IsLeft  => LeftUnsafe<Error, A>(Error),
                _                    => default
            };

        [Pure, MethodImpl(AffOpt.mops)]
        public EitherAsync<Error, A> ToEitherAsync() =>
            data.State switch
            {
                EitherStatus.IsRight => RightAsync<Error, A>(Value),
                EitherStatus.IsLeft  => LeftAsync<Error, A>(Error),
                _                    => default
            };

        [Pure, MethodImpl(AffOpt.mops)]
        public Eff<A> ToEff() =>
            data.State switch
            {
                EitherStatus.IsRight => SuccessEff<A>(Value),
                EitherStatus.IsLeft  => FailEff<A>(Error),
                _                    => default
            };

        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<A> ToAff() =>
            data.State switch
            {
                EitherStatus.IsRight => SuccessAff<A>(Value),
                EitherStatus.IsLeft  => FailAff<A>(Error),
                _                    => default
            };

        public A ThrowIfFail()
        {
            if (IsFail)
            {
                ExceptionDispatchInfo.Capture(Error).Throw();
            }

            return Value;
        }
    }
}
