namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpAlt(IteratorIO<A> xs, IteratorIO<A> ys) : IteratorIO<A>
    {
        public override string ToString() => 
            $"{xs} | {ys}";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t))
                                     : ys.NextIO());

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }

        public override IteratorIO<A> Using() =>
            new OpAlt(xs.Using(), ys.Using());
    }
    
    internal sealed class OpAltMemo(IteratorIO<A> xs, Memo<IteratorIO, A> ys) : IteratorIO<A>
    {
        public override string ToString() => 
            $"{xs} | {ys}";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            xs.NextIO() >> (x => x is (Exist<A> h, var t)
                                     ? IO.pure(((Head<A>)h, t))
                                     : ys.Value.As().NextIO());

        public override void Dispose() =>
            xs.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpAltMemo(xs.Using(), ys);
    }
}
