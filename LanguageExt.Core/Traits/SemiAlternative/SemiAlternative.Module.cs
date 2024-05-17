namespace LanguageExt.Traits;

/// <summary>
/// A semigroup on functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static class SemiAlternative
{
    public static K<F, A> combine<F, A>(K<F, A> ma, K<F, A> mb)
        where F : SemiAlternative<F> =>
        F.Combine(ma, mb);
}
