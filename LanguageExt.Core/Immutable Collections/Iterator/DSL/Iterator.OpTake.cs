namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpTake(Iterator<A> iter, int remain) : Iterator<A>
    {
        public override string ToString() => 
            $"Take({remain}:{iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remain <= 0
                ? Head.Nil<A>()
                : iter is (Exist<A> head, var tail)
                    ? (head, new OpTake(tail, remain - 1))
                    : Head.Nil<A>();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            remain <= 0
                ? IO.pure(Head.Nil<A>())
                : iter.NextIO() * (ht => ht switch
                                          {
                                              (Exist<A> head, var tail) => (head, new OpTake(tail, remain - 1)),
                                              _                         => Head.Nil<A>()
                                          });

        public override void Dispose() =>
            iter.Dispose();

        public override Iterator<A> Using() =>
            new OpTake(iter.Using(), remain);
    }
}
