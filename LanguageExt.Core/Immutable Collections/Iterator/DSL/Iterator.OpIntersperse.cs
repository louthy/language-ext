namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpIntersperseOn(Iterator<A> xs, A x) : Iterator<A>
    {
        public override string ToString() => 
            $"Intersperse({xs} with {x})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> (var h), var t)
                ? Head.Exist(h, new OpIntersperseOff(t, x))
                : Head.Nil<A>();
    }
    
    internal sealed class OpIntersperseOff(Iterator<A> xs, A x) : Iterator<A>
    {
        public override string ToString() => 
            $"Intersperse({xs} with {x})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            xs is (Exist<A> (var h), var t)
                ? Head.Exist(h, new OpIntersperseOff(t, x))
                : Head.Nil<A>();
    }
}
