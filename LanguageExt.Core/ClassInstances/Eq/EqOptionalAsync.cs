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
    public struct EqOptionalAsync<EQ, OPTION, OA, A> : EqAsync<OA>
        where EQ     : struct, Eq<A>
        where OPTION : struct, OptionalAsync<OA, A>
    {
        public static readonly EqOptionalAsync<EQ, OPTION, OA, A> Inst = default(EqOptionalAsync<EQ, OPTION, OA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public async Task<bool> EqualsAsync(OA x, OA y)
        {
            if (x.IsNull()) return y.IsNull();
            if (y.IsNull()) return false;
            if (ReferenceEquals(x, y)) return true;

            var isSomes = await Task.WhenAll(default(OPTION).IsSome(x), default(OPTION).IsSome(y));

            var xIsSome = isSomes[0];
            var yIsSome = isSomes[1];
            var xIsNone = !xIsSome;
            var yIsNone = !yIsSome;

            return xIsNone && yIsNone
                ? true
                : xIsNone || yIsNone
                    ? false
                    : await default(OPTION).MatchAsync(x,
                        SomeAsync: a =>
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
        public Task<int> GetHashCodeAsync(OA x) =>
            default(HashableOptionalAsync<EQ, OPTION, OA, A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Compare the equality of any type in the Optional type-class
    /// </summary>
    public struct EqOptionalAsync<OPTION, OA, A> : EqAsync<OA>
        where OPTION : struct, OptionalAsync<OA, A>
    {
        public static readonly EqOptionalAsync<OPTION, OA, A> Inst = default(EqOptionalAsync<OPTION, OA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public Task<bool> EqualsAsync(OA x, OA y) =>
            default(EqOptionalAsync<EqDefault<A>, OPTION, OA, A>).EqualsAsync(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(OA x) =>
            default(HashableOptionalAsync<OPTION, OA, A>).GetHashCodeAsync(x);
    }
}
