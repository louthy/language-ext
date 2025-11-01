using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class EitherT<M> :
    CoproductK<EitherT<M>>,
    Bimonad<EitherT<M>>
    where M : Monad<M>
{
    static K<EitherT<M>, A, B> CoproductCons<EitherT<M>>.Left<A, B>(A value) => 
        EitherT<A, M, B>.Left(value);

    static K<EitherT<M>, A, B> CoproductCons<EitherT<M>>.Right<A, B>(B value) => 
        EitherT<A, M, B>.Right(value);

    static K<EitherT<M>, A, C> CoproductK<EitherT<M>>.Match<A, B, C>(
        Func<A, C> Left, 
        Func<B, C> Right,
        K<EitherT<M>, A, B> fab) => 
        EitherT<A, M, C>.Lift(fab.As2().Match(Left, Right));

    static K<EitherT<M>, L1, B> Bifunctor<EitherT<M>>.BiMap<L, A, L1, B>(
        Func<L, L1> first, 
        Func<A, B> second, 
        K<EitherT<M>, L, A> fab) => 
        new EitherT<L1, M, B>(fab.As2().runEither.Map(e => e.BiMap(first, second)));

    static K<EitherT<M>, L1, A> Bimonad<EitherT<M>>.BindFirst<L, L1, A>(
        K<EitherT<M>, L, A> ma,
        Func<L, K<EitherT<M>, L1, A>> f) =>
        new EitherT<L1, M, A>(ma.As2()
                                .runEither
                                .Bind(e => e switch
                                           {
                                               Either.Right<L, A>(var r) => M.Pure(Either<L1, A>.Right(r)),
                                               Either.Left<L, A>(var l)  => f(l).As2().runEither,
                                               _                         => throw new NotSupportedException()
                                           }));

    static K<EitherT<M>, L, B> Bimonad<EitherT<M>>.BindSecond<L, A, B>(
        K<EitherT<M>, L, A> ma,
        Func<A, K<EitherT<M>, L, B>> f) =>
        new EitherT<L, M, B>(ma.As2()
                               .runEither
                               .Bind(e => e switch
                                          {
                                              Either.Right<L, A>(var r) => f(r).As2().runEither,
                                              Either.Left<L, A>(var l)  => M.Pure(Either<L, B>.Left(l)),
                                              _                         => throw new NotSupportedException()
                                          }));
}
