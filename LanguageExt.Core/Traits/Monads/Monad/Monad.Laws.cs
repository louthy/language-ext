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
        return ApplicativeLaw<F>.validate(equals) >>>
               leftIdentityLaw(equals)            >>>
               rightIdentityLaw(equals)           >>>
               associativityLaw(equals)           >>>
               recurIsSameAsBind(equals);
    }

    public static Validation<Error, Unit> recurIsSameAsBind(Func<K<F, int>, K<F, int>, bool>? equals = null)
    {
        var example = Seq(1, 2, 3, 4, 5, 6, 7, 8, 9);

        var result1 = Monad.recur((0, example), rec);
        var result2 = Monad.unsafeRecur((0, example), rec);
        var result3 = bind(example);

        equals ??= (fa, fb) => fa.Equals(fb);
        if(!equals(result1, result2)) throw new Exception("Bug in MonadLaw or Monad.unsafeRecur. Contact language-ext maintainer via the repo.");
        if (!equals(result1, result3))
        {
            return Validation.Fail<Error, Unit>(
                Error.New($"Monad trait implementation for {typeof(F).Name}.Recur gives a different "    +
                          $"result to the equivalent recursive {typeof(F).Name}.Bind.  This suggests "   +
                          $"an implementation bug, most likely in {typeof(F).Name}.Recur, but possibly " +
                          $"in {typeof(F).Name}.Bind."));
        }

        return Validation.Success<Error, Unit>(unit);
        
        K<F, Next<(int Total, Seq<int> Values), int>> rec((int Total, Seq<int> Values) pair) =>
            pair.Values switch
            {
                []          => F.Pure(Next.Done<(int, Seq<int>), int>(pair.Total)),
                var (x, xs) => F.Pure(Next.Loop<(int, Seq<int>), int>((pair.Total + x, xs))) 
            };

        K<F, int> bind(Seq<int> values) =>
            values switch
            {
                [var x]     => F.Pure(x),
                var (x, xs) => bind(xs) * (t => x + t)
            };
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
