using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests.TraitTests.Domain;

public sealed class VectorSpaceDomainTypeTests
{
    [Fact]
    public void Operators()
    {
        var debit = AccountBalanceDelta.From(125.50m).SuccValue;

        var credit = AccountBalanceDelta.FromM(-25.50m).Run().As().Value.SuccValue;

        Assert.Equal(100.00m, (debit + credit).To());
        Assert.Equal(151.00m, (debit - credit).To());
        Assert.Equal(-125.50m, (-debit).To());
        Assert.Equal(251.00m, (debit * 2m).To());
        Assert.Equal(62.75m, (debit / 2m).To());
        Assert.Equal(0m, AccountBalanceDelta.Origin.To());
    }
}

file sealed class AccountBalanceDelta :
    VectorSpace<AccountBalanceDelta, decimal>,
    DomainTypeFactory<AccountBalanceDelta, decimal>,
    DomainTypeFactoryM<AccountBalanceDelta, Identity, decimal>
{
    private readonly decimal _value;

    private AccountBalanceDelta(decimal value) =>
        this._value = value;

    public decimal To() =>
        _value;

    public static Fin<AccountBalanceDelta> From(decimal repr) =>
        Fin.Succ(new AccountBalanceDelta(repr));

    public static FinT<Identity, AccountBalanceDelta> FromM(decimal repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, AccountBalanceDelta>(value),
            Fail: static error => FinT.Fail<Identity, AccountBalanceDelta>(error));

    public static AccountBalanceDelta AdditiveIdentity { get; } =
        new(0m);

    public static AccountBalanceDelta Origin =>
        AdditiveIdentity;

    public static AccountBalanceDelta operator +(AccountBalanceDelta left, AccountBalanceDelta right) =>
        new(left._value + right._value);

    public static AccountBalanceDelta operator -(AccountBalanceDelta left, AccountBalanceDelta right) =>
        new(left._value - right._value);

    public static AccountBalanceDelta operator -(AccountBalanceDelta value) =>
        new(-value._value);

    public static AccountBalanceDelta operator *(AccountBalanceDelta value, decimal scalar) =>
        new(value._value * scalar);

    public static AccountBalanceDelta operator /(AccountBalanceDelta value, decimal scalar) =>
        new(value._value / scalar);

    public bool Equals(AccountBalanceDelta? other) =>
        other is not null && _value == other._value;

    public override bool Equals(object? obj) =>
        obj is AccountBalanceDelta other && Equals(other);

    public override int GetHashCode() =>
        _value.GetHashCode();

    public static bool operator ==(AccountBalanceDelta? left, AccountBalanceDelta? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(AccountBalanceDelta? left, AccountBalanceDelta? right) =>
        !(left == right);
}
