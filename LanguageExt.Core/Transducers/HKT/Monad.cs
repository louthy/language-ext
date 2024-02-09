using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface Monad<M> : Functor<M> 
    where M : Monad<M>
{
    public static abstract Monad<M, A> Pure<A>(A value);

    public static abstract Monad<M, B> Bind<A, B>(Monad<M, A> ma, Transducer<A, Monad<M, B>> f);
    
    public static virtual Monad<M, B> Bind<A, B>(Monad<M, A> ma, Func<A, Monad<M, B>> f) =>
        M.Bind(ma, lift(f));

    public static virtual Monad<M, A> Flatten<A>(Monad<M, Monad<M, A>> mma) =>
        M.Bind(mma, identity);
    
    public static virtual Monad<M, B> Map<A, B>(Monad<M, A> ma, Transducer<A, B> f) =>
        M.Bind(ma, f.Map(M.Pure));
    
    public static virtual Monad<M, B> Map<A, B>(Monad<M, A> ma, Func<A, B> f) =>
        M.Map(ma, lift(f));
    
    static Functor<M, B> Functor<M>.Map<A, B>(Functor<M, A> ma, Transducer<A, B> f) =>
        M.Bind(ma.AsMonad(), f.Map(M.Pure));
}

public interface Monad<M, A> : Functor<M, A>
    where M : Monad<M>
{
    public Monad<M, A> AsMonad() => this;
}
