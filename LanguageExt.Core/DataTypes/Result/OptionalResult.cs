using System;
using System.Diagnostics.Contracts;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    public enum OptionalResultState : byte
    {
        Bottom,
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
    public struct OptionalResult<A> : IEquatable<OptionalResult<A>>
    {
        public static readonly OptionalResult<A> None = default(OptionalResult<A>);

        internal readonly OptionalResultState State;
        internal readonly Option<A> Value;
        internal Exception Exception;

        /// <summary>
        /// Constructor of a concrete value
        /// </summary>
        /// <param name="value"></param>
        [Pure]
        public OptionalResult(Option<A> value)
        {
            State = OptionalResultState.Success;
            Value = value;
            Exception = null;
        }

        /// <summary>
        /// Constructor of an error value
        /// </summary>
        /// <param name="e"></param>
        [Pure]
        public OptionalResult(Exception e)
        {
            State = OptionalResultState.Faulted;
            Exception = e;
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
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaulted => 
            State == OptionalResultState.Faulted;

        /// <summary>
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaultedOrNone => 
            State == OptionalResultState.Bottom ||
            State == OptionalResultState.Faulted ||
            Value.IsNone;

        /// <summary>
        /// True if the struct is in an invalid state
        /// </summary>
        [Pure]
        public bool IsBottom =>
            State == OptionalResultState.Bottom;

        /// <summary>
        /// Convert the value to a showable string
        /// </summary>
        [Pure]
        public override string ToString() =>
            IsBottom
                ? "(Bottom)"
                : IsFaulted
                    ? Exception?.ToString() ?? "(Exception)"
                    : Value.ToString();

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(OptionalResult<A> other) =>
            State == other.State &&
            IsBottom || (IsFaulted
                ? Exception == other.Exception
                : equals<EqDefault<A>, A>(Value, other.Value));

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
        public override int GetHashCode()
        {
            if (IsBottom) return -1;
            if (IsFaulted) return -2;
            return Value.GetHashCode();
        }

        [Pure]
        public static bool operator==(OptionalResult<A> a, OptionalResult<A> b) =>
            a.Equals(b);

        [Pure]
        public static bool operator !=(OptionalResult<A> a, OptionalResult<A> b) =>
            !(a==b);

        public readonly static OptionalResult<A> Bottom =
            default(OptionalResult<A>);

        [Pure]
        public A IfFailOrNone(A defaultValue) =>
            IsBottom || IsFaulted || Value.IsNone
                ? defaultValue
                : Value.Value;

        [Pure]
        public Option<A> IfFail(Func<Exception, A> f) =>
            IsBottom
                ? f(BottomException.Default)
                : IsFaulted
                    ? f(Exception)
                    : Value;

        public Unit IfFailOrNone(Action f)
        {
            if (IsBottom || IsFaulted || Value.IsNone) f();
            return unit;
        }

        public Unit IfFail(Action<Exception> f)
        {
            if (IsBottom) f(BottomException.Default);
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
            IsFaulted || Value.IsNone
                ? new OptionalResult<B>(Exception)
                : new OptionalResult<B>(f(Value.Value));

        [Pure]
        public async Task<OptionalResult<B>> MapAsync<B>(Func<A, Task<B>> f) =>
            IsFaulted || Value.IsNone
                ? new OptionalResult<B>(Exception)
                : new OptionalResult<B>(await f(Value.Value));
    }
}
