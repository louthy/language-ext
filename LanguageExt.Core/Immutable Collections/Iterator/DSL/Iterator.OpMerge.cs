namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpMerge(Iterator<A> xs, Iterator<A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"Merge({xs}, {ys})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            (xs, ys) switch
            {
                ((Exist<A> lh, var lt), (Exist<A> (var rh), var rt)) =>
                    (lh, rh.Cons(() => lt.Merge(rt))),

                ((Exist<A>, _) left, _) =>
                    left.Next(),

                (_, (Exist<A>, _) right) =>
                    right.Next(),

                _ => (Nil<A>.Default, Nil.Default)
            };
    }
}
