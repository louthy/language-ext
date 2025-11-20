using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A>(K<EitherT<L, M>, A> self)
        where M : Monad<M>, SemigroupK<M>
    {
        public static EitherT<L, M, A> operator +(K<EitherT<L, M>, A> lhs, K<EitherT<L, M>, A> rhs) =>
            new(lhs.As().runEither.Combine(rhs.As().runEither));

        public static EitherT<L, M, A> operator +(K<EitherT<L, M>, A> lhs, Pure<A> rhs) =>
            new(lhs.As().runEither.Combine(M.Pure(Either.Right<L, A>(rhs.Value))));
    }
    
    extension<E, L, M, A>(K<EitherT<L, M>, A> self)
        where M : Monad<M>, SemigroupK<M>, Fallible<E, M>
    {
        public static EitherT<L, M, A> operator +(K<EitherT<L, M>, A> lhs, Fail<E> rhs) =>
            new(lhs.As().runEither.Combine(M.Fail<Either<L, A>>(rhs.Value)));
    }    
}
