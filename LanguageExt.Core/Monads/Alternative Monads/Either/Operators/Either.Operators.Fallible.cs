using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherExtensions
{
    extension<L, A>(Either<L, A> self)
    {
        public static Either<L, A> operator |(Either<L, A> lhs, CatchM<L, Either<L>, A> rhs) =>
            +lhs.Catch(rhs);

        public static Either<L, A> operator |(Either<L, A> lhs, Fail<L> rhs) =>
            +lhs.Catch(rhs);
    }    
    
    extension<L, A>(K<Either<L>, A> self)
    {
        public static Either<L, A> operator |(K<Either<L>, A> lhs, CatchM<L, Either<L>, A> rhs) =>
            +lhs.Catch(rhs);

        public static Either<L, A> operator |(K<Either<L>, A> lhs, Fail<L> rhs) =>
            +lhs.Catch(rhs);
    }
}
