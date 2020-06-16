using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqStck<EQ, A> : Eq<Stck<A>> where EQ : struct, Eq<A>
    {
        public static readonly EqStck<EQ, A> Inst = default(EqStck<EQ, A>);

        [Pure]
        public bool Equals(Stck<A> x, Stck<A> y)
        {
            if (x.Count != y.Count) return false;

            using var enumx = x.GetEnumerator();
            using var enumy = y.GetEnumerator();
            var count = x.Count;

            for (int i = 0; i < count; i++)
            {
                enumx.MoveNext();
                enumy.MoveNext();
                if (!default(EQ).Equals(enumx.Current, enumy.Current)) return false;
            }
            return true;
        }


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            default(HashableStck<EQ, A>).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(Stck<A> x, Stck<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Stck<A> x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqStck<A> : Eq<Stck<A>>
    {
        public static readonly EqStck<A> Inst = default(EqStck<A>);

        [Pure]
        public bool Equals(Stck<A> x, Stck<A> y) =>
            default(EqStck<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            default(HashableStck<A>).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(Stck<A> x, Stck<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Stck<A> x) =>
            GetHashCode(x).AsTask();
    }
}
