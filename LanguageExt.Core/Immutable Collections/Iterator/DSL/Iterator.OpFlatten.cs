using LanguageExt.Traits;

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

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpFlatten(iter.Using());
    }
    
    internal sealed class OpFlatten2(Iterator<K<Iterator, A>> iter) : Iterator<A>
    {
        public override string ToString() => 
            $"Flatten({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            iter is (Exist<K<Iterator, A>> (var hs), var t)
                ? hs.As().Combine(t.Flatten()).Next()
                : (Nil<A>.Default, Nil.Default);

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpFlatten2(iter.Using());
    }
}
