using LanguageExt.Traits;

namespace LanguageExt;

public partial class Identity
{
    public static Identity<A> Pure<A>(A value) =>
        new (value);
    
    static K<Identity, A> PureK<A>(A value) =>
        Pure(value);
}
