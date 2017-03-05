using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="MA">The data-type to make monadic</typeparam>
    /// <typeparam name="A">The data-type bound value</typeparam>
    [Typeclass]
    public interface Monad<MA, A> : Monad<Unit, Unit, MA, A>, Foldable<MA, A>
    {
        MA Return(A x);
    }

    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="Env">The input type to the monadic computation</typeparam>
    /// <typeparam name="Out">The output type of the monadic computation</typeparam>
    /// <typeparam name="MA">The data-type to make monadic</typeparam>
    /// <typeparam name="A">The data-type bound value</typeparam>
    [Typeclass]
    public interface Monad<Env, Out, MA, A> : Foldable<Env, MA, A>
    {
        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="MONADB">Type-class of the return value</typeparam>
        /// <typeparam name="MB">Type of the monad to return</typeparam>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of type MB derived from Monad of B</returns>
        [Pure]
        MB Bind<MONADB, MB, B>(MA ma, Func<A, MB> f) 
            where MONADB : struct, Monad<Env, Out, MB, B>;

        /// <summary>
        /// Lazy monad construction
        /// </summary>
        MA Return(Func<Env, A> f);

        /// <summary>
        /// Used for double dispatch by the bind function for monadic types that
        /// need to construct an output value/state (like MState and MWriter).  For
        /// all other monad types just return mb.
        /// </summary>
        /// <param name="maOutput">Output from the first part of a monadic bind</param>
        /// <param name="mb">Monadic to invoke.  Get the results from this to combine with
        /// maOutput and then re-wrap</param>
        /// <returns>Monad with the combined output</returns>
        MA BindReturn(Out maOutput, MA mb);

        /// <summary>
        /// Monad identity
        /// </summary>
        MA Id(Func<Env, MA> ma);

        /// <summary>
        /// Monad identity asynchronously
        /// </summary>
        MA IdAsync(Func<Env, Task<MA>> ma);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        MA Fail(Exception err = null);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        MA Fail(object err);

        /// <summary>
        /// Associative binary operation
        /// </summary>
        [Pure]
        MA Plus(MA a, MA b);

        /// <summary>
        /// Neutral element (None in Option for example)
        /// </summary>
        [Pure]
        MA Zero();
    }
}
