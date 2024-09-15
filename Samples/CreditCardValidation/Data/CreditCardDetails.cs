// This is a toy sample that demonstrates credit-card validation.  It shouldn't be considered complete,
// it simply demonstrates usage of the Validation applicative/monad for capturing multiple errors when
// doing complex validation.
//
// For more information, see Paul Louth's blog article on Validation:
// https://paullouth.com/higher-kinds-in-c-with-language-ext-part-5-validation/

namespace CreditCardValidation.Data;

/// <summary>
/// Complete credit card details
/// </summary>
/// <param name="CardNumber">Credit card number</param>
/// <param name="Expiry">Expiry date</param>
/// <param name="CVV">Card security code</param>
public record CreditCardDetails(CardNumber CardNumber, Expiry Expiry, CVV CVV)
{
    public static CreditCardDetails Make(CardNumber cardNo, Expiry expiry, CVV cvv) =>
        new (cardNo, expiry, cvv);
    
    public override string ToString() =>
        $"CreditCard({CardNumber}, {Expiry}, {CVV})";
}
