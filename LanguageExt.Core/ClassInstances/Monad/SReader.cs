using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct SReader<Env, A> : ReaderMonadValue<Reader<Env, A>, Env, A>
    {
        static readonly Reader<Env, A> bottom = new Reader<Env, A>(_ => (default(A), default(Env), true));

        [Pure]
        public (A, Env, bool) Eval(Reader<Env, A> r, Env env) =>
            r.eval(env);

        [Pure]
        public Reader<Env, A> Lift((A, Env, bool) value) =>
            new Reader<Env, A>(_ => value);

        [Pure]
        public Reader<Env, A> Lift(Func<Env, (A, Env, bool)> f) =>
            new Reader<Env, A>(f);

        [Pure]
        public Reader<Env, A> Bottom => bottom;
    }
}