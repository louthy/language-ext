using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;

namespace LanguageExt
{
    public class Writer<MonoidW, W, A> where MonoidW : struct, Monoid<W>
    {
        /// <summary>
        /// Evaluate the state monad
        /// </summary>
        internal Func<W, (A Value, W Output, bool IsBottom)> eval;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Writer(Func<W, (A, W, bool)> f) =>
            eval = f;
    }
}
