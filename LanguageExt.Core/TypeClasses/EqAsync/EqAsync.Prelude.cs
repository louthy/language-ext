using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static Task<bool> equalsAsync<EqA, A>(Task<A> x, Task<A> y) where EqA : struct, Eq<A> =>
            default(EqTaskAsync<EqA, A>).EqualsAsync(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static Task<bool> equalsAsync<EqA, A>(OptionAsync<A> x, OptionAsync<A> y) where EqA : struct, Eq<A> =>
            default(EqOptionalAsync<EqA, MOptionAsync<A>, OptionAsync<A>, A>).EqualsAsync(x, y);
    }
}
