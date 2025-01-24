using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public class PipeT<IN, OUT, M> : MonadT<PipeT<IN, OUT, M>, M>
    where M : Monad<M>
{
    static K<PipeT<IN, OUT, M>, B> Monad<PipeT<IN, OUT, M>>.Bind<A, B>(
        K<PipeT<IN, OUT, M>, A> ma, 
        Func<A, K<PipeT<IN, OUT, M>, B>> f) => 
        throw new NotImplementedException();

    static K<PipeT<IN, OUT, M>, B> Functor<PipeT<IN, OUT, M>>.Map<A, B>(
        Func<A, B> f, 
        K<PipeT<IN, OUT, M>, A> ma) => 
        throw new NotImplementedException();

    static K<PipeT<IN, OUT, M>, A> Applicative<PipeT<IN, OUT, M>>.Pure<A>(A value) => 
        throw new NotImplementedException();

    static K<PipeT<IN, OUT, M>, B> Applicative<PipeT<IN, OUT, M>>.Apply<A, B>(
        K<PipeT<IN, OUT, M>, Func<A, B>> mf, 
        K<PipeT<IN, OUT, M>, A> ma) => 
        throw new NotImplementedException();

    static K<PipeT<IN, OUT, M>, A> MonadT<PipeT<IN, OUT, M>, M>.Lift<A>(K<M, A> ma) => 
        throw new NotImplementedException();

    static K<PipeT<IN, OUT, M>, A> MonadIO<PipeT<IN, OUT, M>>.LiftIO<A>(IO<A> ma) => 
        throw new NotImplementedException();

    static K<PipeT<IN, OUT, M>, IO<A>> MonadIO<PipeT<IN, OUT, M>>.ToIO<A>(K<PipeT<IN, OUT, M>, A> ma) => 
        throw new NotImplementedException();
}
