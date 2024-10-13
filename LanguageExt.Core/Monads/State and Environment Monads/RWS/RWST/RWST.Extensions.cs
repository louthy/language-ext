using LanguageExt.Traits;

namespace LanguageExt;

public static class RWSTExtensions
{
    public static RWST<R, W, S, M, A> As<R, W, S, M, A>(this K<RWST<R, W, S, M>, A> ma) 
        where M : Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        (RWST<R, W, S, M, A>)ma;
}
