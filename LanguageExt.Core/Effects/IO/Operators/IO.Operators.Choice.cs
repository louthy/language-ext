using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A>(K<IO, A> self)
    {
        public static IO<A> operator |(K<IO, A> lhs, K<IO, A> rhs) =>
            +lhs.Choose(rhs);

        public static IO<A> operator |(K<IO, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(rhs.ToIO());
    }
}
