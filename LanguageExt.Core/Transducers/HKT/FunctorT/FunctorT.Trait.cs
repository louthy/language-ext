using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

/// <summary>
/// FunctorT trait
/// </summary>
/// <typeparam name="F">Self referring trait</typeparam>
/// <typeparam name="G">Inner functor trait</typeparam>
public interface FunctorT<F, G>  
    where F : FunctorT<F, G>
    where G : Functor<G>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract FunctorT<F, G, B> Map<A, B>(FunctorT<F, G, A> ma, Transducer<A, B> f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual FunctorT<F, G, B> Map<A, B>(FunctorT<F, G, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}
