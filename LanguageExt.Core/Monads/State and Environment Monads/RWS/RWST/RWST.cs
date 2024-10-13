using System;
using LanguageExt.Traits;

namespace LanguageExt;

public record RWST<R, W, S, M, A>(Func<(R Env, W Output, S State), K<M, (A Value, W Output, S State)>> runRWST): 
    K<RWST<R, W, S, M>, A>
    where M : Monad<M>, SemiAlternative<M>
    where W : Monoid<W>
{
    
}
