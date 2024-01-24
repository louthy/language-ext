using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct EqException : Eq<Exception>
{
    [Pure]
    public static int GetHashCode(Exception x) =>
        HashableException.GetHashCode(x);

    [Pure]
    public static bool Equals(Exception x, Exception y) =>
        x.GetType() == y.GetType();
}
