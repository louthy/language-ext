#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    /// <summary>
    /// HashMap IteratorIO
    /// </summary>
    internal class IterHashMap<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : IteratorIO<(K Key, V Value)>
        where EqK : Eq<K>
    {
        (Head<(K Key, V Value)> Head, IteratorIO<(K Key, V Value)> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head, new IterHashMap<EqK, K, V>(tail))
                : Head.NilIO<(K Key, V Value)>();

        public override IO<(Head<(K Key, V Value)> Head, IteratorIO<(K Key, V Value)> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"HashMap{items}";

        public override IteratorIO<(K, V)> Using() =>
            this;
    }

    /// <summary>
    /// HashMap IteratorIO keys
    /// </summary>
    internal class IterHashMapKey<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : IteratorIO<K>
        where EqK : Eq<K>
    {
        (Head<K> Head, IteratorIO<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.Key, new IterHashMapKey<EqK, K, V>(tail))
                : Head.NilIO<K>();

        public override IO<(Head<K> Head, IteratorIO<K> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"HashMap{items}";

        public override IteratorIO<K> Using() =>
            this;
    }

    /// <summary>
    /// HashMap IteratorIO values
    /// </summary>
    internal class IterHashMapValue<EqK, K, V>(TrieMap.IteratorState<EqK, K, V> items) : IteratorIO<V>
        where EqK : Eq<K>
    {
        (Head<V> Head, IteratorIO<V> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.Value, new IterHashMapValue<EqK, K, V>(tail))
                : Head.NilIO<V>();

        public override IO<(Head<V> Head, IteratorIO<V> Tail)> NextIO() =>
            IO.pure(Next());

        public override string ToString() =>
            $"HashMap{items}";

        public override IteratorIO<V> Using() =>
            this;
    }
}
