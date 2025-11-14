using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        public static EitherT<L, M, A> operator +(K<EitherT<L, M>, A> lhs, K<EitherT<L, M>, A> rhs) =>
            +lhs.Combine(rhs);

        public static EitherT<L, M, A> operator +(K<EitherT<L, M>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(EitherT.Right<L, M, A>(rhs.Value));

        public static EitherT<L, M, A> operator +(K<EitherT<L, M>, A> lhs, Fail<L> rhs) =>
            +lhs.Combine(EitherT.Left<L, M, A>(rhs.Value));
    }
}
