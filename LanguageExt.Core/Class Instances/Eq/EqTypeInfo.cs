using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Reflection;

namespace LanguageExt.ClassInstances;

public struct EqTypeInfo : Eq<TypeInfo>
{
    [Pure]
    public static bool Equals(TypeInfo x, TypeInfo y) =>
        x.Equals(y);

    [Pure]
    public static int GetHashCode(TypeInfo x) =>
        HashableTypeInfo.GetHashCode(x);
}
