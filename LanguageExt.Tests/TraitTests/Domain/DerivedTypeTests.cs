using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests.TraitTests.Domain;

public sealed class DerivedTypeDomainTypeTests
{
    [Fact]
    public void SameAsBase()
    {
        var invoiceAmount = InvoiceAmount.From(250m).SuccValue;

        Assert.Equal(250m, invoiceAmount.To());
        Assert.Equal(250m, invoiceAmount.ToBase().To());
    }
}

file sealed class InvoiceAmount :
    DerivedTypeFactory<InvoiceAmount, AccountingAmount, decimal>,
    DerivedTypeFactoryM<InvoiceAmount, AccountingAmount, Identity, decimal>
{
    private readonly AccountingAmount amount;

    private InvoiceAmount(AccountingAmount amount) =>
        this.amount = amount;

    public AccountingAmount ToBase() =>
        amount;

    public decimal To() =>
        amount.To();

    public static InvoiceAmount New(AccountingAmount @base) =>
        new(@base);

    public static Fin<InvoiceAmount> From(decimal repr) =>
        AccountingAmount.From(repr).Map(New);

    public static FinT<Identity, InvoiceAmount> FromM(decimal repr) =>
        AccountingAmount.FromM(repr).Map(New);
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
