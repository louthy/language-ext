using System;
using System.Collections.Generic;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    [Typeclass]
    public interface Monad<MA, A> : Foldable<MA, A>
    {
        // TODO: Need a lazy Return to unify the delegate based monads and 
        //       the strict monads.

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="MONADB">Type-class of the return value</typeparam>
        /// <typeparam name="MB">Type of the monad to return</typeparam>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of type MB derived from Monad of B</returns>
        MB Bind<MONADB, MB, B>(MA ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B>;

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        MA Return(A x);

        ///// <summary>
        ///// Monad return
        ///// </summary>
        ///// <param name="xs">The bound monad value(s)</param>
        ///// <returns>Monad of A</returns>
        MA FromSeq(IEnumerable<A> xs);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        MA Fail(Exception err = null);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        MA Fail(object err);
    }

}
