#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class Iterator
{
    /// <summary>
    /// HashSet iterator
    /// </summary>
    internal class IterHashSet<EqK, K>(TrieSet.IteratorState<EqK, K> items) : Iterator<K>
        where EqK : Eq<K>
    {
        public TrieSet.IteratorState<EqK, K> Items => items;

        public override (Head<K> Head, Iterator<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head, new IterHashSet<EqK, K>(tail))
                : Head.Nil<K>();

        public override IO<(Head<K> Head, Iterator<K> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"HashSet{items}";

        public override Iterator<K> Using() =>
            this;
    }
}
