using System;
using LanguageExt.HKT;

namespace LanguageExt;

public partial class ResourceT<M> : MonadT<ResourceT<M>, M>
    where M : MonadIO<M>
{
    static K<ResourceT<M>, B> Monad<ResourceT<M>>.Bind<A, B>(K<ResourceT<M>, A> ma, Func<A, K<ResourceT<M>, B>> f) => 
        throw new NotImplementedException();

    static K<ResourceT<M>, B> Functor<ResourceT<M>>.Map<A, B>(Func<A, B> f, K<ResourceT<M>, A> ma) => 
        throw new NotImplementedException();

    static K<ResourceT<M>, A> Applicative<ResourceT<M>>.Pure<A>(A value) => 
        throw new NotImplementedException();

    static K<ResourceT<M>, B> Applicative<ResourceT<M>>.Apply<A, B>(K<ResourceT<M>, Func<A, B>> mf, K<ResourceT<M>, A> ma) => 
        throw new NotImplementedException();

    static K<ResourceT<M>, B> Applicative<ResourceT<M>>.Action<A, B>(K<ResourceT<M>, A> ma, K<ResourceT<M>, B> mb) => 
        throw new NotImplementedException();

    static K<ResourceT<M>, A> MonadT<ResourceT<M>, M>.Lift<A>(K<M, A> ma) => 
        throw new NotImplementedException();
}
