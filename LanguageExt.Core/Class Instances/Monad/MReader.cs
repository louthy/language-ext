using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MReader<Env, A> : 
        MonadReader<Env, A>, 
        Monad<Env, Unit, Reader<Env, A>, A>
    {
        public static readonly MReader<Env, A> Inst = new MReader<Env, A>();

        [Pure]
        public MB Bind<MONADB, MB, B>(Reader<Env, A> ma, Func<A, MB> f) where MONADB : struct, Monad<Env, Unit, MB, B> =>
            default(MONADB).Run(env =>
            {
                var resA = ma(env);
                if (resA.IsFaulted) return default(MONADB).Fail(resA.ErrorInt);
                return f(resA.Value);
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Reader<Env, A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Env, Unit, MB, B> =>
            default(MONADB).RunAsync(env =>
            {
                var resA = ma(env);
                if (resA.IsFaulted) return default(MONADB).Fail(resA.ErrorInt).AsTask();
                return f(resA.Value).AsTask();
            });

        [Pure]
        public Reader<Env, A> Fail(object err = null) => _ =>
            ReaderResult<A>.New(Common.Error.FromObject(err));

        [Pure]
        public Reader<Env, A> Reader(Func<Env, A> f) => env =>
            ReaderResult<A>.New(f(env));

        [Pure]
        public Reader<Env, Env> Ask() => env =>
            ReaderResult<Env>.New(env);

        [Pure]
        public Reader<Env, A> Local(Reader<Env, A> ma, Func<Env, Env> f) => env =>
            ma(f(env));

        [Pure]
        public Reader<Env, A> Return(Func<Env, A> f) => env =>
            ReaderResult<A>.New(f(env));

        [Pure]
        public Reader<Env, A> Run(Func<Env, Reader<Env, A>> f) => env =>
            f(env)(env);

        [Pure]
        public Reader<Env, A> Plus(Reader<Env, A> ma, Reader<Env, A> mb) => env =>
        {
            var resA = ma(env);
            return resA.IsFaulted
                ? mb(env)
                : resA;
        };

        [Pure]
        public Reader<Env, A> Zero() =>
            _ => ReaderResult<A>.Bottom;

        [Pure]
        public Func<Env, S> Fold<S>(Reader<Env, A> fa, S state, Func<S, A, S> f) => env =>
        {
            var resA = fa(env);
            return resA.IsFaulted
                ? state
                : f(state, resA.Value);
        };

        [Pure]
        public Func<Env, S> FoldBack<S>(Reader<Env, A> fa, S state, Func<S, A, S> f) =>
            Fold(fa, state, f);

        [Pure]
        public Func<Env, int> Count(Reader<Env, A> fa) =>
            Fold(fa, 0, (_, __) => 1);

        [Pure]
        public Reader<Env, A> BindReturn(Unit _, Reader<Env, A> mb) => env =>
            mb(env);

        [Pure]
        public Reader<Env, A> Apply(Func<A, A, A> f, Reader<Env, A> fa, Reader<Env, A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
    }
}
