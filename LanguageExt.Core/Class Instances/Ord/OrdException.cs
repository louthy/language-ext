using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

public struct OrdException : Ord<Exception>
{
    [Pure]
    public static int GetHashCode(Exception x) =>
        HashableException.GetHashCode(x);

    [Pure]
    public static bool Equals(Exception x, Exception y) =>
        EqException.Equals(x, y);

    [Pure]
    public static int Compare(Exception x, Exception y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(x, null)) return -1;
        if (ReferenceEquals(y, null)) return 1;
        return string.Compare(x.GetType().FullName, y.Message.GetType().FullName, StringComparison.Ordinal);
    }
}
