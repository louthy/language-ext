using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Calculate a hash-code for an enumerable
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(IEnumerable<A> xs) =>
            FNV32.Hash<HashableDefault<A>, A>(xs);

        /// <summary>
        /// Calculate a hash-code for an enumerable by using the 
        /// Hashable class-instance to calculate each item's hash
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<HashA, A>(IEnumerable<A> xs) where HashA : struct, Hashable<A> =>
            FNV32.Hash<HashA, A>(xs);

        //
        //  The following overloads are to avoid boxing when using the 
        //  hash function, perhaps as a first-class function for a map
        //

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(Arr<A> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<K, V>(HashMap<K, V> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<EqK, K, V>(HashMap<EqK, K, V> xs) where EqK : struct, Eq<K> =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(HashSet<A> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<EqA, A>(HashSet<EqA, A> xs) where EqA : struct, Eq<A> =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(Lst<A> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<Pred, A>(Lst<Pred, A> xs) where Pred : struct, Pred<ListInfo> =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<PredList, PredItem, A>(Lst<PredList, PredItem, A> xs) 
            where PredList : struct, Pred<ListInfo>
            where PredItem : struct, Pred<A> =>
                xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<K, V>(Map<K, V> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<OrdK, K, V>(Map<OrdK, K, V> xs) where OrdK : struct, Ord<K> =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(Que<A> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(Seq<A> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(Set<A> xs) =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<OrdA, A>(Set<OrdA, A> xs) where OrdA : struct, Ord<A> =>
            xs.GetHashCode();

        /// <summary>
        /// Calculate a hash-code for the collection provided
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int hash<A>(Stck<A> xs) =>
            xs.GetHashCode();
    }
}
