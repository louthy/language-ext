using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Defines a factory responsible for constructing domain values from an input representation.
/// </summary>
/// <typeparam name="SELF">
/// The concrete factory type, enabling static polymorphism.
/// </typeparam>
/// <typeparam name="TYPE">
/// The domain type produced by this factory.
/// </typeparam>
/// <typeparam name="IN">
/// The input type used to construct the domain value.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="DomainFactory{SELF, TYPE, IN}"/> separates the construction of domain values
/// from their representation. This allows different creation strategies (validation,
/// transformation, or contextual mapping) without coupling them to the domain type itself.
/// </para>
/// <para>
/// Conceptually:
/// </para>
/// <list type="bullet">
/// <item>DomainType defines "what it is" (representation).</item>
/// <item>DomainFactory defines "how it is created".</item>
/// </list>
///
/// <para>
/// Factories return <see cref="Fin{T}"/> to model safe construction without exceptions,
/// allowing validation failures to be handled explicitly.
/// </para>
/// </remarks>
public interface DomainFactory<SELF, TYPE, IN>
    where SELF : DomainFactory<SELF, TYPE, IN>
    where TYPE : DomainType<TYPE>
{
    /// <summary>
    /// Attempts to create a domain value from the specified input.
    /// </summary>
    /// <param name="repr">The input value used to construct the domain type.</param>
    /// <returns>
    /// A successful <see cref="Fin{T}"/> containing the constructed domain value,
    /// or a failure containing an <see cref="Error"/>.
    /// </returns>
    public static abstract Fin<TYPE> From(IN repr);

    /// <summary>
    /// Creates a domain value from the specified input, throwing if construction fails.
    /// </summary>
    /// <param name="repr">The input value used to construct the domain type.</param>
    /// <returns>The constructed domain value.</returns>
    /// <exception cref="Exception">
    /// Thrown when the domain value cannot be constructed.
    /// </exception>
    public static virtual TYPE FromUnsafe(IN repr) =>
        SELF.From(repr).ThrowIfFail();
}


/// <summary>
/// Defines a factory responsible for constructing domain values from an input representation.
/// </summary>
/// <typeparam name="SELF">
/// The concrete factory type, enabling static polymorphism.
/// </typeparam>
/// <typeparam name="TYPE">
/// The domain type produced by this factory.
/// </typeparam>
/// <typeparam name="IN">
/// The input type used to construct the domain value.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="DomainFactory{SELF, TYPE, IN}"/> separates the construction of domain values
/// from their representation. This allows different creation strategies (validation,
/// transformation, or contextual mapping) without coupling them to the domain type itself.
/// </para>
/// <para>
/// Conceptually:
/// </para>
/// <list type="bullet">
/// <item>DomainType defines "what it is" (representation).</item>
/// <item>DomainFactory defines "how it is created".</item>
/// </list>
///
/// <para>
/// Factories return <see cref="Fin{T}"/> to model safe construction without exceptions,
/// allowing validation failures to be handled explicitly.
/// </para>
/// </remarks>
public interface DomainFactory<SELF, IN> : DomainFactory<SELF, SELF, IN>
    where SELF : DomainFactory<SELF, IN>, DomainType<SELF>;
