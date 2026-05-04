using System;
using System.Collections.Generic;
using System.Text;
using DomainTypesExamples.Invariants;
using DomainTypesExamples.Literals;

namespace DomainTypesExamples.Roots;

public sealed class UserId : Identifier<UserId, int>
{
    private readonly int _value;

    private UserId(int value) =>
        _value = value;

    public int To() =>
        _value;

    public override string ToString() =>
        _value.ToString();

    public bool Equals(UserId? other) =>
        _value.Equals(other?._value);

    public override bool Equals(object? obj) => 
        ReferenceEquals(this, obj) || obj is UserId other && Equals(other);

    public override int GetHashCode() => _value.GetHashCode();

    public static Fin<UserId> From(int repr) =>
        GreaterThan<N0, int>
            .Validate(
                repr,
                (_, value) => Error.New($"{nameof(UserId)} must be positive. Sent: {value}"))
            .Map(value => new UserId(value));

    public static bool operator ==(UserId? left, UserId? right) =>
        Equals(left, right);

    public static bool operator !=(UserId? left, UserId? right) =>
        !(left == right);
}
