using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    extension<F, A>(K<Validation<F>, A> self)
    {
        public static Validation<F, A> operator |(K<Validation<F>, A> lhs, CatchM<F, Validation<F>, A> rhs) =>
            +lhs.Catch(rhs);

        public static Validation<F, A> operator |(K<Validation<F>, A> lhs, Fail<F> rhs) =>
            +lhs.Catch(rhs);
    }
}
