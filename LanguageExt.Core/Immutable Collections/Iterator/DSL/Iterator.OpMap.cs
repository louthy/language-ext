using System;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpMap<X>(Iterator<X> iter, Func<X, A> f) : Iterator<A>
    {
        public override string ToString() => 
            $"Map({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            go(iter.Next(), f);

        static (Head<A> Head, Iterator<A> Tail) go((Head<X> Head, Iterator<X> Tail) input, Func<X, A> f) =>
            input is (Exist<X>(var value), var next)
                ? Head.Exist(f(value), next.Map(f))
                : Head.Nil<A>();
    }
    
    internal sealed class OpMap2<X>(Iterator<X> iter, Func<X, long, A> f, long offset) : Iterator<A>
    {
        public override string ToString() => 
            $"Map({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            go(iter.Next(), f, offset);

        static (Head<A> Head, Iterator<A> Tail) go((Head<X> Head, Iterator<X> Tail) input, Func<X, long, A> f, long off) =>
            input is (Exist<X>(var value), var next)
                ? Head.Exist(f(value, off), next.Map(f, off + 1))
                : Head.Nil<A>();
    }
}
