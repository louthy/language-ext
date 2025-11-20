using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public abstract partial record Validation<F, A>
{
    public sealed record Fail(F Value) : Validation<F, A>
    {
        /// <summary>
        /// Is the Validation in a Success state?
        /// </summary>
        [Pure]
        public override bool IsSuccess =>
            false;

        /// <summary>
        /// Is the Validation in a Fail state?
        /// </summary>
        [Pure]
        public override bool IsFail =>
            true;

        /// <summary>
        /// Invokes the Success or Fail function depending on the state of the Validation
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a Success state</param>
        /// <param name="Fail">Function to invoke if in a Fail state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public override B Match<B>(Func<F, B> Fail, Func<A, B> Succ) =>
            Fail(Value);

        /// <summary>
        /// Show the structure as a string
        /// </summary>
        [Pure]
        public override string ToString() =>
            Value is null ? "Fail(null)" : $"Fail({Value})";

        /// <summary>
        /// Get a hash code for the structure
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value is null ? 0 : HashableDefault<F>.GetHashCode(Value);

        /// <summary>
        /// Span of left value
        /// </summary>
        [Pure]
        public override ReadOnlySpan<F> FailSpan() =>
            new([Value]);

        /// <summary>
        /// Empty span
        /// </summary>
        [Pure]
        public override ReadOnlySpan<A> SuccessSpan() =>
            ReadOnlySpan<A>.Empty;

        /// <summary>
        /// Compare this structure to another to find its relative ordering
        /// </summary>
        [Pure]
        public override int CompareTo<OrdF, OrdA>(Validation<F, A> other) =>
            other switch
            {
                Fail l => OrdF.Compare(Value, l.Value),
                _      => -1
            };

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public override bool Equals<EqF, EqA>(Validation<F, A> other) =>
            other switch
            {
                Fail l => EqF.Equals(Value, l.Value),
                _      => false
            };

        /// <summary>
        /// Unsafe access to the right-value 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        internal override A SuccessValue =>
            throw new InvalidCastException();

        /// <summary>
        /// Unsafe access to the left-value 
        /// </summary>
        internal override F FailValue =>
            Value;

        /// <summary>
        /// Maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="F">Fail</typeparam>
        /// <typeparam name="A">Success</typeparam>
        /// <typeparam name="B">Mapped Validation type</typeparam>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public override Validation<F, B> Map<B>(Func<A, B> f) =>
            new Validation<F, B>.Fail(Value);

        /// <summary>
        /// Bi-maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="F">Fail</typeparam>
        /// <typeparam name="A">Success</typeparam>
        /// <typeparam name="L2">Fail return</typeparam>
        /// <typeparam name="R2">Success return</typeparam>
        /// <param name="Succ">Success map function</param>
        /// <param name="Fail">Fail map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public override Validation<L2, R2> BiMap<L2, R2>(Func<F, L2> Fail, Func<A, R2> Succ) =>
            new Validation<L2, R2>.Fail(Fail(Value));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="F">Fail</typeparam>
        /// <typeparam name="A">Success</typeparam>
        /// <typeparam name="B">Resulting bound value</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Bound Validation</returns>
        [Pure]
        public override Validation<F, B> Bind<B>(Func<A, Validation<F, B>> f) =>
            new Validation<F, B>.Fail(Value);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public override Validation<L2, R2> BiBind<L2, R2>(
            Func<F, Validation<L2, R2>> Fail,
            Func<A, Validation<L2, R2>> Succ) =>
            Fail(Value);
    }
}
