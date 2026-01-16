namespace LanguageExt;

public abstract partial class Iterator<A>
{
    /// <summary>
    /// Nil iterator case
    ///
    /// The end of the sequence.
    /// </summary>
    internal sealed class Nil : Iterator<A>
    {
        public static readonly Iterator<A> Default = new Nil();

        public override string ToString() => 
            "Nil";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            Head.Nil<A>();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            IO.pure(Head.Nil<A>());

        public override Iterator<A> Using() =>
            this;
    }
}
