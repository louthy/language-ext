using DomainTypesExamples.Roots;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace DomainTypesExamples.ValueObjects;

public readonly struct PaymentRate :
    RefinedTypeFactory<PaymentRate, Money<UF>, decimal>
{
    readonly Money<UF> value;

    private PaymentRate(Money<UF> value) =>
        this.value = value;

    public Money<UF> ToBase() =>
        value;

    public decimal To() =>
        value.To();

    public static Fin<PaymentRate> From(Money<UF> repr) =>
        repr > Money<UF>.Zero
            ? new PaymentRate(repr)
            : Error.New($"{nameof(PaymentRate)} must be higher than zero. Sent: {repr}.");

    public static Fin<PaymentRate> From(decimal amount) =>
        Money<UF>
            .From(amount)
            .Bind(From);

    public Money<UF> For(Time duration)
    {
        var hours = Convert.ToDecimal(duration.Hours);

        return (value * hours).Round();
    }

    public override string ToString() =>
        $"{value}/hour";
}
