using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

public class Eff : Monad<Eff>, Alternative<Eff> 
{
    /// <summary>
    /// Resource acquisition
    /// </summary>
    /// <param name="acquire">Resource to acquire</param>
    /// <param name="release">Function to release the resource</param>
    /// <typeparam name="X"></typeparam>
    /// <returns>Effect with the resource tracked.  Call `release(a)` to free the resource manually</returns>
    public static Eff<A> use<A>(IO<A> acquire, Func<A, IO<Unit>> release) =>
        new(new Eff<MinRT, A>(ReaderT<MinRT>.lift(ResourceT<IO>.use(acquire, release))));

    /// <summary>
    /// Resource acquisition
    /// </summary>
    /// <param name="acquire">Resource to acquire</param>
    /// <typeparam name="X"></typeparam>
    /// <returns>Effect with the resource tracked.  Call `release(a)` to free the resource manually</returns>
    public static Eff<A> use<A>(IO<A> acquire) where A : IDisposable =>
        use(acquire, dispose);

    static IO<Unit> dispose<A>(A value) where A : IDisposable =>
        IO.lift(() => value.Dispose());
    
    /// <summary>
    /// Resource release
    /// </summary>
    /// <param name="resource">Resource to release</param>
    public static Eff<Unit> release<A>(A resource) =>
        new(new Eff<MinRT, Unit>(ReaderT<MinRT>.lift(ResourceT<IO>.release(resource))));

    static K<Eff, B> Monad<Eff>.Bind<A, B>(K<Eff, A> ma, Func<A, K<Eff, B>> f) => 
        ma.As().Bind(f);

    static K<Eff, B> Functor<Eff>.Map<A, B>(Func<A, B> f, K<Eff, A> ma) => 
        ma.As().Map(f);

    static K<Eff, A> Applicative<Eff>.Pure<A>(A value) => 
        Eff<A>.Pure(value);

    static K<Eff, B> Applicative<Eff>.Apply<A, B>(K<Eff, Func<A, B>> mf, K<Eff, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Eff, B> Applicative<Eff>.Action<A, B>(K<Eff, A> ma, K<Eff, B> mb) => 
        ma.As().Action(mb.As());

    static K<Eff, A> Alternative<Eff>.Empty<A>() => 
        Eff<A>.Fail(Errors.None);

    static K<Eff, A> Alternative<Eff>.Or<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        ma.As() | mb.As();
    
    public class R<RT> : Monad<R<RT>>, Alternative<R<RT>> 
        where RT : HasIO<RT>
    {
        static K<R<RT>, B> Monad<R<RT>>.Bind<A, B>(K<R<RT>, A> ma, Func<A, K<R<RT>, B>> f) => 
            ma.As().Bind(f);

        static K<R<RT>, B> Functor<R<RT>>.Map<A, B>(Func<A, B> f, K<R<RT>, A> ma) => 
            ma.As().Map(f);

        static K<R<RT>, A> Applicative<R<RT>>.Pure<A>(A value) => 
            Eff<RT, A>.Pure(value);

        static K<R<RT>, B> Applicative<R<RT>>.Apply<A, B>(K<R<RT>, Func<A, B>> mf, K<R<RT>, A> ma) => 
            mf.As().Apply(ma.As());

        static K<R<RT>, B> Applicative<R<RT>>.Action<A, B>(K<R<RT>, A> ma, K<R<RT>, B> mb) => 
            ma.As().Action(mb.As());

        static K<R<RT>, A> Alternative<R<RT>>.Empty<A>() => 
            Eff<RT, A>.Fail(Errors.None);

        static K<R<RT>, A> Alternative<R<RT>>.Or<A>(K<R<RT>, A> ma, K<R<RT>, A> mb) => 
            ma.As() | mb.As();
    }
}
