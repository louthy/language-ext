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

    static K<TryT<M>, B> Functor<TryT<M>>.Map<A, B>(Func<A, B> f, K<TryT<M>, A> ma) => 
        ma.As().Map(f);

    static K<TryT<M>, A> Applicative<TryT<M>>.Pure<A>(A value) => 
        TryT<M, A>.Succ(value);

    static K<TryT<M>, B> Applicative<TryT<M>>.Apply<A, B>(K<TryT<M>, Func<A, B>> mf, K<TryT<M>, A> ma) =>
        new TryT<M, B>(mf.As().runTry.Bind(
                           mf1 => ma.As().runTry.Bind(
                               ma1 => M.Pure(mf1.Apply(ma1)))));

    static K<TryT<M>, B> Applicative<TryT<M>>.Action<A, B>(K<TryT<M>, A> ma, K<TryT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<TryT<M>, A> MonadT<TryT<M>, M>.Lift<A>(K<M, A> ma) => 
        TryT<M, A>.Lift(ma);
    
    static K<TryT<M>, A> MonadIO<TryT<M>>.LiftIO<A>(IO<A> ma) => 
        TryT<M, A>.Lift(M.LiftIOMaybe(ma));

    static K<TryT<M>, A> MonoidK<TryT<M>>.Empty<A>() =>
        Fail<A>(Error.Empty);
 
    static K<TryT<M>, A> SemigroupK<TryT<M>>.Combine<A>(K<TryT<M>, A> ma, K<TryT<M>, A> mb) =>
        new TryT<M, A>(ma.Run().Bind(
                           lhs => lhs switch
                                  {
                                      Fin.Succ<A> (var x) =>
                                          M.Pure(Try.Succ(x)),

                                      Fin.Fail<A> (var e1) =>
                                          mb.Run().Bind(
                                              r => r switch
                                                   {
                                                       Fin.Succ<A> (var x)  => M.Pure(Try.Succ(x)),
                                                       Fin.Fail<A> (var e2) => M.Pure(Try.Fail<A>(e1 + e2)),
                                                       _                    => throw new NotSupportedException()
                                                   }),
                                      _ => throw new NotSupportedException()
                                  }));

    static K<TryT<M>, A> Choice<TryT<M>>.Choose<A>(K<TryT<M>, A> ma, K<TryT<M>, A> mb) =>
        new TryT<M, A>(ma.Run().Bind(
                           lhs => lhs switch
                                  {
                                      Fin.Succ<A> (var x) => M.Pure(Try.Succ(x)),
                                      Fin.Fail<A>         => mb.As().runTry,
                                      _                   => throw new NotSupportedException()
                                  }));

    static K<TryT<M>, A> Choice<TryT<M>>.Choose<A>(K<TryT<M>, A> ma, Func<K<TryT<M>, A>> mb) => 
        new TryT<M, A>(ma.Run().Bind(
                           lhs => lhs switch
                                  {
                                      Fin.Succ<A> (var x) => M.Pure(Try.Succ(x)),
                                      Fin.Fail<A>         => mb().As().runTry,
                                      _                   => throw new NotSupportedException()
                                  }));

    static K<TryT<M>, A> Fallible<Error, TryT<M>>.Fail<A>(Error error) =>
        Fail<A>(error);

    static K<TryT<M>, A> Fallible<Error, TryT<M>>.Catch<A>(
        K<TryT<M>, A> fa,
        Func<Error, bool> Predicate,
        Func<Error, K<TryT<M>, A>> Fail) =>
        fa.As().BindFail(e => Predicate(e) ? Fail(e).As() : TryT<M, A>.Fail(e));
}
