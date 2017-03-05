using System;
using System.Diagnostics.Contracts;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Represents the result of an operation:
    /// 
    ///     Some(A) | None | Exception
    /// 
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct OptionalResult<A> : IEquatable<OptionalResult<A>>
    {
        public static readonly OptionalResult<A> None = new OptionalResult<A>();

        readonly bool IsValid;
        internal readonly Option<A> Value;
        internal Exception Exception;

        /// <summary>
        /// Constructor of a concrete value
        /// </summary>
        /// <param name="value"></param>
        [Pure]
        public OptionalResult(Option<A> value)
        {
            IsValid = true;
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
            IsValid = true;
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
        public bool IsFaulted => Exception != null || IsBottom;

        /// <summary>
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaultedOrNone => Exception != null || IsBottom || Value.IsNone;

        /// <summary>
        /// True if the struct is in an invalid state
        /// </summary>
        [Pure]
        public bool IsBottom => !IsValid;

        /// <summary>
        /// Convert the value to a showable string
        /// </summary>
        [Pure]
        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(OptionalResult<A> other) =>
            IsBottom == other.IsBottom &&
            IsFaulted
                ? Exception == other.Exception
                : equals<EqDefault<A>, A>(Value, other.Value);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is OptionalResult<A> && Equals((OptionalResult<A>)obj);

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
            new OptionalResult<A>(BottomException.Default);

        [Pure]
        public A IfFail(A defaultValue) =>
            IsFaulted || Value.IsNone
                ? defaultValue
                : Value.Value;

        [Pure]
        public Option<A> IfFail(Func<Exception, A> f) =>
            IsFaulted
                ? f(Exception)
                : Value;

        public Unit IfFail(Action f)
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
