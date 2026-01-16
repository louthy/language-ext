namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpCombine(Iterator<A> xs, Iterator<A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"Combine({xs}, {ys})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> h, var t)
                ? (h, t.Combine(ys))
                : ys.Next();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t.Combine(ys)))
                                     : ys.NextIO());

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override Iterator<A> Using() =>
            new OpCombine(xs.Using(), ys.Using());
    }
}
