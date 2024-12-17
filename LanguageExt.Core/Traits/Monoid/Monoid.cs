using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Monoid trait
/// <para>
/// A monoid is a type with an identity and an associative binary operation.
/// </para>
/// </summary>
/// <typeparam name="A">The type being described as a monoid</typeparam>
public interface Monoid<A> : Semigroup<A>
    where A : Monoid<A>
{
    /// <summary>
    /// Identity
    /// </summary>
    [Pure]
    public static abstract A Empty { get; }
}
