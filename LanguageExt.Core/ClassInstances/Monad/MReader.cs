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
            default(MReader<Env, Env>).Bind<MReader<Env, B>, Reader<Env, B>, B>(
                default(MReader<Env, A>).Ask(),
                env => default(MReader<Env, B>).Return(f(env)));

        [Pure]
        public Reader<Env, Env> Ask() =>
            Reader(x => x);

        [Pure]
        public Reader<Env, A> Local(Func<Env, Env> f, Reader<Env, A> ma) =>
            default(MReader<Env, A>).Bind<MReader<Env, A>, Reader<Env, A>, A>(ma, a =>
            default(MReader<Env, A>).Return(env => (a, f(env), false)));

        [Pure]
        public Reader<Env, A> Return(A x) =>
            new Reader<Env, A>(env => (x,env,false));

        [Pure]
        public (A, Env, bool) Eval(Reader<Env, A> ma, Env state, bool bottom) =>
            bottom
                ? (default(A), state, bottom)
                : ma.Eval(state);
    }
}
