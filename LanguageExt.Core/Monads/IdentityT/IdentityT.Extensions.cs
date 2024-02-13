using LanguageExt.HKT;

namespace LanguageExt;

public static class IdentityTExt
{
    public static IdentityT<M, A> As<M, A>(this K<IdentityT<M>, A> ma) 
        where M : MonadIO<M> =>
        (IdentityT<M, A>)ma;
}
