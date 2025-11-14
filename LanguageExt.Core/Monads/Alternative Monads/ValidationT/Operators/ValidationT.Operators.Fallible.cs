using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        public static ValidationT<FF, M, A> operator |(K<ValidationT<FF, M>, A> lhs, CatchM<FF, ValidationT<FF, M>, A> rhs) =>
            +lhs.Catch(rhs);

        public static ValidationT<FF, M, A> operator |(K<ValidationT<FF, M>, A> lhs, Fail<FF> rhs) =>
            +lhs.Catch(rhs);
    }
}
