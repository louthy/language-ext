using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class StateT<S>
{
    public static StateT<S, M, A> lift<M, A>(K<M, A> ma)  
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, A>.Lift(ma);
}

public partial class StateT<S, M>
{
    public static StateT<S, M, A> pure<A>(A value) => 
        StateT<S, M, A>.Pure(value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> liftIO<A>(IO<A> effect) =>
        StateT<S, M, A>.LiftIO(effect);
}

public class StateT
{
    public static StateT<S, M, A> pure<S, M, A>(A value)  
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, A>.Pure(value);

    public static StateT<S, M, A> lift<S, M, A>(K<M, A> ma)  
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, A>.Lift(ma);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, A> liftIO<S, M, A>(IO<A> effect)
        where M : Monad<M>, Choice<M> =>
        StateT<S, M, A>.LiftIO(effect);
    
    public static StateT<S, M, S> get<M, S>() 
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, S>.Get;
    
    public static StateT<S, M, A> gets<M, S, A>(Func<S, A> f) 
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, A>.Gets(f);

    public static StateT<S, M, A> getsM<M, S, A>(Func<S, K<M, A>> f) 
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, A>.GetsM(f);

    public static StateT<S, M, Unit> put<M, S>(S state)  
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, Unit>.Put(state);

    public static StateT<S, M, Unit> modify<M, S>(Func<S, S> f)  
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, Unit>.Modify(f);

    public static StateT<S, M, Unit> modifyM<M, S>(Func<S, K<M, S>> f)  
        where M : Monad<M>, Choice<M> => 
        StateT<S, M, Unit>.ModifyM(f);
}
