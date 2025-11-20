using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class OptionTExtensions
{
    extension<M, A>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        public static OptionT<M, A> operator |(K<OptionT<M>, A> lhs, CatchM<Unit, OptionT<M>, A> rhs) =>
            +lhs.Catch(rhs);

        public static OptionT<M, A> operator |(K<OptionT<M>, A> lhs, Fail<Unit> rhs) =>
            +lhs.Catch(rhs);

        public static OptionT<M, A> operator |(K<OptionT<M>, A> lhs, Unit rhs) =>
            +lhs.Catch(rhs);
    }
}
