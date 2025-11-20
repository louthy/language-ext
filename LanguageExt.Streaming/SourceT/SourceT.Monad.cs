using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class SourceT<M> : 
    MonadUnliftIO<SourceT<M>>,
    Alternative<SourceT<M>>
    where M : MonadIO<M>, Alternative<M>
{
    static K<SourceT<M>, B> Monad<SourceT<M>>.Bind<A, B>(K<SourceT<M>, A> ma, Func<A, K<SourceT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<SourceT<M>, B> Functor<SourceT<M>>.Map<A, B>(Func<A, B> f, K<SourceT<M>, A> ma) => 
        ma.As().Map(f);

    static K<SourceT<M>, A> Applicative<SourceT<M>>.Pure<A>(A value) =>
        new PureSourceT<M, A>(value);

    static K<SourceT<M>, B> Applicative<SourceT<M>>.Apply<A, B>(K<SourceT<M>, Func<A, B>> mf, K<SourceT<M>, A> ma) => 
        ma.As().ApplyBack(mf.As());

    static K<SourceT<M>, A> SemigroupK<SourceT<M>>.Combine<A>(K<SourceT<M>, A> fa, K<SourceT<M>, A> fb) =>
        fa.As().Combine(fb.As());

    static K<SourceT<M>, A> Choice<SourceT<M>>.Choose<A>(K<SourceT<M>, A> fa, K<SourceT<M>, A> fb) =>
        fa.As().Choose(fb.As());

    static K<SourceT<M>, A> Choice<SourceT<M>>.Choose<A>(K<SourceT<M>, A> fa, Func<K<SourceT<M>, A>> fb) => 
        fa.As().Choose(() => fb().As());

    static K<SourceT<M>, A> MonoidK<SourceT<M>>.Empty<A>() =>
        EmptySourceT<M ,A>.Default;

    static K<SourceT<M>, A> MonadIO<SourceT<M>>.LiftIO<A>(IO<A> ma) =>
        SourceT.liftIO<M, A>(ma);

    static K<SourceT<M>, IO<A>> MonadUnliftIO<SourceT<M>>.ToIO<A>(K<SourceT<M>, A> ma) => 
        new ToIOSourceT<M, A>(ma.As());

    static K<SourceT<M>, B> MonadUnliftIO<SourceT<M>>.MapIO<A, B>(K<SourceT<M>, A> ma, Func<IO<A>, IO<B>> f) =>
        new MapIOSourceT<M, A, B>(ma.As(), f);
}
