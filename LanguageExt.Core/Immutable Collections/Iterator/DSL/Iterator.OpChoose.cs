using System;

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
                if (option.IsSome) return (new Exist<A>(option.Value!), t.Choose(f));
            }
            return (Nil<A>.Default, Nil.Default);
        }
    }
}
