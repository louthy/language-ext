using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoiceUnsafe<EQA, EQB, CHOICE, CH, A, B> : Eq<CH>
        where CHOICE : struct, ChoiceUnsafe<CH, A, B>
        where EQA    : struct, Eq<A>
        where EQB    : struct, Eq<B>
    {
        public static readonly EqChoiceUnsafe<EQA, EQB, CHOICE, CH, A, B> Inst = default(EqChoiceUnsafe<EQA, EQB, CHOICE, CH, A, B>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(CHOICE).MatchUnsafe(x,
                Left: a =>
                    default(CHOICE).MatchUnsafe( y, 
                        Left: b => equals<EQA, A>(a, b),
                        Right: _ => false),
                Right: a =>
                    default(CHOICE).MatchUnsafe(y, 
                        Left: _ => false,
                        Right: b => equals<EQB, B>(a, b)),
                Bottom: () => default(CHOICE).IsBottom(y));

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoiceUnsafe<EQA, EQB, CHOICE, CH, A, B>).GetHashCode(x);
    }

    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoiceUnsafe<EQB, CHOICE, CH, A, B> : Eq<CH>
        where CHOICE : struct, ChoiceUnsafe<CH, A, B>
        where EQB : struct, Eq<B>
    {
        public static EqChoiceUnsafe<EQB, CHOICE, CH, A, B> Inst = default(EqChoiceUnsafe<EQB, CHOICE, CH, A, B>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(EqChoiceUnsafe<EqDefault<A>, EQB, CHOICE, CH, A, B>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoiceUnsafe<EQB, CHOICE, CH, A, B>).GetHashCode(x);
    }

    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoiceUnsafe<CHOICE, CH, A, B> : Eq<CH>
        where CHOICE : struct, ChoiceUnsafe<CH, A, B>
    {
        public static EqChoiceUnsafe<CHOICE, CH, A, B> Inst = default(EqChoiceUnsafe<CHOICE, CH, A, B>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(EqChoiceUnsafe<EqDefault<A>, EqDefault<B>, CHOICE, CH, A, B>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoiceUnsafe<CHOICE, CH, A, B>).GetHashCode(x);
    }
}
