using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class Pipe<RT, IN, OUT> : 
    MonadT<Pipe<RT, IN, OUT>, Eff<RT>>,
    MonadUnliftIO<Pipe<RT, IN, OUT>>
{
    static K<Pipe<RT, IN, OUT>, B> Monad<Pipe<RT, IN, OUT>>.Bind<A, B>(
        K<Pipe<RT, IN, OUT>, A> ma, 
        Func<A, K<Pipe<RT, IN, OUT>, B>> f) => 
        ma.As().Bind(x => f(x).As());

    static K<Pipe<RT, IN, OUT>, B> Functor<Pipe<RT, IN, OUT>>.Map<A, B>(
        Func<A, B> f, 
        K<Pipe<RT, IN, OUT>, A> ma) => 
        ma.As().Map(f);

    static K<Pipe<RT, IN, OUT>, A> Applicative<Pipe<RT, IN, OUT>>.Pure<A>(A value) => 
        Pipe.pure<RT, IN, OUT, A>(value);

    static K<Pipe<RT, IN, OUT>, B> Applicative<Pipe<RT, IN, OUT>>.Apply<A, B>(
        K<Pipe<RT, IN, OUT>, Func<A, B>> mf,
        K<Pipe<RT, IN, OUT>, A> ma) =>
        ma.As().ApplyBack(mf.As());

    static K<Pipe<RT, IN, OUT>, B> Applicative<Pipe<RT, IN, OUT>>.Action<A, B>(
        K<Pipe<RT, IN, OUT>, A> ma, 
        K<Pipe<RT, IN, OUT>, B> mb) =>
        Pipe.liftM<RT, IN, OUT, B>(ma.As().Run().Action(mb.As().Run()));

    static K<Pipe<RT, IN, OUT>, A> Applicative<Pipe<RT, IN, OUT>>.Actions<A>(IEnumerable<K<Pipe<RT, IN, OUT>, A>> fas) =>
        Pipe.liftM<RT, IN, OUT, A>(fas.Select(fa => fa.As().Run()).Actions());

    static K<Pipe<RT, IN, OUT>, A> Applicative<Pipe<RT, IN, OUT>>.Actions<A>(IAsyncEnumerable<K<Pipe<RT, IN, OUT>, A>> fas) =>
        Pipe.liftM<RT, IN, OUT, A>(fas.Select(fa => fa.As().Run()).Actions());

    static K<Pipe<RT, IN, OUT>, A> MonadT<Pipe<RT, IN, OUT>, Eff<RT>>.Lift<A>(K<Eff<RT>, A> ma) => 
        Pipe.liftM<RT, IN, OUT, A>(ma);

    static K<Pipe<RT, IN, OUT>, A> MonadIO<Pipe<RT, IN, OUT>>.LiftIO<A>(IO<A> ma) => 
        Pipe.liftIO<RT, IN, OUT, A>(ma);

    static K<Pipe<RT, IN, OUT>, B> MonadUnliftIO<Pipe<RT, IN, OUT>>.MapIO<A, B>(K<Pipe<RT, IN, OUT>, A> ma, Func<IO<A>, IO<B>> f) => 
        ma.As().MapIO(f);

    static K<Pipe<RT, IN, OUT>, IO<A>> MonadUnliftIO<Pipe<RT, IN, OUT>>.ToIO<A>(K<Pipe<RT, IN, OUT>, A> ma) => 
        ma.MapIO(IO.pure);
}
