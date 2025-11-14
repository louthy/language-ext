using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinExtensions
{
    extension<A>(K<Fin, A> self)
    {
        public static Fin<A> operator |(K<Fin, A> lhs, K<Fin, A> rhs) =>
            +lhs.Choose(rhs);

        public static Fin<A> operator |(K<Fin, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(rhs.ToFin());
    }
}
