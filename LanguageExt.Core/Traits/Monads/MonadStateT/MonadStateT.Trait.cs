using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface MonadStateT<MState, S, out M> : MonadT<MState, M> 
    where M : Monad<M>
    where MState : MonadStateT<MState, S, M>
{
    public static abstract K<MState, Unit> Put(S value);

    public static abstract K<MState, Unit> Modify(Func<S, S> modify) ;

    public static abstract K<MState, A> Gets<A>(Func<S, A> f);

    public static virtual K<MState, S> Get =>
        MonadStateT.gets<MState, S, M, S>(identity);
}
