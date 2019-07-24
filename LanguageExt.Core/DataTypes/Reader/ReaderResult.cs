using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    /// <summary>
    /// Convenience methods for returning from a Reader<Env,A> computation
    /// </summary>
    public static class ReaderResult
    {
        public static (A Value, Env Environment, bool IsBottom) ToReader<Env, A>(this (A, Env) self) =>
            self.Add(false);

        public static (A Value, Env Environment, bool IsBottom) Return<Env, A>(A value, Env env) =>
            (value, env, false);

        public static (A Value, Env Environment, bool IsBottom) Fail<Env, A>() =>
            (default(A), default(Env), true);

        public static (Unit Value, Env Environment, bool IsBottom) Fail<Env>() =>
            (default(Unit), default(Env), true);
    }
}
