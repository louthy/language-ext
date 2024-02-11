using System;

namespace LanguageExt.HKT;

/// <summary>
/// Functor module
/// </summary>
public static class Functor
{
    public static K<F, B> map<F, A, B>(Func<A, B> f, K<F, A> ma) 
        where F : Functor<F> =>
        F.Map(f, ma);
}
