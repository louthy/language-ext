using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public class PipeT<IN, OUT, M> : MonadT<PipeT<IN, OUT, M>, M>
    where M : Monad<M>
{
    static K<PipeT<IN, OUT, M>, B> Monad<PipeT<IN, OUT, M>>.Bind<A, B>(
        K<PipeT<IN, OUT, M>, A> ma, 
        Func<A, K<PipeT<IN, OUT, M>, B>> f) => 
        ma.As().Bind(f);

    static K<PipeT<IN, OUT, M>, B> Functor<PipeT<IN, OUT, M>>.Map<A, B>(
        Func<A, B> f, 
        K<PipeT<IN, OUT, M>, A> ma) => 
        ma.As().Map(f);

    static K<PipeT<IN, OUT, M>, A> Applicative<PipeT<IN, OUT, M>>.Pure<A>(A value) => 
        PipeT.pure<IN, OUT, M, A>(value);

    static K<PipeT<IN, OUT, M>, B> Applicative<PipeT<IN, OUT, M>>.Apply<A, B>(
        K<PipeT<IN, OUT, M>, Func<A, B>> mf,
        K<PipeT<IN, OUT, M>, A> ma) =>
        mf.As().Apply(ma.As());

    static K<PipeT<IN, OUT, M>, A> MonadT<PipeT<IN, OUT, M>, M>.Lift<A>(K<M, A> ma) => 
        PipeT.liftM<IN, OUT, M, A>(ma);

    static K<PipeT<IN, OUT, M>, A> MonadIO<PipeT<IN, OUT, M>>.LiftIO<A>(IO<A> ma) => 
        PipeT.liftIO<IN, OUT, M, A>(ma);

    static K<PipeT<IN, OUT, M>, B> MonadIO<PipeT<IN, OUT, M>>.MapIO<A, B>(K<PipeT<IN, OUT, M>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapIO(f);

    static K<PipeT<IN, OUT, M>, IO<A>> MonadIO<PipeT<IN, OUT, M>>.ToIO<A>(K<PipeT<IN, OUT, M>, A> ma) => 
        ma.MapIO(IO.pure);
}
