using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct EqResult<A> : Eq<Result<A>>
    {
        [Pure]
        public static bool Equals(Result<A> x, Result<A> y) =>
            x.IsBottom && y.IsBottom ||
            x.IsFaulted && y.IsFaulted && EqTypeInfo.Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo()) ||
            EqDefault<A>.Equals(x.Value, y.Value);

        [Pure]
        public static int GetHashCode(Result<A> x) =>
            HashableResult<A>.GetHashCode(x);
    }

    public struct EqOptionalResult<A> : Eq<OptionalResult<A>>
    {
        [Pure]
        public static bool Equals(OptionalResult<A> x, OptionalResult<A> y) =>
            x.IsBottom && y.IsBottom ||
            x.IsFaulted && y.IsFaulted && EqTypeInfo.Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo()) ||
            EqOption<A>.Equals(x.Value, y.Value);

        [Pure]
        public static int GetHashCode(OptionalResult<A> x) =>
            HashableOptionalResult<A>.GetHashCode(x);
            
        [Pure]
        public static Task<bool> EqualsAsync(OptionalResult<A> x, OptionalResult<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public static Task<int> GetHashCodeAsync(OptionalResult<A> x) =>
            GetHashCode(x).AsTask();
    }
}
