using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// RWS monad type class
    /// </summary>
    [Typeclass]
    public interface MonadRWS<SRwsA, RwsA, MonoidW, R, W, S, A> : 
        MonadReader<SRwsA, RwsA, R, A>,
        MonadWriter<SRwsA, RwsA, MonoidW, W, A>,
        MonadState<SRwsA, RwsA, S, A>
        where SRwsA   : struct, 
            ReaderMonadValue<RwsA, R, A>,
            WriterMonadValue<RwsA, W, A>, 
            StateMonadValue<RwsA, S, A>, 
            MonadValue<RwsA, (R, W, S), A>
        where MonoidW : struct, Monoid<W>
    {
    }
}
