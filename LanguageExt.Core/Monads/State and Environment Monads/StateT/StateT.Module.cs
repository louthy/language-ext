using System;
using LanguageExt.Traits;

namespace LanguageExt;


/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class StateT<S>
{
    public static StateT<S, M, A> lift<M, A>(K<M, A> ma)  
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, A>.Lift(ma);
}

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class StateT<S, M>
{
    public static StateT<S, M, A> Pure<A>(A value) => 
        StateT<S, M, A>.Pure(value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> liftIO<A>(IO<A> effect) =>
        StateT<S, M, A>.LiftIO(effect);
}

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class StateT
{
    public static StateT<S, M, B> bind<S, M, A, B>(StateT<S, M, A> ma, Func<A, StateT<S, M, B>> f) 
        where M : Monad<M>, Alternative<M> => 
        ma.As().Bind(f);

    public static StateT<S, M, B> map<S, M, A, B>(Func<A, B> f, StateT<S, M, A> ma)  
        where M : Monad<M>, Alternative<M> => 
        ma.As().Map(f);

    public static StateT<S, M, A> Pure<S, M, A>(A value)  
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, A>.Pure(value);

    public static StateT<S, M, B> apply<S, M, A, B>(StateT<S, M, Func<A, B>> mf, StateT<S, M, A> ma)  
        where M : Monad<M>, Alternative<M> => 
        mf.As().Bind(x =>ma.As().Map(x));

    public static StateT<S, M, B> action<S, M, A, B>(StateT<S, M, A> ma, StateT<S, M, B> mb) 
        where M : Monad<M>, Alternative<M> => 
        ma.As().Bind(_ => mb);

    public static StateT<S, M, A> lift<S, M, A>(K<M, A> ma)  
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, A>.Lift(ma);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> liftIO<S, M, A>(IO<A> effect)
        where M : Monad<M>, Alternative<M> =>
        StateT<S, M, A>.LiftIO(effect);
    
    public static StateT<S, M, S> get<S, M>() 
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, S>.Get;
    
    public static StateT<S, M, A> gets<S, M, A>(Func<S, A> f) 
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, A>.Gets(f);

    public static StateT<S, M, A> getsM<S, M, A>(Func<S, K<M, A>> f) 
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, A>.GetsM(f);

    public static StateT<S, M, Unit> put<S, M>(S state)  
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, Unit>.Put(state);

    public static StateT<S, M, Unit> modify<S, M>(Func<S, S> f)  
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, Unit>.Modify(f);

    public static StateT<S, M, Unit> modifyM<S, M>(Func<S, K<M, S>> f)  
        where M : Monad<M>, Alternative<M> => 
        StateT<S, M, Unit>.ModifyM(f);
}
