using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryExtensions
{
    extension<A>(K<Try, A> self)
    {
        public static Try<A> operator |(K<Try, A> lhs, K<Try, A> rhs) =>
            +lhs.Choose(rhs);

        public static Try<A> operator |(K<Try, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(rhs.ToTry());
    }
}
