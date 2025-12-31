using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Source : 
    Monad<Source>, 
    MonoidK<Source>,
    Alternative<Source>
{
    static K<Source, B> Monad<Source>.Bind<A, B>(K<Source, A> ma, Func<A, K<Source, B>> f) => 
        ma.As().Bind(f);

    static K<Source, B> Monad<Source>.Recur<A, B>(A value, Func<A, K<Source, Next<A, B>>> f) => 
        Monad.unsafeRecur(value, f);

    static K<Source, B> Functor<Source>.Map<A, B>(Func<A, B> f, K<Source, A> ma) => 
        ma.As().Map(f);

    static K<Source, A> Applicative<Source>.Pure<A>(A value) =>
        new Source<A>(SourceT.pure<IO, A>(value));

    static K<Source, B> Applicative<Source>.Apply<A, B>(K<Source, Func<A, B>> mf, K<Source, A> ma) => 
        ma.As().ApplyBack(mf.As());

    static K<Source, B> Applicative<Source>.Apply<A, B>(K<Source, Func<A, B>> mf, Memo<Source, A> ma) =>
        ma.Value.As().ApplyBack(mf.As());

    static K<Source, A> SemigroupK<Source>.Combine<A>(K<Source, A> fa, K<Source, A> fb) =>
        fa.As().Combine(fb.As());

    static K<Source, A> Choice<Source>.Choose<A>(K<Source, A> fa, K<Source, A> fb) =>
        fa.As().Choose(fb.As());

    public static K<Source, A> Choose<A>(K<Source, A> fa, Memo<Source, A> fb) => 
        fa.As().Choose(fb);

    static K<Source, A> Alternative<Source>.Empty<A>() =>
        Source<A>.Empty;

    static K<Source, A> MonoidK<Source>.Empty<A>() =>
        Source<A>.Empty;
}
