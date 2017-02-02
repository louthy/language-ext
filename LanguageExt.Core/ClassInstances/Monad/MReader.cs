using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct MReader<Env, A> : MonadReader<Env, A>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(Reader<Env, A> ma, Func<A, MB> f) where MONADB : struct, Monad<Env, MB, B> =>
            default(MONADB).Return(env =>
            {
                var (x, _, bottom) = ma.Eval(env);
                if (bottom) return (default(B), env, true);
                return default(MONADB).Eval(f(x), env, false);
            });

        [Pure]
        public Reader<Env, A> Fail(object err) =>
            Reader<Env, A>.Bottom;

        [Pure]
        public Reader<Env, A> Fail(Exception err = null) =>
            Reader<Env, A>.Bottom;

        [Pure]
        public Reader<Env, A> Return(Func<Env, (A, Env, bool)> f) =>
            new Reader<Env, A>(f);

        [Pure]
        public Reader<Env, B> Reader<B>(Func<Env, B> f) =>
            new Reader<Env, B>(env => (f(env), env, false));

        [Pure]
        public Reader<Env, Env> Ask =>
            ReaderEnv<Env>.Ask;

        [Pure]
        public Reader<Env, A> Local(Func<Env, Env> f, Reader<Env, A> ma) =>
            new Reader<Env, A>(env =>
            {
                var e = f(env);
                var (a, _, b) = ma.Eval(e);
                if (b) return (a, env, b);
                return (a, e, b);
            });

        [Pure]
        public Reader<Env, A> Return(A x) =>
            new Reader<Env, A>(env => (x,env,false));

        [Pure]
        public (A, Env, bool) Eval(Reader<Env, A> ma, Env state, bool bottom) =>
            bottom
                ? (default(A), state, bottom)
                : ma.Eval(state);
    }

    internal class ReaderEnv<Env>
    {
        public static readonly Reader<Env, Env> Ask = new Reader<Env, Env>(e => (e, e, false));
    }
}
