using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Reflection;

namespace LanguageExt.ClassInstances;

public struct OrdResult<A> : Ord<Result<A>>
{
    public static int Compare(Result<A> x, Result<A> y)
    {
        if (x.IsBottom   && y.IsBottom) return 0;
        if (x.IsBottom   && !y.IsBottom) return -1;
        if (!x.IsBottom  && y.IsBottom) return 1;
        if (x.IsFaulted  && y.IsFaulted) return 0;
        if (x.IsFaulted  && !y.IsFaulted) return -1;
        if (!x.IsFaulted && y.IsFaulted) return 1;
        return OrdDefault<A>.Compare(x.Value, y.Value);
    }

    public static bool Equals(Result<A> x, Result<A> y) =>
        x.IsBottom && y.IsBottom ||
        x.IsFaulted && y.IsFaulted && EqTypeInfo.Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo()) ||
        EqDefault<A>.Equals(x.Value, y.Value);

    public static int GetHashCode(Result<A> x) =>
        x.IsBottom    ? -2
        : x.IsFaulted ? -1
                        : x.Value?.GetHashCode() ?? 0;
}

public struct OrdOptionalResult<A> : Ord<OptionalResult<A>>
{
    public static int Compare(OptionalResult<A> x, OptionalResult<A> y)
    {
        if (x.IsBottom   && y.IsBottom) return 0;
        if (x.IsBottom   && !y.IsBottom) return -1;
        if (!x.IsBottom  && y.IsBottom) return 1;
        if (x.IsFaulted  && y.IsFaulted) return 0;
        if (x.IsFaulted  && !y.IsFaulted) return -1;
        if (!x.IsFaulted && y.IsFaulted) return 1;
        return OrdOption<A>.Compare(x.Value, y.Value);
    }

    public static bool Equals(OptionalResult<A> x, OptionalResult<A> y) =>
        x.IsBottom && y.IsBottom ||
        x.IsFaulted && y.IsFaulted && EqTypeInfo.Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo()) ||
        EqOption<A>.Equals(x.Value, y.Value);

    public static int GetHashCode(OptionalResult<A> x) =>
        x.IsBottom    
            ? -2
            : x.IsFaulted 
                ? -1
                : EqOption<A>.GetHashCode(x.Value);
}
