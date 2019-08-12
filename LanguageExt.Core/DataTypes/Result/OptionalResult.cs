using System;
using System.Diagnostics.Contracts;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal enum OptionalResultState : byte
    {
        Faulted,
        Success
    }

    /// <summary>
    /// Represents the result of an operation:
    /// 
    ///     Some(A) | None | Exception
    /// 
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <remarks>
    /// `Result<A>` (and `OptionalResult<A>`) is purely there to represent a concrete result value of a invoked lazy operation 
    /// (like `Try<A>`).  You're not really meant to consume it directly.
    /// 
    /// For example:
    /// 
    ///     var ma = Try(...);
    ///     var ra = ma(); // This will produce a `Result<A>` because that's what the `Try` delegate returns
    /// 
    /// But you should be matching on the result, or using any of the other convenient extension methods to get a concrete value:
    /// 
    ///     var ma = Try(...);
    ///     var ra1 = ma.IfFail(0);
    ///     var ra1 = ma.Match(Succ: x => x + 10, Fail: 0);
    ///     // ... etc ...
    /// </remarks>
    public struct OptionalResult<A> : IEquatable<OptionalResult<A>>, IComparable<OptionalResult<A>>
    {
        internal static readonly OptionalResult<A> None = new OptionalResult<A>(Prelude.None);
        internal static readonly OptionalResult<A> Bottom = default(OptionalResult<A>);

        internal readonly OptionalResultState State;
        internal readonly Option<A> Value;
        Exception exception;

        internal Exception Exception => exception ?? BottomException.Default;

        /// <summary>
        /// Constructor of a concrete value
        /// </summary>
        /// <param name="value"></param>
        [Pure]
        public OptionalResult(Option<A> value)
        {
            State = OptionalResultState.Success;
            Value = value;
            exception = null;
        }

        /// <summary>
        /// Constructor of an error value
        /// </summary>
        /// <param name="e"></param>
        [Pure]
        public OptionalResult(Exception e)
        {
            State = OptionalResultState.Faulted;
            exception = e;
            Value = Option<A>.None;
        }

        /// <summary>
        /// Implicit conversion operator from A to Result<A>
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator OptionalResult<A>(A value) =>
            new OptionalResult<A>(value);

        /// <summary>
        /// Implicit conversion operator from Option<A> to Result<A>
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator OptionalResult<A>(Option<A> value) =>
            new OptionalResult<A>(value);

        /// <summary>
        /// Implicit conversion operator from Option<A> to Result<A>
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator OptionalResult<A>(OptionNone value) =>
            new OptionalResult<A>(value);

        /// <summary>
        /// True if the result is in a bottom state
        /// </summary>
        [Pure]
        public bool IsBottom =>
            State == OptionalResultState.Faulted && (exception == null || exception is BottomException);

        /// <summary>
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaulted => 
            State == OptionalResultState.Faulted;

        /// <summary>
        /// True if the result is valid, but None
        /// </summary>
        [Pure]
        public bool IsNone =>
            State == OptionalResultState.Success && Value.IsNone;

        /// <summary>
        /// True if the result is valid and Some
        /// </summary>
        [Pure]
        public bool IsSome =>
            State == OptionalResultState.Success && Value.IsSome;

        /// <summary>
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaultedOrNone => 
            State == OptionalResultState.Faulted ||
            Value.IsNone;

        /// <summary>
        /// Convert the value to a showable string
        /// </summary>
        [Pure]
        public override string ToString() =>
            IsFaulted
                ? exception?.ToString() ?? "(Bottom)"
                : Value.ToString();

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(OptionalResult<A> other) =>
            default(EqOptionalResult<A>).Equals(this, other);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is OptionalResult<A> rhs && Equals(rhs);

        /// <summary>
        /// Get hash code for bound value
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            default(EqOptionalResult<A>).GetHashCode(this);

        [Pure]
        public static bool operator==(OptionalResult<A> a, OptionalResult<A> b) =>
            default(EqOptionalResult<A>).Equals(a, b);

        [Pure]
        public static bool operator !=(OptionalResult<A> a, OptionalResult<A> b) =>
            !(a==b);

        [Pure]
        public A IfFailOrNone(A defaultValue) =>
            IsFaulted || Value.IsNone
                ? defaultValue
                : Value.Value;

        [Pure]
        public Option<A> IfFail(Func<Exception, A> f) =>
            IsFaulted
                ? f(Exception)
                : Value;

        public Unit IfFailOrNone(Action f)
        {
            if (IsFaulted || Value.IsNone) f();
            return unit;
        }

        public Unit IfFail(Action<Exception> f)
        {
            if (IsFaulted) f(Exception);
            return unit;
        }

        public Unit IfSucc(Action<A> f)
        {
            if (!IsFaulted && Value.IsSome) f(Value.Value);
            return unit;
        }

        [Pure]
        public R Match<R>(Func<A, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            IsFaulted
                ? Fail(Exception)
                : Value.Match(Some, None);

        internal Result<A> ToResult() =>
            IsFaulted
                ? new Result<A>(Exception)
                : Value.IsSome
                    ? new Result<A>(Value.Value)
                    : new Result<A>(default(A));

        [Pure]
        public OptionalResult<B> Map<B>(Func<A, B> f) =>
            IsFaulted
                ? new OptionalResult<B>(Exception)
                : Value.IsNone
                    ? new OptionalResult<B>(Option<B>.None)
                    : new OptionalResult<B>(f(Value.Value));

        [Pure]
        public async Task<OptionalResult<B>> MapAsync<B>(Func<A, Task<B>> f) =>
            IsFaulted
                ? new OptionalResult<B>(Exception)
                : Value.IsNone
                    ? new OptionalResult<B>(Option<B>.None)
                    : new OptionalResult<B>(await f(Value.Value));

        [Pure]
        public int CompareTo(OptionalResult<A> other) =>
            default(OrdOptionalResult<A>).Compare(this, other);

        [Pure]
        public static bool operator <(OptionalResult<A> a, OptionalResult<A> b) =>
            a.CompareTo(b) < 0;

        [Pure]
        public static bool operator <=(OptionalResult<A> a, OptionalResult<A> b) =>
            a.CompareTo(b) <= 0;

        [Pure]
        public static bool operator >(OptionalResult<A> a, OptionalResult<A> b) =>
            a.CompareTo(b) > 0;

        [Pure]
        public static bool operator >=(OptionalResult<A> a, OptionalResult<A> b) =>
            a.CompareTo(b) >= 0;
    }
}
