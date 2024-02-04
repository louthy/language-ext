using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Reflection;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct EqResult<A> : Eq<Result<A>>
{
    [Pure]
    public static bool Equals(Result<A> x, Result<A> y) =>
        x.IsBottom && y.IsBottom ||
        x.IsFaulted && y.IsFaulted && EqTypeInfo.Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo()) ||
        EqDefault<A>.Equals(x.Value!, y.Value!);

    [Pure]
    public static int GetHashCode(Result<A> x) =>
        HashableResult<A>.GetHashCode(x);
}
