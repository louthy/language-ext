using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Reflection;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqTypeInfo : Eq<TypeInfo>
    {
        [Pure]
        public bool Equals(TypeInfo x, TypeInfo y) =>
            x.Equals(y);

        [Pure]
        public int GetHashCode(TypeInfo x) =>
            default(HashableTypeInfo).GetHashCode(x);
 
        [Pure]
        public Task<bool> EqualsAsync(TypeInfo x, TypeInfo y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(TypeInfo x) =>
            GetHashCode(x).AsTask();
    }
}
