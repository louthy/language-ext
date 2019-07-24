using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class MapOrdExtensions
    {
        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<OrdK, K, U> Map<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<V, U> mapper)
            where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Atomically maps the map to a new map
        /// </summary>
        /// <returns>Mapped items in a new map</returns>
        [Pure]
        public static Map<OrdK, K, U> Map<OrdK, K, V, U>(this Map<OrdK, K, V> self, Func<K, V, U> mapper) where OrdK : struct, Ord<K> =>
            new Map<OrdK, K, U>(MapModule.Map(self.Value.Root, mapper), self.Value.Rev);

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public static int Count<OrdK, K, V>(this Map<OrdK, K, V> self) where OrdK : struct, Ord<K> =>
            self.Count;

        [Pure]
        public static int Sum<OrdK, K>(this Map<OrdK, K, int> self) where OrdK : struct, Ord<K> =>
            self.Values.Sum();
   }
}