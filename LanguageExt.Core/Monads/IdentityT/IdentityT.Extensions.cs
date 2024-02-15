using LanguageExt.Traits;

namespace LanguageExt;

public static class IdentityTExt
{
    public static IdentityT<M, A> As<M, A>(this K<IdentityT<M>, A> ma) 
        where M : Monad<M> =>
        (IdentityT<M, A>)ma;
}
