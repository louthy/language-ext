using System;

namespace LanguageExt;

public partial class Iterator
{
    internal sealed class OpZip<A, B>(Iterator<A> xs, Iterator<B> ys) : Iterator<(A First, B Second)>
    {
        public override string ToString() => 
            $"Zip({xs}, {ys})";

        public override (Head<(A First, B Second)> Head, Iterator<(A First, B Second)> Tail) Next() =>
            (xs, ys) switch
            {
                ((Exist<A> (var lh), var lt), (Exist<B> (var rh), var rt)) =>
                    (new Exist<(A First, B Second)>((lh, rh)), lt.Zip(rt)),

                _ => Head.Nil<(A, B)>()
            };
    }
    
    internal sealed class OpZip<A, B, C>(Iterator<A> xs, Iterator<B> ys, Func<A, B, C> join) : Iterator<C>
    {
        public override string ToString() => 
            $"Zip({xs}, {ys})";

        public override (Head<C> Head, Iterator<C> Tail) Next() =>
            (xs, ys) switch
            {
                ((Exist<A> (var lh), var lt), (Exist<B> (var rh), var rt)) =>
                    (new Exist<C>(join(lh, rh)), lt.Zip(rt, join)),

                _ => Head.Nil<C>()
            };
    }
}
