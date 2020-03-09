using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct HashableException : Hashable<Exception>
    {
        [Pure]
        public int GetHashCode(Exception x) =>
            x?.Message?.GetHashCode() ?? 0;

        [Pure]
        public Task<int> GetHashCodeAsync(Exception x) =>
            GetHashCode(x).AsTask();
    }
}
