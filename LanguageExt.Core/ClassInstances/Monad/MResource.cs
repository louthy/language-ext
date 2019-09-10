
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    public struct MResource<A> :
        MonadResource,
        Monad<Unit,HashMap<IDisposable, bool>,Resource<A>, A>
    {
        public static readonly MResource<A> Inst = default;

        public Resource<A> Apply(Func<A, A, A> f, Resource<A> ma, Resource<A> mb) =>
            Bind<MResource<A>, Resource<A>, A>(ma, a1 =>
                Inst.Bind<MResource<A>, Resource<A>, A>(mb, a2 =>
                    Inst.Return(_ => f(a1, a2))));

        public MB Bind<MONADB, MB, B>(Resource<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, HashMap<IDisposable, bool>, MB, B> =>
            default(MONADB).Run(_ =>
            {
                var (a, acquired, faulted) = ma();
                if (faulted)
                {
                    acquired.Dispose();
                    return default(MONADB).Fail();
                }
                try
                {
                    return default(MONADB).BindReturn(acquired, f(a));
                }
                catch
                {
                    acquired.Dispose();
                    throw;
                }
            });

        public MB BindAsync<MONADB, MB, B>(Resource<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, HashMap<IDisposable, bool>, MB, B> =>
            default(MONADB).RunAsync(_ =>
            {
                var (a, acquired, faulted) = ma();
                if (faulted)
                {
                    acquired.Dispose();
                    return default(MONADB).Fail().AsTask();
                }
                try
                {
                    return default(MONADB).BindReturn(acquired, f(a)).AsTask();
                }
                catch
                {
                    acquired.Dispose();
                    throw;
                }
            });

        public Resource<A> BindReturn(HashMap<IDisposable, bool> acquired, Resource<A> mb)
        {
            var (b, acquiredB, faulted) = mb();
            return () => 
                faulted
                    ? (default, default, true)
                    : (b, acquired.AddOrUpdateRange(acquiredB), false);
        }

        public Func<Unit, int> Count(Resource<A> fa) => _ =>
        {
            var (a, acquired, faulted) = fa();
            try
            {
                return acquired.Count();
            }
            finally
            {
                acquired.Dispose();
            }
        };

        public Resource<A> Fail(object err = null) => () =>
            (default, default, true);
    

        public Func<Unit, S> Fold<S>(Resource<A> fa, S state, Func<S, A, S> f) => _ =>
        {
            var (a, acquired, faulted) = fa();
            try
            {
                return faulted
                    ? state
                    : f(state, a);
            }
            finally
            {
                acquired.Dispose();
            }
        };

        public Func<Unit, S> FoldBack<S>(Resource<A> fa, S state, Func<S, A, S> f) =>
            Fold(fa, state, f);

        public Resource<A> Plus(Resource<A> ma, Resource<A> mb)
        {
            var result = ma();
            return result.IsFaulted
                ? mb
                : () => result;
        }

        public Resource<Unit> Release<R>(Resource<R> ma) where R : IDisposable => () =>
        {
            var (released, acquired, faulted) = ma();
            released.Dispose();
            return (unit, acquired.SetItem(released, false), faulted);
        };

        public Resource<A> Return(Func<Unit, A> f) => () =>
            (f(unit), default, false);

        public Resource<A> Run(Func<Unit, Resource<A>> ma) => () =>
            ma(unit)();

        public Resource<R> Use<R>(Func<R> acquire) where R : IDisposable =>
            () =>
            {
                var acquired = acquire();
                return (acquired, HashMap((acquired as IDisposable, true)), false);
            };

        public Resource<A> Zero() => () =>
            (default, default, true);
    }
}