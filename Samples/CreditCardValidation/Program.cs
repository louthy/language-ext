using CreditCardValidation.Control;

// This is a toy sample that demonstrates credit-card validation.  It shouldn't be considered complete,
// it simply demonstrates usage of the Validation applicative/monad for capturing multiple errors when
// doing complex validation.
//
// For more information, see Paul Louth's blog article on Validation:
// https://paullouth.com/higher-kinds-in-c-with-language-ext-part-5-validation/

Console.WriteLine(CreditCard.Validate("4560005094752584", "12-2024", "123"));
Console.WriteLine(CreditCard.Validate("00000", "00-2345", "WXYZ"));
