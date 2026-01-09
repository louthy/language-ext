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
                ? (new Exist<A>(head.Key), new IterLstFwd(tail))
                : (Nil<A>.Default, Nil.Default);
    
        public override string ToString() => 
            $"Lst{items.ToString()}";
    }
    
    /// <summary>
    /// Set iterator (backward)
    /// </summary>
    internal class IterLstBkwd(Lst.IteratorState<A> items) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            items.StepBack(out var head, out var tail)
                ? (new Exist<A>(head.Key), new IterLstFwd(tail))
                : (Nil<A>.Default, Nil.Default);
    
        public override string ToString() => 
            $"Lst{items.ToString()}";
    }
}
