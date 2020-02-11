using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Queue hashing
    /// </summary>
    public struct HashableQue<HashA, A> : Hashable<Que<A>> where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            Prelude.hash<HashA, A>(x);
    }

    /// <summary>
    /// Queue hashing
    /// </summary>
    public struct HashableQue<A> : Hashable<Que<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            default(HashableQue<EqDefault<A>, A>).GetHashCode(x);
    }

}
