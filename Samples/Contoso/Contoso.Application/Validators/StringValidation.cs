using System;
using Contoso.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso
{
    public static partial class Validators
    {
        public static Func<string, Validation<Error, string>> NotLongerThan(int maxLength) =>
            str =>
                str.Length > maxLength
                    ? StringHelper.Fail($"{str} must not be longer than {maxLength}")
                    : StringHelper.Succ(str);

        public static Validation<Error, string> NotEmpty(string str) =>
            string.IsNullOrEmpty(str)
                ? StringHelper.Fail("Must not be empty")
                : StringHelper.Succ(str);

        private static class StringHelper
        {
            public static Validation<Error, string> Fail(string error) =>
                Fail<Error, string>(error);

            public static Validation<Error, string> Succ(string str) =>
                Success<Error, string>(str);
        }
    }
}
