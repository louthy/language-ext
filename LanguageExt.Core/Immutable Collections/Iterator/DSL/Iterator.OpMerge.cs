namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpMerge(Iterator<A> xs, Iterator<A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"Merge({xs}, {ys})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            (xs.Next(), ys.Next()) switch
            {
                ((Exist<A> lh, var lt), (Exist<A> (var rh), var rt)) =>
                    (lh, rh.Cons(() => lt.Merge(rt))),

                ((Exist<A> lh, var lt), _) =>
                    (lh, lt),

                (_, (Exist<A> rh, var rt)) =>
                    (rh, rt),

                _ => Head.Nil<A>()
            };

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            (((Head<A> Head, Iterator<A> Tail) x, (Head<A> Head, Iterator<A> Tail) y) =>
                 (x, y) switch
                 {
                     ((Exist<A> lh, var lt), (Exist<A> (var rh), var rt)) =>
                         (lh, rh.Cons(() => lt.Merge(rt))),

                     ((Exist<A> lh, var lt), _) =>
                         (lh, lt),

                     (_, (Exist<A> rh, var rt)) =>
                         (rh, rt),

                     _ => Head.Nil<A>()
                 })
              * xs.NextIO()
              * ys.NextIO();

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override Iterator<A> Using() =>
            new OpMerge(xs.Using(), ys.Using());
    }
}
