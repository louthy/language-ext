using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A>(K<Eff<RT>, A> self)
    {
        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, CatchM<Error, Eff<RT>, A> rhs) =>
            lhs.Catch(rhs).As();

        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, Fail<Error> rhs) =>
            lhs.Catch(rhs).As();

        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, Error rhs) =>
            lhs.Catch(rhs).As();
    }
}
