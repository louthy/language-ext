using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Error monoid
    /// </summary>
    public readonly struct MError : Monoid<Error>
    {
        [Pure]
        public Error Append(Error x, Error y) => x + y;

        [Pure]
        public Error Empty() => Errors.None;
    }
}
