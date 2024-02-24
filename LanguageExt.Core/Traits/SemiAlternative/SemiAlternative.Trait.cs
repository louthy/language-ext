namespace LanguageExt.Traits;

/// <summary>
/// A semigroup on functors
/// </summary>
/// <typeparam name="F">Functor</typeparam>
public interface SemiAlternative<F> : Functor<F>
    where F : SemiAlternative<F>
{
    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static abstract K<F, A> Or<A>(K<F, A> ma, K<F, A> mb);
}
