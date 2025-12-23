using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Eff : 
    MonadUnliftIO<Eff>,
    Final<Eff>,
    Fallible<Eff>,
    Readable<Eff, MinRT>,
    MonoidK<Eff>,
    Alternative<Eff>,
    Natural<Eff, Eff<MinRT>>,
    CoNatural<Eff, Eff<MinRT>>
{
    static K<Eff, B> Monad<Eff>.Bind<A, B>(K<Eff, A> ma, Func<A, K<Eff, B>> f) =>
        new Eff<B>(ma.As().effect.Bind(f));

    static K<Eff, B> Monad<Eff>.Recur<A, B>(A value, Func<A, K<Eff, Next<A, B>>> f) =>
        lift<B>(async env =>
                {
                    while (true)
                    {
                        var mnext = await f(value).As().RunAsync(env);
                        if (mnext.IsFail) return Fin.Fail<B>(mnext.FailValue);
                        var next = (Next<A, B>)mnext;
                        if (next.IsDone) return Fin.Succ(next.Done);
                        value = next.Loop;
                    }
                });

    static K<Eff, B> Functor<Eff>.Map<A, B>(Func<A, B> f, K<Eff, A> ma) => 
        new Eff<B>(ma.As().effect.Map(f));

    static K<Eff, A> Applicative<Eff>.Pure<A>(A value) => 
        Eff<A>.Pure(value);

    static K<Eff, B> Applicative<Eff>.Apply<A, B>(K<Eff, Func<A, B>> mf, K<Eff, A> ma) => 
        new Eff<B>(mf.As().effect.Apply(ma.As().effect));

    static K<Eff, B> Applicative<Eff>.Apply<A, B>(K<Eff, Func<A, B>> mf, Memo<Eff, A> ma) =>
        new Eff<B>(mf.As().effect.Apply(Memo.transform<Eff, Eff<MinRT>, A>(ma)).As());

    static K<Eff, A> Applicative<Eff>.Actions<A>(IterableNE<K<Eff, A>> fas) => 
        new Eff<A>(fas.Map(fa => fa.As().effect).Actions().As()); 

    static K<Eff, A> Applicative<Eff>.Actions<A>(IAsyncEnumerable<K<Eff, A>> fas) => 
        new Eff<A>(fas.Select(fa => fa.As().effect).Actions().As()); 

    static K<Eff, A> MonoidK<Eff>.Empty<A>() => 
        Eff<A>.Fail(Errors.None);

    static K<Eff, A> Alternative<Eff>.Empty<A>() => 
        Eff<A>.Fail(Errors.None);

    static K<Eff, A> Choice<Eff>.Choose<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        new Eff<A>(ma.As().effect.Choose(mb.As().effect).As());

    static K<Eff, A> Choice<Eff>.Choose<A>(K<Eff, A> ma, Memo<Eff, A> mb) => 
        new Eff<A>(ma.As().effect.Choose(Memo.transform<Eff, Eff<MinRT>, A>(mb)).As());

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

    static K<Eff, A> Final<Eff>.Finally<X, A>(K<Eff, A> fa, K<Eff, X> @finally) =>
        new Eff<A>(fa.As().effect.Finally(@finally.As().effect).As());

    static K<Eff<MinRT>, A> Natural<Eff, Eff<MinRT>>.Transform<A>(K<Eff, A> fa) => 
        fa.As().effect;

    static K<Eff, A> CoNatural<Eff, Eff<MinRT>>.CoTransform<A>(K<Eff<MinRT>, A> fa) => 
        new Eff<A>(fa.As());
}
