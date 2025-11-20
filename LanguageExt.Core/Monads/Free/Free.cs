using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Free monad makes any functor into a monad 
/// </summary>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record Free<F, A> : K<Free<F>, A>
    where F : Functor<F>
{
    public Free<F, B> Map<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public Free<F, B> Select<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public Free<F, B> Bind<B>(Func<A, Free<F, B>> f) =>
        this.Kind().Bind(f).As();
    
    public Free<F, B> Bind<B>(Func<A, K<Free<F>, B>> f) =>
        this.Kind().Bind(f).As();
    
    public Free<F, C> SelectMany<B, C>(Func<A, K<Free<F>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}

/// <summary>
/// Terminal case for the free monad
/// </summary>
/// <param name="Value">Terminal value</param>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public sealed record Pure<F, A>(A Value) : Free<F, A>
    where F : Functor<F>;

/// <summary>
/// Monadic bind case for the free monad
/// </summary>
/// <param name="Value">Functor that yields a `Free` monad</param>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public sealed record Bind<F, A>(K<F, Free<F, A>> Value) : Free<F, A>
    where F : Functor<F>;
