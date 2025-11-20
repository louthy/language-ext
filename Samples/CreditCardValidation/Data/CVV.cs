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
/// CVV code
/// </summary>
/// <param name="Number">Integer representation of the code</param>
public class CVV(int Number) :
    DomainType<CVV, int>,
    Identifier<CVV>
{
    public override string ToString() =>
        $"{Number:000}";

    public static Fin<CVV> From(int repr) =>
        repr is >= 0 and <= 999
            ? new CVV(repr)
            : Error.New("invalid CVV number");

    public int To() => 
        Number;

    public override int GetHashCode() => 
        Number.GetHashCode();

    public bool Equals(CVV? other) => 
        To().Equals(other?.To());

    public override bool Equals(object? obj) =>
        obj is CVV cvv && Equals(cvv);

    public static bool operator ==(CVV? left, CVV? right) => 
        left?.To() == right?.To();

    public static bool operator !=(CVV? left, CVV? right) => 
        !(left == right);
}
