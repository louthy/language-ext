using System;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public readonly struct ExchangeRate<FROM, TO> :
    DomainTypeFactory<ExchangeRate<FROM, TO>, decimal>
    where FROM : Currency, new()
    where TO : Currency, new()
{
    readonly decimal Value;

    private ExchangeRate(decimal value) =>
        Value = value;

    public FROM Source =>
        Currency<FROM>.Value;

    public TO Target =>
        Currency<TO>.Value;

    public decimal To() =>
        Value;

    internal static ExchangeRate<FROM, TO> New(decimal value) =>
        new(value);

    public static Fin<ExchangeRate<FROM, TO>> From(decimal repr) =>
        PositiveDecimal
            .Validate(
                repr,
                value => Error.New(
                    $"{nameof(ExchangeRate<FROM, TO>)} must be positive. Sent: {value}."))
            .Map(value => new ExchangeRate<FROM, TO>(value));

    public Money<TO> Apply(Money<FROM> money) =>
        Money<TO>.New(money.To() * Value);

    public ExchangeRate<TO, FROM> Invert() =>
        ExchangeRate<TO, FROM>.New(1m / Value);

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
