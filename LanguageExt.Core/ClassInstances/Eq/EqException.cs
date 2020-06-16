using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqException : Eq<Exception>
    {
        public int GetHashCode(Exception x) =>
            default(HashableException).GetHashCode(x);

        public bool Equals(Exception x, Exception y) =>
            (x?.Message ?? "") == (y?.Message ?? "");
  
        [Pure]
        public Task<bool> EqualsAsync(Exception x, Exception y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Exception x) => 
            GetHashCode(x).AsTask();      
    }
}
