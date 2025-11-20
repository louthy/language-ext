using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
namespace LanguageExt.Traits;

/// <summary>
/// Functions that test that monad laws hold for the `F` monad provided.
/// </summary>
/// <para>
///  * Homomorphism
///  * Identity
///  * Interchange
///  * Monad
///  * Composition
/// </para>
/// <remarks>
/// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
/// can be proven to be true.  If your monad doesn't have `Equals` then you must provide
/// the optional `equals` parameter so that the equality of outcomes can be tested.
/// </remarks>
/// <typeparam name="F">Functor type</typeparam>
public static class MonadLaw<F>
    where F : Monad<F>
{
    /// <summary>
    /// Assert that the monad laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your monad doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Unit assert(Func<K<F, int>, K<F, int>, bool>? equals = null) =>
        validate(equals)
           .IfFail(errors => errors.Throw());

    /// <summary>
    /// Validate that the monad laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your monad doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> validate(Func<K<F, int>, K<F, int>, bool>? equals = null)
    {
        equals ??= (fa, fb) => fa.Equals(fb);
        return ApplicativeLaw<F>.validate(equals) >>
               leftIdentityLaw(equals)            >>
               rightIdentityLaw(equals)           >>
               associativityLaw(equals);
    }

    /// <summary>
    /// Validate the left-identity law
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your monad doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> leftIdentityLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var a   = 100;
        var h   = (int x) => F.Pure(x);
        var lhs = F.Pure(a).Bind(h);
        var rhs = h(a);
        
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Monad left-identity law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Validate the right-identity law
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your monad doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> rightIdentityLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var a   = 100;
        var m   = F.Pure(a);
        var lhs = m.Bind(F.Pure);
        var rhs = m;
        
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Monad right-identity law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Validate the associativity law
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your monad doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> associativityLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var m = F.Pure(100);
        var g = (int x) => F.Pure(x * 10);
        var h = (int x) => F.Pure(x + 83);
        
        var lhs = m.Bind(g).Bind(h);
        var rhs = m.Bind(x => g(x).Bind(h));
        
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Monad associativity law does not hold for {typeof(F).Name}");
    }

}
