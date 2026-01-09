namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpCombine(Iterator<A> xs, Iterator<A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"Combine({xs}, {ys})";

        protected override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> h, var t)
                ? (h, t.Combine(ys))
                : ys.Next();
    }
}
