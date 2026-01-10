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
                ? (new Exist<(K Key, V Value)>(head), new IterHashMap<EqK, K, V>(tail))
                : (Nil<(K Key, V Value)>.Default, Nil.Default);

        public override string ToString() =>
            $"HashMap{items}";

        public override Iterator<(K, V)> Using() =>
            this;
    }

    /// <summary>
    /// HashMap iterator keys
    /// </summary>
    internal class IterHashMapKey<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : Iterator<K>
        where EqK : Eq<K>
    {
        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? (new Exist<K>(head.Key), new IterHashMapKey<EqK, K, V>(tail))
                : (Nil<K>.Default, Nil.Default);

        public override string ToString() =>
            $"HashMap{items}";

        public override Iterator<K> Using() =>
            this;
    }

    /// <summary>
    /// HashMap iterator values
    /// </summary>
    internal class IterHashMapValue<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : Iterator<V>
        where EqK : Eq<K>
    {
        public override (Head<V> Head, Iterator<V> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? (new Exist<V>(head.Value), new IterHashMapValue<EqK, K, V>(tail))
                : (Nil<V>.Default, Nil.Default);

        public override string ToString() =>
            $"HashMap{items}";

        public override Iterator<V> Using() =>
            this;
    }
}
