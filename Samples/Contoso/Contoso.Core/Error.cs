using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;

namespace Contoso.Core
{
    public class Error : NewType<Error, string>
    {
        public Error(string str) : base(str) { }
        public static implicit operator Error(string str) => New(str);
    }

    public static class ErrorExtensions
    {
        public static Error Join(this Seq<Error> errors) => string.Join("; ", errors);
    }
}
