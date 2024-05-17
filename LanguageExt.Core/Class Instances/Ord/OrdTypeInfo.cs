using System;
using LanguageExt.Traits;
using System.Reflection;

namespace LanguageExt.ClassInstances;

public struct OrdTypeInfo : Ord<TypeInfo>
{
    public static int Compare(TypeInfo x, TypeInfo y) =>
        string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);

    public static bool Equals(TypeInfo x, TypeInfo y) =>
        EqTypeInfo.Equals(x, y);

    public static int GetHashCode(TypeInfo x) =>
        EqTypeInfo.GetHashCode(x);
}
