using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public class ConsumerT<IN, M> : MonadT<ConsumerT<IN, M>, M>
    where M : Monad<M>
{
    static K<ConsumerT<IN, M>, B> Monad<ConsumerT<IN, M>>.Bind<A, B>(
        K<ConsumerT<IN, M>, A> ma, 
        Func<A, K<ConsumerT<IN, M>, B>> f) => 
        throw new NotImplementedException();

    static K<ConsumerT<IN, M>, B> Functor<ConsumerT<IN, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ConsumerT<IN, M>, A> ma) => 
        throw new NotImplementedException();

    static K<ConsumerT<IN, M>, A> Applicative<ConsumerT<IN, M>>.Pure<A>(A value) => 
        throw new NotImplementedException();

    static K<ConsumerT<IN, M>, B> Applicative<ConsumerT<IN, M>>.Apply<A, B>(
        K<ConsumerT<IN, M>, Func<A, B>> mf, 
        K<ConsumerT<IN, M>, A> ma) => 
        throw new NotImplementedException();

    static K<ConsumerT<IN, M>, A> MonadT<ConsumerT<IN, M>, M>.Lift<A>(K<M, A> ma) => 
        throw new NotImplementedException();

    static K<ConsumerT<IN, M>, A> MonadIO<ConsumerT<IN, M>>.LiftIO<A>(IO<A> ma) => 
        throw new NotImplementedException();

    static K<ConsumerT<IN, M>, IO<A>> MonadIO<ConsumerT<IN, M>>.ToIO<A>(K<ConsumerT<IN, M>, A> ma) => 
        throw new NotImplementedException();
}
