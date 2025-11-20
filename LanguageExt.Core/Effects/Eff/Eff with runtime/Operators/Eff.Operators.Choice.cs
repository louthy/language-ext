using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A>(K<Eff<RT>, A> self)
    {
        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, K<Eff<RT>, A> rhs) =>
            lhs.Choose(rhs).As();

        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, K<Eff, A> rhs) =>
            lhs.Choose(rhs.As().WithRuntime<RT>()).As();

        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, Pure<A> rhs) =>
            lhs.Choose(rhs.ToEff<RT>()).As();
    }
}
