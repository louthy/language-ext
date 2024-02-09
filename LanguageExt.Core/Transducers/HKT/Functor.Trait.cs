using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface Functor<F>  
    where F : Functor<F>
{
    public static abstract Functor<F, B> Map<A, B>(Functor<F, A> ma, Transducer<A, B> f);

    public static virtual Functor<F, B> Map<A, B>(Functor<F, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}

public interface FunctorT<F, G>  
    where F : FunctorT<F, G>
    where G : Functor<G>
{
    public static abstract FunctorT<F, G, B> Map<A, B>(FunctorT<F, G, A> ma, Transducer<A, B> f);

    public static virtual FunctorT<F, G, B> Map<A, B>(FunctorT<F, G, A> ma, Func<A, B> f) =>
        F.Map(ma, lift(f));
}
