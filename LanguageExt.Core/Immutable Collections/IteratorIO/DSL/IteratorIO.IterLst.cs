using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Lst IteratorIO (forward)
    /// </summary>
    internal class IterLstFwd(Lst.IteratorState<A> items) : IteratorIO<A>
    {
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            items.Step(out var head, out var tail)
                ? Head.ExistIO(head.Key, new IterLstFwd(tail))
                : Head.NilIO<A>();

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Lst{items.ToString()}";

        public override IteratorIO<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Set IteratorIO (backward)
    /// </summary>
    internal class IterLstBkwd(Lst.IteratorState<A> items) : IteratorIO<A>
    {
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? Head.ExistIO(head.Key, new IterLstFwd(tail))
                : Head.NilIO<A>();

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Lst{items.ToString()}";

        public override IteratorIO<A> Using() =>
            this;
    }
}
