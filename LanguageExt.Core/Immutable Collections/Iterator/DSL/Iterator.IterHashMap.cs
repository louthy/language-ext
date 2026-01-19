#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class Iterator
{
    /// <summary>
    /// HashMap iterator
    /// </summary>
    internal class IterHashMap<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : Iterator<(K Key, V Value)>
        where EqK : Eq<K>
    {
        public override (Head<(K Key, V Value)> Head, Iterator<(K Key, V Value)> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head, new IterHashMap<EqK, K, V>(tail))
                : Head.Nil<(K Key, V Value)>();

        public override string ToString() =>
            $"HashMap{items}";
    }

    /// <summary>
    /// HashMap iterator keys
    /// </summary>
    internal class IterHashMapKey<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : Iterator<K>
        where EqK : Eq<K>
    {
        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.Key, new IterHashMapKey<EqK, K, V>(tail))
                : Head.Nil<K>();

        public override string ToString() =>
            $"HashMap{items}";
    }

    /// <summary>
    /// HashMap iterator values
    /// </summary>
    internal class IterHashMapValue<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : Iterator<V>
        where EqK : Eq<K>
    {
        public override (Head<V> Head, Iterator<V> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.Value, new IterHashMapValue<EqK, K, V>(tail))
                : Head.Nil<V>();

        public override string ToString() =>
            $"HashMap{items}";
    }
}
