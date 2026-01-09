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

        protected override (Head<A> Head, Iterator<A> Tail) Next() =>
            (new Exist<A>(head), tail());

        public override string ToString() => 
            $"{head}...";
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

        protected override (Head<A> Head, Iterator<A> Tail) Next() =>
            (new Exist<A>(head), tail);

        public override string ToString() => 
            $"{head}, {tail}";
    }    
}
