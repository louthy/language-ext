using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static class HashMapEqExtensions
{
    /// <summary>
    /// Number of items in the map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<EqK, K, V>(this HashMap<EqK, K, V> self) where EqK : Eq<K> =>
        self.Count;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sum<EqK, K>(this HashMap<EqK, K, int> self) where EqK : Eq<K> =>
        self.Values.Sum();

}
