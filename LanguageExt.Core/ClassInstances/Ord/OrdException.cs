using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdException : Ord<Exception>
    {
        public int GetHashCode(Exception x) =>
            default(HashableException).GetHashCode(x);

        public bool Equals(Exception x, Exception y) =>
            default(EqException).Equals(x, y);

        public int Compare(Exception x, Exception y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;
            return x.Message.CompareTo(y.Message);
        }
       
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Exception x, Exception y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Exception x) =>
            GetHashCode(x).AsTask();        
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Exception x, Exception y) =>
            Compare(x, y).AsTask();    
    }
}
