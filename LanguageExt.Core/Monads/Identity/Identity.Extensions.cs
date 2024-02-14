using LanguageExt.Traits;

namespace LanguageExt;

public static class IdentityExt
{
    public static Identity<A> As<A>(this K<Identity, A> ma) =>
        (Identity<A>)ma;
}
