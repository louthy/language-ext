using System;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    /// <summary>
    /// Map IteratorIO (forward)
    /// </summary>
    internal class IterMapFwd<K, V>(Map.IteratorState<K, V> items) : IteratorIO<(K Key, V Value)>
    {
        (Head<(K Key, V Value)> Head, IteratorIO<(K Key, V Value)> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.KeyValue, new IterMapFwd<K, V>(tail))
                : Head.NilIO<(K, V)>();

        public override IO<(Head<(K Key, V Value)> Head, IteratorIO<(K Key, V Value)> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override IteratorIO<(K, V)> Using() =>
            this;
    }

    /// <summary>
    /// Map IteratorIO keys (forward)
    /// </summary>
    internal class IterMapKeyFwd<K, V>(Map.IteratorState<K, V> items) : IteratorIO<K>
    {
        (Head<K> Head, IteratorIO<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.KeyValue.Key, new IterMapKeyFwd<K, V>(tail))
                : Head.NilIO<K>();

        public override IO<(Head<K> Head, IteratorIO<K> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override IteratorIO<K> Using() =>
            this;
    }

    /// <summary>
    /// Map IteratorIO values (forward)
    /// </summary>
    internal class IterMapValueFwd<K, V>(Map.IteratorState<K, V> items) : IteratorIO<V>
    {
        (Head<V> Head, IteratorIO<V> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.KeyValue.Value, new IterMapValueFwd<K, V>(tail))
                : Head.NilIO<V>();

        public override IO<(Head<V> Head, IteratorIO<V> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override IteratorIO<V> Using() =>
            this;
    }

    /// <summary>
    /// Map IteratorIO (backward)
    /// </summary>
    internal class IterMapBkwd<K, V>(Map.IteratorState<K, V> items) : IteratorIO<(K Key, V Value)>
    {
        (Head<(K Key, V Value)> Head, IteratorIO<(K Key, V Value)> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.ExistIO(head.KeyValue, new IterMapBkwd<K, V>(tail))
                : Head.NilIO<(K, V)>();

        public override IO<(Head<(K Key, V Value)> Head, IteratorIO<(K Key, V Value)> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override IteratorIO<(K, V)> Using() =>
            this;
    }

    /// <summary>
    /// Map IteratorIO keys (backward)
    /// </summary>
    internal class IterMapKeyBkwd<K, V>(Map.IteratorState<K, V> items) : IteratorIO<K>
    {
        (Head<K> Head, IteratorIO<K> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.ExistIO(head.KeyValue.Key, new IterMapKeyBkwd<K, V>(tail))
                : Head.NilIO<K>();

        public override IO<(Head<K> Head, IteratorIO<K> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override IteratorIO<K> Using() =>
            this;
    }

    /// <summary>
    /// Map IteratorIO values (backward)
    /// </summary>
    internal class IterMapValueBkwd<K, V>(Map.IteratorState<K, V> items) : IteratorIO<V>
    {
        (Head<V> Head, IteratorIO<V> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.ExistIO(head.KeyValue.Value, new IterMapValueBkwd<K, V>(tail))
                : Head.NilIO<V>();

        public override IO<(Head<V> Head, IteratorIO<V> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"Map{items.ToString()}";

        public override IteratorIO<V> Using() =>
            this;
    }
}
