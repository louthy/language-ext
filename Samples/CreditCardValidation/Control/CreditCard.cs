// This is a toy sample that demonstrates credit-card validation.  It shouldn't be considered complete,
// it simply demonstrates usage of the Validation applicative/monad for capturing multiple errors when
// doing complex validation.
//
// For more information, see Paul Louth's blog article on Validation:
// https://paullouth.com/higher-kinds-in-c-with-language-ext-part-5-validation/

using System.Numerics;
using CreditCardValidation.Data;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace CreditCardValidation.Control;

public static class CreditCard
{
    public static Validation<Error, CreditCardDetails> Validate(string cardNo, string expiryDate, string cvv) =>
        fun<CardNumber, Expiry, CVV, CreditCardDetails>(CreditCardDetails.Make)
           .Map(ValidateCardNumber(cardNo))
           .Apply(ValidateExpiryDate(expiryDate))
           .Apply(ValidateCVV(cvv))
           .As();

    static Validation<Error, CardNumber> ValidateCardNumber(string cardNo) =>
        (ValidateAllDigits(cardNo), ValidateLength(cardNo, 16))
            .Apply((digits, _) => digits.ToSeq())
            .Bind(ValidateLuhn)
            .Map(CardNumber.FromUnsafe)
            .As()
            .MapFail(e => Error.New("card number not valid", e));    

    static Validation<Error, Expiry> ValidateExpiryDate(string expiryDate) =>
        expiryDate.Split('\\', '/', '-', ' ') switch
        {
            [var month, var year] =>
                from my  in ValidateInt(month) & ValidateInt(year)
                from exp in Expiry.From(my[0], my[1]).ToValidation()
                from _   in ValidateInRange(exp, Expiry.NextTenYears)
                select exp,
            
            _ => Fail(Error.New($"expected expiry-date in the format: MM/YYYY, but got: {expiryDate}"))
        };

    static Validation<Error, A> ValidateInRange<A>(A value, Range<A> range)
        where A : IComparisonOperators<A, A, bool> =>
        range.InRange(value)
            ? Pure(value)
            : Fail(Error.New($"expected value in range of {range.From} to {range.To}, but got: {value}"));

    static Validation<Error, CVV> ValidateCVV(string cvv) =>
        fun<int, string, CVV>((code, _) => new CVV(code))
           .Map(ValidateInt(cvv).MapFail(_ => Error.New("CVV code should be a number")))
           .Apply(ValidateLength(cvv, 3).MapFail(_ => Error.New("CVV code should be 3 digits in length")))
           .As(); 

    static Validation<Error, Iterable<int>> ValidateAllDigits(string value) =>
        value.AsIterable()
             .Traverse(CharToDigit)
             .As();

static Validation<Error, int> ValidateInt(string value) =>
    ValidateAllDigits(value).Map(_ => int.Parse(value));

    static Validation<Error, string> ValidateLength(string value, int length) =>
        ValidateLength(value.AsIterable(), length)
            .Map(_ => value);

    static Validation<Error, K<F, A>> ValidateLength<F, A>(K<F, A> fa, int length)
        where F : Foldable<F> =>
        fa.Count == length
            ? Pure(fa)
            : Fail(Error.New($"expected length to be {length}, but got: {fa.Count}"));

    static Validation<Error, int> CharToDigit(char ch) =>
        ch is >= '0' and <= '9'
            ? Pure(ch - '0')
            : Fail(Error.New($"expected a digit, but got: {ch}"));
    
    static Validation<Error, Seq<int>> ValidateLuhn(Seq<int> digits)
    {
        var checkDigit = 0;
        for (var i = digits.Length - 2; i >= 0; --i)
        {
            checkDigit += ((i & 1) is 0) switch
                          {
                              true  => digits[i] > 4 ? digits[i] * 2 - 9 : digits[i] * 2,
                              false => digits[i]
                          };
        }

        return (10 - checkDigit % 10) % 10 == digits.Last
                   ? Pure(digits)
                   : Fail(Error.New("invalid card number"));
    }
}
