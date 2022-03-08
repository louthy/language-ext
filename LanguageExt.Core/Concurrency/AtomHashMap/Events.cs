using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Event sent from the `AtomHashMap` type whenever an operation successfully modifies the underlying data 
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public delegate void AtomHashMapChangeEvent<K, V>(
        HashMap<K, V> Previous, 
        HashMap<K, V> Current,
        HashMap<K, Change<V>> Changes); 
    /// <summary>
    /// Event sent from the `AtomHashMap` type whenever an operation successfully modifies the underlying data 
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public delegate void AtomHashMapChangeEvent<EqK, K, V>(
        HashMap<EqK, K, V> Previous, 
        HashMap<EqK, K, V> Current,
        HashMap<EqK, K, Change<V>> Changes)
        where EqK : struct, Eq<K>; 
}
