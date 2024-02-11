using System;

namespace LanguageExt.HKT;

/// <summary>
/// Functor trait
/// </summary>
/// <typeparam name="F">Self referring type</typeparam>
public interface Functor<F>  
    where F : Functor<F>
{
    public static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> ma);
}
