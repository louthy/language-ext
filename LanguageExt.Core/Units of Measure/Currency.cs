using LanguageExt.Traits.Domain;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Represents a currency definition used as a type-level marker for <see cref="Money{C}"/>.
/// </summary>
/// <remarks>
/// Implement this interface in the consuming domain to define the currencies it supports.
/// Keeping currencies user-defined avoids imposing language, naming, region, or business assumptions
/// on applications.
/// </remarks>
public interface Currency :
    DomainType<Currency, string>
{
    /// <summary>
    /// Stable currency code, usually ISO-4217-like, such as CLP, USD, EUR, or a domain-specific code.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Human-readable currency name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Display symbol used when formatting money values.
    /// </summary>
    string Symbol { get; }

    /// <summary>
    /// Number of decimal places used when rounding and formatting this currency.
    /// </summary>
    int Decimals { get; }

    /// <summary>
    /// Returns the canonical representation of the currency.
    /// </summary>
    string DomainType<Currency, string>.To() =>
        Code;
}

/// <summary>
/// Provides a cached singleton-like value for a type-level currency.
/// </summary>
/// <typeparam name="C">Currency type.</typeparam>
public static class Currency<C>
    where C : Currency, new()
{
    /// <summary>
    /// Cached currency instance for the currency type.
    /// </summary>
    public static C Value { get; } =
        new();
}
