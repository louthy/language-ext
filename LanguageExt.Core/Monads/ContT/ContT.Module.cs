using System;
using LanguageExt.Traits;

namespace LanguageExt.ContT;

public class ContT
{
    /// <summary>
    /// Construct the terminal condition for the monad transformer
    /// </summary>
    /// <param name="value">Pure value to terminate with</param>
    /// <returns>`ContT`</returns>
    public static ContT<A, M, A> Pure<M, A>(A value) 
        where M : Applicative<M> =>
        new(f => f(value));
    
    public static ContT<R, M, A> lift<R, M, A>(Func<Func<A, K<M, R>>, K<M, R>> f) 
        where M : Applicative<M> =>
        new(f);
}
