using LanguageExt.Traits;

namespace LanguageExt;

public static class Memo
{
    /// <summary>
    /// Natural transformation for memoised higher-kinded structures
    /// </summary>
    /// <param name="ma">Structure to transform</param>
    /// <typeparam name="F">Original structure</typeparam>
    /// <typeparam name="G">New structure</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Naturally transformed structure</returns>
    public static Memo<G, A> transform<F, G, A>(Memo<F, A> ma)
        where F : Natural<F, G> =>
        new(() => F.Transform(ma.Value));
    
    /// <summary>
    /// Natural transformation for memoised higher-kinded structures
    /// </summary>
    /// <param name="ma">Structure to transform</param>
    /// <typeparam name="F">New structure</typeparam>
    /// <typeparam name="G">Original structure</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Naturally transformed structure</returns>
    public static Memo<F, A> cotransform<F, G, A>(Memo<G, A> ma)
        where F : CoNatural<F, G> =>
        new(() => F.CoTransform(ma.Value));
}
