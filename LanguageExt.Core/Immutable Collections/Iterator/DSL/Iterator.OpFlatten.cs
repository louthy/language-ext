namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpFlatten(Iterator<Iterator<A>> iter) : Iterator<A>
    {
        public override string ToString() => 
            $"Flatten({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            iter is (Exist<Iterator<A>> (var hs), var t)
                ? hs.Combine(t.Flatten()).Next()
                : (Nil<A>.Default, Nil.Default);
    }
}
