namespace LanguageExt;

public abstract record Head<A>
{
    protected Head()
    {
    }
}

public sealed record Exist<A>(A Value) : Head<A>
{
    public void Deconstruct(out A head) => 
        head = Value;
}

public sealed record Nil<A> : Head<A>
{
    public static Head<A> Default = new Nil<A>();
}
