using System;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpFilter(Iterator<A> iter, Func<A, bool> f) : Iterator<A>
    {
        public override string ToString() => 
            $"Filter({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            for (var i = iter; i is (Exist<A> h, var t); i = t)
            {
                if (f(h.Value)) return (h, t);
            }
            return (Nil<A>.Default, Nil.Default);
        }

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpFilter(iter.Using(), f);
    }
}
