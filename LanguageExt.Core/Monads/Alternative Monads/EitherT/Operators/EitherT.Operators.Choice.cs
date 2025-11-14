using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        public static EitherT<L, M, A> operator |(K<EitherT<L, M>, A> lhs, K<EitherT<L, M>, A> rhs) =>
            +lhs.Choose(rhs);

        public static EitherT<L, M, A> operator |(K<EitherT<L, M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(EitherT.lift<L, M, A>(rhs.ToEither<L>()));
    }
}
