using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public abstract partial record Validation<F, A>
{
    public sealed record Success(A Value) : Validation<F, A>
    {
        /// <summary>
        /// Is the Validation in a Success state?
        /// </summary>
        [Pure]
        public override bool IsSuccess =>
            true;

        /// <summary>
        /// Is the Validation in a Left state?
        /// </summary>
        [Pure]
        public override bool IsFail =>
            false;

        /// <summary>
        /// Invokes the Success or Left function depending on the state of the Validation
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <param name="Success">Function to invoke if in a Success state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public override B Match<B>(
            Func<F, B> Fail,
            Func<A, B> Succ) =>
            Succ(Value);

        /// <summary>
        /// Show the structure as a string
        /// </summary>
        [Pure]
        public override string ToString() =>
            Value is null ? "Success(null)" : $"Success({Value})";

        /// <summary>
        /// Get a hash code for the structure
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value is null ? 0 : HashableDefault<A>.GetHashCode(Value);

        /// <summary>
        /// Empty span
        /// </summary>
        [Pure]
        public override ReadOnlySpan<F> FailSpan() =>
            ReadOnlySpan<F>.Empty;

        /// <summary>
        /// Span of right value
        /// </summary>
        [Pure]
        public override ReadOnlySpan<A> SuccessSpan() =>
            new([Value]);

        /// <summary>
        /// Compare this structure to another to find its relative ordering
        /// </summary>
        [Pure]
        public override int CompareTo<OrdF, OrdA>(Validation<F, A> other) =>
            other switch
            {
                Success r => OrdA.Compare(Value, r.Value),
                _         => 1
            };

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public override bool Equals<EqF, EqA>(Validation<F, A> other) =>
            other switch
            {
                Success r => EqA.Equals(Value, r.Value),
                _         => false
            };

        /// <summary>
        /// Unsafe access to the right-value 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        internal override A SuccessValue =>
            Value;

        /// <summary>
        /// Unsafe access to the left-value 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        internal override F FailValue =>
            throw new InvalidCastException();

        /// <summary>
        /// Maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="F">Left</typeparam>
        /// <typeparam name="A">Success</typeparam>
        /// <typeparam name="B">Mapped Validation type</typeparam>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public override Validation<F, B> Map<B>(Func<A, B> f) =>
            new Validation<F, B>.Success(f(Value));

        /// <summary>
        /// Bi-maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="F">Left</typeparam>
        /// <typeparam name="A">Success</typeparam>
        /// <typeparam name="L2">Left return</typeparam>
        /// <typeparam name="R2">Success return</typeparam>
        /// <param name="Success">Success map function</param>
        /// <param name="Fail">Left map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public override Validation<L2, R2> BiMap<L2, R2>(
            Func<F, L2> Fail,
            Func<A, R2> Succ) =>
            new Validation<L2, R2>.Success(Succ(Value));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="F">Left</typeparam>
        /// <typeparam name="A">Success</typeparam>
        /// <typeparam name="B">Resulting bound value</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Bound Validation</returns>
        [Pure]
        public override Validation<F, B> Bind<B>(Func<A, Validation<F, B>> f) =>
            f(Value);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public override Validation<L2, R2> BiBind<L2, R2>(
            Func<F, Validation<L2, R2>> Fail,
            Func<A, Validation<L2, R2>> Succ) =>
            Succ(Value);
    }
}
