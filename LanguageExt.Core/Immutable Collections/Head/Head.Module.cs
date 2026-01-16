namespace LanguageExt;

public static class Head
{
    public static (Head<A> Head, Iterator<A> Tail) Nil<A>() =>
        (Head<A>.Nil, Iterator<A>.Nil.Default);

    public static (Head<A> Head, Iterator<A> Tail) Exist<A>(A value) =>
        Exist(value, Iterator.empty<A>());
    
    public static (Head<A> Head, Iterator<A> Tail) Exist<A>(A value, Iterator<A> tail) =>
        (new Exist<A>(value), tail);
}
