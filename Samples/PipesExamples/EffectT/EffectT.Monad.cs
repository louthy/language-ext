using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public class EffectT<M> : MonadT<EffectT<M>, M>
    where M : Monad<M>
{
    static K<EffectT<M>, B> Monad<EffectT<M>>.Bind<A, B>(K<EffectT<M>, A> ma, Func<A, K<EffectT<M>, B>> f) => 
        throw new NotImplementedException();

    static K<EffectT<M>, B> Functor<EffectT<M>>.Map<A, B>(Func<A, B> f, K<EffectT<M>, A> ma) => 
        throw new NotImplementedException();

    static K<EffectT<M>, A> Applicative<EffectT<M>>.Pure<A>(A value) => 
        throw new NotImplementedException();

    static K<EffectT<M>, B> Applicative<EffectT<M>>.Apply<A, B>(K<EffectT<M>, Func<A, B>> mf, K<EffectT<M>, A> ma) => 
        throw new NotImplementedException();

    static K<EffectT<M>, A> MonadT<EffectT<M>, M>.Lift<A>(K<M, A> ma) => 
        throw new NotImplementedException();

    static K<EffectT<M>, A> MonadIO<EffectT<M>>.LiftIO<A>(IO<A> ma) => 
        throw new NotImplementedException();

    static K<EffectT<M>, IO<A>> MonadIO<EffectT<M>>.ToIO<A>(K<EffectT<M>, A> ma) => 
        throw new NotImplementedException();
}
