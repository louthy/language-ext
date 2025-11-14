using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt;

public partial class Fin<A>
{
    /// <summary>
    /// Fail case for the `Fin` union-type
    /// </summary>
    /// <param name="Error">Error value</param>
    public sealed class Fail(Error Error) : Fin<A>
    {
        /// <summary>
        /// Value accessor
        /// </summary>
        public Error Error { get; } = Error;

        /// <summary>
        /// Is the structure in a Success state?
        /// </summary>
        [Pure]
        public override bool IsSucc =>
            false;

        /// <summary>
        /// Is the structure in a Fail state?
        /// </summary>
        [Pure]
        public override bool IsFail =>
            true;

        /// <summary>
        /// Invokes the Succ or Fail function depending on the state of the structure
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a Succ state</param>
        /// <param name="Fail">Function to invoke if in a Fail state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public override B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
            Fail(Error);

        /// <summary>
        /// Show the structure as a string
        /// </summary>
        [Pure]
        public override string ToString() =>
            $"Fail({Error})";

        [Pure]
        public override int GetHashCode<HashA>() =>
            -1;

        /// <summary>
        /// Empty span
        /// </summary>
        [Pure]
        public override ReadOnlySpan<Error> FailSpan() =>
            new([Error]);

        /// <summary>
        /// Span of right value
        /// </summary>
        [Pure]
        public override ReadOnlySpan<A> SuccSpan() =>
            ReadOnlySpan<A>.Empty;

        /// <summary>
        /// Compare this structure to another to find its relative ordering
        /// </summary>
        [Pure]
        public override int CompareTo<OrdA>(Fin<A> other) =>
            other is Fail
                ? 0
                : -1;    

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public override bool Equals<EqA>(Fin<A> other) =>
            other is Fail;

        /// <summary>
        /// Unsafe access to the success value 
        /// </summary>
        internal override A SuccValue =>
            throw new InvalidCastException();

        /// <summary>
        /// Unsafe access to the fail value 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        internal override Error FailValue =>
            Error;

        /// <summary>
        /// Maps the value in the structure
        /// </summary>
        /// <param name="f">Map function</param>
        /// <returns>Mapped structure</returns>
        [Pure]
        public override Fin<B> Map<B>(Func<A, B> f) =>
            new Fin<B>.Fail(Error);

        /// <summary>
        /// Maps the value in the structure
        /// </summary>
        /// <param name="f">Map function</param>
        /// <returns>Mapped structure</returns>
        [Pure]
        public override Fin<A> MapFail(Func<Error, Error> f) =>
            new Fail(f(Error));

        /// <summary>
        /// Bi-maps the structure
        /// </summary>
        /// <returns>Mapped Either</returns>
        [Pure]
        public override Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
            new Fin<B>.Fail(Fail(Error));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Resulting bound value</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Bound structure</returns>
        [Pure]
        public override Fin<B> Bind<B>(Func<A, Fin<B>> f) =>
            new Fin<B>.Fail(Error);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public override Fin<B> BiBind<B>(
            Func<A, Fin<B>> Succ,
            Func<Error, Fin<B>> Fail) =>
            Fail(Error);

        public void Deconstruct(out Error value) =>
            value = Error;
    }
}
