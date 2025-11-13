using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<A>(K<Eff, A> self)
    {
        public static Eff<A> operator |(K<Eff, A> lhs, CatchM<Error, Eff, A> rhs) =>
            lhs.Catch(rhs).As();

        public static Eff<A> operator |(K<Eff, A> lhs, Fail<Error> rhs) =>
            lhs.Catch(rhs).As();

        public static Eff<A> operator |(K<Eff, A> lhs, Error rhs) =>
            lhs.Catch(rhs).As();
    }
}
