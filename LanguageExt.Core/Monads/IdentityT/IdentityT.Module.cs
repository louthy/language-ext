using LanguageExt.Traits;

namespace LanguageExt;

public class IdentityT
{
    public static IdentityT<M, A> Pure<M, A>(A value) 
        where M : Monad<M>, Choice<M> =>
        new (M.Pure(value));

    public static IdentityT<M, A> lift<M, A>(K<M, A> value) 
        where M : Monad<M>, Choice<M> =>
        new (value);
}
