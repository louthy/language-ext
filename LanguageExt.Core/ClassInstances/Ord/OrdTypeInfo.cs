using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct OrdTypeInfo : Ord<TypeInfo>
    {
        public int Compare(TypeInfo x, TypeInfo y) =>
            x.ToString().CompareTo(y.ToString());

        public bool Equals(TypeInfo x, TypeInfo y) =>
            default(EqTypeInfo).Equals(x, y);

        public int GetHashCode(TypeInfo x) =>
            default(EqTypeInfo).GetHashCode(x);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(TypeInfo x, TypeInfo y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(TypeInfo x) =>
            GetHashCode(x).AsTask();         
          
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(TypeInfo x, TypeInfo y) =>
            Compare(x, y).AsTask();     
    }
}
