
using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

/// <summary>
/// Logical OR of the terms
/// </summary>
/// <typeparam name="A">Bound value to test</typeparam>
/// <typeparam name="Term1">First term</typeparam>
/// <typeparam name="Term2">Second term</typeparam>
public struct Exists<A, Term1, Term2> : Pred<A>
    where Term1 : Pred<A>
    where Term2 : Pred<A>
{
    [Pure]
    public static bool True(A value) =>
        Term1.True(value) ||
        Term2.True(value);
}

/// <summary>
/// Logical OR of the terms
/// </summary>
/// <typeparam name="A">Bound value to test</typeparam>
/// <typeparam name="Term1">First term</typeparam>
/// <typeparam name="Term2">Second term</typeparam>
public struct Exists<A, Term1, Term2, Term3> : Pred<A>
    where Term1 : Pred<A>
    where Term2 : Pred<A>
    where Term3 : Pred<A>
{
    [Pure]
    public static bool True(A value) =>
        Term1.True(value) ||
        Term2.True(value) ||
        Term3.True(value);
}

/// <summary>
/// Logical OR of the terms
/// </summary>
/// <typeparam name="A">Bound value to test</typeparam>
/// <typeparam name="Term1">First term</typeparam>
/// <typeparam name="Term2">Second term</typeparam>
public struct Exists<A, Term1, Term2, Term3, Term4> : Pred<A>
    where Term1 : Pred<A>
    where Term2 : Pred<A>
    where Term3 : Pred<A>
    where Term4 : Pred<A>
{
    [Pure]
    public static bool True(A value) =>
        Term1.True(value) ||
        Term2.True(value) ||
        Term3.True(value) ||
        Term4.True(value);
}

/// <summary>
/// Logical OR of the terms
/// </summary>
/// <typeparam name="A">Bound value to test</typeparam>
/// <typeparam name="Term1">First term</typeparam>
/// <typeparam name="Term2">Second term</typeparam>
/// <typeparam name="Term3">Third term</typeparam>
/// <typeparam name="Term4">Fourth term</typeparam>
/// <typeparam name="Term5">Fifth term</typeparam>
public struct Exists<A, Term1, Term2, Term3, Term4, Term5> : Pred<A>
    where Term1 : Pred<A>
    where Term2 : Pred<A>
    where Term3 : Pred<A>
    where Term4 : Pred<A>
    where Term5 : Pred<A>
{
    [Pure]
    public static bool True(A value) =>
        Term1.True(value) ||
        Term2.True(value) ||
        Term3.True(value) ||
        Term4.True(value) ||
        Term5.True(value);
}
