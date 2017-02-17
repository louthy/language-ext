using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the Choice type-class.  Taking into
    /// account the ordering of both possible bound values.
    /// </summary>
    public struct OrdChoice<ORDA, ORDB, CHOICE, CH, A, B> : Ord<CH>
        where CHOICE   : struct, Choice<CH, A, B>
        where ORDA     : struct, Ord<A>
        where ORDB     : struct, Ord<B>
    {
        public static readonly OrdChoice<ORDA, ORDB, CHOICE, CH, A, B> Inst = default(OrdChoice<ORDA, ORDB, CHOICE, CH, A, B>);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(CH x, CH y) =>
            default(CHOICE).Match( x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => compare<ORDA, A>(a, b),
                                             Choice2: _ => 1),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => -1,
                                             Choice2: b => compare<ORDB, B>(a, b)));

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(CHOICE).Match(x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => equals<ORDA, A>(a, b),
                                             Choice2: _ => false),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => false,
                                             Choice2: b => equals<ORDB, B>(a, b)));


        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(CHOICE).Match(x,
                Choice1: a => a.IsNull() ? 0 : a.GetHashCode(),
                Choice2: b => b.IsNull() ? 0 : b.GetHashCode());
    }

    /// <summary>
    /// Compare the equality of any type in the Choice type-class.  Taking into
    /// account only the 'success bound value' of B.
    /// </summary>
    public struct OrdChoice<ORD, CHOICE, CH, A, B> : Ord<CH>
        where CHOICE : struct, Choice<CH, A, B>
        where ORD    : struct, Ord<B>
    {
        public static readonly OrdChoice<ORD, CHOICE, CH, A, B> Inst = default(OrdChoice<ORD, CHOICE, CH, A, B>);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(CH x, CH y) =>
            default(CHOICE).Match(x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => 0,
                               Choice2: _ => 1),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => -1,
                               Choice2: b => compare<ORD, B>(a, b)));

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(CHOICE).Match(x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => true,
                               Choice2: _ => false),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => false,
                               Choice2: b => equals<ORD, B>(a, b)));

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(CHOICE).Match(x,
                Choice1: a => a.IsNull() ? 0 : a.GetHashCode(),
                Choice2: b => b.IsNull() ? 0 : b.GetHashCode());
    }
}
