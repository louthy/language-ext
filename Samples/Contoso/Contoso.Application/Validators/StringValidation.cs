using System;
using Contoso.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso.Application.Validators
{
    public static class StringValidation
    {
        public static Func<string, Validation<Error, string>> MaxStringLength(int maxLength) =>
            str =>
                str.Length > maxLength
                    ? Fail($"{str} must not be longer than {maxLength}")
                    : Succ(str);

        public static Validation<Error, string> NotEmpty(string str) =>
            string.IsNullOrEmpty(str)
                ? Fail("Must not be empty")
                : Succ(str);

        private static Validation<Error, string> Fail(string error) =>
            Fail<Error, string>(error);

        private static Validation<Error, string> Succ(string str) =>
            Success<Error, string>(str);
    }
}
