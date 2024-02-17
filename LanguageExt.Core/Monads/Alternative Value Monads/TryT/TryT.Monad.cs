using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `TryT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class TryT<M> : MonadT<TryT<M>, M>
    where M : Monad<M>
{
    static K<TryT<M>, B> Monad<TryT<M>>.Bind<A, B>(K<TryT<M>, A> ma, Func<A, K<TryT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<TryT<M>, B> Functor<TryT<M>>.Map<A, B>(Func<A, B> f, K<TryT<M>, A> ma) => 
        ma.As().Map(f);

    static K<TryT<M>, A> Applicative<TryT<M>>.Pure<A>(A value) => 
        TryT<M, A>.Succ(value);

    static K<TryT<M>, B> Applicative<TryT<M>>.Apply<A, B>(K<TryT<M>, Func<A, B>> mf, K<TryT<M>, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<TryT<M>, B> Applicative<TryT<M>>.Action<A, B>(K<TryT<M>, A> ma, K<TryT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<TryT<M>, A> MonadT<TryT<M>, M>.Lift<A>(K<M, A> ma) => 
        TryT<M, A>.Lift(ma);
    
    static K<TryT<M>, A> Monad<TryT<M>>.LiftIO<A>(IO<A> ma) => 
        TryT<M, A>.Lift(M.LiftIO(ma));
}
