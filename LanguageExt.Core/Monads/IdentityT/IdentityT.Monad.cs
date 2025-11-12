using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identity module
/// </summary>
public class IdentityT<M> : 
    MonadT<IdentityT<M>, M>, 
    Choice<IdentityT<M>>,
    MonadUnliftIO<IdentityT<M>>
    where M : Monad<M>, Choice<M>
{
 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //
    
    static K<IdentityT<M>, B> Monad<IdentityT<M>>.Bind<A, B>(K<IdentityT<M>, A> ma, Func<A, K<IdentityT<M>, B>> f) =>
        ma.As().Bind(f);

    static K<IdentityT<M>, B> Functor<IdentityT<M>>.Map<A, B>(Func<A, B> f, K<IdentityT<M>, A> ma) => 
        ma.As().Map(f);

    static K<IdentityT<M>, A> Applicative<IdentityT<M>>.Pure<A>(A value) => 
        IdentityT.Pure<M, A>(value);

    static K<IdentityT<M>, B> Applicative<IdentityT<M>>.Apply<A, B>(K<IdentityT<M>, Func<A, B>> mf, K<IdentityT<M>, A> ma) =>
        mf.As().Bind(f => ma.As().Map(f));

    static K<IdentityT<M>, B> Applicative<IdentityT<M>>.Action<A, B>(K<IdentityT<M>, A> ma, K<IdentityT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<IdentityT<M>, A> MonadT<IdentityT<M>, M>.Lift<A>(K<M, A> ma) =>
        IdentityT.lift(ma);

    static K<IdentityT<M>, A> MonadIO<IdentityT<M>>.LiftIO<A>(IO<A> ma) => 
        IdentityT.lift(M.LiftIOMaybe(ma));

    static K<IdentityT<M>, IO<A>> MonadUnliftIO<IdentityT<M>>.ToIO<A>(K<IdentityT<M>, A> ma) =>
        new IdentityT<M, IO<A>>(M.ToIOMaybe(ma.As().Value));

    static K<IdentityT<M>, B> MonadUnliftIO<IdentityT<M>>.MapIO<A, B>(K<IdentityT<M>, A> ma, Func<IO<A>, IO<B>> f) => 
        new IdentityT<M, B>(M.MapIOMaybe(ma.As().Value, f));

    static K<IdentityT<M>, A> SemigroupK<IdentityT<M>>.Combine<A>(K<IdentityT<M>, A> ma, K<IdentityT<M>, A> mb) =>
        new IdentityT<M, A>(M.Combine(ma.As().Value, mb.As().Value));

    static K<IdentityT<M>, A> Choice<IdentityT<M>>.Choose<A>(K<IdentityT<M>, A> ma, K<IdentityT<M>, A> mb) =>
        new IdentityT<M, A>(M.Combine(ma.As().Value, mb.As().Value));

    static K<IdentityT<M>, A> Choice<IdentityT<M>>.Choose<A>(K<IdentityT<M>, A> ma, Func<K<IdentityT<M>, A>> mb) => 
        new IdentityT<M, A>(M.Combine(ma.As().Value, mb().As().Value));
}
