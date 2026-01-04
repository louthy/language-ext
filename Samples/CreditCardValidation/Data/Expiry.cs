// This is a toy sample that demonstrates credit-card validation.  It shouldn't be considered complete,
// it simply demonstrates usage of the Validation applicative/monad for capturing multiple errors when
// doing complex validation.
//
// For more information, see Paul Louth's blog article on Validation:
// https://paullouth.com/higher-kinds-in-c-with-language-ext-part-5-validation/

using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace CreditCardValidation.Data;

/// <summary>
/// Expiry date MM/YYYY
/// </summary>
/// <param name="Value">Base12 number representing year and month</param>
public readonly struct Expiry(Base12 Value) :
    DomainType<Expiry, Base12>,
    Locus<Expiry, MonthSpan, int>
{
    public static Expiry AdditiveIdentity { get; } =
        new (Base12.Zero);

    public static Expiry One { get; } = 
        new(new Base12(1, 0));

    public static Expiry MinusOne { get; } = 
        new(new Base12(-1, 0));
    
    public static Expiry Now
    {
        get
        {
            var now = DateTime.Now;
            return new Expiry(new Base12(now.Year, (uint)(now.Month - 1)));
        }
    }

    public static Range<Expiry> NextTenYears =>
        new (Now, Now + MonthSpan.TenYears, One, MinusOne, NextTenYears1, NextTenYears1Rev);

    static Fin<Expiry> DomainType<Expiry, Base12>.From(Base12 repr) => 
        new Expiry(repr);

    public static Fin<Expiry> From(int month, int year) => 
        month is >= 1 and <= 12
            ? new Expiry(new Base12(year, (uint)(month - 1)))
            : Error.New("invalid date: month out of range, should be 1 - 12");

    public Base12 To() => 
        Value;

    public bool Equals(Expiry other) => 
        To() == other.To();

    public override bool Equals(object? obj) => 
        obj is Expiry other && Equals(other);

    public override int GetHashCode() => 
        To().GetHashCode();

    public int CompareTo(Expiry other) =>
        To().CompareTo(other.To());

    public static bool operator ==(Expiry left, Expiry right) => 
        left.Equals(right);

    public static bool operator !=(Expiry left, Expiry right) => 
        !(left == right);

    public static Expiry operator +(Expiry left, MonthSpan right) =>
        new(left.To() + Base12.From(right.To()));

    public static Expiry operator -(Expiry left, MonthSpan right) =>
        new(left.To() - Base12.From(right.To()));

    public static MonthSpan operator -(Expiry left, Expiry right) =>
        new(left.To().ToBase10() - right.To().ToBase10());

    public static Expiry operator -(Expiry value) =>
        new(-value.To());

    public static bool operator >(Expiry left, Expiry right) =>
        left.To() > right.To();

    public static bool operator >=(Expiry left, Expiry right) =>
        left.To() >= right.To();

    public static bool operator <(Expiry left, Expiry right) =>
        left.To() < right.To();
    
    public static bool operator <=(Expiry left, Expiry right) =>
        left.To() <= right.To();

    public override string ToString() =>
        $"{To().To().LeastSig + 1}/{To().To().MostSig}";

    static IEnumerable<Expiry> NextTenYears1 
    {
        get
        {
            var current = Now;
            for (var i = 0; i < 10; i++)
            {
                yield return current;
                current += MonthSpan.OneYear;
            }
        }
    }
    
    static IEnumerable<Expiry> NextTenYears1Rev
    {
        get
        {
            var current = Now;
            for (var i = 9; i >= 0; i--)
            {
                yield return current;
                current -= MonthSpan.OneYear;
            }
        }
    }
}
