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
                ? (new Exist<(K Key, V Value)>(head.KeyValue), new IterMapFwd<K, V>(tail))
                : (LanguageExt.Nil<(K Key, V Value)>.Default, Nil.Default);

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override Iterator<(K, V)> Using() =>
            this;
    }

    /// <summary>
    /// Map iterator keys (forward)
    /// </summary>
    internal class IterMapKeyFwd<K, V>(Map.IteratorState<K, V> items) : Iterator<K>
    {
        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? (new Exist<K>(head.KeyValue.Key), new IterMapKeyFwd<K, V>(tail))
                : (LanguageExt.Nil<K>.Default, Nil.Default);

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override Iterator<K> Using() =>
            this;
    }

    /// <summary>
    /// Map iterator values (forward)
    /// </summary>
    internal class IterMapValueFwd<K, V>(Map.IteratorState<K, V> items) : Iterator<V>
    {
        public override (Head<V> Head, Iterator<V> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? (new Exist<V>(head.KeyValue.Value), new IterMapValueFwd<K, V>(tail))
                : (Nil<V>.Default, Nil.Default);

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override Iterator<V> Using() =>
            this;
    }

    /// <summary>
    /// Map iterator (backward)
    /// </summary>
    internal class IterMapBkwd<K, V>(Map.IteratorState<K, V> items) : Iterator<(K Key, V Value)>
    {
        public override (Head<(K Key, V Value)> Head, Iterator<(K Key, V Value)> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? (new Exist<(K Key, V Value)>(head.KeyValue), new IterMapFwd<K, V>(tail))
                : (Nil<(K Key, V Value)>.Default, Nil.Default);

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override Iterator<(K, V)> Using() =>
            this;
    }

    /// <summary>
    /// Map iterator keys (backward)
    /// </summary>
    internal class IterMapKeyBkwd<K, V>(Map.IteratorState<K, V> items) : Iterator<K>
    {
        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? (new Exist<K>(head.KeyValue.Key), new IterMapKeyFwd<K, V>(tail))
                : (Nil<K>.Default, Nil.Default);

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override Iterator<K> Using() =>
            this;
    }

    /// <summary>
    /// Map iterator values (backward)
    /// </summary>
    internal class IterMapValueBkwd<K, V>(Map.IteratorState<K, V> items) : Iterator<V>
    {
        public override (Head<V> Head, Iterator<V> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? (new Exist<V>(head.KeyValue.Value), new IterMapValueFwd<K, V>(tail))
                : (Nil<V>.Default, Nil.Default);

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override Iterator<V> Using() =>
            this;
    }
}
