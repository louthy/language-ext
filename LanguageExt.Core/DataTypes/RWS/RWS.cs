using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public class RWS<MonoidW, R, W, S, A> : State<(R,W,S), A>
        where MonoidW : struct, Monoid<W>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        internal RWS(Func<(R, W, S), (A, (R, W, S), bool)> f) : base(null) =>
            eval = f;
    }
}