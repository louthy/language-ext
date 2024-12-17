using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
namespace LanguageExt.Traits;

/// <summary>
/// Functions that test that functor-laws hold for the `F` functor provided.
/// </summary>
/// <remarks>
/// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
/// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
/// the optional `equals` parameter so that the equality of outcomes can be tested.
/// </remarks>
/// <typeparam name="F">Functor type</typeparam>
public static class FunctorLaw<F>
    where F : Functor<F>
{
    /// <summary>
    /// Assert that the functor laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Unit assert(K<F, int> fa, Func<K<F, int>, K<F, int>, bool>? equals = null) =>
        validate(fa, equals)
           .IfFail(errors => errors.Throw());

    /// <summary>
    /// Validate that the functor laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> validate(K<F, int> fa, Func<K<F, int>, K<F, int>, bool>? equals = null)
    {
        equals ??= (fa, fb) => fa.Equals(fb);
        return identityLaw(fa, equals) >> 
               compositionLaw(fa, f, g, equals);
    }

    /// <summary>
    /// Validate the identity law
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> identityLaw(K<F, int> lhs, Func<K<F, int>, K<F, int>, bool> equals)
    {
        var rhs = lhs.Map(identity);
        return equals(lhs, rhs) 
                   ? unit
                   : Error.New($"Functor identity-law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Validate the composition law
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your functor doesn't have `Equals` then you must provide
    /// the optional `equals` parameter so that the equality of outcomes can be tested.
    /// </remarks>
    public static Validation<Error, Unit> compositionLaw(
        K<F, int> fa, 
        Func<int, int> f,
        Func<int, int> g, 
        Func<K<F, int>, K<F, int>, bool> equals)
    {
        var lhs = fa.Map(a => g(f(a)));
        var rhs = fa.Map(f).Map(g);
        return equals(lhs, rhs)
                   ? unit
                   : Error.New($"Functor composition-law does not hold for {typeof(F).Name}");
    }
    
    static int f(int x) =>
        x + 1;
    
    static int g(int x) =>
        x * 2;
}
