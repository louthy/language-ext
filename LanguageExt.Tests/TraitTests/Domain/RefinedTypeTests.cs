using LanguageExt.Common;
using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests.TraitTests.Domain;

public sealed class RefinedTypeDomainTypeTests
{
    [Fact]
    public void Equality()
    {
        var amount = PositiveInvoiceAmount.From(123m).SuccValue.To();

        var baseAmount = PositiveInvoiceAmount.FromM(123m).Run().As().Value.SuccValue.ToBase().To();

        Assert.Equal(123m, amount);
        Assert.Equal(123m, baseAmount);
    }

    [Fact]
    public void Validations()
    {
        Assert.True(PositiveInvoiceAmount.From(0m).IsFail);
        Assert.True(PositiveInvoiceAmount.From(-1m).IsFail);
    }
}

file sealed class PositiveInvoiceAmount :
    RefinedTypeFactory<PositiveInvoiceAmount, AccountingAmount, decimal>,
    RefinedTypeFactoryM<PositiveInvoiceAmount, AccountingAmount, Identity, decimal>
{
    private readonly AccountingAmount amount;

    private PositiveInvoiceAmount(AccountingAmount amount) =>
        this.amount = amount;

    public AccountingAmount ToBase() =>
        amount;

    public decimal To() =>
        amount.To();

    public static Fin<PositiveInvoiceAmount> From(AccountingAmount repr) =>
        repr.To() > 0m
            ? Fin.Succ(new PositiveInvoiceAmount(repr))
            : Fin.Fail<PositiveInvoiceAmount>(Error.New("Invoice amount must be greater than zero."));

    public static Fin<PositiveInvoiceAmount> From(decimal repr) =>
        AccountingAmount.From(repr).Bind(From);

    public static FinT<Identity, PositiveInvoiceAmount> FromM(AccountingAmount repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, PositiveInvoiceAmount>(value),
            Fail: static error => FinT.Fail<Identity, PositiveInvoiceAmount>(error));

    public static FinT<Identity, PositiveInvoiceAmount> FromM(decimal repr) =>
        AccountingAmount.FromM(repr).Bind(FromM);
}

file sealed class AccountingAmount :
    Magnitude<AccountingAmount, decimal>,
    DomainTypeFactory<AccountingAmount, decimal>,
    DomainTypeFactoryM<AccountingAmount, Identity, decimal>
{
    private readonly decimal amount;

    private AccountingAmount(decimal amount) =>
        this.amount = amount;

    public decimal To() =>
        amount;

    public static Fin<AccountingAmount> From(decimal repr) =>
        Fin.Succ(new AccountingAmount(repr));

    public static FinT<Identity, AccountingAmount> FromM(decimal repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, AccountingAmount>(value),
            Fail: static error => FinT.Fail<Identity, AccountingAmount>(error));

    public static AccountingAmount AdditiveIdentity { get; } =
        new(0m);

    public static AccountingAmount Origin =>
        AdditiveIdentity;

    public static AccountingAmount operator +(AccountingAmount left, AccountingAmount right) =>
        new(left.amount + right.amount);

    public static AccountingAmount operator -(AccountingAmount left, AccountingAmount right) =>
        new(left.amount - right.amount);

    public static AccountingAmount operator -(AccountingAmount value) =>
        new(-value.amount);

    public static AccountingAmount operator *(AccountingAmount value, decimal scalar) =>
        new(value.amount * scalar);

    public static AccountingAmount operator /(AccountingAmount value, decimal scalar) =>
        new(value.amount / scalar);

    public int CompareTo(AccountingAmount? other) =>
        other is null ? 1 : amount.CompareTo(other.amount);

    public static bool operator <(AccountingAmount left, AccountingAmount right) =>
        left.CompareTo(right) < 0;

    public static bool operator >(AccountingAmount left, AccountingAmount right) =>
        left.CompareTo(right) > 0;

    public static bool operator <=(AccountingAmount left, AccountingAmount right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >=(AccountingAmount left, AccountingAmount right) =>
        left.CompareTo(right) >= 0;

    public bool Equals(AccountingAmount? other) =>
        other is not null && amount == other.amount;

    public override bool Equals(object? obj) =>
        obj is AccountingAmount other && Equals(other);

    public override int GetHashCode() =>
        amount.GetHashCode();

    public static bool operator ==(AccountingAmount? left, AccountingAmount? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(AccountingAmount? left, AccountingAmount? right) =>
        !(left == right);
}
