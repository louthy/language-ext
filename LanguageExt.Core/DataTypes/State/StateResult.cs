using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    /// <summary>
    /// Convenience methods for returning from a State<S,A> computation
    /// </summary>
    public static class StateResult
    {
        public static (A Value, S State, bool IsBottom) ToState<S, A>(this (A, S) self) =>
            self.Add(false);

        public static (A Value, S State, bool IsBottom) Return<S, A>(A value, S state) =>
            (value, state, false);

        public static (Unit Value, S State, bool IsBottom) Return<S>(S state) =>
            (Unit.Default, state, false);

        public static (A Value, S State, bool IsBottom) Fail<S, A>() =>
            (default(A), default(S), true);

        public static (Unit Value, S State, bool IsBottom) Fail<S>() =>
            (default(Unit), default(S), true);
    }
}
