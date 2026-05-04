using DomainTypesExamples.Invariants;
using DomainTypesExamples.Literals;

namespace DomainTypesExamples.Roots.ValueObjects;

public sealed class UserName : DomainType<UserName, string>
{
    private static Error GetOutOfRangeMsg(StringSizeBetween<N1, N128> r, string v) =>
        Error.New($"The user name must be between {r.Min} and {r.Max} chars, sent: {v.Length}");

    private readonly string _value;

    private UserName(string value) =>
        _value = value;

    public static Fin<UserName> From(string repr) =>
        StringSizeBetween<N1, N128>.Validate(repr, GetOutOfRangeMsg) * 
        (UserName (v) => new UserName(v));

    public override string ToString() => _value;

    public string To() => _value;
}
