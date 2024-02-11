using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

/// <summary>
/// Monoid trait
/// <para>
/// A monoid is a type with an identity and an associative binary operation.
/// </para>
/// </summary>
/// <typeparam name="A">The type being described as a monoid</typeparam>
[Trait("M*")]
public interface Monoid<A> : Semigroup<A>
{
    /// <summary>
    /// Identity
    /// <summary>
    [Pure]
    public static abstract A Empty { get; }
}
