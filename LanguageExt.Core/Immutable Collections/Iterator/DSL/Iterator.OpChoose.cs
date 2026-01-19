using System;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpChoose<X>(Iterator<X> iter, Func<X, Option<A>> f) : Iterator<A>
    {
        public override string ToString() => 
            $"Choose({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            for (var i = iter; i is (Exist<X> (var h), var t); i = t)
            {
                var option = f(h);
                if (option.IsSome) return Head.Exist(option.Value!, t.Choose(f));
            }
            return Head.Nil<A>();
        }
    }
}
