using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public class ConsumerT<IN, M> : MonadT<ConsumerT<IN, M>, M>
    where M : Monad<M>
{
    static K<ConsumerT<IN, M>, B> Monad<ConsumerT<IN, M>>.Bind<A, B>(
        K<ConsumerT<IN, M>, A> ma, 
        Func<A, K<ConsumerT<IN, M>, B>> f) => 
        ma.As().Bind(f);

    static K<ConsumerT<IN, M>, B> Functor<ConsumerT<IN, M>>.Map<A, B>(
        Func<A, B> f, 
        K<ConsumerT<IN, M>, A> ma) => 
        ma.As().Map(f);

    static K<ConsumerT<IN, M>, A> Applicative<ConsumerT<IN, M>>.Pure<A>(A value) => 
        ConsumerT.pure<IN, M, A>(value);

    static K<ConsumerT<IN, M>, B> Applicative<ConsumerT<IN, M>>.Apply<A, B>(
        K<ConsumerT<IN, M>, Func<A, B>> mf,
        K<ConsumerT<IN, M>, A> ma) =>
        mf.As().Apply(ma.As());

    static K<ConsumerT<IN, M>, A> MonadT<ConsumerT<IN, M>, M>.Lift<A>(K<M, A> ma) =>
        ConsumerT.liftM<IN, M, A>(ma);

    static K<ConsumerT<IN, M>, A> MonadIO<ConsumerT<IN, M>>.LiftIO<A>(IO<A> ma) => 
        ConsumerT.liftIO<IN, M, A>(ma); 

    static K<ConsumerT<IN, M>, B> MonadIO<ConsumerT<IN, M>>.MapIO<A, B>(K<ConsumerT<IN, M>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapIO(f);

    static K<ConsumerT<IN, M>, IO<A>> MonadIO<ConsumerT<IN, M>>.ToIO<A>(K<ConsumerT<IN, M>, A> ma) => 
        ma.MapIO(IO.pure);
}
