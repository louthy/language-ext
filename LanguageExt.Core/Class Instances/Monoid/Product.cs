/*
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Trait;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Numbers form a monoid under addition.
/// </summary>
/// <typeparam name="A">The type of the number being added.</typeparam>
public struct Product<NUM, A> : Monoid<A> where NUM : Num<A>
{
    public static readonly Product<NUM, A> Inst = default;

    [Pure]
    public static A Append(A x, A y) =>
        product<NUM, A>(x, y);

    [Pure]
    public static A Empty =>
        fromInteger<NUM, A>(1);
}
*/
