#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    /// <summary>
    /// HashSet IteratorIO
    /// </summary>
    internal class IterHashSet<EqK, K>(TrieSet.IteratorState<EqK, K> items) : IteratorIO<K>
        where EqK : Eq<K>
    {
        public TrieSet.IteratorState<EqK, K> Items => items;

        (Head<K> Head, IteratorIO<K> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head, new IterHashSet<EqK, K>(tail))
                : Head.NilIO<K>();

        public override IO<(Head<K> Head, IteratorIO<K> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() =>
            $"HashSet{items}";

        public override IteratorIO<K> Using() =>
            this;
    }
}
