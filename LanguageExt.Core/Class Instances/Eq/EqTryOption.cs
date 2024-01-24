using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Compare the equality of any type in the TryOption trait
/// </summary>
public struct EqTryOption<EQ, A> : Eq<TryOption<A>>
    where EQ : Eq<A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="lhs">The left hand side of the equality operation</param>
    /// <param name="rhs">The right hand side of the equality operation</param>
    /// <returns>True if lhs and rhs are equal</returns>
    [Pure]
    public static bool Equals(TryOption<A> lhs, TryOption<A> rhs)
    {
        var x = lhs.Try();
        var y = rhs.Try();
        if (x.IsFaulted && y.IsFaulted) return true;
        if (x.IsFaulted || y.IsFaulted) return false;
        return equals<EQ, A>(x.Value, y.Value);
    }

    [Pure]
    public static int GetHashCode(TryOption<A> x) =>
        HashableTryOption<EQ, A>.GetHashCode(x);
}

/// <summary>
/// Compare the equality of any type in the TryOption trait
/// </summary>
public struct EqTryOption<A> : Eq<TryOption<A>>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="lhs">The left hand side of the equality operation</param>
    /// <param name="rhs">The right hand side of the equality operation</param>
    /// <returns>True if lhs and rhs are equal</returns>
    [Pure]
    public static bool Equals(TryOption<A> lhs, TryOption<A> rhs) =>
        EqTryOption<EqDefault<A>, A>.Equals(lhs, rhs);

    [Pure]
    public static int GetHashCode(TryOption<A> x) =>
        HashableTryOption<A>.GetHashCode(x);
}
