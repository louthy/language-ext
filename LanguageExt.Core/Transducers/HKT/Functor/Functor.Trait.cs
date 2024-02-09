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
    
    public static abstract Functor<F, B> Map<A, B>(Functor<F, A> ma, Transducer<A, B> f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual Functor<F, B> Map<A, B>(Functor<F, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}
