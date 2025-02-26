namespace LanguageExt.Traits;

/// <summary>
/// Natural isomorphism
/// </summary>
/// <typeparam name="F">Functor</typeparam>
/// <typeparam name="G">Functor</typeparam>
public interface NaturalIso<F, G> : 
    Natural<F, G>, 
    CoNatural<F, G>;
