using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// State monad
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public class State<S, A>
    {
        /// <summary>
        /// Evaluate the state monad
        /// </summary>
        internal Func<S, (A Value, S State, bool IsBottom)> eval;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="f"></param>
        internal State(Func<S, (A, S, bool)> f) =>
            eval = f;
    }
}