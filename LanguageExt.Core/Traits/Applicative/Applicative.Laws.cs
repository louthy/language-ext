using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
namespace LanguageExt.Traits;

/// <summary>
/// Functions that test that Applicative-functor laws hold for the `F` applicative-functor provided.
/// </summary>
/// <para>
///  * Homomorphism
///  * Identity
///  * Interchange
///  * Applicative-Functor
///  * Composition
/// </para>
/// <remarks>
/// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
/// can be proven to be true.  If your applicative-functor doesn't have `Equals` then you must provide
/// the optional `equals` parameter so that the equality of outcomes can be tested.
/// </remarks>
/// <typeparam name="F">Applicative functor type</typeparam>
public static class ApplicativeLaw<F>
    where F : Applicative<F>
{
    /// <summary>
    /// Assert that the applicative-functor laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your applicative-functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Unit assert(Func<K<F, int>, K<F, int>, bool>? equals = null) =>
        validate(equals)
           .IfFail(errors => errors.Throw());

    /// <summary>
    /// Validate that the applicative-functor laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your applicative-functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> validate(Func<K<F, int>, K<F, int>, bool>? equals = null)
    {
        equals ??= (fa, fb) => fa.Equals(fb);
        var fa = F.Pure(1);
        return FunctorLaw<F>.validate(fa, equals) >>
               homomorphismLaw(equals)            >>
               interchangeLaw(equals)             >>
               compositionLaw(equals)             >>
               functorLaw(equals)                 >>
               identityLaw(equals);
    }

    /// <summary>
    /// Validate the homomorphism law
    /// </summary>
    /// <remarks>
    /// Homomorphism
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your applicative-functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> homomorphismLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var f  = (int x) => x * 2;
        var a  = 100;
        
        var ff = F.Pure(f);
        var fa = F.Pure(a);

        var lhs = ff.Apply(fa);
        var rhs = F.Pure(f(a));
        
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Applicative homomorphism law does not hold for {typeof(F).Name}");
    }
    
    /// <summary>
    /// Validate the interchange law
    /// </summary>
    /// <remarks>
    /// Interchange
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your applicative-functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> interchangeLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var ap = (int x) => (Func<int, int> f) => f(x);
        
        var f = (int x) => x * 2;
        var a = 100;
        
        var ff = F.Pure(f);
        var fa = F.Pure(a);

        var lhs = ff.Apply(fa);
        var rhs = F.Pure(ap(a)).Apply(ff);
        
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Applicative interchange law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Validate the identity law
    /// </summary>
    /// <remarks>
    /// Identity
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your applicative-functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> identityLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var lhs = F.Pure(100);
        var rhs = F.Pure<Func<int, int>>(identity).Apply(lhs);
        
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Applicative identity law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Validate the composition law
    /// </summary>
    /// <remarks>
    /// Composition
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> compositionLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        // (b -> c) -> (a -> b) -> a -> c
        Func<Func<int, int>, Func<Func<int, int>, Func<int, int>>> compose = 
            g => f => a => g(f(a));

        var xs = F.Pure(10);
        var us = F.Pure((int x) => x + 1);
        var vs = F.Pure((int x) => x * 2);

        var lhs = compose.Map(us).Apply(vs).Apply(xs); 
        var rhs = us.Apply(vs.Apply(xs));
        
        return equals(lhs, rhs)
                   ? unit
                   : Error.New($"Applicative composition law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Validate the functor law
    /// </summary>
    /// <remarks>
    /// Applicative-Functor
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> functorLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var g = (int x) => x * 1;
        var x = F.Pure(10);

        var lhs = g.Map(x);
        var rhs = F.Pure(g).Apply(x);
        
        return equals(lhs, rhs)
                   ? unit
                   : Error.New($"Applicative functor law does not hold for {typeof(F).Name}");
    }
}
