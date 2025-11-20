namespace LanguageExt.Traits;

/// <summary>
/// Natural monomorphic transformation 
/// </summary>
/// <remarks>
/// Monomorphism means that there's one arrow between `F` and `G`.  Therefore, there's also a `CoNatural` between
/// `G` and `F` (`CoNatural` is the dual of `Natural`, so its arrows are flipped, so a `CoNatural` between `G` and `F`
/// is isomorphic to a `Natural` between `F` and `G`).  That's why `NaturalMono` derives from both `Natural` and
/// `CoNatural` and can provide a default implementation for `CoTransform`.
/// </remarks>
/// <remarks>
/// This type wouldn't need to exist is C# was better at type-unification.   Use this when you want a unidirectional
/// natural-transformation.  Use `NaturalIso` when you want a bidirectional natural-transformation.
/// </remarks>
/// <remarks>
/// Functors are referenced, because that's the true definition in category-theory, but
/// there is no requirement in language-ext for FA or GA to be functors.  It is just typically
/// true that FA and GA will also be functors.
/// </remarks>
/// <typeparam name="F">From functor</typeparam>
/// <typeparam name="G">To functor</typeparam>
public interface NaturalMono<out F, in G> : Natural<F, G>, CoNatural<G, F>
    where F : Natural<F, G> 
{
    static K<G, A> CoNatural<G, F>.CoTransform<A>(K<F, A> fa) => 
        F.Transform(fa);
}
