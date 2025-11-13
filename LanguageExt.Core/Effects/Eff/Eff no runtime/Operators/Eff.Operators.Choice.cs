using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<A>(K<Eff, A> self)
    {
        public static Eff<A> operator |(K<Eff, A> lhs, K<Eff, A> rhs) =>
            lhs.Choose(rhs).As();

        public static Eff<A> operator |(K<Eff, A> lhs, Pure<A> rhs) =>
            lhs.Choose(rhs.ToEff()).As();
    }

    extension<RT, A>(K<Eff, A> self)
    {
        public static Eff<RT, A> operator |(K<Eff, A> lhs, K<Eff<RT>, A> rhs) =>
            lhs.As().WithRuntime<RT>().Choose(rhs).As();
    }
}
