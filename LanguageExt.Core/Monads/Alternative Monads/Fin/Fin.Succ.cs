using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt;

public partial class Fin<A>
{
    /// <summary>
    /// Success case for the `Fin` union-type
    /// </summary>
    /// <param name="Value"></param>
    public sealed class Succ(A Value) : Fin<A>
    {
        /// <summary>
        /// Value accessor
        /// </summary>
        public A Value { get; } = Value;

        /// <summary>
        /// Is the structure in a Success state?
        /// </summary>
        [Pure]
        public override bool IsSucc =>
            true;

        /// <summary>
        /// Is the structure in a Fail state?
        /// </summary>
        [Pure]
        public override bool IsFail =>
            false;

        /// <summary>
        /// Invokes the Succ or Fail function depending on the state of the structure
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a Succ state</param>
        /// <param name="Fail">Function to invoke if in a Fail state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public override B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
            Succ(Value);

        /// <summary>
        /// Show the structure as a string
        /// </summary>
        [Pure]
        public override string ToString() =>
            Value is null ? "Succ(null)" : $"Succ({Value})";

        /// <summary>
        /// Get a hash code for the structure
        /// </summary>
        public override int GetHashCode<HashA>() =>
            Value is null ? 0 :  HashA.GetHashCode(Value);

        /// <summary>
        /// Empty span
        /// </summary>
        [Pure]
        public override ReadOnlySpan<Error> FailSpan() =>
            ReadOnlySpan<Error>.Empty;

        /// <summary>
        /// Span of right value
        /// </summary>
        [Pure]
        public override ReadOnlySpan<A> SuccSpan() =>
            new([Value]);

        /// <summary>
        /// Compare this structure to another to find its relative ordering
        /// </summary>
        [Pure]
        public override int CompareTo<OrdA>(Fin<A>? other) =>
            other switch
            {
                Succ r => OrdA.Compare(Value, r.Value),
                _      => 1
            };

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public override bool Equals<EqA>(Fin<A> other) =>
            other switch
            {
                Succ r => EqA.Equals(Value, r.Value),
                _      => false
            };

        /// <summary>
        /// Unsafe access to the success value 
        /// </summary>
        internal override A SuccValue =>
            Value;

        /// <summary>
        /// Unsafe access to the fail value 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        internal override Error FailValue =>
            throw new InvalidCastException();

        /// <summary>
        /// Maps the value in the structure
        /// </summary>
        /// <param name="f">Map function</param>
        /// <returns>Mapped structure</returns>
        [Pure]
        public override Fin<B> Map<B>(Func<A, B> Succ) =>
            new Fin<B>.Succ(Succ(Value));

        /// <summary>
        /// Maps the value in the structure
        /// </summary>
        /// <param name="f">Map function</param>
        /// <returns>Mapped structure</returns>
        [Pure]
        public override Fin<A> MapFail(Func<Error, Error> f) =>
            this;

        /// <summary>
        /// Bi-maps the structure
        /// </summary>
        /// <returns>Mapped Either</returns>
        [Pure]
        public override Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
            new Fin<B>.Succ(Succ(Value));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Resulting bound value</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Bound structure</returns>
        [Pure]
        public override Fin<B> Bind<B>(Func<A, Fin<B>> f) =>
            f(Value);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public override Fin<B> BiBind<B>(
            Func<A, Fin<B>> Succ,
            Func<Error, Fin<B>> Fail) =>
            Succ(Value);

        public void Deconstruct(out A value) =>
            value = Value;
    }
}
