using LanguageExt.Traits;

namespace LanguageExt;

public static class RWSTExtensions
{
    public static RWST<R, W, S, M, A> As<R, W, S, M, A>(this K<RWST<R, W, S, M>, A> ma) 
        where M : Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        (RWST<R, W, S, M, A>)ma;
    
    public static K<M, (A Value, W Output, S State)> Run<R, W, S, M, A>(
        this K<RWST<R, W, S, M>, A> ma, R env, W output, S state) 
        where M : Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        ma.As().runRWST((env, output, state));
    
    public static K<M, (A Value, W Output, S State)> Run<R, W, S, M, A>(
        this K<RWST<R, W, S, M>, A> ma, R env, S state) 
        where M : Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        ma.As().runRWST((env, W.Empty, state));
}
