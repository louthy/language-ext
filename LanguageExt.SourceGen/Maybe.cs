namespace LanguageExt.SourceGen;

public static class Maybe
{
    /// <summary>
    /// Maybe constructor
    /// </summary>
    public static Maybe<A> Just<A>(A value) =>
        new Just<A>(value);

    /// <summary>
    /// Maybe constructor
    /// </summary>
    public static Maybe<A> Nothing<A>() =>
        SourceGen.Nothing<A>.Default;
}

/// <summary>
/// Optional type
/// </summary>
public abstract record Maybe<A>
{
    public abstract Maybe<B> Map<B>(Func<A, B> f);
    public abstract Maybe<B> Bind<B>(Func<A, Maybe<B>> f);
    public Maybe<B> Select<B>(Func<A, B> f) => Map(f);
    public Maybe<B> SelectMany<B>(Func<A, Maybe<B>> f) => Bind(f);
    public Maybe<C> SelectMany<B, C>(Func<A, Maybe<B>> bind, Func<A, B, C> proj) =>
        Bind(x => bind(x).Map(y => proj(x, y)));
}

public record Just<A>(A Value) : Maybe<A>
{
    public override Maybe<B> Map<B>(Func<A, B> f) => 
        new Just<B>(f(Value));

    public override Maybe<B> Bind<B>(Func<A, Maybe<B>> f) =>
        f(Value);
}

public record Nothing<A> : Maybe<A>
{
    public static readonly Maybe<A> Default = new Nothing<A>();

    public override Maybe<B> Map<B>(Func<A, B> f) =>
        Nothing<B>.Default;

    public override Maybe<B> Bind<B>(Func<A, Maybe<B>> f) =>
        Nothing<B>.Default;
}
