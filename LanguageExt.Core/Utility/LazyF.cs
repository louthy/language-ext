using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Makes `K〈F, A〉` lazy
/// </summary>
/// <remarks>
/// This is more of a utility type right now, so it hasn't been fleshed out like other Applicatives
/// </remarks>
public readonly record struct LazyF<F, A>(Func<K<F, A>> runLazyF) : K<LazyF<F>, A>
    where F : Applicative<F>, MonoidK<F>;

/// <summary>
/// LazyF module
/// </summary>
public static class LazyF
{
    public static LazyF<F, A> lift<F, A>(K<F, A> fa)
        where F : Applicative<F>, MonoidK<F> =>
        new (() => fa);       
    
    public static LazyF<F, A> lift<F, A>(K<LazyF<F>, A> fa)
        where F : Applicative<F>, MonoidK<F> =>
        new (fa.Run);       
    
    public static LazyF<F, A> lazy<F, A>(Func<K<F, A>> f)
        where F : Applicative<F>, MonoidK<F> =>
        new (f);       
    
    public static LazyF<F, A> lazy<F, A>(Func<K<LazyF<F>, A>> f)
        where F : Applicative<F>, MonoidK<F> =>
        new (() => f().Run());       
    
    public static LazyF<F, A> lazy<F, A>(Func<LazyF<F, A>> f)
        where F : Applicative<F>, MonoidK<F> =>
        new (() => f().Run());       
    
    public static LazyF<F, A> As<F, A>(this K<LazyF<F>, A> ma) 
        where F : Applicative<F>, MonoidK<F> =>
        (LazyF<F, A>)ma;
    
    public static K<F, A> Run<F, A>(this K<LazyF<F>, A> ma)
        where F : Applicative<F>, MonoidK<F> =>
        ((LazyF<F, A>)ma).runLazyF();
}

/// <summary>
/// LazyF trait implementations
/// </summary>
public class LazyF<F> : Applicative<LazyF<F>>, MonoidK<LazyF<F>>
    where F : Applicative<F>, MonoidK<F>
{
    public static K<LazyF<F>, B> Map<A, B>(Func<A, B> f, K<LazyF<F>, A> ma) =>
        new LazyF<F, B>(() => ma.Run().Map(f));

    public static K<LazyF<F>, A> Pure<A>(A value) => 
        new LazyF<F, A>(() => F.Pure(value));

    public static K<LazyF<F>, B> Apply<A, B>(K<LazyF<F>, Func<A, B>> mf, K<LazyF<F>, A> ma) => 
        new LazyF<F, B>(() => mf.Run().Apply(ma.Run()));

    public static K<LazyF<F>, A> Combine<A>(K<LazyF<F>, A> lhs, K<LazyF<F>, A> rhs) => 
        new LazyF<F, A>(() => lhs.Run().Combine(rhs.Run()));

    public static K<LazyF<F>, A> Empty<A>() => 
        new LazyF<F,A>(F.Empty<A>);
}
