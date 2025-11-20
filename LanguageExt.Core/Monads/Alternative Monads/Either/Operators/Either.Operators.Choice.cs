using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherExtensions
{
    extension<L, A>(Either<L, A> self)
    {
        public static Either<L, A> operator |(Either<L, A> lhs, Either<L, A> rhs) =>
            lhs.Choose(rhs).As();

        public static Either<L, A> operator |(Either<L, A> lhs, Pure<A> rhs) =>
            lhs.Choose(rhs.ToEither<L>()).As();
    }
    
    extension<L, A>(K<Either<L>, A> self)
    {
        public static Either<L, A> operator |(K<Either<L>, A> lhs, K<Either<L>, A> rhs) =>
            lhs.Choose(rhs).As();

        public static Either<L, A> operator |(K<Either<L>, A> lhs, Pure<A> rhs) =>
            lhs.Choose(rhs.ToEither<L>()).As();
    }
}
