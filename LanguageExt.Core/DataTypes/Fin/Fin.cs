using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using LanguageExt.Common;
using LanguageExt.DataTypes.Serialisation;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Equivalent of `Either<Error, A>`
    /// Called `Fin` because this should be used as the concrete result of a computation
    /// </summary>
    public struct Fin<A>
    {
        internal readonly EitherData<Error, A> data;

        /// <summary>
        /// Ctor
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        Fin(EitherData<Error, A> data) =>
            this.data = data;

        [Pure, MethodImpl(IO.mops)]
        public static Fin<A> Succ(A value) => 
            value is null
                ? throw new ValueIsNullException(nameof(value))
                : new Fin<A>(EitherData.Right<Error, A>(value));

        [Pure, MethodImpl(IO.mops)]
        public static Fin<A> Fail(Error error) => 
            new Fin<A>(EitherData.Left<Error, A>(error));

        [Pure]
        public bool IsSucc
        {
            [MethodImpl(IO.mops)]
            get => data.State == EitherStatus.IsRight;
        }

        [Pure]
        public bool IsFail
        {
            [MethodImpl(IO.mops)]
            get => data.State == EitherStatus.IsLeft || data.State == EitherStatus.IsBottom;
        }
        
        [Pure, MethodImpl(IO.mops)]
        public static implicit operator Fin<A>(A value) =>
            Fin<A>.Succ(value);
        
        [Pure, MethodImpl(IO.mops)]
        public static implicit operator Fin<A>(Error error) =>
            Fin<A>.Fail(error);

        [Pure, MethodImpl(IO.mops)]
        public static explicit operator A(Fin<A> ma) =>
            ma.IsSucc
                ? ma.Value
                : throw new EitherIsNotRightException();

        [Pure, MethodImpl(IO.mops)]
        public static explicit operator Error(Fin<A> ma) =>
            ma.IsFail
                ? ma.Error
                : throw new EitherIsNotLeftException();

        internal A Value 
        { 
            [MethodImpl(IO.mops)]
            get => data.State == EitherStatus.IsBottom
                ? throw new BottomException()
                : data.Right;  
        }

        internal Error Error 
        { 
            [MethodImpl(IO.mops)]
            get => data.State == EitherStatus.IsBottom 
                ? Error.Bottom 
                : data.Left;  
        }
        
        [Pure, MethodImpl(IO.mops)]
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

        [Pure, MethodImpl(IO.mops)]
        internal Fin<B> Cast<B>() =>
            new Fin<B>(
                IsSucc
                    ? convert<B>(Value)
                        .Map(EitherData.Right<Error, B>)
                        .IfNone(() => EitherData.Left<Error, B>(Error.New($"Can't cast success value of `Err<{nameof(A)}>` to `Err<{nameof(B)}>` ")))
                    : EitherData.Left<Error, B>(Error));

        [Pure, MethodImpl(IO.mops)]
        public override string ToString() =>
            IsSucc
                ? $"Succ({Value})"
                : $"Fail({Error})";

        [Pure, MethodImpl(IO.mops)]
        public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
            IsSucc
                ? Succ(Value)
                : Fail(Error);

        [Pure, MethodImpl(IO.mops)]
        public A IfFail(Func<Error, A> Fail) =>
            IsSucc
                ? Value
                : Fail(Error);

        [Pure, MethodImpl(IO.mops)]
        public Unit IfFail(Action<Error> Fail)
        {
            if (IsFail)
            {
                Fail(Error);
            }
            return default;
        }

        [Pure, MethodImpl(IO.mops)]
        public Unit IfSucc(Action<A> Succ)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            return default;
        }

        [Pure, MethodImpl(IO.mops)]
        public Unit Iter(Action<A> Succ)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            return default;
        }

        [Pure, MethodImpl(IO.mops)]
        public Fin<A> Do(Action<A> Succ)
        {
            if (IsSucc)
            {
                Succ(Value);
            }
            return this;
        }

        [Pure, MethodImpl(IO.mops)]
        public S Fold<S>(S state, Func<S, A, S> f) =>
            IsSucc
                ? f(state, Value)
                : state;

        [Pure, MethodImpl(IO.mops)]
        public bool Exists(Func<A, bool> f) =>
            IsSucc && f(Value);

        [Pure, MethodImpl(IO.mops)]
        public bool ForAll(Func<A, bool> f) =>
            IsFail || (IsSucc && f(Value));

        public Unit ThrowIfFail()
        {
            if (IsFail)
            {
                ExceptionDispatchInfo.Capture(Error).Throw();
            }

            return unit;
        }
    }
}
