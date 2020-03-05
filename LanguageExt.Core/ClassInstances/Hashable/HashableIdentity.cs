using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Identity hashing
    /// </summary>
    public struct HashableIdentity<A> : Hashable<Identity<A>>
    {
        public static readonly HashableIdentity<A> Inst = default(HashableIdentity<A>);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Identity<A> x) =>
            x.GetHashCode();
    }
}
