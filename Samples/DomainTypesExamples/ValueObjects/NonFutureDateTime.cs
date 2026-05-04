using DomainTypesExamples.Capabilities;
using DomainTypesExamples.Invariants;

namespace DomainTypesExamples.ValueObjects;

public sealed class NonFutureDateTime : DomainType<NonFutureDateTime, DateTimeOffset>
{
    private readonly DateTimeOffset _value;

    private NonFutureDateTime(DateTimeOffset value) =>
        _value = value;

    public DateTimeOffset To() =>
        _value;

    public override string ToString() => $"{_value:g}";

    public sealed class Factory<RT> : 
        DomainFactoryM<Factory<RT>, Eff<RT>, NonFutureDateTime, DateTimeOffset>
        where RT : HasTime<RT>
    {
        public static FinT<Eff<RT>, NonFutureDateTime> FromM(DateTimeOffset repr) =>
            DateTimeOffsetNotFromFuture<RT>
                .ValidateM(repr, GenerateFutureDateMsg)
                .Map(value => new NonFutureDateTime(value));

        private static Eff<RT, Error> GenerateFutureDateMsg(DateTimeOffset value) =>
            Error.New($"{nameof(NonFutureDateTime)} cannot be created from a future date. Sent: {value:g}");
    }
}
