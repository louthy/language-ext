using LanguageExt.Transducers;

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
/// <typeparam name="B">Lower kind output type</typeparam>
public interface KArr<F, A, B>
{
    /// <summary>
    /// Transducer from `A` to `B`
    /// </summary>
    Transducer<A, B> Morphism { get; }
}

/// <summary>
/// Kind (for sum-types)
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
/// <typeparam name="X">Left source-type</typeparam>
/// <typeparam name="Y">Left destination-type</typeparam>
/// <typeparam name="A">Right destination-type</typeparam>
/// <typeparam name="B">Right destination-type</typeparam>
public interface KArr<F, X, Y, A, B> : KArr<F, Sum<X, A>, Sum<Y, B>>
{
}


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
public interface KStar<F, A> : KArr<F, Unit, A>
{
}

/// <summary>
/// Kind (for sum-types)
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
/// <typeparam name="X">Left source-type</typeparam>
/// <typeparam name="A">Right destination-type</typeparam>
public interface KStar<F, X, A> : KStar<F, Sum<X, A>>
{
}
