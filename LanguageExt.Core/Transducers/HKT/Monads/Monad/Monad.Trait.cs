using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

/// <summary>
/// Monad trait
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
public interface Monad<M> : Applicative<M> 
    where M : Monad<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract Monad<M, B> Bind<A, B>(Monad<M, A> ma, Transducer<A, Monad<M, B>> f);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual Monad<M, B> Bind<A, B>(Monad<M, A> ma, Func<A, Monad<M, B>> f) =>
        M.Bind(ma, lift(f));

    public static virtual Monad<M, A> Flatten<A>(Monad<M, Monad<M, A>> mma) =>
        M.Bind(mma, identity);

    // Functor
    
    public static virtual Monad<M, B> Map<A, B>(Transducer<A, B> f, Monad<M, A> ma) =>
        M.Bind(ma, f.Map(M.Pure));
    
    public static virtual Monad<M, B> Map<A, B>(Func<A, B> f, Monad<M, A> ma) =>
        M.Map(lift(f), ma);
    
    static Functor<M, B> Functor<M>.Map<A, B>(Transducer<A, B> f, Functor<M, A> ma) =>
        M.Bind(ma.AsMonad(), f.Map(M.Pure));
    
    // Applicative

    public new static virtual Monad<M, A> Pure<A>(A value) =>
        (Monad<M, A>)Applicative.pure<M, A>(value);

    public static virtual Monad<M, B> Apply<A, B>(Monad<M, Transducer<A, B>> mf, Monad<M, A> ma) =>
        (Monad<M, B>)Applicative.apply(mf, ma);
    
    public static virtual Monad<M, B> Apply<A, B>(Monad<M, Func<A, B>> mf, Monad<M, A> ma) =>
        (Monad<M, B>)Applicative.action(mf, ma);
    
    public static virtual Monad<M, B> Action<A, B>(Monad<M, A> ma, Monad<M, B> mb) =>
        (Monad<M, B>)Applicative.action(ma, mb);
}
