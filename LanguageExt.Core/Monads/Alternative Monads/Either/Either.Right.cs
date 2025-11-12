using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public abstract partial record Either<L, R>
{
    public sealed record Right(R Value) : Either<L, R>
    {
        /// <summary>
        /// Is the Either in a Right state?
        /// </summary>
        [Pure]
        public override bool IsRight =>
            true;

        /// <summary>
        /// Is the Either in a Left state?
        /// </summary>
        [Pure]
        public override bool IsLeft =>
            false;

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public override B Match<B>(Func<L, B> Left, Func<R, B> Right) =>
            Right(Value);

        /// <summary>
        /// Show the structure as a string
        /// </summary>
        [Pure]
        public override string ToString() =>
            Value is null ? "Right(null)" : $"Right({Value})";

        /// <summary>
        /// Get a hash code for the structure
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value is null ? 0 : HashableDefault<R>.GetHashCode(Value);

        /// <summary>
        /// Empty span
        /// </summary>
        [Pure]
        public override ReadOnlySpan<L> LeftSpan() =>
            ReadOnlySpan<L>.Empty;

        /// <summary>
        /// Span of right value
        /// </summary>
        [Pure]
        public override ReadOnlySpan<R> RightSpan() =>
            new([Value]);

        /// <summary>
        /// Singleton enumerable if in a right state, otherwise empty enumerable
        /// </summary>
        [Pure]
        public override IEnumerable<R> AsEnumerable()
        {
            yield return Value;
        }

        /// <summary>
        /// Compare this structure to another to find its relative ordering
        /// </summary>
        [Pure]
        public override int CompareTo<OrdL, OrdR>(Either<L, R> other) =>
            other switch
            {
                Right r => OrdR.Compare(Value, r.Value),
                _       => 1
            };

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public override bool Equals<EqL, EqR>(Either<L, R> other) =>
            other switch
            {
                Right r => EqR.Equals(Value, r.Value),
                _       => false
            };

        /// <summary>
        /// Unsafe access to the right-value 
        /// </summary>
        /// <exception cref="EitherIsNotRightException"></exception>
        internal override R RightValue =>
            Value;

        /// <summary>
        /// Unsafe access to the left-value 
        /// </summary>
        /// <exception cref="EitherIsNotLeftException"></exception>
        internal override L LeftValue =>
            throw new EitherIsNotLeftException();

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="B">Mapped Either type</typeparam>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public override Either<L, B> Map<B>(Func<R, B> f) =>
            new Either<L, B>.Right(f(Value));

        /// <summary>
        /// Bi-maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="L2">Left return</typeparam>
        /// <typeparam name="R2">Right return</typeparam>
        /// <param name="Right">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public override Either<L2, R2> BiMap<L2, R2>(Func<L, L2> Left, Func<R, R2> Right) =>
            new Either<L2, R2>.Right(Right(Value));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="B">Resulting bound value</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Bound Either</returns>
        [Pure]
        public override Either<L, B> Bind<B>(Func<R, Either<L, B>> f) =>
            f(Value);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public override Either<L2, R2> BiBind<L2, R2>(
            Func<L, Either<L2, R2>> Left,
            Func<R, Either<L2, R2>> Right) =>
            Right(Value);
    }
}
