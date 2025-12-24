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
    Writable<WriterT<W, M>, W>
    where M : Monad<M>
    where W : Monoid<W>
{
    static K<WriterT<W, M>, B> Monad<WriterT<W, M>>.Bind<A, B>(K<WriterT<W, M>, A> ma, Func<A, K<WriterT<W, M>, B>> f) => 
        ma.As().Bind(f);

    static K<WriterT<W, M>, B> Monad<WriterT<W, M>>.Recur<A, B>(A value, Func<A, K<WriterT<W, M>, Next<A, B>>> f) => 
        new WriterT<W, M, B>(
            output =>
            {
                return M.Recur(f(value).As().runWriter(output), go);

                K<M, Next<K<M, (Next<A, B>, W)>, (B, W)>> go(K<M, (Next<A, B> Next, W Output)> ma) =>
                    ma >> (n => n.Next switch
                                {
                                    { IsDone: true, Done: var x } =>
                                        M.Pure(Next.Done<K<M, (Next<A, B>, W)>, (B, W)>((x, n.Output))),

                                    { IsLoop: true, Loop: var x } =>
                                        M.Pure(Next.Loop<K<M, (Next<A, B>, W)>, (B, W)>(f(x).As().Run(n.Output))),

                                    _ => throw new NotSupportedException()
                                });
            });

    static K<WriterT<W, M>, B> Functor<WriterT<W, M>>.Map<A, B>(Func<A, B> f, K<WriterT<W, M>, A> ma) => 
        ma.As().Map(f);

    static K<WriterT<W, M>, A> Applicative<WriterT<W, M>>.Pure<A>(A value) => 
        WriterT<W, M, A>.Pure(value);

    static K<WriterT<W, M>, B> Applicative<WriterT<W, M>>.Apply<A, B>(K<WriterT<W, M>, Func<A, B>> mf, K<WriterT<W, M>, A> ma) => 
        mf.As().Bind(x => ma.As().Map(x));

    static K<WriterT<W, M>, B> Applicative<WriterT<W, M>>.Apply<A, B>(K<WriterT<W, M>, Func<A, B>> mf, Memo<WriterT<W, M>, A> ma) => 
        mf.As().Bind(x => ma.Value.As().Map(x));

    static K<WriterT<W, M>, A> MonadT<WriterT<W, M>, M>.Lift<A>(K<M, A> ma) => 
        WriterT<W, M, A>.Lift(ma);
    
    static K<WriterT<W, M>, A> Maybe.MonadIO<WriterT<W, M>>.LiftIOMaybe<A>(IO<A> ma) =>
        WriterT<W, M, A>.Lift(M.LiftIOMaybe(ma));

    static K<WriterT<W, M>, Unit> Writable<WriterT<W, M>, W>.Tell(W item) =>
        new WriterT<W, M, Unit>(w => M.Pure((unit, w + item)));

    static K<WriterT<W, M>, (A Value, W Output)> Writable<WriterT<W, M>, W>.Listen<A>(K<WriterT<W, M>, A> ma) =>
        ma.As().Listen;

    static K<WriterT<W, M>, A> Writable<WriterT<W, M>, W>.Pass<A>(
        K<WriterT<W, M>, (A Value, Func<W, W> Function)> action) =>
        new WriterT<W, M, A>(
            w => action.As()
                       .Run()
                       .Map(afw => (afw.Value.Value, w + afw.Value.Function(afw.Output))));
}
