using System;

namespace LanguageExt.HKT;

/// <summary>
/// FunctorT module
/// </summary>
public static class FunctorT
{
    public static FunctorT<F, G, B> map<F, G, A, B>(FunctorT<F, G, A> ma, Transducer<A, B> f)
        where F : FunctorT<F, G>
        where G : Functor<G> =>
        F.Map(ma, f);

    public static FunctorT<F, G, B> map<F, G, A, B>(FunctorT<F, G, A> ma, Func<A, B> f)
        where F : FunctorT<F, G>
        where G : Functor<G> =>
        F.Map(ma, f);

    public static MonadT<F, G, A> AsMonad<F, G, A>(this FunctorT<F, G, A> fa)
        where F : FunctorT<F, G>, MonadT<F, G>
        where G : Functor<G>, Monad<G> =>
        (MonadT<F, G, A>)fa;
}

