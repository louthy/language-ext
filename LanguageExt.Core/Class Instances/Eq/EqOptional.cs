using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the Optional type-class
    /// </summary>
    public struct EqOptional<EQ, OPTION, OA, A> : Eq<OA>
        where EQ     : struct, Eq<A>
        where OPTION : struct, Optional<OA, A>
    {
        public static readonly EqOptional<EQ, OPTION, OA, A> Inst = default(EqOptional<EQ, OPTION, OA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(OA x, OA y)
        {
            if (x.IsNull()) return y.IsNull();
            if (y.IsNull()) return false;
            if (ReferenceEquals(x, y)) return true;

            var xIsSome = default(OPTION).IsSome(x);
            var yIsSome = default(OPTION).IsSome(y);
            var xIsNone = !xIsSome;
            var yIsNone = !yIsSome;

            return xIsNone && yIsNone
                ? true
                : xIsNone || yIsNone
                    ? false
                    : default(OPTION).Match(x,
                        Some: a =>
                            default(OPTION).Match(y,
                                Some: b => @equals<EQ, A>(a, b),
                                None: () => false),
                        None: () => false);
        }


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(OA x) =>
            x.IsNull() ? 0 : x.GetHashCode();
   
        [Pure]
        public Task<bool> EqualsAsync(OA x, OA y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(OA x) => 
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Compare the equality of any type in the Optional type-class
    /// </summary>
    public struct EqOptional<OPTION, OA, A> : Eq<OA>
        where OPTION : struct, Optional<OA, A>
    {
        public static readonly EqOptional<OPTION, OA, A> Inst = default(EqOptional<OPTION, OA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(OA x, OA y) =>
            default(EqOptional<EqDefault<A>, OPTION, OA, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(OA x) =>
            default(EqOptional<EqDefault<A>, OPTION, OA, A>).GetHashCode(x);
   
        [Pure]
        public Task<bool> EqualsAsync(OA x, OA y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(OA x) => 
            GetHashCode(x).AsTask();
    }
}
