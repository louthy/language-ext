using System;

namespace LanguageExt.Traits;

/// <summary>
/// Functor module
/// </summary>
public static class Functor
{
    /// <summary>
    /// Functor map.  Maps all contained values of `A` to values of `B`
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <param name="fa">Functor structure</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Mapped functor</returns>
    public static K<F, B> map<F, A, B>(Func<A, B> f, K<F, A> fa) 
        where F : Functor<F> =>
        F.Map(f, fa);
    
    /// <summary>
    /// Ignores the bound value result and instead maps it to `Unit`
    /// </summary>
    /// <param name="fa">Functor that returns a bound value that should be ignored</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Functor with unit bound value</returns>
    public static K<F, Unit> ignore<F, A>(K<F, A> fa)
        where F : Functor<F> =>
        fa.Map(_ => default(Unit));
}
