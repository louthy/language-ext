using System;
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

    /// <summary>
    /// Property that contains the trait in record form.  This allows the trait to be passed
    /// around as a value rather than resolved as a type.  It helps us get around limitations
    /// in the C# constraint system.
    /// </summary>
    public new static virtual MonoidInstance<A> Instance { get; } =
        new (Empty: A.Empty, Combine: Semigroup.combine);
}
