using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class OptionTExtensions
{
    extension<M, A>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        public static OptionT<M, A> operator |(K<OptionT<M>, A> lhs, K<OptionT<M>, A> rhs) =>
            +lhs.Choose(rhs);

        public static OptionT<M, A> operator |(K<OptionT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(OptionT.lift<M, A>(rhs.ToOption()));
    }
}
