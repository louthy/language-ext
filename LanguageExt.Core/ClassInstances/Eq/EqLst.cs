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
    public struct EqLst<EQ, A> : Eq<Lst<A>> where EQ : struct, Eq<A>
    {
        public static readonly EqLst<EQ, A> Inst = default(EqLst<EQ, A>);

        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y)
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
        public int GetHashCode(Lst<A> x) =>
            default(HashableLst<EQ, A>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(Lst<A> x, Lst<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Lst<A> x) => 
            GetHashCode(x).AsTask();      
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqLst<A> : Eq<Lst<A>> 
    {
        public static readonly EqLst<A> Inst = default(EqLst<A>);

        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y) =>
            default(EqLst<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Lst<A> x) =>
            default(HashableLst<A>).GetHashCode(x);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public Task<bool> EqualsAsync(Lst<A> x, Lst<A> y) =>
            Equals(x, y).AsTask();
        
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(Lst<A> x) =>
            GetHashCode(x).AsTask();
    }
}
