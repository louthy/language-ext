using System;
using LanguageExt.Traits;
using L = LanguageExt;

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
                if (f(h.Value)) return (h, t.Filter(f));
            }
            return Head.Nil<A>();
        }
    }
}
