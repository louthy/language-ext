#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;

public static class HashMapEqExtensions
{
    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<EqK, K, V>(this HashMap<EqK, K, V> self) where EqK : struct, Eq<K> =>
        self.Count;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sum<EqK, K>(this HashMap<EqK, K, int> self) where EqK : struct, Eq<K> =>
        self.Values.Sum();

}
