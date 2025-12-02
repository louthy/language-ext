using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface Alternative<F> : Choice<F>, Applicative<F>
    where F : Alternative<F>
{
    /// <summary>
    /// Identity
    /// </summary>
    [Pure]
    public static abstract K<F, A> Empty<A>(); 
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static virtual K<F, A> Choice<A>(in Seq<K<F, A>> ms)
    {
        if(ms.IsEmpty) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms.Tail)
        {
            r |= m;
        }
        return r;
    }

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static virtual K<F, A> Choice<A>(in ReadOnlySpan<K<F, A>> ms)
    {
        if(ms.Length == 0) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms)
        {
            r |= m;
        }
        return r;
    }
        
    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Some<A>(K<F, A> fa)
    {
        return some_v();
        
        K<F, Seq<A>> many_v() =>
            F.Choose(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            Cached<A>.cons * fa * memoK(many_v);
    }
    
    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Many<A>(K<F, A> fa)
    {
        return many_v();
        
        K<F, Seq<A>> many_v() =>
            some_v() | F.Pure(Seq<A>());

        K<F, Seq<A>> some_v() =>
            Cached<A>.cons * fa * memoK(many_v);
    }
    
    /// <summary>
    /// Skip zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Unit</returns>
    [Pure]
    public static virtual K<F, Unit> SkipMany<A>(K<F, A> fa)
    {
        return many_v();
        
        K<F, Unit> many_v() =>
            some_v() | F.Pure(unit);

        K<F, Unit> some_v() =>
            Cached<A>.ignore * fa * memoK(many_v);
    }
    
    // -- | @'endBy' p sep@ parses /zero/ or more occurrences of @p@, separated and
    // -- ended by @sep@. Returns a list of values returned by @p@.
    // --
    // -- > cStatements = cStatement `endBy` semicolon
    // endBy :: Alternative m => m a -> m sep -> m [a]
    // endBy p sep = many (p <* sep)
    public static K<F, Seq<A>> EndBy<A, SEP>(K<F, A> p, K<F, SEP> sep) =>
        F.Many(p << sep);
    
    // -- | @'endBy1' p sep@ parses /one/ or more occurrences of @p@, separated and
    // -- ended by @sep@. Returns a list of values returned by @p@.
    // endBy1 :: Alternative m => m a -> m sep -> m [a]
    // endBy1 p sep = some (p <* sep)    

    /// <summary>
    /// Combine two alternatives
    /// </summary>
    /// <param name="ma">Left alternative</param>
    /// <param name="mb">Right alternative</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Alternative structure with an `Either` lifted into it</returns>
    [Pure]
    public static virtual K<F, Either<A, B>> Either<A, B>(K<F, A> ma, K<F, B> mb) =>
        (Left<A, B>) * ma | (Right<A, B>) * mb; 
        
    static class Cached<A>
    {
        public static readonly Func<A, Func<Unit, Unit>> ignore =
            static _ => _ => default;
        
        public static readonly Func<A, Func<Seq<A>, Seq<A>>> cons =
            static x => xs => x.Cons(xs);
    }
}
