using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    public static K<F, A> cotransform<F, G, A>(K<G, A> ga)
        where F : CoNatural<F, G> =>
        CoNatural.transform<F, G, A>(ga);
}
