using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Ignores the bound value result and instead maps it to `Unit`
    /// </summary>
    /// <param name="fa">Functor that returns a bound value that should be ignored</param>
    /// <typeparam name="F">Functor trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Functor with unit bound value</returns>
    public static K<F, Unit> ignore<F, A>(K<F, A> fa)
        where F : Functor<F> =>
        map(_ => default(Unit), fa);
    
    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public static K<F, B> map<F, A, B>(Func<A, B> f, K<F, A> ma) 
        where F : Functor<F> =>
        ma.Map(f);
}
