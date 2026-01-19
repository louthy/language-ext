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
    }
    
    internal sealed class OpAltMemo(Iterator<A> xs, Memo<Iterator, A> ys) : Iterator<A>
    {
        public override string ToString() => 
            $"{xs} | {ys}";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> h, var t)
                ? (h, t)
                : ys.Value.As().Next();
    }
}
