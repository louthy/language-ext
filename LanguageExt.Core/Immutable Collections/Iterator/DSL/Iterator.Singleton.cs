namespace LanguageExt;

public abstract partial class Iterator<A>
{
    /// <summary>
    /// Single item list
    /// </summary>
    public sealed class Singleton(A Value) : Iterator<A>
    {
        public override string ToString() => 
            $"{Value}";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            Head.Exist(Value);

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            IO.pure(Head.Exist(Value));
        
        public override Iterator<A> Using() =>
            this;
    }
}
