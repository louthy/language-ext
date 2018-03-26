using System;
using System.Diagnostics.Contracts;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    public enum ResultState : byte
    {
        Faulted,
        Success
    }

    /// <summary>
    /// Represents the result of an operation:
    /// 
    ///     A | Exception
    /// 
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct Result<A> : IEquatable<Result<A>>, IComparable<Result<A>>
    {
        public readonly static Result<A> Bottom = default(Result<A>);

        internal readonly ResultState State;
        internal readonly A Value;
        internal Exception Exception;

        /// <summary>
        /// Constructor of a concrete value
        /// </summary>
        /// <param name="value"></param>
        [Pure]
        public Result(A value)
        {
            State = ResultState.Success;
            Value = value;
            Exception = null;
        }

        /// <summary>
        /// Constructor of an error value
        /// </summary>
        /// <param name="e"></param>
        [Pure]
        public Result(Exception e)
        {
            State = ResultState.Faulted;
            Exception = e;
            Value = default(A);
        }

        /// <summary>
        /// Implicit conversion operator from A to Result<A>
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator Result<A>(A value) =>
            new Result<A>(value);

        /// <summary>
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaulted => 
            State == ResultState.Faulted;

        /// <summary>
        /// True if the struct is in an invalid state
        /// </summary>
        [Pure]
        public bool IsBottom => 
            State == ResultState.Faulted && (Exception == null || Exception is BottomException);

        /// <summary>
        /// True if the struct is in an success
        /// </summary>
        [Pure]
        public bool IsSuccess => 
            State == ResultState.Success;

        /// <summary>
        /// Convert the value to a showable string
        /// </summary>
        [Pure]
        public override string ToString() =>
            IsFaulted
                ? Exception?.ToString() ?? "(Bottom)"
                : Value?.ToString() ?? "(null)";

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(Result<A> other) =>
            default(EqResult<A>).Equals(this, other);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is Result<A> rhs && Equals(rhs);

        /// <summary>
        /// Get hash code for bound value
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            default(EqResult<A>).GetHashCode(this);

        [Pure]
        public static bool operator==(Result<A> a, Result<A> b) =>
            default(EqResult<A>).Equals(a, b);

        [Pure]
        public static bool operator !=(Result<A> a, Result<A> b) =>
            !(a==b);

        [Pure]
        public A IfFail(A defaultValue) =>
            IsFaulted
                ? defaultValue
                : Value;

        [Pure]
        public A IfFail(Func<Exception, A> f) =>
            IsFaulted
                ? f(Exception ?? BottomException.Default)
                : Value;

        public Unit IfFail(Action<Exception> f)
        {
            if (IsFaulted) f(Exception ?? BottomException.Default);
            return unit;
        }

        public Unit IfSucc(Action<A> f)
        {
            if (IsSuccess) f(Value);
            return unit;
        }

        [Pure]
        public R Match<R>(Func<A, R> Succ, Func<Exception, R> Fail) =>
            IsBottom
                ? Fail(BottomException.Default)
                : IsFaulted
                    ? Fail(Exception)
                    : Succ(Value);

        [Pure]
        internal OptionalResult<A> ToOptional() =>
            IsFaulted
                ? new OptionalResult<A>(Exception)
                : new OptionalResult<A>(Optional(Value));

        [Pure]
        public Result<B> Map<B>(Func<A, B> f) =>
            IsBottom
                ? Result<B>.Bottom
                : IsFaulted 
                    ? new Result<B>(Exception)
                    : new Result<B>(f(Value));

        [Pure]
        public async Task<Result<B>> MapAsync<B>(Func<A, Task<B>> f) =>
            IsBottom
                ? Result<B>.Bottom
                : IsFaulted
                    ? new Result<B>(Exception)
                    : new Result<B>(await f(Value));

        [Pure]
        public int CompareTo(Result<A> other) =>
            default(OrdResult<A>).Compare(this, other);

        [Pure]
        public static bool operator <(Result<A> a, Result<A> b) =>
            a.CompareTo(b) < 0;

        [Pure]
        public static bool operator <=(Result<A> a, Result<A> b) =>
            a.CompareTo(b) <= 0;

        [Pure]
        public static bool operator >(Result<A> a, Result<A> b) =>
            a.CompareTo(b) > 0;

        [Pure]
        public static bool operator >=(Result<A> a, Result<A> b) =>
            a.CompareTo(b) >= 0;
    }
}
