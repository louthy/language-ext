namespace LanguageExt.HKT;

/// <summary>
/// Applicative functor interface
/// </summary>
/// <typeparam name="F">Applicative trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Applicative<F, A> : Functor<F, A>
    where F : Applicative<F>;
