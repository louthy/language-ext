using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class RWS<R, W, S, A> : State<(R,W,S), A>
    {
        public static new readonly RWS<R, W, S, A> Bottom = new RWS<R, W, S, A>((default(A), default((R,W,S)), true));

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        internal RWS((A, (R, W, S), bool) value) : base(value) =>
            eval = state => value;

        internal RWS(Func<(R, W, S), (A, (R, W, S), bool)> f) : base(default((A, (R, W, S), bool))) =>
            eval = f ?? (s => (default(A), s, true));
    }
}