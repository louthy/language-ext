using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `FinT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class FinT<M> : 
    MonadT<FinT<M>, M>, 
    Fallible<FinT<M>>,
    Alternative<FinT<M>>,
    Natural<FinT<M>, EitherT<Error, M>>,
    Natural<FinT<M>, OptionT<M>>,
    Natural<FinT<M>, TryT<M>>,
    MonadIO<FinT<M>>
    where M : Monad<M>
{
    static K<FinT<M>, B> Monad<FinT<M>>.Bind<A, B>(K<FinT<M>, A> ma, Func<A, K<FinT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<FinT<M>, B> Functor<FinT<M>>.Map<A, B>(Func<A, B> f, K<FinT<M>, A> ma) => 
        ma.As().Map(f);

    static K<FinT<M>, A> Applicative<FinT<M>>.Pure<A>(A value) => 
        FinT.Succ<M, A>(value);

    static K<FinT<M>, B> Applicative<FinT<M>>.Apply<A, B>(K<FinT<M>, Func<A, B>> mf, K<FinT<M>, A> ma) =>
        new FinT<M, B>(
            M.Pure((Fin<Func<A, B>> ff) => 
                       (Fin<A> fa) => 
                           ff.Apply(fa).As())
             .Apply(mf.As().runFin)
             .Apply(ma.As().runFin));

    static K<FinT<M>, B> Applicative<FinT<M>>.Action<A, B>(K<FinT<M>, A> ma, K<FinT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<FinT<M>, A> MonadT<FinT<M>, M>.Lift<A>(K<M, A> ma) => 
        FinT.lift(ma);
        
    static K<FinT<M>, A> MonadIO<FinT<M>>.LiftIO<A>(IO<A> ma) => 
        FinT.lift(M.LiftIOMaybe(ma));

    static K<FinT<M>, A> MonoidK<FinT<M>>.Empty<A>() =>
        FinT.Fail<M, A>(Error.Empty);

    static K<FinT<M>, A> SemigroupK<FinT<M>>.Combine<A>(K<FinT<M>, A> ma, K<FinT<M>, A> mb) =>
        new FinT<M, A>(
            M.Bind(ma.As().runFin,
                   ea => ea switch
                         {
                             Fin<A>.Succ => M.Pure(ea),
                             Fin<A>.Fail => mb.As().runFin.Map(fb => ea.Combine(fb).As()),
                             _           => M.Pure(ea)
                         }));

    static K<FinT<M>, A> Choice<FinT<M>>.Choose<A>(K<FinT<M>, A> ma, K<FinT<M>, A> mb) =>
        new FinT<M, A>(
            M.Bind(ma.As().runFin,
                   ea => ea switch
                         {
                             Fin<A>.Succ => M.Pure(ea),
                             Fin<A>.Fail => mb.As().runFin,
                             _           => M.Pure(ea)
                         }));

    static K<FinT<M>, A> Choice<FinT<M>>.Choose<A>(K<FinT<M>, A> ma, Func<K<FinT<M>, A>> mb) => 
        new FinT<M, A>(
            M.Bind(ma.As().runFin,
                   ea => ea switch
                         {
                             Fin<A>.Succ => M.Pure(ea),
                             Fin<A>.Fail => mb().As().runFin,
                             _           => M.Pure(ea)
                         }));

    static K<FinT<M>, A> Fallible<Error, FinT<M>>.Fail<A>(Error error) =>
        FinT.Fail<M, A>(error);

    static K<FinT<M>, A> Fallible<Error, FinT<M>>.Catch<A>(
        K<FinT<M>, A> fa, Func<Error, bool> Predicate,
        Func<Error, K<FinT<M>, A>> Fail) =>
        fa.As().BindFail(l => Predicate(l) ? Fail(l).As() : FinT.Fail<M, A>(l));

    static K<EitherT<Error, M>, A> Natural<FinT<M>, EitherT<Error, M>>.Transform<A>(K<FinT<M>, A> fa) => 
        new EitherT<Error, M, A>(fa.As().runFin.Map(Natural.transform<Fin, Either<Error>, A>).Map(ma => ma.As()));

    static K<OptionT<M>, A> Natural<FinT<M>, OptionT<M>>.Transform<A>(K<FinT<M>, A> fa) => 
        new OptionT<M, A>(fa.As().runFin.Map(Natural.transform<Fin, Option, A>).Map(ma => ma.As()));

    static K<TryT<M>, A> Natural<FinT<M>, TryT<M>>.Transform<A>(K<FinT<M>, A> fa) => 
        new TryT<M, A>(fa.As().runFin.Map(Natural.transform<Fin, Try, A>).Map(ma => ma.As()));
}
