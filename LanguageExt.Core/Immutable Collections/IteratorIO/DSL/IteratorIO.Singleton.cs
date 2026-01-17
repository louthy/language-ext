namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    /// <summary>
    /// Single item list
    /// </summary>
    public sealed class Singleton(A Value) : IteratorIO<A>
    {
        public override string ToString() => 
            $"{Value}";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            IO.pure(Head.ExistIO(Value));
        
        public override IteratorIO<A> Using() =>
            this;
    }
}
