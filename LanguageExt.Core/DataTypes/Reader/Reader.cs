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
    /// Reader monad
    /// </summary>
    /// <typeparam name="Env">Environment type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public class Reader<Env, A> : NewType<Reader<Env, A>, (A, Env, bool)>
    {
        /// <summary>
        /// Bottom
        /// </summary>
        public static readonly Reader<Env, A> Bottom = new Reader<Env, A>((default(A), default(Env), true));

        /// <summary>
        /// Evaluate the reader monad
        /// </summary>
        internal readonly Func<Env, (A Value, Env Environment, bool IsBottom)> eval;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        internal Reader((A, Env, bool) value) : base(value) =>
            eval = state => value;

        internal Reader(Func<Env, (A, Env, bool)> f) : base(default((A, Env, bool))) =>
            eval = f ?? (e => (default(A), e, true));

        [Pure]
        internal static Reader<Env, A> From(Func<Env, (A, Env, bool)> f) =>
            new Reader<Env, A>(f);
    }
}