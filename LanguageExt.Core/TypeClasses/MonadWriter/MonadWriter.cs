using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// State monad type class
    /// </summary>
    [Typeclass]
    public interface MonadWriter<SWriterA, SWA, MonoidW, W, A>
        where SWriterA : struct, WriterMonadValue<SWA, W, A>
        where MonoidW  : struct, Monoid<W>
    {
        /// <summary>
        /// Tells the monad what you want it to hear.  The monad carries this 'packet'
        /// upwards, merging it if needed (hence the Monoid requirement).
        /// </summary>
        /// <typeparam name="W">Type of the value tell</typeparam>
        /// <param name="what">The value to tell</param>
        /// <returns>Updated writer monad</returns>
        [Pure]
        SSU Tell<SWriterU, SSU>(W what) where SWriterU : struct, WriterMonadValue<SSU, W, Unit>;

        /// <summary>
        /// 'listen' is an action that executes the monad and adds
        /// its output to the value of the computation.
        /// </summary>
        [Pure]
        SSAW Listen<SWriterAW, SSAW>(SWA ma) where SWriterAW : struct, WriterMonadValue<SSAW, W, (A, W)>;
    }
}
