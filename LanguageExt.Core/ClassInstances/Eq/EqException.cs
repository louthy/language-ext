using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqException : Eq<Exception>
    {
        public int GetHashCode(Exception x) =>
            default(HashableException).GetHashCode(x);

        public bool Equals(Exception x, Exception y) =>
            (x?.Message ?? "") == (y?.Message ?? "");
    }
}
