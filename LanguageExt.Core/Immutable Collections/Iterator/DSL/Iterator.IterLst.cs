using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Lst iterator (forward)
    /// </summary>
    internal class IterLstFwd(Lst.IteratorState<A> items) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.Exist(head.Key, new IterLstFwd(tail))
                : Head.Nil<A>();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Lst{items.ToString()}";

        public override Iterator<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Set iterator (backward)
    /// </summary>
    internal class IterLstBkwd(Lst.IteratorState<A> items) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.Exist(head.Key, new IterLstFwd(tail))
                : Head.Nil<A>();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Lst{items.ToString()}";

        public override Iterator<A> Using() =>
            this;
    }
}
