using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface MonadT<M, N> : FunctorT<M, N> 
    where M : MonadT<M, N>
    where N : Monad<N>
{
    public static abstract MonadT<M, N, A> Pure<A>(A value);

    public static abstract MonadT<M, N, A> Lift<A>(Monad<N, A> ma);

    public static abstract MonadT<M, N, B> Bind<A, B>(MonadT<M, N, A> ma, Transducer<A, MonadT<M, N, B>> f);
    
    public static virtual MonadT<M, N, B> Bind<A, B>(MonadT<M, N, A> ma, Func<A, MonadT<M, N, B>> f) =>
        M.Bind(ma, lift(f));

    public static virtual MonadT<M, N, A> Flatten<A>(MonadT<M, N, MonadT<M, N, A>> mma) =>
        M.Bind(mma, identity);
    
    public static virtual MonadT<M, N, B> Map<A, B>(MonadT<M, N, A> ma, Transducer<A, B> f) =>
        M.Bind(ma, f.Map(M.Pure));
    
    public static virtual MonadT<M, N, B> Map<A, B>(MonadT<M, N, A> ma, Func<A, B> f) =>
        M.Map(ma, lift(f));
    
    static FunctorT<M, N, B> FunctorT<M, N>.Map<A, B>(FunctorT<M, N, A> ma, Transducer<A, B> f) =>
        M.Map(ma.AsMonad(), f);
}

public interface MonadT<M, N, A> : FunctorT<M, N, A>
    where M : MonadT<M, N>
    where N : Monad<N>;
