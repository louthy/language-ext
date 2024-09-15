// This is a toy sample that demonstrates credit-card validation.  It shouldn't be considered complete,
// it simply demonstrates usage of the Validation applicative/monad for capturing multiple errors when
// doing complex validation.
//
// For more information, see Paul Louth's blog article on Validation:
// https://paullouth.com/higher-kinds-in-c-with-language-ext-part-5-validation/

using LanguageExt;
using LanguageExt.Traits.Domain;

namespace CreditCardValidation.Data;

/// <summary>
/// Time span in months
/// </summary>
/// <param name="Value">Number of months to span</param>
public readonly struct MonthSpan(int Value) :
    DomainType<MonthSpan, int>,
    Amount<MonthSpan, int>
{
    public static readonly MonthSpan OneYear = new(12);
    public static readonly MonthSpan TenYears = new(12 * 10);
    
    static Fin<MonthSpan> DomainType<MonthSpan, int>.From(int repr) => 
        new MonthSpan(repr);

    public static MonthSpan From(int repr) => 
        new (repr);

    public int To() => 
        Value;
    
    public bool Equals(MonthSpan other) => 
        To() == other.To();

    public override bool Equals(object? obj) => 
        obj is MonthSpan other && Equals(other);

    public override int GetHashCode() => 
        To().GetHashCode();

    public int CompareTo(MonthSpan other) => 
        To().CompareTo(other.To());

    public static bool operator ==(MonthSpan left, MonthSpan right) => 
        left.To() == right.To();

    public static bool operator !=(MonthSpan left, MonthSpan right) => 
        !(left == right);

    public static MonthSpan operator -(MonthSpan value) =>
        From(-value.To());

    public static MonthSpan operator +(MonthSpan left, MonthSpan right) => 
        From(left.To() + right.To());

    public static MonthSpan operator -(MonthSpan left, MonthSpan right) => 
        From(left.To() - right.To());

    public static MonthSpan operator *(MonthSpan left, int right) => 
        From(left.To() * right);

    public static MonthSpan operator /(MonthSpan left, int right) => 
        From(left.To() * right);

    public static bool operator >(MonthSpan left, MonthSpan right) => 
        left.To() > right.To();

    public static bool operator >=(MonthSpan left, MonthSpan right) => 
        left.To() >= right.To();

    public static bool operator <(MonthSpan left, MonthSpan right) => 
        left.To() < right.To();

    public static bool operator <=(MonthSpan left, MonthSpan right) => 
        left.To() <= right.To();
}
