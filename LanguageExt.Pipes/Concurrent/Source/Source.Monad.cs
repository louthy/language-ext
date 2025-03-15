using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public partial class Source : 
    Monad<Source>, 
    Alternative<Source>
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

    public static K<Source, A> Choose<A>(K<Source, A> fa, Func<K<Source, A>> fb) => 
        fa.As().Choose(() => fb().As());

    static K<Source, A> MonoidK<Source>.Empty<A>() =>
        EmptySource<A>.Default;
}
