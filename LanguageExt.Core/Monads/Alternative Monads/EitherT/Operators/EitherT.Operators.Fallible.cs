using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        public static EitherT<L, M, A> operator |(K<EitherT<L, M>, A> lhs, CatchM<L, EitherT<L, M>, A> rhs) =>
            +lhs.Catch(rhs);

        public static EitherT<L, M, A> operator |(K<EitherT<L, M>, A> lhs, Fail<L> rhs) =>
            +lhs.Catch(rhs);
    }
}
