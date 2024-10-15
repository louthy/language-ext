using System;
using static LanguageExt.Prelude;
namespace LanguageExt.Traits;

/// <summary>
/// Functions that test that functor-laws hold for the `F` functor provided.
/// </summary>
/// <remarks>
/// NOTE: `Equals` must be implemented for the `K<F, A>` derived-type, so that the laws
/// can be proven to be true.
/// </remarks>
/// <typeparam name="F">Functor type</typeparam>
public static class FunctorLaw<F>
    where F : Functor<F>
{
    public static bool lawsHold(K<F, int> fa, Func<K<F, int>, K<F, int>, bool>? equals = null)
    {
        equals ??= (fa, fb) => fa.Equals(fb);
        return identityLaw(fa, equals) && compositionLaw(fa, f, g, equals);
    }

    public static bool identityLaw<A>(
        K<F, A> fa, 
        Func<K<F, A>, K<F, A>, bool> equals)
    {
        var fa1 = fa.Map(identity);
        return equals(fa, fa1);
    }

    public static bool compositionLaw<A, B, C>(
        K<F, A> fa, Func<A, B> f,
        Func<B, C> g, 
        Func<K<F, C>, K<F, C>, bool> equals)
    {
        var fa1 = fa.Map(a => g(f(a)));
        var fa2 = fa.Map(f).Map(g);
        return equals(fa1, fa2);
    }
    
    static int f(int x) =>
        x + 1;
    
    static int g(int x) =>
        x * 2;
}
