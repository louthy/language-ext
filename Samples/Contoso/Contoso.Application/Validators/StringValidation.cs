using System;
using Contoso.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso
{
    public static partial class Validators
    {
        public static Func<string, Validation<Error, string>> NotLongerThan(int maxLength) =>
            str => Optional(str)
                .Where(s => s.Length <= maxLength)
                .ToValidation<Error>($"{str} must not be longer than {maxLength}");

        public static Validation<Error, string> NotEmpty(string str) =>
            Optional(str)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToValidation<Error>("Empty string");
    }
}
