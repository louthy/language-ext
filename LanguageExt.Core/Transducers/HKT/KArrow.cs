using System;
using static LanguageExt.Prelude;

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
/// <typeparam name="Env">Lower kind input type</typeparam>
/// <typeparam name="G">Higher kind</typeparam>
/// <typeparam name="A">Lower kind output type</typeparam>
public interface KArrow<F, Env, G, A> 
//    where F : KLift<F, Env, G>
//    where G : KLift<G>
{
    /// <summary>
    /// Transducer from `A` to `B`
    /// </summary>
    Transducer<Env, KStar<G, A>> Morphism { get; }

    /*
    public KStar<G, A> Invoke(Env env) =>
        Morphism.Invoke(env);
        */

    public KArrow<F, Env, G, A> AsKind() => this;
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
/// <typeparam name="B">Lower kind output type</typeparam>
public interface KArrow<F, A, B> 
    //where F : KLift<F, A>
{
    /// <summary>
    /// Transducer from `A` to `B`
    /// </summary>
    Transducer<A, B> Morphism { get; }
    
    public KArrow<F, A, B> AsKind() => this;
}
