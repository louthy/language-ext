using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class Free
{
    /// <summary>
    /// Terminal case for the free monad
    /// </summary>
    /// <param name="value">Terminal value</param>
    /// <typeparam name="F">Functor type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public static Free<F, A> pure<F, A>(A value) 
        where F : Functor<F> =>
        new Pure<F, A>(value);

    /// <summary>
    /// Lift the functor into the free monad
    /// </summary>
    /// <param name="value">Functor that yields a `Free` monad</param>
    /// <typeparam name="F">Functor type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public static Free<F, A> lift<F, A>(K<F, A> value) 
        where F : Functor<F> =>
        new Bind<F, A>(value.Map(pure<F, A>));    
    
    /// <summary>
    /// Monadic bind case for the free monad
    /// </summary>
    /// <param name="value">Functor that yields a `Free` monad</param>
    /// <typeparam name="F">Functor type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public static Free<F, A> bind<F, A>(K<F, Free<F, A>> value) 
        where F : Functor<F> =>
        new Bind<F, A>(value);

    /// <summary>
    /// Lift a natural transformation from `F` to `G` into a natural-transformation from
    /// `Free F` to `Free G`.
    /// </summary>
    /// <param name="fb">Free monad in F</param>
    /// <typeparam name="N">Natural transformation</typeparam>
    /// <typeparam name="F">Functor</typeparam>
    /// <typeparam name="G">Functor</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Free monad in G</returns>
    public static Free<G, A> hoist<F, G, A>(Free<F, A> fb)
        where F : Natural<F, G>, Functor<F>
        where G : Functor<G> =>
        fb switch
        {
            Pure<F, A>(var x)  => pure<G, A>(x),
            Bind<F, A>(var xs) => bind(F.Transform(xs).Map(hoist<F, G, A>)),
            _                  => throw new NotSupportedException()
        };
}
