using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    public delegate (A Value, W Output, S State, bool IsFaulted) RWS<MonoidW, R, W, S, A>(R env, S state)
        where MonoidW : struct, Monoid<W>;
}
