namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpAlt(Iterator<A> xs, Iterator<A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"{xs} | {ys}";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> h, var t)
                ? (h, t)
                : ys.Next();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t))
                                     : ys.NextIO());

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }

        public override Iterator<A> Using() =>
            new OpAlt(xs.Using(), ys.Using());
    }
    
    internal sealed class OpAltMemo(Iterator<A> xs, Memo<Iterator, A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"{xs} | {ys}";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> h, var t)
                ? (h, t)
                : ys.Value.As().Next();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t))
                                     : ys.Value.As().NextIO());

        public override void Dispose() =>
            xs.Dispose();
        
        public override Iterator<A> Using() =>
            new OpAltMemo(xs.Using(), ys);
    }
}
