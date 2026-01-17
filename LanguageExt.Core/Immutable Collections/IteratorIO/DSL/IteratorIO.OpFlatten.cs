using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpFlatten(IteratorIO<IteratorIO<A>> iter) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Flatten({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() >> (n => n is (Exist<IteratorIO<A>> (var hs), var t)
                                       ? hs.Combine(t.Flatten()).NextIO()
                                       : IO.pure(Head.NilIO<A>()));

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpFlatten(iter.Using());
    }
    
    internal sealed class OpFlatten2(IteratorIO<K<IteratorIO, A>> iter) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Flatten({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            iter.NextIO() >> (n => n is (Exist<K<IteratorIO, A>> (var hs), var t)
                                       ? hs.As().Combine(t.Flatten()).NextIO()
                                       : IO.pure(Head.NilIO<A>()));

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpFlatten2(iter.Using());
    }
}
