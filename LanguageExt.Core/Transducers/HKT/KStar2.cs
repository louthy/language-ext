namespace LanguageExt.HKT;

/// <summary>
/// Kind
/// </summary>
/// <remarks>
/// Provides a way to implement higher-kinded types.  Higher-kinds allow parametric polymorphism over not just
/// the lower kind (i.e. the `A` in `F<A>`, but also the 'containing' types, like the `F` in `F<A>`;
///
/// So, for example, `F<A>` would be parametric in both `F` and `A`.  `F` is the higher-kind, `A` is the lower-kind.
///
/// C# doesn't have support for higher-kinded types, so we hack it by using regular parametric polymorphism.  The`F`
/// is used as a 'dictionary' type, i.e. a trait.  So we can (for example) create a type called `Option`
/// (non-parametric) and use that as the discriminator (we can only work with other `Option` bases `K` types) and as
/// a dictionary lookup for functionality.    
/// </remarks>
/// <typeparam name="F">Higher kind</typeparam>
/// <typeparam name="A">Lower kind input type</typeparam>
public interface KStar2<F, X, A> 
    //where F : KLift2<F, X>
{
    /// <summary>
    /// Transducer from `A` to `B`
    /// </summary>
    Transducer<Unit, Sum<X, A>> Morphism { get; }
    
    /// <summary>
    /// Convert any derived type into its base kind
    /// </summary>
    public KStar2<F, X, A> AsKind() => 
        this;
}
