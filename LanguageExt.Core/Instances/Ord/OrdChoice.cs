using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
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
        public bool Equals(CH x, CH y) =>
            default(CHOICE).Match(x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => equals<ORDA, A>(a, b),
                                             Choice2: _ => false),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => false,
                                             Choice2: b => equals<ORDB, B>(a, b)));
    }

    /// <summary>
    /// Compare the equality of any type in the Choice type-class.  Taking into
    /// account only the 'success bound value' of B.
    /// </summary>
    public struct OrdChoice<ORD, CHOICE, CH, A, B> : Ord<CH>
        where CHOICE : struct, Choice<CH, A, B>
        where ORD    : struct, Ord<B>
    {
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
        public bool Equals(CH x, CH y) =>
            default(CHOICE).Match(x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => true,
                               Choice2: _ => false),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => false,
                               Choice2: b => equals<ORD, B>(a, b)));
    }
}
