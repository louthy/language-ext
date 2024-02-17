#nullable enable
using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Trait;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the Optional trait
    /// </summary>
    public struct EqOptional<EQ, OPTION, OA, A> : Eq<OA>
        where EQ     : Eq<A>
        where OPTION : Optional<OA, A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals(OA x, OA y)
        {
            if (x.IsNull()) return y.IsNull();
            if (y.IsNull()) return false;
            if (ReferenceEquals(x, y)) return true;

            var xIsSome = OPTION.IsSome(x);
            var yIsSome = OPTION.IsSome(y);
            var xIsNone = !xIsSome;
            var yIsNone = !yIsSome;

            return xIsNone &&
                   yIsNone || !xIsNone &&
                   !yIsNone            &&
                   OPTION.Match(x,
                                Some: a =>
                                    OPTION.Match(y,
                                                 Some: b => equals<EQ, A>(a, b),
                                                 None: () => false),
                                None: () => false);
        }


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static int GetHashCode(OA x) =>
            x.IsNull() ? 0 : x.GetHashCode();
   
        [Pure]
        public static Task<bool> EqualsAsync(OA x, OA y) =>
            Equals(x, y).AsTask();

        [Pure]
        public static Task<int> GetHashCodeAsync(OA x) => 
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Compare the equality of any type in the Optional trait
    /// </summary>
    public struct EqOptional<OPTION, OA, A> : Eq<OA>
        where OPTION : Optional<OA, A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals(OA x, OA y) =>
            EqOptional<EqDefault<A>, OPTION, OA, A>.Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public static int GetHashCode(OA x) =>
            EqOptional<EqDefault<A>, OPTION, OA, A>.GetHashCode(x);
   
        [Pure]
        public static Task<bool> EqualsAsync(OA x, OA y) =>
            Equals(x, y).AsTask();

        [Pure]
        public static Task<int> GetHashCodeAsync(OA x) => 
            GetHashCode(x).AsTask();
    }
}
