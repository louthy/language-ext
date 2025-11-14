using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinTExtensions
{
    extension<M, A>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        public static FinT<M, A> operator |(K<FinT<M>, A> lhs, K<FinT<M>, A> rhs) =>
            +lhs.Choose(rhs);

        public static FinT<M, A> operator |(K<FinT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(FinT.lift<M, A>(rhs.ToFin()));
    }
}
