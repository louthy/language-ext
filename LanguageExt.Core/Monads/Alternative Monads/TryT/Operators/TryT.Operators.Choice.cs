using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryTExtensions
{
    extension<M, A>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, K<TryT<M>, A> rhs) =>
            +lhs.Choose(rhs);

        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(TryT.Succ<M, A>(rhs.Value));
    }
}
