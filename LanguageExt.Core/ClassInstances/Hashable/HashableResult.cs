using LanguageExt.TypeClasses;
using System.Reflection;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct HashableResult<A> : Hashable<Result<A>>
    {
        public int GetHashCode(Result<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : x.Value?.GetHashCode() ?? 0;
    }

    public struct HashableOptionalResult<A> : Hashable<OptionalResult<A>>
    {
        public int GetHashCode(OptionalResult<A> x) =>
            x.IsBottom ? -2
          : x.IsFaulted ? -1
          : default(HashableOption<A>).GetHashCode(x.Value);
    }
}
