using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public partial class SourceT<M> : 
    Monad<SourceT<M>>, 
    Alternative<SourceT<M>>
    where M : Monad<M>, Alternative<M>
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
}
