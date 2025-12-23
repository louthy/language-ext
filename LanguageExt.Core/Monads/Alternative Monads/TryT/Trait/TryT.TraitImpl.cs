using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `TryT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class TryT<M> : 
    Fallible<TryT<M>>, 
    MonadT<TryT<M>, M>, 
    Alternative<TryT<M>>,
    MonadIO<TryT<M>>
    where M : Monad<M>
{
    static K<TryT<M>, B> Monad<TryT<M>>.Bind<A, B>(K<TryT<M>, A> ma, Func<A, K<TryT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<TryT<M>, B> Monad<TryT<M>>.Recur<A, B>(A value, Func<A, K<TryT<M>, Next<A, B>>> f) => 
        new TryT<M, B>(
            M.Recur<A, Try<B>>(
                value,
                a => f(a).As()
                         .Run()
                         .Map(e => e switch
                                   {
                                       Fin<Next<A, B>>.Fail(var err)            => Next.Done<A, Try<B>>(err), 
                                       Fin<Next<A, B>>.Succ({ IsDone: true } n) => Next.Done<A, Try<B>>(Try.Succ(n.Done)), 
                                       Fin<Next<A, B>>.Succ({ IsLoop: true } n) => Next.Loop<A, Try<B>>(n.Loop),
                                       _                                        => throw new NotSupportedException()
                                   })));

    static K<TryT<M>, B> Functor<TryT<M>>.Map<A, B>(Func<A, B> f, K<TryT<M>, A> ma) => 
        ma.As().Map(f);

    static K<TryT<M>, A> Applicative<TryT<M>>.Pure<A>(A value) => 
        TryT.Succ<M, A>(value);

    static K<TryT<M>, B> Applicative<TryT<M>>.Apply<A, B>(K<TryT<M>, Func<A, B>> mf, K<TryT<M>, A> ma) =>
        new TryT<M, B>(mf.As().runTry.Bind(
                           mf1 => ma.As().runTry.Bind(
                               ma1 => M.Pure(mf1.Apply(ma1)))));
    
    static K<TryT<M>, B> Applicative<TryT<M>>.Apply<A, B>(K<TryT<M>, Func<A, B>> mf, Memo<TryT<M>, A> ma) =>
        new TryT<M, B>(mf.As().runTry.Bind(
                           mf1 => ma.Value.As().runTry.Bind(
                               ma1 => M.Pure(mf1.Apply(ma1)))));

    static K<TryT<M>, A> MonadT<TryT<M>, M>.Lift<A>(K<M, A> ma) => 
        TryT.lift(ma);
    
    static K<TryT<M>, A> MonadIO<TryT<M>>.LiftIO<A>(IO<A> ma) => 
        TryT.liftIOMaybe<M, A>(ma);

    static K<TryT<M>, A> Alternative<TryT<M>>.Empty<A>() =>
        TryT.Fail<M, A>(Error.Empty);

    static K<TryT<M>, A> Choice<TryT<M>>.Choose<A>(K<TryT<M>, A> ma, K<TryT<M>, A> mb) =>
        new TryT<M, A>(ma.Run().Bind(
                           lhs => lhs switch
                                  {
                                      Fin<A>.Succ (var x) => M.Pure(Try.Succ(x)),
                                      Fin<A>.Fail         => mb.As().runTry,
                                      _                      => throw new NotSupportedException()
                                  }));

    static K<TryT<M>, A> Choice<TryT<M>>.Choose<A>(K<TryT<M>, A> ma, Memo<TryT<M>, A> mb) => 
        new TryT<M, A>(ma.Run().Bind(
                           lhs => lhs switch
                                  {
                                      Fin<A>.Succ (var x) => M.Pure(Try.Succ(x)),
                                      Fin<A>.Fail         => mb.Value.As().runTry,
                                      _                      => throw new NotSupportedException()
                                  }));

    static K<TryT<M>, A> Fallible<Error, TryT<M>>.Fail<A>(Error error) =>
        TryT.Fail<M, A>(error);

    static K<TryT<M>, A> Fallible<Error, TryT<M>>.Catch<A>(
        K<TryT<M>, A> fa,
        Func<Error, bool> Predicate,
        Func<Error, K<TryT<M>, A>> Fail) =>
        fa.As().BindFail(e => Predicate(e) ? Fail(e).As() : TryT.Fail<M, A>(e));
}
