using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    extension<M, A>(K<SourceT<M>, A> self)
        where M : MonadIO<M>, Alternative<M>
    {
        public static SourceT<M, A> operator |(K<SourceT<M>, A> lhs, K<SourceT<M>, A> rhs) =>
            +lhs.Choose(rhs);

        public static SourceT<M, A> operator |(K<SourceT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(SourceT.pure<M, A>(rhs.Value));
    }
}
