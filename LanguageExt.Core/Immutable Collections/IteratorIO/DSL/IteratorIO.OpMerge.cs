namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpMerge(IteratorIO<A> xs, IteratorIO<A> ys) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Merge({xs}, {ys})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            (((Head<A> Head, IteratorIO<A> Tail) x, (Head<A> Head, IteratorIO<A> Tail) y) =>
                 (x, y) switch
                 {
                     ((Exist<A> lh, var lt), (Exist<A> (var rh), var rt)) =>
                         (lh, rh.Cons(() => lt.Merge(rt))),

                     ((Exist<A> lh, var lt), _) =>
                         (lh, lt),

                     (_, (Exist<A> rh, var rt)) =>
                         (rh, rt),

                     _ => Head.NilIO<A>()
                 })
              * xs.NextIO()
              * ys.NextIO();

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override IteratorIO<A> Using() =>
            new OpMerge(xs.Using(), ys.Using());
    }
}
