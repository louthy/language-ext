using System;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Represents a positive exchange rate from one currency to another.
/// </summary>
/// <typeparam name="FROM">Source currency type.</typeparam>
/// <typeparam name="TO">Target currency type.</typeparam>
/// <remarks>
/// This type is the explicit evidence required to convert <see cref="Money{FROM}"/>
/// into <see cref="Money{TO}"/>. Currency conversion is intentionally not implicit,
/// so money values with different currencies cannot be accidentally added or compared.
/// </remarks>
public readonly struct ExchangeRate<FROM, TO> :
    DomainTypeFactory<ExchangeRate<FROM, TO>, decimal>
    where FROM : Currency, new()
    where TO : Currency, new()
{
    readonly decimal Value;

    private ExchangeRate(decimal value) =>
        Value = value;
    
    /// <summary>
    /// Source currency of the exchange rate.
    /// </summary>
    public FROM Source =>
        Currency<FROM>.Value;

    /// <summary>
    /// Target currency of the exchange rate.
    /// </summary>
    public TO Target =>
        Currency<TO>.Value;

    /// <summary>
    /// Returns the decimal representation of the rate.
    /// </summary>
    public decimal To() =>
        Value;

    /// <summary>
    /// Creates an exchange rate without validation.
    /// </summary>
    /// <remarks>
    /// Intended for internal composition only, where positivity has already been proven.
    /// </remarks>
    internal static ExchangeRate<FROM, TO> New(decimal value) =>
        new(value);

    /// <summary>
    /// Safely creates an exchange rate from a positive decimal value.
    /// </summary>
    /// <param name="repr">The amount of target currency equivalent to one unit of source currency.</param>
    /// <returns>A successful exchange rate when the value is positive; otherwise a failure.</returns>
    public static Fin<ExchangeRate<FROM, TO>> From(decimal repr) =>
        PositiveDecimal
            .Validate(
                repr,
                value => Error.New(
                    $"{nameof(ExchangeRate<FROM, TO>)} must be positive. Sent: {value}."))
            .Map(value => new ExchangeRate<FROM, TO>(value));

    /// <summary>
    /// Converts money from the source currency into the target currency.
    /// </summary>
    public Money<TO> Apply(Money<FROM> money) =>
        Money<TO>.New(money.To() * Value);

    /// <summary>
    /// Returns the inverse exchange rate.
    /// </summary>
    public ExchangeRate<TO, FROM> Invert() =>
        ExchangeRate<TO, FROM>.New(1m / Value);

    /// <summary>
    /// Composes this exchange rate with another compatible rate.
    /// </summary>
    /// <typeparam name="NEXT">The final target currency type.</typeparam>
    public ExchangeRate<FROM, NEXT> Then<NEXT>(ExchangeRate<TO, NEXT> next)
        where NEXT : Currency, new() =>
        ExchangeRate<FROM, NEXT>.New(Value * next.To());

    public override string ToString() =>
        $"1 {Source.Code} = {Value} {Target.Code}";
}

file sealed class PositiveDecimal :
    Rule<PositiveDecimal, decimal>
{
    public static bool Check(decimal value) =>
        value > 0m;
}
