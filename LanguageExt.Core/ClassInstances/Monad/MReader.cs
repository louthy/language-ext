using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct MReader<SReaderA, SRA, Env, A> : MonadReader<SReaderA, SRA, Env, A>, Monad<Env, SRA, A>
        where SReaderA : struct, ReaderMonadValue<SRA, Env, A>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(SRA ma, Func<A, MB> f) where MONADB : struct, Monad<Env, MB, B> =>
            default(MONADB).Return(env =>
            {
                var (x, _, bottom) = default(SReaderA).Eval(ma, env);
                if (bottom) return (default(B), env, true);
                return default(MONADB).Eval(f(x), env);
            });

        [Pure]
        public SRA Fail(object err) =>
            default(SReaderA).Bottom;

        [Pure]
        public SRA Fail(Exception err = null) =>
            default(SReaderA).Bottom;

        [Pure]
        public SRA Return(Func<Env, (A, Env, bool)> f) =>
            default(SReaderA).Lift(f);

        [Pure]
        public SRA Reader(Func<Env, A> f) =>
            default(SReaderA).Lift(env => (f(env), env, false));

        [Pure]
        public SREnv Ask<SReaderEnv, SREnv>()
            where SReaderEnv : struct, ReaderMonadValue<SREnv, Env, Env> =>
                default(SReaderEnv).Lift(env =>
                    (env, env, false));

        [Pure]
        public SRA Local(Func<Env, Env> f, SRA ma) =>
            default(SReaderA).Lift(env =>
            {
                var e = f(env);
                var (a, _, b) = default(SReaderA).Eval(ma, e);
                if (b) return (a, env, b);
                return (a, e, b);
            });

        [Pure]
        public SRA Return(A x) =>
            default(SReaderA).Lift(env => (x,env,false));

        [Pure]
        public (A, Env, bool) Eval(SRA ma, Env state) =>
            default(SReaderA).Eval(ma, state);
    }
}
