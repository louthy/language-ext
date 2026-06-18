#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class HashMapExtensions
{
    public static HashMap<Key, V> As<Key, V>(this K<HashMap<Key>, V> ma) =>
        (HashMap<Key, V>)ma;

    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<(K Key, V Value)> AsQueryable<K, V>(this HashMap<K, V> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        source.Value.AsQueryable();
}
