using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class WriterT<W, M> : 
    MonadT<WriterT<W, M>, M>, 
    SemiAlternative<WriterT<W, M>>,
    WriterM<WriterT<W, M>, W>
    where M : Monad<M>, SemiAlternative<M>
    where W : Monoid<W>
{
    static K<WriterT<W, M>, B> Monad<WriterT<W, M>>.Bind<A, B>(K<WriterT<W, M>, A> ma, Func<A, K<WriterT<W, M>, B>> f) => 
        ma.As().Bind(f);

    static K<WriterT<W, M>, B> Functor<WriterT<W, M>>.Map<A, B>(Func<A, B> f, K<WriterT<W, M>, A> ma) => 
        ma.As().Map(f);

    static K<WriterT<W, M>, A> Applicative<WriterT<W, M>>.Pure<A>(A value) => 
        WriterT<W, M, A>.Pure(value);

    static K<WriterT<W, M>, B> Applicative<WriterT<W, M>>.Apply<A, B>(K<WriterT<W, M>, Func<A, B>> mf, K<WriterT<W, M>, A> ma) => 
        mf.As().Bind(x => ma.As().Map(x));

    static K<WriterT<W, M>, B> Applicative<WriterT<W, M>>.Action<A, B>(K<WriterT<W, M>, A> ma, K<WriterT<W, M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<WriterT<W, M>, A> MonadT<WriterT<W, M>, M>.Lift<A>(K<M, A> ma) => 
        WriterT<W, M, A>.Lift(ma);
    

    static K<WriterT<W, M>, A> Monad<WriterT<W, M>>.LiftIO<A>(IO<A> ma) =>
        WriterT<W, M, A>.Lift(M.LiftIO(ma));

    static K<WriterT<W, M>, A> SemiAlternative<WriterT<W, M>>.Or<A>(K<WriterT<W, M>, A> ma, K<WriterT<W, M>, A> mb) => 
        new WriterT<W, M, A>(w => M.Or(ma.As().runWriter(w), mb.As().runWriter(w)));

    static K<WriterT<W, M>, Unit> WriterM<WriterT<W, M>, W>.Tell(W item) =>
        new WriterT<W, M, Unit>(w => M.Pure((unit, w + item)));

    static K<WriterT<W, M>, (A Value, W Output)> WriterM<WriterT<W, M>, W>.Listen<A>(K<WriterT<W, M>, A> ma) =>
        ma.As().Listen();

    static K<WriterT<W, M>, A> WriterM<WriterT<W, M>, W>.Pass<A>(
        K<WriterT<W, M>, (A Value, Func<W, W> Function)> action) =>
        new WriterT<W, M, A>(
            w => action.As()
                       .Run()
                       .Map(afw => (afw.Value.Value, w + afw.Value.Function(afw.Output))));
}
