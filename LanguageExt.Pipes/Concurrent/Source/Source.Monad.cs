using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public partial class Source : 
    Monad<Source>, 
    Alternative<Source>,
    Transducable<Source>
{
    static K<Source, B> Monad<Source>.Bind<A, B>(K<Source, A> ma, Func<A, K<Source, B>> f) => 
        ma.As().Bind(f);

    static K<Source, B> Functor<Source>.Map<A, B>(Func<A, B> f, K<Source, A> ma) => 
        ma.As().Map(f);

    static K<Source, A> Applicative<Source>.Pure<A>(A value) =>
        new PureSource<A>(value);

    static K<Source, B> Applicative<Source>.Apply<A, B>(K<Source, Func<A, B>> mf, K<Source, A> ma) => 
        ma.As().ApplyBack(mf.As());

    static K<Source, A> SemigroupK<Source>.Combine<A>(K<Source, A> fa, K<Source, A> fb) =>
        fa.As().Combine(fb.As());

    static K<Source, A> Choice<Source>.Choose<A>(K<Source, A> fa, K<Source, A> fb) =>
        fa.As().Choose(fb.As());

    static K<Source, A> MonoidK<Source>.Empty<A>() =>
        EmptySource<A>.Default;

    static K<Source, B> Transducable<Source>.Transform<A, B>(K<Source, A> ma, K<Transducer<A>, B> tb) => 
        new TransformSource<A, B>(ma.As(), tb.As());

    static S Transducable<Source>.Reduce<S, A>(K<Source, A> ma, S initial, Reducer<S, A> reducer) => 
        ma.As().Reduce(initial, reducer);
}
