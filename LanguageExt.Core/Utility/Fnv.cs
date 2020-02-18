using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Fowler–Noll–Vo 32bit hash function
    /// </summary>
    /// <remarks>
    /// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
    /// </remarks>
    internal static class FNV32
    {
        /// <summary>
        /// Offset basis for a FNV-1 or FNV-1a 32 bit hash
        /// </summary>
        public const int OffsetBasis = -2128831035;

        /// <summary>
        /// Prime for FNV-1 or FNV-1a 32 bit hash
        /// </summary>
        public const int Prime = 16777619;

        /// <summary>
        /// Calculate the hash code for an array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash<HashA, A>(A[] items, int offsetBasis = OffsetBasis) where HashA : struct, Hashable<A>
        {
            int hash = offsetBasis;
            if (items == null) return hash;

            unchecked
            {
                Span<A> span = items;
                foreach (var item in span)
                {
                    hash = (default(HashA).GetHashCode(item) ^ hash) * Prime;
                }
                return hash;
            }
        }

        /// <summary>
        /// Calculate the hash code for an array slice
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash<HashA, A>(A[] items, int start, int length, int offsetBasis = OffsetBasis) where HashA : struct, Hashable<A>
        {
            int hash = offsetBasis;
            if (items == null) return hash;

            unchecked
            {
                var span = new Span<A>(items, start, length);
                foreach (var item in span)
                {
                    hash = (default(HashA).GetHashCode(item) ^ hash) * Prime;
                }
                return hash;
            }
        }

        /// <summary>
        /// Calculate the hash code for an enumerable
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash<HashA, A>(IEnumerable<A> items, int offsetBasis = OffsetBasis) where HashA : struct, Hashable<A>
        {
            int hash = offsetBasis;
            if (items == null) return hash;

            unchecked
            {
                foreach (var item in items)
                {
                    hash = (default(HashA).GetHashCode(item) ^ hash) * Prime;
                }
                return hash;
            }
        }
    }
}
