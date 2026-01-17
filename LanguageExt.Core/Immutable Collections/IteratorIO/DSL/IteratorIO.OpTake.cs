namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpTake(IteratorIO<A> iter, int remain) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Take({remain}:{iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            remain <= 0
                ? IO.pure(Head.NilIO<A>())
                : iter.NextIO() * (ht => ht switch
                                          {
                                              (Exist<A> head, var tail) => (head, new OpTake(tail, remain - 1)),
                                              _                         => Head.NilIO<A>()
                                          });

        public override void Dispose() =>
            iter.Dispose();

        public override IteratorIO<A> Using() =>
            new OpTake(iter.Using(), remain);
    }
}
