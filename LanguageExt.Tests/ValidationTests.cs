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
            var res1 = ValidateCreditCard("Paul", "1234567891012345", "10", "2020");
            Assert.True(res1.IsSuccess);
        }

        [Fact]
        public void InValidCreditCardNumberTest()
        {
            // Invalid crea
            var res2 = ValidateCreditCard("Paul", "ABCDEF567891012345", "10", "2020");
            Assert.True(res2.IsFail);

            res2.Match(
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
            // Invalid crea
            var res2 = ValidateCreditCard("Paul", "ABCDEF567891012345", "1", "2001");
            Assert.True(res2.IsFail);

            res2.Match(
                Succ: _ => Assert.True(false, "should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 3);
                    Assert.True(errors.Head.Value == "only numbers are allowed");
                    Assert.True(errors.Tail.Head.Value == "can not exceed 16 characters");
                    Assert.True(errors.Tail.Tail.Head.Value == "card has expired");
                });
        }

        public static Validation<Error, string> AsciiOnly(string str) =>
            str.ForAll(c => c <= 0x7f)
                ? Success<Error, string>(str)
                : Fail<Error, string>(Error.New("only ascii characters are allowed"));

        public static Func<string, Validation<Error, string>> MaxStrLength(int max) =>
            str =>
                str.Length <= max
                    ? Success<Error, string>(str)
                    : Fail<Error, string>(Error.New($"can not exceed {max} characters"));

        public static Validation<Error, string> DigitsOnly(string str) =>
            str.ForAll(Char.IsDigit)
                ? Success<Error, string>(str)
                : Fail<Error, string>(Error.New($"only numbers are allowed"));

        public static Validation<Error, int> ToInt(string str) =>
            parseInt(str).ToValidation(Error.New("must be a number"));

        public static Validation<Error, int> ValidMonth(int month) =>
            month >= 1 && month <= 12
                ? Success<Error, int>(month)
                : Fail<Error, int>(Error.New($"invalid month"));

        public static Validation<Error, int> PositiveNumber(int value) =>
            value > 0
                ? Success<Error, int>(value)
                : Fail<Error, int>(Error.New($"must be positive"));

        public static Func<int, int, Validation<Error, (int month, int year)>> ValidExpiration(int currentMonth, int currentYear) =>
            (month, year) =>
                year > currentYear || (year == currentYear && month >= currentMonth)
                    ? Success<Error, (int, int)>((month, year))
                    : Fail<Error, (int, int)>(Error.New($"card has expired"));

        public static Validation<Error, string> ValidateCardHolder(string cardHolder) =>
            (AsciiOnly(cardHolder), MaxStrLength(10)(cardHolder)).Apply((s, _) => s);

        public static Validation<Error, CreditCard> ValidateCreditCard(string cardHolder, string number, string expMonth, string expYear)
        {
            var cardHolderV = ValidateCardHolder(cardHolder);
            var numberV = DigitsOnly(number) | MaxStrLength(16)(number);
            var validToday = ValidExpiration(DateTime.Now.Month, DateTime.Now.Year);

            var monthYear = from m in ToInt(expMonth).Bind(ValidMonth)
                            from y in ToInt(expYear).Bind(PositiveNumber)
                            from my in validToday(m, y)
                            select my;

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
