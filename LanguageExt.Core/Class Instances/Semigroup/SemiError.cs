using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Error semigroup
    /// </summary>
    public readonly struct SemiError : Semigroup<Error>
    {
        [Pure]
        public Error Append(Error x, Error y) => x + y;
    }
}
