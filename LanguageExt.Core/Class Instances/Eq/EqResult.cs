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
        public bool Equals(Result<A> x, Result<A> y) =>
            (x.IsBottom && y.IsBottom) ||
            (x.IsFaulted && y.IsFaulted && default(EqTypeInfo).Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo())) ||
            (default(EqDefault<A>).Equals(x.Value, y.Value));

        [Pure]
        public int GetHashCode(Result<A> x) =>
            default(HashableResult<A>).GetHashCode(x);
        
        [Pure]
        public Task<bool> EqualsAsync(Result<A> x, Result<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Result<A> x) =>
            GetHashCode(x).AsTask();
    }

    public struct EqOptionalResult<A> : Eq<OptionalResult<A>>
    {
        [Pure]
        public bool Equals(OptionalResult<A> x, OptionalResult<A> y) =>
            (x.IsBottom && y.IsBottom) ||
            (x.IsFaulted && y.IsFaulted && default(EqTypeInfo).Equals(x.Exception.GetType().GetTypeInfo(), y.Exception.GetType().GetTypeInfo())) ||
            (default(EqOption<A>).Equals(x.Value, y.Value));

        [Pure]
        public int GetHashCode(OptionalResult<A> x) =>
            default(HashableOptionalResult<A>).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(OptionalResult<A> x, OptionalResult<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(OptionalResult<A> x) =>
            GetHashCode(x).AsTask();
    }
}
