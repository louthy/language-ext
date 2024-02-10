using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

/// <summary>
/// Functor trait
/// </summary>
/// <typeparam name="F">Self referring type</typeparam>
public interface Functor<F>  
    where F : Functor<F>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract Functor<F, B> Map<A, B>(Transducer<A, B> f, Functor<F, A> ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual Functor<F, B> Map<A, B>(Func<A, B> f, Functor<F, A> ma) =>
        F.Map(lift(f), ma);
}
