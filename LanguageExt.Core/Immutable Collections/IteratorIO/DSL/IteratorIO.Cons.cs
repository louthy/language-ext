using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Cons IteratorIO
    /// </summary>
    internal class Cons(A head, Func<IteratorIO<A>> tail) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.lift(tail) * (t => Head.ExistIO(head, t));

        public override string ToString() => 
            $"{head}...";

        public override IteratorIO<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Cons IteratorIO
    /// </summary>
    internal class ConsStrict(A head, IteratorIO<A> tail) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Head.ExistIO(head, tail));

        public override string ToString() => 
            $"{head}, {tail}";

        public override void Dispose() =>
            tail.Dispose();

        public override IteratorIO<A> Using() =>
            new ConsStrict(head, tail.Using());
        
        public override IteratorIO<A> Strict() => 
            new ConsStrict(head, tail.Strict());
    }    
}
