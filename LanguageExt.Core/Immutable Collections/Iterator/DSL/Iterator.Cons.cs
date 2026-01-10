using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Cons iterator
    /// </summary>
    internal class Cons : Iterator<A>
    {
        readonly A head;
        readonly Func<Iterator<A>> tail;

        public Cons(A head, Func<Iterator<A>> tail)
        {
            this.head = head;
            this.tail = tail;
        }

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            (new Exist<A>(head), tail());

        public override string ToString() => 
            $"{head}...";

        public override Iterator<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Cons iterator
    /// </summary>
    internal class ConsStrict : Iterator<A>
    {
        readonly A head;
        readonly Iterator<A> tail;

        public ConsStrict(A head, Iterator<A> tail)
        {
            this.head = head;
            this.tail = tail;
        }

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            (new Exist<A>(head), tail);

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
