using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    public static K<G, A> transform<F, G, A>(K<F, A> fa)
        where F : Natural<F, G> =>
        Natural.transform<F, G, A>(fa);
}
