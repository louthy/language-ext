using System;
using System.Globalization;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Represents an amount of money in a single statically-known currency.
/// </summary>
/// <typeparam name="C">Currency type.</typeparam>
/// <remarks>
/// Money values can only be added, subtracted, compared, or divided by values of the same currency.
/// This prevents accidental operations between different currencies at compile time.
/// Use <see cref="ExchangeRate{FROM,TO}"/> to convert money explicitly.
/// </remarks>
public readonly struct Money<C> :
    DomainTypeFactory<Money<C>, decimal>,
    Magnitude<Money<C>, decimal>
    where C : Currency, new()
{
    private readonly decimal _value;

    private Money(decimal value) =>
        _value = value;

    /// <summary>
    /// Currency associated with this money value.
    /// </summary>
    public C Currency =>
        Currency<C>.Value;

    /// <summary>
    /// Raw decimal amount.
    /// </summary>
    public decimal Amount =>
        _value;

    /// <summary>
    /// Returns the canonical decimal representation.
    /// </summary>
    public decimal To() =>
        _value;

    /// <summary>
    /// Safely creates a money value.
    /// </summary>
    public static Fin<Money<C>> From(decimal repr) =>
        new Money<C>(repr);

    /// <summary>
    /// Creates a money value without additional validation.
    /// </summary>
    internal static Money<C> New(decimal value) =>
        new(value);

    /// <summary>
    /// Zero value for this currency.
    /// </summary>
    public static Money<C> AdditiveIdentity { get; } =
        new(0m);

    /// <summary>
    /// Alias for the additive identity.
    /// </summary>
    public static Money<C> Zero =>
        AdditiveIdentity;

    /// <summary>
    /// One unit of this currency.
    /// </summary>
    public static Money<C> One { get; } =
        new(1m);

    /// <summary>
    /// Adds two money values with the same currency.
    /// </summary>
    public Money<C> Add(Money<C> rhs) =>
        new(_value + rhs._value);

    /// <summary>
    /// Subtracts two money values with the same currency.
    /// </summary>
    public Money<C> Subtract(Money<C> rhs) =>
        new(_value - rhs._value);

    /// <summary>
    /// Multiplies this money value by a scalar.
    /// </summary>
    public Money<C> Multiply(decimal rhs) =>
        new(_value * rhs);

    /// <summary>
    /// Divides this money value by a scalar.
    /// </summary>
    public Money<C> Divide(decimal rhs) =>
        new(_value / rhs);

    /// <summary>
    /// Safely divides this money value by a non-zero scalar.
    /// </summary>
    public Fin<Money<C>> DivideSafe(decimal rhs)
    {
        var iValue = _value;

        return NonZeroDecimal
            .Validate(
                rhs,
                value => Error.New($"{nameof(Money<C>)} cannot be divided by zero."))
            .Map(value => new Money<C>(iValue / value));
    }

    /// <summary>
    /// Returns the ratio between two money values of the same currency.
    /// </summary>
    public decimal RatioTo(Money<C> rhs) =>
        _value / rhs._value;

    /// <summary>
    /// Safely returns the ratio between two money values of the same currency.
    /// </summary>
    public Fin<decimal> RatioToSafe(Money<C> rhs)
    {
        var iValue = _value;

        return NonZeroDecimal
            .Validate(
                rhs._value,
                _ => Error.New($"{nameof(Money<C>)} ratio cannot divide by zero money."))
            .Map(value => iValue / value);
    }

    /// <summary>
    /// Returns the absolute value.
    /// </summary>
    public Money<C> Abs() =>
        new(Math.Abs(_value));

    /// <summary>
    /// Rounds the amount using the decimal places defined by the currency.
    /// </summary>
    public Money<C> Round(MidpointRounding mode = MidpointRounding.ToEven)
    {
        var currency = Currency;

        return new Money<C>(
            decimal.Round(_value, currency.Decimals, mode));
    }

    /// <summary>
    /// Converts this money value into another currency using an explicit exchange rate.
    /// </summary>
    public Money<TO> Convert<TO>(ExchangeRate<C, TO> rate)
        where TO : Currency, new() =>
        Money<TO>.New(_value * rate.To()); 
    
    public static Money<C> operator +(Money<C> lhs, Money<C> rhs) => lhs.Add(rhs); 
    
    public static Money<C> operator -(Money<C> lhs, Money<C> rhs) => lhs.Subtract(rhs); 
    
    public static Money<C> operator -(Money<C> value) => new(-value._value); 
    
    public static Money<C> operator *(Money<C> lhs, decimal rhs) => lhs.Multiply(rhs); 
    
    public static Money<C> operator *(decimal lhs, Money<C> rhs) => rhs.Multiply(lhs); 
    
    public static Money<C> operator /(Money<C> lhs, decimal rhs) => lhs.Divide(rhs); 
    
    public static decimal operator /(Money<C> lhs, Money<C> rhs) => lhs.RatioTo(rhs); 
    
    public static bool operator ==(Money<C> lhs, Money<C> rhs) => lhs.Equals(rhs); 
    
    public static bool operator !=(Money<C> lhs, Money<C> rhs) => !lhs.Equals(rhs); 
    
    public static bool operator >(Money<C> lhs, Money<C> rhs) => lhs._value > rhs._value; 
    
    public static bool operator <(Money<C> lhs, Money<C> rhs) => lhs._value < rhs._value; 
    
    public static bool operator >=(Money<C> lhs, Money<C> rhs) => lhs._value >= rhs._value; 
    
    public static bool operator <=(Money<C> lhs, Money<C> rhs) => lhs._value <= rhs._value;

    public bool Equals(Money<C> other) => _value == other._value;

    public int CompareTo(Money<C> other) => 
        _value.CompareTo(other._value);

    public override string ToString()
    {
        var code = Currency.Code; 
        var symbol = Currency.Symbol; 
        var decimals = Currency.Decimals; 
        
        return $"{symbol}{_value.ToString($"N{decimals}", CultureInfo.InvariantCulture)} {code}";
    }
}

file sealed class NonZeroDecimal :
    Rule<NonZeroDecimal, decimal>
{
    public static bool Check(decimal value) =>
        value != 0m;
}
