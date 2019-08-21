using LanguageExt.TypeClasses;
using System.Reflection;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct EqResult<A> : Eq<Result<A>>
    {
        public bool Equals(Result<A> x, Result<A> y) =>
            (x.IsBottom && y.IsBottom) ||
            (x.IsFaulted && y.IsFaulted && default(EqTypeInfo).Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo())) ||
            (default(EqDefault<A>).Equals(x.Value, y.Value));

        public int GetHashCode(Result<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : x.Value?.GetHashCode() ?? 0;
    }

    public struct EqOptionalResult<A> : Eq<OptionalResult<A>>
    {
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
