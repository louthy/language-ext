namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    /// <summary>
    /// Lift an Iterator into an IteratorIO
    /// </summary>
    public sealed class Lift(Iterator<A> Items) : IteratorIO<A>
    {
        public override string ToString() => 
            Items.ToString();

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            IO.lift(() => Items is (Exist<A> (var head), var tail)
                              ? Head.ExistIO(head, new Lift(tail))
                              : Head.NilIO<A>());
        
        public override IteratorIO<A> Using() =>
            this;
    }
}
