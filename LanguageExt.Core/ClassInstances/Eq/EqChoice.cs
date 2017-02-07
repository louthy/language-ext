using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoice<EQA, EQB, CHOICE, CH, A, B> : Eq<CH>
        where CHOICE : struct, Choice<CH, A, B>
        where EQA    : struct, Eq<A>
        where EQB    : struct, Eq<B>
    {
        public static readonly EqChoice<EQA, EQB, CHOICE, CH, A, B> Inst = default(EqChoice<EQA, EQB, CHOICE, CH, A, B>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(CH x, CH y) =>
            default(CHOICE).Match(x,
                Choice1: a =>
                    default(CHOICE).Match(y, Choice1: b => equals<EQA, A>(a, b),
                                             Choice2: _ => false),
                Choice2: a =>
                    default(CHOICE).Match(y, Choice1: _ => false,
                                             Choice2: b => equals<EQB, B>(a, b)),
                Bottom: () => default(CHOICE).IsBottom(y));

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(CH x) =>
            default(CHOICE).Match(x,
                Choice1: a => a.IsNull() ? 0 : a.GetHashCode(),
                Choice2: a => a.IsNull() ? 0 : a.GetHashCode());
    }

    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoice<EQB, CHOICE, CH, A, B> : Eq<CH>
        where CHOICE : struct, Choice<CH, A, B>
        where EQB : struct, Eq<B>
    {
        public static EqChoice<EQB, CHOICE, CH, A, B> Inst = default(EqChoice<EQB, CHOICE, CH, A, B>);

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
                                             Choice2: b => equals<EQB, B>(a, b)),
                Bottom: () => default(CHOICE).IsBottom(y));

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(CH x) =>
            default(CHOICE).Match(x,
                Choice1: a => a.IsNull() ? 0 : a.GetHashCode(),
                Choice2: a => a.IsNull() ? 0 : a.GetHashCode());
    }
}
