using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `EitherT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class EitherT<L, M> : MonadT<EitherT<L, M>, M>, SemiAlternative<EitherT<L, M>>
    where M : Monad<M>
{
    static K<EitherT<L, M>, B> Monad<EitherT<L, M>>.Bind<A, B>(K<EitherT<L, M>, A> ma, Func<A, K<EitherT<L, M>, B>> f) => 
        ma.As().Bind(f);

    static K<EitherT<L, M>, B> Functor<EitherT<L, M>>.Map<A, B>(Func<A, B> f, K<EitherT<L, M>, A> ma) => 
        ma.As().Map(f);

    static K<EitherT<L, M>, A> Applicative<EitherT<L, M>>.Pure<A>(A value) => 
        EitherT<L, M, A>.Right(value);

    static K<EitherT<L, M>, B> Applicative<EitherT<L, M>>.Apply<A, B>(K<EitherT<L, M>, Func<A, B>> mf, K<EitherT<L, M>, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<EitherT<L, M>, B> Applicative<EitherT<L, M>>.Action<A, B>(K<EitherT<L, M>, A> ma, K<EitherT<L, M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<EitherT<L, M>, A> MonadT<EitherT<L, M>, M>.Lift<A>(K<M, A> ma) => 
        EitherT<L, M, A>.Lift(ma);
        
    static K<EitherT<L, M>, A> Monad<EitherT<L, M>>.LiftIO<A>(IO<A> ma) => 
        EitherT<L, M, A>.Lift(M.LiftIO(ma));

    static K<EitherT<L, M>, A> SemigroupK<EitherT<L, M>>.Combine<A>(K<EitherT<L, M>, A> ma, K<EitherT<L, M>, A> mb) => 
        new EitherT<L, M, A>( 
            M.Bind(ma.As().runEither, 
                ea => ea switch
                      {
                          Either.Right<L, A> => M.Pure(ea),
                          Either.Left<L, A>  => mb.As().runEither,
                          _                  => M.Pure(ea)
                      }));
}
