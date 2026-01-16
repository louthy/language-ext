using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Cons iterator
    /// </summary>
    internal class Cons(A head, Func<Iterator<A>> tail) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            Head.Exist(head, tail());

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.lift(tail) * (t => Head.Exist(head, t));

        public override string ToString() => 
            $"{head}...";

        public override Iterator<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Cons iterator
    /// </summary>
    internal class ConsStrict(A head, Iterator<A> tail) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            Head.Exist(head, tail);

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.pure(Head.Exist(head, tail));

        public override string ToString() => 
            $"{head}, {tail}";

        public override void Dispose() =>
            tail.Dispose();

        public override Iterator<A> Using() =>
            new ConsStrict(head, tail.Using());
        
        public override Iterator<A> Strict() => 
            new ConsStrict(head, tail.Strict());
    }    
}
