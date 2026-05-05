using DomainTypesExamples.Roots;

namespace DomainTypesExamples.ValueObjects;

public readonly record struct PaymentPolicy(
    PaymentRate NormalRate,
    decimal OvertimeMultiplier)
{
    public static Fin<PaymentPolicy> From(
        PaymentRate normalRate,
        decimal overtimeMultiplier) =>
        overtimeMultiplier >= 1m
            ? new PaymentPolicy(normalRate, overtimeMultiplier)
            : Error.New(
                $"{nameof(PaymentPolicy)} required an overtime multiplier higher or equal to 1.");

    public Payment Calculate(WorkDay workday)
    {
        var tracked = workday.TrackedDuration;

        var normalHours =
            tracked <= WorkDay.DefaultWorkDayDuration
                ? tracked
                : WorkDay.DefaultWorkDayDuration;

        var overtimeHours = tracked - normalHours; 

        var normalPayment = NormalRate.For(normalHours);

        var overtimePayment =
            (NormalRate.For(overtimeHours) * OvertimeMultiplier).Round();

        return new Payment(
            NormalTime: normalHours,
            OverTime: overtimeHours,
            Normal: normalPayment,
            Over: overtimePayment);
    }
}

public sealed record Payment(
    Time NormalTime,
    Time OverTime,
    Money<UF> Normal,
    Money<UF> Over)
{
    public Money<UF> Total =>
        Normal + Over;
}
