using System;
using LanguageExt.Traits;

namespace LanguageExt;


/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class ReaderT<Env>
{
    public static ReaderT<Env, M, A> lift<M, A>(K<M, A> ma)  
        where M : Monad<M>, Choice<M> => 
        ReaderT<Env, M, A>.Lift(ma);
}

public partial class ReaderT<Env, M>
{
    public static ReaderT<Env, M, A> pure<A>(A value) => 
        ReaderT<Env, M, A>.Pure(value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> liftIO<A>(IO<A> effect) =>
        ReaderT<Env, M, A>.LiftIO(effect);
}

/// <summary>
/// `MonadReaderT` trait implementation for `ReaderT` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class ReaderT
{
    public static ReaderT<Env, M, A> pure<Env, M, A>(A value)  
        where M : Monad<M>, Choice<M> => 
        ReaderT<Env, M, A>.Pure(value);

    public static ReaderT<Env, M, A> lift<Env, M, A>(K<M, A> ma)  
        where M : Monad<M>, Choice<M> => 
        ReaderT<Env, M, A>.Lift(ma);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> liftIO<Env, M, A>(IO<A> effect)
        where M : Monad<M>, Choice<M> =>
        ReaderT<Env, M, A>.LiftIO(effect);
    
    public static ReaderT<Env, M, Env> ask<M, Env>() 
        where M : Monad<M>, Choice<M> => 
        ReaderT<Env, M, Env>.Asks(Prelude.identity);

    public static ReaderT<Env, M, A> asks<M, A, Env>(Func<Env, A> f)  
        where M : Monad<M>, Choice<M> => 
        ReaderT<Env, M, A>.Asks(f);

    public static ReaderT<Env, M, A> asksM<M, Env, A>(Func<Env, K<M, A>> f)
        where M : Monad<M>, Choice<M> =>
        ReaderT<Env, M, A>.AsksM(f);

    public static ReaderT<Env, M, A> local<Env, M, A>(Func<Env, Env> f, ReaderT<Env, M, A> ma) 
        where M : Monad<M>, Choice<M> => 
        ma.As().Local(f);

    public static ReaderT<Env, M, A> with<Env, SubEnv, M, A>(Func<Env, SubEnv> f, ReaderT<SubEnv, M, A> ma) 
        where M : Monad<M>, Choice<M> => 
        ma.As().With(f);
}
