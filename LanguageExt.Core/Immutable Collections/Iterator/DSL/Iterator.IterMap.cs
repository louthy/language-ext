using System;

namespace LanguageExt;

public abstract partial class Iterator
{
    /// <summary>
    /// Map iterator (forward)
    /// </summary>
    internal class IterMapFwd<K, V>(Map.IteratorState<K, V> items) : Iterator<(K Key, V Value)>
    {
        public override (Head<(K Key, V Value)> Head, Iterator<(K Key, V Value)> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.KeyValue, new IterMapFwd<K, V>(tail))
                : Head.Nil<(K, V)>();

        public override string ToString() =>
            $"Map{items.ToString()}";
    }

    /// <summary>
    /// Map iterator keys (forward)
    /// </summary>
    internal class IterMapKeyFwd<K, V>(Map.IteratorState<K, V> items) : Iterator<K>
    {
        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.KeyValue.Key, new IterMapKeyFwd<K, V>(tail))
                : Head.Nil<K>();

        public override string ToString() =>
            $"Map{items.ToString()}";
    }

    /// <summary>
    /// Map iterator values (forward)
    /// </summary>
    internal class IterMapValueFwd<K, V>(Map.IteratorState<K, V> items) : Iterator<V>
    {
        public override (Head<V> Head, Iterator<V> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.KeyValue.Value, new IterMapValueFwd<K, V>(tail))
                : Head.Nil<V>();

        public override string ToString() =>
            $"Map{items.ToString()}";
    }

    /// <summary>
    /// Map iterator (backward)
    /// </summary>
    internal class IterMapBkwd<K, V>(Map.IteratorState<K, V> items) : Iterator<(K Key, V Value)>
    {
        public override (Head<(K Key, V Value)> Head, Iterator<(K Key, V Value)> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.Exist(head.KeyValue, new IterMapBkwd<K, V>(tail))
                : Head.Nil<(K, V)>();

        public override string ToString() =>
            $"Map{items.ToString()}";
    }

    /// <summary>
    /// Map iterator keys (backward)
    /// </summary>
    internal class IterMapKeyBkwd<K, V>(Map.IteratorState<K, V> items) : Iterator<K>
    {
        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.Exist(head.KeyValue.Key, new IterMapKeyBkwd<K, V>(tail))
                : Head.Nil<K>();

        public override string ToString() =>
            $"Map{items.ToString()}";
    }

    /// <summary>
    /// Map iterator values (backward)
    /// </summary>
    internal class IterMapValueBkwd<K, V>(Map.IteratorState<K, V> items) : Iterator<V>
    {
        public override (Head<V> Head, Iterator<V> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.Exist(head.KeyValue.Value, new IterMapValueBkwd<K, V>(tail))
                : Head.Nil<V>();

        public override string ToString() =>
            $"Map{items.ToString()}";
    }
}
