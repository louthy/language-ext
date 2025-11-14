using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherExtensions
{
    extension<L, A>(Either<L, A> self)
    {
        public static Either<L, A> operator +(Either<L, A> lhs, Either<L, A> rhs) =>
            +lhs.Combine(rhs);

        public static Either<L, A> operator +(Either<L, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(rhs.ToEither<L>());

        public static Either<L, A> operator +(Either<L, A> lhs, Fail<L> rhs) =>
            +lhs.Combine(Either.Left<L, A>(rhs.Value));
    }
    
    extension<L, A>(K<Either<L>, A> self)
    {
        public static Either<L, A> operator +(K<Either<L>, A> lhs, K<Either<L>, A> rhs) =>
            +lhs.Combine(rhs);

        public static Either<L, A> operator +(K<Either<L>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(rhs.ToEither<L>());
        
        public static Either<L, A> operator +(K<Either<L>, A> lhs, Fail<L> rhs) =>
            +lhs.Combine(Either.Left<L, A>(rhs.Value));
    }
}
