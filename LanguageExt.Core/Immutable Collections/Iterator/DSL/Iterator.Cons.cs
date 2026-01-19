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

        public override string ToString() => 
            $"{head}...";
    }
    
    /// <summary>
    /// Cons iterator
    /// </summary>
    internal class ConsStrict(A head, Iterator<A> tail) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            Head.Exist(head, tail);

        public override string ToString() => 
            $"{head}, {tail}";
        
        public override Iterator<A> Strict() => 
            new ConsStrict(head, tail.Strict());
    }    
}
