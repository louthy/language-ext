using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpCombine(IteratorIO<A> xs, IteratorIO<A> ys) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Combine({xs}, {ys})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t.Combine(ys)))
                                     : ys.NextIO());

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override IteratorIO<A> Using() =>
            new OpCombine(xs.Using(), ys.Using());
    }
    
    internal sealed class OpCombine2(IteratorIO<A> xs, Func<IteratorIO<A>> ys) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Combine({xs}, {ys})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t.Combine(ys)))
                                     : ys().NextIO());

        public override void Dispose() => 
            xs.Dispose();

        public override IteratorIO<A> Using() =>
            new OpCombine2(xs.Using(), ys);
    }
}
