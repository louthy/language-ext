using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Set IteratorIO (forward)
    /// </summary>
    internal class IterSetFwd(Set.IteratorState<A> items) : IteratorIO<A>
    {
        public Set.IteratorState<A> Items => items;
        
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.Key, new IterSetFwd(tail))
                : Head.NilIO<A>();

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Set{items.ToString()}";

        public override IteratorIO<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Set IteratorIO (backward)
    /// </summary>
    internal class IterSetBkwd(Set.IteratorState<A> items) : IteratorIO<A>
    {
        public Set.IteratorState<A> Items => items;
        
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.ExistIO(head.Key, new IterSetBkwd(tail))
                : Head.NilIO<A>();

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Set{items.ToString()}";

        public override IteratorIO<A> Using() =>
            this;
    }
}
