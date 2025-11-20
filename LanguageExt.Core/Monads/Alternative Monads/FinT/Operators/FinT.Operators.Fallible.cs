using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinTExtensions
{
    extension<M, A>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        public static FinT<M, A> operator |(K<FinT<M>, A> lhs, CatchM<Error, FinT<M>, A> rhs) =>
            +lhs.Catch(rhs);

        public static FinT<M, A> operator |(K<FinT<M>, A> lhs, Fail<Error> rhs) =>
            +lhs.Catch(rhs);

        public static FinT<M, A> operator |(K<FinT<M>, A> lhs, Error rhs) =>
            +lhs.Catch(rhs);
    }
}
