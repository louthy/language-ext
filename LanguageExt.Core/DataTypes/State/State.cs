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
    public class State<S, A> : NewType<State<S, A>, (A, S, bool)>
    {
        public static readonly State<S, A> Bottom = new State<S, A>((default(A), default(S), true));

        /// <summary>
        /// Evaluate the state monad
        /// </summary>
        internal Func<S, (A Value, S State, bool IsBottom)> eval;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        internal State((A, S, bool) value) : base(value) =>
            eval = state => value;

        internal State(Func<S, (A, S, bool)> f) : base(default((A, S, bool))) =>
            eval = f ?? (s => (default(A), s, true));

        [Pure]
        internal static State<S, A> From(Func<S, (A, S, bool)> f) =>
            new State<S, A>(f);
    }
}