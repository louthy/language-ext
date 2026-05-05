using System;
using System.Globalization;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public readonly struct Money<C> :
    DomainTypeFactory<Money<C>, decimal>,
    Magnitude<Money<C>, decimal>
    where C : Currency, new()
{
    private readonly decimal Value;

    internal Money(decimal value) =>
        Value = value;

    public C Currency =>
        Currency<C>.Value;

    public decimal Amount =>
        Value;

    public decimal To() =>
        Value;

    public static Fin<Money<C>> From(decimal repr) =>
        new Money<C>(repr);

    internal static Money<C> New(decimal value) =>
        new(value);

    public static Money<C> AdditiveIdentity { get; } =
        new(0m);

    public static Money<C> Zero =>
        AdditiveIdentity;

    public static Money<C> One { get; } =
        new(1m);

    public int CompareTo(Money<C> other) =>
        Value.CompareTo(other.Value);

    public bool Equals(Money<C> other) =>
        Value == other.Value;

    public override bool Equals(object? obj) =>
        obj is Money<C> other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(typeof(C), Value);

    public override string ToString()
    {
        var code = Currency.Code;
        var symbol = Currency.Symbol;
        var decimals = Currency.Decimals;

        return
            $"{symbol}{Value.ToString($"N{decimals}", CultureInfo.InvariantCulture)} {code}";
    }

    public Money<C> Add(Money<C> rhs) =>
        new(Value + rhs.Value);

    public Money<C> Subtract(Money<C> rhs) =>
        new(Value - rhs.Value);

    public Money<C> Multiply(decimal rhs) =>
        new(Value * rhs);

    public Money<C> Divide(decimal rhs) =>
        new(Value / rhs);

    public Fin<Money<C>> DivideSafe(decimal rhs)
    {
        var iValue = Value; 
        
        return NonZeroDecimal
            .Validate(
                rhs,
                value => Error.New($"{nameof(Money<C>)} cannot be divided by zero."))
            .Map(value => new Money<C>(iValue / value));
    }

    public decimal RatioTo(Money<C> rhs) =>
        Value / rhs.Value;

    public Fin<decimal> RatioToSafe(Money<C> rhs)
    {
        var iValue = Value;

        return NonZeroDecimal
            .Validate(
                rhs.Value,
                _ => Error.New($"{nameof(Money<C>)} ratio cannot divide by zero money."))
            .Map(value => iValue / value);
    }

    public Money<C> Abs() =>
        new(Math.Abs(Value));

    public Money<C> Round(MidpointRounding mode = MidpointRounding.ToEven)
    {
        var currency = Currency;

        return new Money<C>(
            decimal.Round(Value, currency.Decimals, mode));
    }

    public Money<TO> Convert<TO>(ExchangeRate<C, TO> rate)
        where TO : Currency, new() =>
        Money<TO>.New(Value * rate.To());

    public static Money<C> operator +(Money<C> lhs, Money<C> rhs) =>
        lhs.Add(rhs);

    public static Money<C> operator -(Money<C> lhs, Money<C> rhs) =>
        lhs.Subtract(rhs);

    public static Money<C> operator -(Money<C> value) =>
        new(-value.Value);

    public static Money<C> operator *(Money<C> lhs, decimal rhs) =>
        lhs.Multiply(rhs);

    public static Money<C> operator *(decimal lhs, Money<C> rhs) =>
        rhs.Multiply(lhs);

    public static Money<C> operator /(Money<C> lhs, decimal rhs) =>
        lhs.Divide(rhs);

    public static decimal operator /(Money<C> lhs, Money<C> rhs) =>
        lhs.RatioTo(rhs);

    public static bool operator ==(Money<C> lhs, Money<C> rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Money<C> lhs, Money<C> rhs) =>
        !lhs.Equals(rhs);

    public static bool operator >(Money<C> lhs, Money<C> rhs) =>
        lhs.Value > rhs.Value;

    public static bool operator <(Money<C> lhs, Money<C> rhs) =>
        lhs.Value < rhs.Value;

    public static bool operator >=(Money<C> lhs, Money<C> rhs) =>
        lhs.Value >= rhs.Value;

    public static bool operator <=(Money<C> lhs, Money<C> rhs) =>
        lhs.Value <= rhs.Value;
}


file sealed class NonZeroDecimal :
    Rule<NonZeroDecimal, decimal>
{
    public static bool Check(decimal value) =>
        value != 0m;
}
