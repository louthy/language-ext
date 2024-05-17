using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.Traits;

/// <summary>
/// Equality trait
/// </summary>
/// <typeparam name="A">
/// The type for which equality is defined
/// </typeparam>
[Trait("Eq*")]
public interface Eq<in A> : Hashable<A>, Trait
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static abstract bool Equals(A x, A y);
}

public static class Eq
{
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

    public static IEqualityComparer<A> ToEqualityComparer<EqA, A>(this EqA self)
        where EqA : Eq<A> =>
        new EqEqualityComparer<EqA, A>();
}
