using System;
using Contoso.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso
{
    public static partial class Validators
    {
        public static Func<decimal, Validation<Error, decimal>> AtLeast(decimal minimum) =>
            d =>
                d >= minimum
                    ? Succ(d)
                    : Fail($"Must be greater or equal to {minimum}");

        public static Func<decimal, Validation<Error, decimal>> AtMost(decimal max) =>
            d =>
                d <= max
                    ? Succ(d)
                    : Fail($"Must be less than or equal to {max}");

        private static Validation<Error, decimal> Fail(string error) =>
            Fail<Error, decimal>(error);

        private static Validation<Error, decimal> Succ(decimal d) =>
            Success<Error, decimal>(d);

    }
}
