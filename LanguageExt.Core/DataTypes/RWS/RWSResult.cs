using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    /// <summary>
    /// Convenience methods for returning from a RWS<R,W,S,A> computation
    /// </summary>
    public static class RWSResult
    {
        public static (A Value, R Env, W Output, S State, bool IsBottom) ToState<MonoidW, R, W, S, A>(this (A, R, W, S) self)
            where MonoidW : struct, Monoid<W> =>
                self.Add(false);

        public static (A Value, R Env, W Output, S State, bool IsBottom) Return<MonoidW, R, W, S, A>(A value, R env, W output, S state)
            where MonoidW : struct, Monoid<W> =>
                (value, env, output, state, false);

        public static (Unit Value, R Env, W Output, S State, bool IsBottom) Return<MonoidW, R, W, S>((R, W, S) result)
            where MonoidW : struct, Monoid<W> =>
                (Unit.Default, result.Item1, result.Item2, result.Item3, false);

        public static (A Value, R Env, W Output, S State, bool IsBottom) Fail<MonoidW, R, W, S, A>()
            where MonoidW : struct, Monoid<W> =>
                (default(A), default(R), default(W), default(S), true);

        public static (Unit Value, R Env, W Output, S State, bool IsBottom) Fail<MonoidW, R, W, S>()
            where MonoidW : struct, Monoid<W> =>
                (Unit.Default, default(R), default(W), default(S), true);
    }
}
