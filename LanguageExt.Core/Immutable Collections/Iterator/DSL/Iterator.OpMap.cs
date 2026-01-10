using System;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpMap<X>(Iterator<X> iter, Func<X, A> f) : Iterator<A>
    {
        public override string ToString() => 
            $"Map({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            iter is (Exist<X> (var Value), var next)
                ? (new Exist<A>(f(Value)), next.Map(f))
                : (Nil<A>.Default, Nil.Default);

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpMap<X>(iter.Using(), f);
    }
}
