using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class StateT<S, M> : MonadStateT<StateT<S, M>, S, M>, Alternative<StateT<S, M>>
    where M : Monad<M>, Alternative<M>
{
    static K<StateT<S, M>, B> Monad<StateT<S, M>>.Bind<A, B>(K<StateT<S, M>, A> ma, Func<A, K<StateT<S, M>, B>> f) => 
        ma.As().Bind(f);

    static K<StateT<S, M>, B> Functor<StateT<S, M>>.Map<A, B>(Func<A, B> f, K<StateT<S, M>, A> ma) => 
        ma.As().Map(f);

    static K<StateT<S, M>, A> Applicative<StateT<S, M>>.Pure<A>(A value) => 
        StateT<S, M, A>.Pure(value);

    static K<StateT<S, M>, B> Applicative<StateT<S, M>>.Apply<A, B>(K<StateT<S, M>, Func<A, B>> mf, K<StateT<S, M>, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<StateT<S, M>, B> Applicative<StateT<S, M>>.Action<A, B>(K<StateT<S, M>, A> ma, K<StateT<S, M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<StateT<S, M>, A> MonadT<StateT<S, M>, M>.Lift<A>(K<M, A> ma) => 
        StateT<S, M, A>.Lift(ma);

    static K<StateT<S, M>, Unit> MonadStateT<StateT<S, M>, S, M>.Put(S value) => 
        StateT<S, M, S>.Put(value);

    static K<StateT<S, M>, Unit> MonadStateT<StateT<S, M>, S, M>.Modify(Func<S, S> modify) => 
        StateT<S, M, S>.Modify(modify);

    static K<StateT<S, M>, A> MonadStateT<StateT<S, M>, S, M>.Gets<A>(Func<S, A> f) =>
        StateT<S, M, A>.Gets(f);

    static K<StateT<S, M>, A> Monad<StateT<S, M>>.LiftIO<A>(IO<A> ma) =>
        StateT<S, M, A>.Lift(M.LiftIO(ma));

    static K<StateT<S, M>, A> Alternative<StateT<S, M>>.Empty<A>() =>
        StateT<S, M, A>.Lift(M.Empty<A>());

    static K<StateT<S, M>, A> Alternative<StateT<S, M>>.Or<A>(K<StateT<S, M>, A> ma, K<StateT<S, M>, A> mb) => 
        new StateT<S, M, A>(state =>
            M.Or(ma.As().runState(state), mb.As().runState(state)));
}
