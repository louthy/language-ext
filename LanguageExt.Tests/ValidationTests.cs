using System;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using Xunit;

namespace LanguageExt.Tests
{
    public class Error : NewType<Error, string>
    {
        public Error(string e) : base(e) { }
    }

    public class ValidationTests
    {
        [Fact]
        public void ValidCreditCardTest()
        {
            // Valid test
            var res = ValidateCreditCard("Paul", "1234567891012345", "10", "2020");

            res.Match(
                Succ: cc =>
                {
                    Assert.True(cc.CardHolder == "Paul");
                    Assert.True(cc.Month == 10);
                    Assert.True(cc.Year == 2020);
                    Assert.True(cc.Number == "1234567891012345");
                },
                Fail: err => Assert.True(false, "should never get here"));
        }

        [Fact]
        public void InValidCreditCardNumberTest()
        {
            var res = ValidateCreditCard("Paul", "ABCDEF567891012345", "10", "2020");
            Assert.True(res.IsFail);

            res.Match(
                Succ: _ => Assert.True(false, "should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 2);
                    Assert.True(errors.Head.Value == "only numbers are allowed");
                    Assert.True(errors.Tail.Head.Value == "can not exceed 16 characters");
                });
        }

        [Fact]
        public void ExpiredAndInValidCreditCardNumberTest()
        {
            var res = ValidateCreditCard("Paul", "ABCDEF567891012345", "1", "2001");
            Assert.True(res.IsFail);

            res.Match(
                Succ: _ => Assert.True(false, "should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 3);
                    Assert.True(errors.Head.Value == "only numbers are allowed");
                    Assert.True(errors.Tail.Head.Value == "can not exceed 16 characters");
                    Assert.True(errors.Tail.Tail.Head.Value == "card has expired");
                });
        }

        /// <summary>
        /// Validates the string has only ASCII characters
        /// </summary>
        public static Validation<Error, string> AsciiOnly(string str) =>
            str.ForAll(c => c <= 0x7f)
                ? Success<Error, string>(str)
                : Fail<Error, string>(Error.New("only ascii characters are allowed"));

        /// <summary>
        /// Creates a delegate that when passed a string will validate that it's below
        /// a specific length
        /// </summary>
        public static Func<string, Validation<Error, string>> MaxStrLength(int max) =>
            str =>
                str.Length <= max
                    ? Success<Error, string>(str)
                    : Fail<Error, string>(Error.New($"can not exceed {max} characters"));

        /// <summary>
        /// Validates that the string passed contains only digits
        /// </summary>
        public static Validation<Error, string> DigitsOnly(string str) =>
            str.ForAll(Char.IsDigit)
                ? Success<Error, string>(str)
                : Fail<Error, string>(Error.New($"only numbers are allowed"));

        /// <summary>
        /// Uses parseInt which returns an Option and converts it to a Validation
        /// value with a default Error if the parse fails
        /// </summary>
        public static Validation<Error, int> ToInt(string str) =>
            parseInt(str).ToValidation(Error.New("must be a number"));

        /// <summary>
        /// Validates that the value passed is a month
        /// </summary>
        public static Validation<Error, int> ValidMonth(int month) =>
            month >= 1 && month <= 12
                ? Success<Error, int>(month)
                : Fail<Error, int>(Error.New($"invalid month"));

        /// <summary>
        /// Validates that the value passed is a positive number
        /// </summary>
        public static Validation<Error, int> PositiveNumber(int value) =>
            value > 0
                ? Success<Error, int>(value)
                : Fail<Error, int>(Error.New($"must be positive"));

        /// <summary>
        /// Takes todays date and builds a delegate that can take a month and year
        /// to see if the credit card has expired.
        /// </summary>
        public static Func<int, int, Validation<Error, (int month, int year)>> ValidExpiration(int currentMonth, int currentYear) =>
            (month, year) =>
                year > currentYear || (year == currentYear && month >= currentMonth)
                    ? Success<Error, (int, int)>((month, year))
                    : Fail<Error, (int, int)>(Error.New($"card has expired"));

        /// <summary>
        /// Validate that the card holder is ASCII and has a maximum of 30 characters
        /// This uses the | operator as a disjunction computation.  If any items are
        /// Failed then the errors are collected and returned.  If they all pass then
        /// the Success value from the first item is propagated.  This only works when
        /// all the operands are of the same type and you only care about the first
        /// success value.  Which in this case is cardHolder for both.
        /// </summary>
        public static Validation<Error, string> ValidateCardHolder(string cardHolder) =>
            AsciiOnly(cardHolder) | MaxStrLength(30)(cardHolder);

        /// <summary>
        /// This is the main validation function for validating a credit card
        /// </summary>
        public static Validation<Error, CreditCard> ValidateCreditCard(string cardHolder, string number, string expMonth, string expYear)
        {
            var cardHolderV = ValidateCardHolder(cardHolder);
            var numberV = DigitsOnly(number) | MaxStrLength(16)(number);
            var validToday = ValidExpiration(DateTime.Now.Month, DateTime.Now.Year);

            // This falls back to monadic behaviour because validToday needs both
            // a month and year to continue.  
            var monthYear = from m in ToInt(expMonth).Bind(ValidMonth)
                            from y in ToInt(expYear).Bind(PositiveNumber)
                            from my in validToday(m, y)
                            select my;

            // The items to validate are placed in a tuple, then you call apply to
            // confirm that all items have passed the validation.  If not then all
            // the errors are collected.  If they have passed then the results are
            // passed to the lambda function allowing the creation of a the 
            // CreditCard object.
            return (cardHolderV, numberV, monthYear).Apply((c, num, my) => new CreditCard(c, num, my.month, my.year));
        }

        public class CreditCard
        {
            public readonly string CardHolder;
            public readonly string Number;
            public readonly int Month;
            public readonly int Year;

            public CreditCard(string c, string num, int month, int year)
            {
                CardHolder = c;
                Number = num;
                Month = month;
                Year = year;
            }
        }
    }
}
