using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface Functor<F, A> where F : Functor<F>
{
    public Functor<F, A> AsFunctor() => this;
}

public interface FunctorT<F, G, A>
    where F : FunctorT<F, G>
    where G : Functor<G>
{
    public FunctorT<F, G, A> AsFunctorT() => this;
}
