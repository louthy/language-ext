namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Seq IteratorIO
    /// </summary>
    internal class IterSeq(Seq<A> items) : IteratorIO<A>
    {
        public Seq<A> Items => items;
        
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            items.IsEmpty
                ? Head.NilIO<A>()
                : Head.ExistIO(items[0], new IterSeq(items.Tail));

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.lift(Next);

        public override string ToString() => 
            $"Seq{items.ToString()}";

        public override void Dispose()
        {
            if (items.Value is SeqIterator<A> s)
            {
                s.Dispose();
            }
        }

        public override IteratorIO<A> Using() =>
            items.Value is SeqIterator<A> s
                ? new IterSeq(new Seq<A>(s.Using()))
                : this;
    }
}
