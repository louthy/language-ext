namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpTake(Iterator<A> iter, int remain) : Iterator<A>
    {
        public override string ToString() => 
            $"Take({remain}:{iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remain <= 0
                ? (Nil<A>.Default, Nil.Default)
                : iter is (Exist<A> head, var tail)
                    ? (head, new OpTake(tail, remain - 1))
                    : (Nil<A>.Default, Nil.Default);

        public override void Dispose() =>
            iter.Dispose();

        public override Iterator<A> Using() =>
            new OpTake(iter.Using(), remain);
    }
}
