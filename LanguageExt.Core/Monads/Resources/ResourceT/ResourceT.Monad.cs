using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class ResourceT<M> : 
    MonadT<ResourceT<M>, M>, 
    Alternative<ResourceT<M>>,
    Resource<ResourceT<M>>
    where M : Monad<M>, Alternative<M>
{
    static K<ResourceT<M>, B> Monad<ResourceT<M>>.Bind<A, B>(K<ResourceT<M>, A> ma, Func<A, K<ResourceT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<ResourceT<M>, B> Functor<ResourceT<M>>.Map<A, B>(Func<A, B> f, K<ResourceT<M>, A> ma) => 
        ma.As().Map(f);

    static K<ResourceT<M>, A> Applicative<ResourceT<M>>.Pure<A>(A value) => 
        ResourceT<M, A>.Pure(value);

    static K<ResourceT<M>, B> Applicative<ResourceT<M>>.Apply<A, B>(K<ResourceT<M>, Func<A, B>> mf, K<ResourceT<M>, A> ma) => 
        mf.As().Bind(f => ma.As().Map(f));

    static K<ResourceT<M>, B> Applicative<ResourceT<M>>.Action<A, B>(K<ResourceT<M>, A> ma, K<ResourceT<M>, B> mb) => 
        ma.As().Bind(_ => mb);

    static K<ResourceT<M>, A> MonadT<ResourceT<M>, M>.Lift<A>(K<M, A> ma) =>
        ResourceT<M, A>.Lift(ma);

    static K<ResourceT<M>, A> Monad<ResourceT<M>>.LiftIO<A>(IO<A> ma) => 
        ResourceT<M, A>.Lift(M.LiftIO(ma));

    static K<ResourceT<M>, B> Monad<ResourceT<M>>.WithRunInIO<A, B>(
        Func<Func<K<ResourceT<M>, A>, IO<A>>, IO<B>> inner) =>
        new ResourceT<M, B>(
            env =>
                M.WithRunInIO<A, B>(
                    run =>
                        inner(ma => run(ma.As().runResource(env)))));

    static K<ResourceT<M>, A> Alternative<ResourceT<M>>.Empty<A>() => 
        ResourceT<M, A>.Lift(M.Empty<A>());

    static K<ResourceT<M>, A> Alternative<ResourceT<M>>.Or<A>(K<ResourceT<M>, A> ma, K<ResourceT<M>, A> mb) =>
        new ResourceT<M, A>(
            res =>
                M.Or(ma.As().runResource(res), mb.As().runResource(res)));

    static K<ResourceT<M>, A> Resource<ResourceT<M>>.Use<A>(IO<A> ma, Func<A, IO<Unit>> release) =>
        use(ma, release);

    static K<ResourceT<M>, Unit> Resource<ResourceT<M>>.Release<A>(A value) => 
        release(value);
}
    
