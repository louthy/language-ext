namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    /// <summary>
    /// Nil IteratorIO case
    ///
    /// The end of the sequence.
    /// </summary>
    internal sealed class Nil : IteratorIO<A>
    {
        public static readonly IteratorIO<A> Default = new Nil();

        public override string ToString() => 
            "Nil";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            IO.pure(Head.NilIO<A>());

        public override IteratorIO<A> Using() =>
            this;
    }
}
