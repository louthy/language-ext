using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Set iterator (forward)
    /// </summary>
    internal class IterSetFwd(Set.IteratorState<A> items) : Iterator<A>
    {
        public Set.IteratorState<A> Items => items;
        
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.Key, new IterSetFwd(tail))
                : Head.Nil<A>();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Set{items.ToString()}";

        public override Iterator<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Set iterator (backward)
    /// </summary>
    internal class IterSetBkwd(Set.IteratorState<A> items) : Iterator<A>
    {
        public Set.IteratorState<A> Items => items;
        
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.Exist(head.Key, new IterSetBkwd(tail))
                : Head.Nil<A>();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Set{items.ToString()}";

        public override Iterator<A> Using() =>
            this;
    }
}
