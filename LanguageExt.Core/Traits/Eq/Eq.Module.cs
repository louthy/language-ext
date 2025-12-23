using System.Collections.Generic;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Eq
{
    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<A>(A x, A y) where A : Eq<A> =>
        A.Equals(x, y);
    
    class EqEqualityComparer<EqA, A> : IEqualityComparer<A>
        where EqA : Eq<A>
    {
        public bool Equals(A? x, A? y) =>
            (x, y) switch
            {
                (null, null) => true,
                (null, _)    => false,
                (_, null)    => false,
                var (nx, ny) => EqA.Equals(nx, ny)
            };

        public int GetHashCode(A obj) =>
            EqA.GetHashCode(obj);
    }

    public static IEqualityComparer<A> Comparer<EqA, A>()
        where EqA : Eq<A> =>
        Cache<EqA, A>.Default;

    static class Cache<EqA, A>
        where EqA : Eq<A>
    {
        public static readonly IEqualityComparer<A> Default = new EqEqualityComparer<EqA, A>();
    }
}
