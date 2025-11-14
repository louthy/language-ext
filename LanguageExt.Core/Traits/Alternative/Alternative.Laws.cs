using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
namespace LanguageExt.Traits;

/// <summary>
/// Functions that test that Alternative laws hold for the `F` Alternative provided.
/// </summary>
/// <para>
///     choose(pure(a), pure(b)) = pure(a)
///     choose(empty(), pure(b)) = pure(b)
///     choose(pure(a), empty()) = pure(a)
/// </para>
/// <remarks>
/// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
/// can be proven to be true.  If your Alternative structure doesn't have `Equals` then you
/// must provide the optional `equals` parameter so that the equality of outcomes can be tested.
/// </remarks>
/// <typeparam name="F">Alternative type</typeparam>
public static class AlternativeLaw<F>
    where F : Alternative<F>, Applicative<F>
{
    /// <summary>
    /// Assert that the Alternative laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your Alternative structure doesn't have `Equals` then
    /// you must provide the optional `equals` parameter so that the equality of outcomes can
    /// be tested.
    /// </remarks>
    public static Unit assert(Func<K<F, int>, K<F, int>, bool>? equals = null) =>
        validate(equals)
           .IfFail(errors => errors.Throw());

    /// <summary>
    /// Validate that the Alternative laws hold
    /// </summary>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your Alternative structure doesn't have `Equals` then
    /// you must provide the optional `equals` parameter so that the equality of outcomes can
    /// be tested.
    /// </remarks>
    public static Validation<Error, Unit> validate(Func<K<F, int>, K<F, int>, bool>? equals = null)
    {
        equals ??= (fa, fb) => fa.Equals(fb);
        return ApplicativeLaw<F>.validate(equals) >>
               leftCatchLaw(equals)               >>
               leftZeroLaw(equals)                >>
               rightZeroLaw(equals);
    }

    /// <summary>
    /// Left-zero law
    /// </summary>
    /// <remarks>
    ///    choose(empty, pure(x)) = pure(x)
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your Alternative structure doesn't have `Equals` then
    /// you must provide the optional `equals` parameter so that the equality of outcomes can
    /// be tested.
    /// </remarks>
    public static Validation<Error, Unit> leftZeroLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var fa = F.Empty<int>();
        var fb = F.Pure(100);
        var fr = choose(fa, fb);

        return equals(fr, fb) 
                   ? unit
                   : Error.New($"Alternative left-zero law does not hold for {typeof(F).Name}");
    }

    /// <summary>
    /// Right-zero law
    /// </summary>
    /// <remarks>
    ///    choose(pure(x), empty) = pure(x)
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your Alternative structure doesn't have `Equals` then
    /// you must provide the optional `equals` parameter so that the equality of outcomes can
    /// be tested.
    /// </remarks>
    public static Validation<Error, Unit> rightZeroLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var fa = F.Pure(100);
        var fb = F.Empty<int>();
        var fr = choose(fa, fb);

        return equals(fr, fa) 
                   ? unit
                   : Error.New($"Alternative right-zero law does not hold for {typeof(F).Name}");
    }    

    /// <summary>
    /// Left catch law
    /// </summary>
    /// <remarks>
    ///    choose(pure(x), pure(y)) = pure(x)
    /// </remarks>
    /// <remarks>
    /// NOTE: `Equals` must be implemented for the `K〈F, *〉` derived-type, so that the laws
    /// can be proven to be true.  If your Alternative structure doesn't have `Equals` then
    /// you must provide the optional `equals` parameter so that the equality of outcomes can
    /// be tested.
    /// </remarks>
    public static Validation<Error, Unit> leftCatchLaw(Func<K<F, int>, K<F, int>, bool> equals)
    {
        var fa = F.Pure(100);
        var fb = F.Pure(200);
        var fr = choose(fa, fb);

        return equals(fr, fa) 
                   ? unit
                   : Error.New($"Choice left-catch law does not hold for {typeof(F).Name}");
    }    
    
}
