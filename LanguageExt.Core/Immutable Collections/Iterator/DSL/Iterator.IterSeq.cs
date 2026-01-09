using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterSeq(Seq<A> items) : Iterator<A>
    {
        protected override (Head<A> Head, Iterator<A> Tail) Next()
        {
            if(items.IsEmpty) return (Nil<A>.Default, Nil.Default);
            return (new Exist<A>(items[0]), new IterSeq(items.Tail));
        }
    
        public override string ToString() => 
            $"Seq{items.ToString()}";
    }
}
