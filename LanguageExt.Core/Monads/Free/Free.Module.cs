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
}
