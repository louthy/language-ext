using System.Collections;

namespace LanguageExt.SourceGen;

public static class List
{
    /// <summary>
    /// Empty list
    /// </summary>
    public static List<A> Nil<A>() => List<A>.Nil;

    /// <summary>
    /// Prepend an item on the list
    /// </summary>
    public static List<A> Cons<A>(this A x, List<A> xs) =>
        new Cons<A>(x, xs, xs.Count() + 1);
}

public abstract record List<A> : IEnumerable<A>
{
    /// <summary>
    /// Empty list
    /// </summary>
    public static readonly List<A> Nil = new Nil<A>();

    /// <summary>
    /// Take the tail of the list
    /// </summary>
    public abstract List<A> Tail();

    /// <summary>
    /// Look at the head of the list
    /// </summary>
    public abstract Maybe<A> Head();

    /// <summary>
    /// True if the list is empty
    /// </summary>
    public abstract bool IsEmpty { get; }

    /// <summary>
    /// Number of items in the list
    /// </summary>
    public abstract int Count();

    /// <summary>
    /// Map items
    /// </summary>
    public abstract List<B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Map items
    /// </summary>
    public List<B> Select<B>(Func<A, B> f) => Map(f);

    public IEnumerator<A> GetEnumerator()
    {
        var x = this;
        while (!x.IsEmpty)
        {
            var c = (Cons<A>)x;
            yield return c.Value;
            x = c.TailList;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record Nil<A> : List<A>
{
    public override List<A> Tail() => 
        this;

    public override Maybe<A> Head() => 
        Maybe.Nothing<A>();

    public override bool IsEmpty => 
        true;

    public override int Count() => 
        0;

    public override List<B> Map<B>(Func<A, B> f) => 
        List<B>.Nil;
}

public record Cons<A>(A Value, List<A> TailList, int Length) : List<A>
{
    public override List<A> Tail() => 
        TailList;

    public override Maybe<A> Head() => 
        Maybe.Just(Value);

    public override bool IsEmpty =>
        false;

    public override int Count() => 
        Length;

    public override List<B> Map<B>(Func<A, B> f) => 
        List.Cons(f(Value), TailList.Map(f));
}
