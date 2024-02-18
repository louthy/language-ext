/*
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Trait;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Numbers form a monoid under addition.
/// </summary>
/// <typeparam name="A">The type of the number being added.</typeparam>
public struct Addition<NUM, A> : Monoid<A> where NUM : Num<A>
{
    [Pure]
    public static A Append(A x, A y) =>
        plus<NUM, A>(x, y);

    [Pure]
    public static A Empty =>
        fromInteger<NUM, A>(0);
}
*/
