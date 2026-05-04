using DomainTypesExamples.Capabilities;
using DomainTypesExamples.ValueObjects;
using DomainTypesExamples.Roots.ValueObjects;

namespace DomainTypesExamples.Roots;

public sealed record User(
    UserId Id,
    UserName Name,
    NonFutureDate CreatedAt,
    Seq<WorkDay> WorkDays)
    : DomainType<User, (int Id, string Name, DateOnly CreatedAt, Seq<WorkDay> WorkDays)>
{
    public UserId Id { get; } = Id;

    public UserName Name { get; } = Name;

    public NonFutureDate CreatedAt { get; } = CreatedAt;

    public (int Id, string Name, DateOnly CreatedAt, Seq<WorkDay> WorkDays) To() =>
        (Id.To(), Name.To(), CreatedAt.To(), WorkDays);

    public sealed class Factory<RT>
        : DomainFactoryM<Factory<RT>, Eff<RT>, User, string>
        where RT : HasTime<RT>, HasSequences<RT>
    {
        public static FinT<Eff<RT>, User> FromM(string repr) =>
            from name in FinT.lift<Eff<RT>, UserName>(UserName.From(repr))
            from nextId in nextRandomInt32<RT>()
            from id in UserId.From(nextId)
            from now in getNow<RT>()
            from createdAt in NonFutureDate.Factory<RT>.FromM(now)
            select new User(
                id,
                name,
                createdAt,
                Seq<WorkDay>());
    }
}

