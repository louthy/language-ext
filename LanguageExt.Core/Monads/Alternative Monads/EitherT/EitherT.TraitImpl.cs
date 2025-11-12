using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `EitherT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class EitherT<L, M> : 
    MonadT<EitherT<L, M>, M>, 
    Fallible<L, EitherT<L, M>>,
    Choice<EitherT<L, M>>,
    Natural<EitherT<L, M>, OptionT<M>>,
    MonadIO<EitherT<L, M>>
    where M : Monad<M>
{
    static K<EitherT<L, M>, B> Monad<EitherT<L, M>>.Bind<A, B>(K<EitherT<L, M>, A> ma, Func<A, K<EitherT<L, M>, B>> f) => 
        ma.As().Bind(f);

    static K<EitherT<L, M>, B> Functor<EitherT<L, M>>.Map<A, B>(Func<A, B> f, K<EitherT<L, M>, A> ma) => 
        ma.As().Map(f);

    static K<EitherT<L, M>, A> Applicative<EitherT<L, M>>.Pure<A>(A value) => 
        EitherT.Right<L, M, A>(value);

    static K<EitherT<L, M>, B> Applicative<EitherT<L, M>>.Apply<A, B>(K<EitherT<L, M>, Func<A, B>> mf, K<EitherT<L, M>, A> ma) =>
        mf.As().Bind(x => ma.As().Map(x));

    static K<EitherT<L, M>, B> Applicative<EitherT<L, M>>.Action<A, B>(K<EitherT<L, M>, A> ma, K<EitherT<L, M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<EitherT<L, M>, A> MonadT<EitherT<L, M>, M>.Lift<A>(K<M, A> ma) => 
        EitherT.lift<L, M, A>(ma);
        
    static K<EitherT<L, M>, A> MonadIO<EitherT<L, M>>.LiftIO<A>(IO<A> ma) => 
        EitherT.lift<L, M, A>(M.LiftIOMaybe(ma));

    static K<EitherT<L, M>, A> Choice<EitherT<L, M>>.Choose<A>(K<EitherT<L, M>, A> ma, K<EitherT<L, M>, A> mb) =>
        new EitherT<L, M, A>(
            M.Bind(ma.As().runEither,
                   ea => ea switch
                         {
                             Either<L, A>.Right => M.Pure(ea),
                             Either<L, A>.Left  => mb.As().runEither,
                             _                  => M.Pure(ea)
                         }));

    static K<EitherT<L, M>, A> Choice<EitherT<L, M>>.Choose<A>(K<EitherT<L, M>, A> ma, Func<K<EitherT<L, M>, A>> mb) =>
        new EitherT<L, M, A>(
            M.Bind(ma.As().runEither,
                   ea => ea switch
                         {
                             Either<L, A>.Right => M.Pure(ea),
                             Either<L, A>.Left  => mb().As().runEither,
                             _                  => M.Pure(ea)
                         }));

    static K<EitherT<L, M>, A> SemigroupK<EitherT<L, M>>.Combine<A>(K<EitherT<L, M>, A> lhs, K<EitherT<L, M>, A> rhs) =>
        lhs.Choose(rhs);

    static K<EitherT<L, M>, A> Fallible<L, EitherT<L, M>>.Fail<A>(L error) =>
        EitherT.Left<L, M, A>(error);

    static K<EitherT<L, M>, A> Fallible<L, EitherT<L, M>>.Catch<A>(
        K<EitherT<L, M>, A> fa, Func<L, bool> Predicate,
        Func<L, K<EitherT<L, M>, A>> Fail) =>
        fa.As().BindLeft(l => Predicate(l) ? Fail(l).As() : EitherT.Left<L, M, A>(l));

    static K<OptionT<M>, A> Natural<EitherT<L, M>, OptionT<M>>.Transform<A>(K<EitherT<L, M>, A> fa) => 
        new OptionT<M, A>(fa.As().runEither.Map(Natural.transform<Either<L>, Option, A>).Map(ma => ma.As()));
}
