using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class IO : Monad<IO>, Alternative<IO>
{
    public static IO<A> Pure<A>(A value) => 
        IO<A>.Pure(value);
    
    static K<IO, B> Monad<IO>.Bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma.As().Bind(f);

    static K<IO, B> Functor<IO>.Map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma.As().Map(f);

    static K<IO, A> Applicative<IO>.Pure<A>(A value) => 
        IO<A>.Pure(value);

    static K<IO, B> Applicative<IO>.Apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<IO, B> Applicative<IO>.Action<A, B>(K<IO, A> ma, K<IO, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<IO, A> MonoidK<IO>.Empty<A>() =>
        IO<A>.Empty;

    static K<IO, A> SemigroupK<IO>.Combine<A>(K<IO, A> ma, K<IO, A> mb) => 
        ma.As() | mb.As();

    static K<IO, A> Monad<IO>.LiftIO<A>(IO<A> ma) => 
        ma;
    
    static K<IO, B> Monad<IO>.WithRunInIO<A, B>(
        Func<Func<K<IO, A>, IO<A>>, IO<B>> inner) =>
        inner(io => io.As());
}
