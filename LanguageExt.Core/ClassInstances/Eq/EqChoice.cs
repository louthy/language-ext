using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
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
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(CHOICE).Match(x,
                Left: a =>
                    default(CHOICE).Match(y, Left: b => equals<EQA, A>(a, b),
                                             Right: _ => false),
                Right: a =>
                    default(CHOICE).Match(y, Left: _ => false,
                                             Right: b => equals<EQB, B>(a, b)),
                Bottom: () => default(CHOICE).IsBottom(y));

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoice<EQA, EQB, CHOICE, CH, A, B>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(CH x, CH y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(CH x) => 
            GetHashCode(x).AsTask();
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
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(EqChoice<EqDefault<A>, EQB, CHOICE, CH, A, B>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoice<EQB, CHOICE, CH, A, B>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(CH x) =>
            GetHashCode(x).AsTask();

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public Task<bool> EqualsAsync(CH x, CH y) =>
            Equals(x, y).AsTask();
    }

    /// <summary>
    /// Compare the equality of any type in the Either type-class
    /// </summary>
    public struct EqChoice<CHOICE, CH, A, B> : Eq<CH>
        where CHOICE : struct, Choice<CH, A, B>
    {
        public static EqChoice<CHOICE, CH, A, B> Inst = default(EqChoice<CHOICE, CH, A, B>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(CH x, CH y) =>
            default(EqChoice<EqDefault<A>, EqDefault<B>, CHOICE, CH, A, B>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoice<CHOICE, CH, A, B>).GetHashCode(x);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public Task<bool> EqualsAsync(CH x, CH y) =>
            Equals(x, y).AsTask();
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(CH x) =>
            GetHashCode(x).AsTask();
    }
}
