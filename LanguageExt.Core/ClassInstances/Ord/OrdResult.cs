using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    public struct OrdResult<A> : Ord<Result<A>>
    {
        public int Compare(Result<A> x, Result<A> y)
        {
            if (x.IsBottom && y.IsBottom) return 0;
            if (x.IsBottom && !y.IsBottom) return -1;
            if (!x.IsBottom && y.IsBottom) return 1;
            if (x.IsFaulted && y.IsFaulted) return 0;
            if (x.IsFaulted && !y.IsFaulted) return -1;
            if (!x.IsFaulted && y.IsFaulted) return 1;
            return default(OrdDefault<A>).Compare(x.Value, y.Value);
        }

        public bool Equals(Result<A> x, Result<A> y) =>
            (x.IsBottom && y.IsBottom) ||
            (x.IsFaulted && y.IsFaulted && default(EqTypeInfo).Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo())) ||
            (default(EqDefault<A>).Equals(x.Value, y.Value));

        public int GetHashCode(Result<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : x.Value?.GetHashCode() ?? 0;
    }

    public struct OrdOptionalResult<A> : Ord<OptionalResult<A>>
    {
        public int Compare(OptionalResult<A> x, OptionalResult<A> y)
        {
            if (x.IsBottom && y.IsBottom) return 0;
            if (x.IsBottom && !y.IsBottom) return -1;
            if (!x.IsBottom && y.IsBottom) return 1;
            if (x.IsFaulted && y.IsFaulted) return 0;
            if (x.IsFaulted && !y.IsFaulted) return -1;
            if (!x.IsFaulted && y.IsFaulted) return 1;
            return default(OrdOption<A>).Compare(x.Value, y.Value);
        }

        public bool Equals(OptionalResult<A> x, OptionalResult<A> y) =>
            (x.IsBottom && y.IsBottom) ||
            (x.IsFaulted && y.IsFaulted && default(EqTypeInfo).Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo())) ||
            (default(EqOption<A>).Equals(x.Value, y.Value));

        public int GetHashCode(OptionalResult<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : default(EqOption<A>).GetHashCode(x.Value);
    }
}
