using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public class ProducerT<OUT, M> : MonadT<ProducerT<OUT, M>, M>
    where M : Monad<M>
{
    static K<ProducerT<OUT, M>, B> Monad<ProducerT<OUT, M>>.Bind<A, B>(
        K<ProducerT<OUT, M>, A> ma, 
        Func<A, K<ProducerT<OUT, M>, B>> f) => 
        throw new NotImplementedException();

    static K<ProducerT<OUT, M>, B> Functor<ProducerT<OUT, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ProducerT<OUT, M>, A> ma) => 
        throw new NotImplementedException();

    static K<ProducerT<OUT, M>, A> Applicative<ProducerT<OUT, M>>.Pure<A>(A value) => 
        throw new NotImplementedException();

    static K<ProducerT<OUT, M>, B> Applicative<ProducerT<OUT, M>>.Apply<A, B>(
        K<ProducerT<OUT, M>, Func<A, B>> mf, 
        K<ProducerT<OUT, M>, A> ma) => 
        throw new NotImplementedException();

    static K<ProducerT<OUT, M>, A> MonadT<ProducerT<OUT, M>, M>.Lift<A>(K<M, A> ma) => 
        throw new NotImplementedException();

    static K<ProducerT<OUT, M>, A> MonadIO<ProducerT<OUT, M>>.LiftIO<A>(IO<A> ma) => 
        throw new NotImplementedException();

    static K<ProducerT<OUT, M>, IO<A>> MonadIO<ProducerT<OUT, M>>.ToIO<A>(K<ProducerT<OUT, M>, A> ma) => 
        throw new NotImplementedException();
}
