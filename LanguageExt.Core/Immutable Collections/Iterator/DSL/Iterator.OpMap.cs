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

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            iter.NextIO().Map(p => go(p, f));

        static (Head<A> Head, Iterator<A> Tail) go((Head<X> Head, Iterator<X> Tail) input, Func<X, A> f) =>
            input is (Exist<X>(var value), var next)
                ? Head.Exist(f(value), next.Map(f))
                : Head.Nil<A>();
        
        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpMap<X>(iter.Using(), f);
    }
}
