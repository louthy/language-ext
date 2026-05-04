using DomainTypesExamples.Capabilities;
using DomainTypesExamples.Invariants;

namespace DomainTypesExamples.ValueObjects;

public sealed class NonFutureDate : DomainType<NonFutureDate, DateOnly>
{
    private readonly DateOnly _value;

    private NonFutureDate(DateOnly value) =>
        _value = value;

    public DateOnly To() =>
        _value;

    public override string ToString() => $"{_value:d}";

    public static FinT<Eff<RT>, NonFutureDate> GetNow<RT>()
        where RT : HasTime<RT> =>
        from now in getNow<RT>()
        from date in Factory<RT>.FromM(now)
        select date;

    public sealed class Factory<RT> : DomainFactoryM<Factory<RT>, Eff<RT>, NonFutureDate, DateOnly>
        where RT : HasTime<RT>
    {

        public static FinT<Eff<RT>, NonFutureDate> FromM(DateOnly repr) =>
            DateNotFromFuture<RT>
                .ValidateM(repr, GenerateFutureDateMsg)
                .Map(value => new NonFutureDate(value));

        public static FinT<Eff<RT>, NonFutureDate> FromM(DateTimeOffset repr) =>
            FromM(repr.ToDateOnly());

        private static Eff<RT, Error> GenerateFutureDateMsg(DateOnly value) =>
            Error.New($"{nameof(NonFutureDate)} cannot be created from a future date. Sent: {value:d}");
    }
}
