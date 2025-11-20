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
/// Credit card number
/// </summary>
/// <param name="Number">Credit card number</param>
public class CardNumber(Seq<int> Number) : 
    DomainType<CardNumber, Seq<int>>,
    Identifier<CardNumber>
{
    public override string ToString() =>
        $"{Number}";

    public static Fin<CardNumber> From(Seq<int> repr) =>
        repr.Count == 16 && repr.ForAll(n => n is >= 0 and <= 9)
            ? new CardNumber(repr)
            : Error.New("card number not valid");

    public static CardNumber FromUnsafe(Seq<int> repr) =>
        new (repr);

    public Seq<int> To() => 
        Number;

    public bool Equals(CardNumber? other) => 
        To() == other?.To();

    public override bool Equals(object? obj) =>
        obj is CardNumber other && Equals(other);

    public override int GetHashCode() => 
        To().GetHashCode();

    public static bool operator ==(CardNumber? left, CardNumber? right) => 
        left?.To() == right?.To();

    public static bool operator !=(CardNumber? left, CardNumber? right) => 
        !(left == right);
}
