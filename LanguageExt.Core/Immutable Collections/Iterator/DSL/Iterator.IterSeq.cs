using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Seq iterator
    /// </summary>
    internal class IterSeq(Seq<A> items) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            items.IsEmpty
                ?(Nil<A>.Default, Nil.Default)
                : (new Exist<A>(items[0]), new IterSeq(items.Tail));
    
        public override string ToString() => 
            $"Seq{items.ToString()}";
    }
}
