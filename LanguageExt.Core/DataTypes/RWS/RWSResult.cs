using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.Core.DataTypes.RWS
{
    /// <summary>
    /// Convenience methods for returning from a RWS<MonoidW, R, W, S, A> computation
    /// </summary>
    public static class RWSResult
    {
        public static (A Value, W Output, S State, bool IsBottom) ToRWS<MonoidW, W, S, A>(this (A Value, S State) self)
            where MonoidW : struct, Monoid<W> => (self.Value, default(MonoidW).Empty(), self.State, false);

        public static (A Value, W Output, S State, bool IsBottom) Return<MonoidW, W, S, A>(A value, S state)
            where MonoidW : struct, Monoid<W> => (value, default(MonoidW).Empty(), state, false);

        public static (Unit Value, W Output, S State, bool IsBottom) Return<MonoidW, W, S>(S state)
            where MonoidW : struct, Monoid<W> => (Unit.Default, default(MonoidW).Empty(), state, false);

        public static (A Value, W Output, S State, bool IsBottom) Fail<MonoidW, W, S, A>() 
            where MonoidW : struct, Monoid<W> => (default(A), default(MonoidW).Empty(), default(S), true);

        public static (Unit Value, W Output, S State, bool IsBottom) Fail<MonoidW, W, S>() 
            where MonoidW : struct, Monoid<W> => (Unit.Default, default(MonoidW).Empty(), default(S), true);
    }
}
