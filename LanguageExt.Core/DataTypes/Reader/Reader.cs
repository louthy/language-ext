using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Reader monad
    /// </summary>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public class Reader<Env, A>
    {
        /// <summary>
        /// Evaluate the reader monad
        /// </summary>
        internal readonly Func<Env, (A Value, Env Environment, bool IsBottom)> eval;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Reader(Func<Env, (A, Env, bool)> f) =>
            eval = f;
    }
}