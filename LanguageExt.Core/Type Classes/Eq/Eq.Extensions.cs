using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class TypeClass
{
    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals<EQ, A>(this A[] x, A[] y) where EQ : Eq<A> =>
        EqArray<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="mx">The left hand side of the equality operation</param>
    /// <param name="my">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals<EQ, A>(this A? mx, A? my)
        where EQ : Eq<A>
        where A : struct =>
        (mx, my) switch
        {
            (null, null) => true,
            (_, null)    => false,
            (null, _)    => false,
            var (x, y)   => EQ.Equals(x.Value, y.Value)
        };

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals<EQ, A>(this IEnumerable<A> x, IEnumerable<A> y) where EQ : Eq<A> =>
        EqEnumerable<EQ, A>.Equals(x, y);
}
