namespace LanguageExt.Traits;

/// <summary>
/// A semigroup on functors
/// </summary>
/// <typeparam name="F">Functor</typeparam>
public interface SemiAlternative<F> : SemigroupK<F>, Functor<F>
    where F : SemiAlternative<F>;
