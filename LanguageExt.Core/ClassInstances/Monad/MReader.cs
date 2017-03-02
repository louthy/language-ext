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

    public struct MReader<Env, A> : MonadReader<SReader<Env, A>, Reader<Env, A>, Env, A>, Monad<Env, Reader<Env, A>, A>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(Reader<Env, A> ma, Func<A, MB> f) where MONADB : struct, Monad<Env, MB, B> =>
            default(MONADB).Return(env =>
            {
                var (x, _, bottom) = default(SReader<Env, A>).Eval(ma, env);
                if (bottom) return (default(B), env, true);
                return default(MONADB).Eval(f(x), env);
            });

        [Pure]
        public Reader<Env, A> Fail(object err) =>
            default(SReader<Env, A>).Bottom;

        [Pure]
        public Reader<Env, A> Fail(Exception err = null) =>
            default(SReader<Env, A>).Bottom;

        [Pure]
        public Reader<Env, A> Return(Func<Env, (A, Env, bool)> f) =>
            default(SReader<Env, A>).Lift(f);

        [Pure]
        public Reader<Env, A> Reader(Func<Env, A> f) =>
            default(SReader<Env, A>).Lift(env => (f(env), env, false));

        [Pure]
        public SREnv Ask<SReaderEnv, SREnv>()
            where SReaderEnv : struct, ReaderMonadValue<SREnv, Env, Env> =>
                default(SReaderEnv).Lift(env =>
                    (env, env, false));

        [Pure]
        public Reader<Env, A> Local(Func<Env, Env> f, Reader<Env, A> ma) =>
            default(SReader<Env, A>).Lift(env =>
            {
                var e = f(env);
                var (a, _, b) = default(SReader<Env, A>).Eval(ma, e);
                if (b) return (a, env, b);
                return (a, e, b);
            });

        [Pure]
        public Reader<Env, A> Return(A x) =>
            default(SReader<Env, A>).Lift(env => (x, env, false));

        [Pure]
        public (A, Env, bool) Eval(Reader<Env, A> ma, Env state) =>
            default(SReader<Env, A>).Eval(ma, state);
    }
}
