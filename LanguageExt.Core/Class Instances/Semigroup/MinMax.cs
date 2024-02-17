using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Trait;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Ordered values form a semigroup under minimum.
/// </summary>
/// <typeparam name="A">The type of the ordered values.</typeparam>
public struct Min<ORD, A> : Semigroup<A> where ORD : Ord<A>
{
    [Pure]
    public static A Append(A x, A y) =>
        lessOrEq<ORD, A>(x, y) ? x : y;
}

/// <summary>
/// Ordered values form a semigroup under maximum.
/// </summary>
/// <typeparam name="A">The type of the ordered values.</typeparam>
public struct Max<ORD, A> : Semigroup<A> where ORD : Ord<A>
{
    [Pure]
    public static  A Append(A x, A y) =>
        lessOrEq<ORD, A>(x, y) ? y : x;
}
