using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Traits;

namespace LanguageExt;

public class Eff : 
    MonadUnliftIO<Eff>,
    Fallible<Eff>,
    Readable<Eff, MinRT>,
    Alternative<Eff>
{
    static K<Eff, B> Monad<Eff>.Bind<A, B>(K<Eff, A> ma, Func<A, K<Eff, B>> f) =>
        new Eff<B>(ma.As().effect.Bind(f));

    static K<Eff, B> Functor<Eff>.Map<A, B>(Func<A, B> f, K<Eff, A> ma) => 
        new Eff<B>(ma.As().effect.Map(f));

    static K<Eff, A> Applicative<Eff>.Pure<A>(A value) => 
        Eff<A>.Pure(value);

    static K<Eff, B> Applicative<Eff>.Apply<A, B>(K<Eff, Func<A, B>> mf, K<Eff, A> ma) => 
        new Eff<B>(mf.As().effect.Apply(ma.As().effect));

    static K<Eff, B> Applicative<Eff>.Action<A, B>(K<Eff, A> ma, K<Eff, B> mb) => 
        new Eff<B>(ma.As().effect.Action(mb.As().effect));

    static K<Eff, A> Applicative<Eff>.Actions<A>(IEnumerable<K<Eff, A>> fas) => 
        new Eff<A>(fas.Select(fa => fa.As().effect).Actions().As()); 

    static K<Eff, A> Applicative<Eff>.Actions<A>(IAsyncEnumerable<K<Eff, A>> fas) => 
        new Eff<A>(fas.Select(fa => fa.As().effect).Actions().As()); 

    static K<Eff, A> MonoidK<Eff>.Empty<A>() => 
        Eff<A>.Fail(Errors.None);

    static K<Eff, A> Choice<Eff>.Choose<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        new Eff<A>(ma.As().effect.Choose(mb.As().effect).As());

    static K<Eff, A> Choice<Eff>.Choose<A>(K<Eff, A> ma, Func<K<Eff, A>> mb) => 
        new Eff<A>(ma.As().effect.Choose(mb().As().effect).As());

    static K<Eff, A> Readable<Eff, MinRT>.Asks<A>(Func<MinRT, A> f) => 
        new Eff<A>(Readable.asks<Eff<MinRT>, MinRT, A>(f).As());

    static K<Eff, A> Readable<Eff, MinRT>.Local<A>(Func<MinRT, MinRT> f, K<Eff, A> ma) => 
        new Eff<A>(Readable.local(f, ma.As().effect).As());

    static K<Eff, A> MonadIO<Eff>.LiftIO<A>(IO<A> ma) =>
        Eff<A>.LiftIO(ma);

    static K<Eff, IO<A>> MonadUnliftIO<Eff>.ToIO<A>(K<Eff, A> ma) =>
        new Eff<IO<A>>(ma.As().effect.ToIO().As());

    static K<Eff, B> MonadUnliftIO<Eff>.MapIO<A, B>(K<Eff, A> ma, Func<IO<A>, IO<B>> f) =>
        new Eff<B>(ma.As().effect.MapIO(f).As());
    
    static K<Eff, A> Fallible<Error, Eff>.Fail<A>(Error error) =>
        Eff<A>.Fail(error);

    static K<Eff, A> Fallible<Error, Eff>.Catch<A>(
        K<Eff, A> fa, Func<Error, bool> Predicate,
        Func<Error, K<Eff, A>> Fail) =>
        new Eff<A>(fa.As().effect.Catch(Predicate, e => Fail(e).As().effect).As());

    static K<Eff, A> SemigroupK<Eff>.Combine<A>(K<Eff, A> lhs, K<Eff, A> rhs) => 
        lhs.Choose(rhs);
}
