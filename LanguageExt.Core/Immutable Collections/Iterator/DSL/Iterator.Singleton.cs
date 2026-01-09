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

        protected override (Head<A> Head, Iterator<A> Tail) Next() =>
            (new Exist<A>(Value), Iterator.Nil<A>());
    }
}
