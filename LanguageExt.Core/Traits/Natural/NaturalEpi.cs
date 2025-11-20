namespace LanguageExt.Traits;

/// <summary>
/// Natural epimorphic transformation 
/// </summary>
/// <remarks>
/// Epimorphism is the dual of monomorphism.  So, `NaturalEpi` is the dual of `NaturalMono`. 
/// </remarks>
/// <remarks>
/// Functors are referenced, because that's the true definition in category-theory, but
/// there is no requirement in language-ext for FA or GA to be functors.  It is just typically
/// true that FA and GA will also be functors.
/// </remarks>
/// <typeparam name="F">From functor</typeparam>
/// <typeparam name="G">To functor</typeparam>
public interface NaturalEpi<in F, out G> : CoNatural<F, G>, Natural<G, F>
    where F : CoNatural<F, G> 
{
    static K<F, A> Natural<G, F>.Transform<A>(K<G, A> fa) => 
        F.CoTransform(fa);
}
