using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct HashableException : Hashable<Exception>
{
    [Pure]
    public static int GetHashCode(Exception x) =>
        x.GetType().GetHashCode();
}
