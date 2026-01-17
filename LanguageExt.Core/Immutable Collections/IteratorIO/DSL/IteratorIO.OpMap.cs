using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpMap<X>(IteratorIO<X> iter, Func<X, A> f) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Map({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO().Map(p => go(p, f));

        static (Head<A> Head, IteratorIO<A> Tail) go((Head<X> Head, IteratorIO<X> Tail) input, Func<X, A> f) =>
            input is (Exist<X>(var value), var next)
                ? Head.ExistIO(f(value), next.Map(f))
                : Head.NilIO<A>();
        
        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpMap<X>(iter.Using(), f);
    }
}
