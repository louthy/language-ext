using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryAsync<A> :
        Optional<TryAsync<A>, A>,
        Monad<TryAsync<A>, A>,
        Foldable<TryAsync<A>, A>,
        BiFoldable<TryAsync<A>, Unit, A>
    {
        public static readonly MTryAsync<A> Inst = default(MTryAsync<A>);

        static TryAsync<A> none = () => throw new BottomException();

        [Pure]
        public TryAsync<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryAsync<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B>
        {
            if (typeof(MB) == typeof(TryAsync<B>) && typeof(MONADB) == typeof(MTryAsync<B>))
            {
                // HACK: This is a hack to get around the type system for async
                //       A better solution is needed

                var mb = from a in ma
                         from b in (TryAsync<B>)(object)f(a)
                         select b;

                return (MB)(object)mb;
            }
            else
            {
                // Synchronous type-safe version
                return ma.Match(
                    Succ: f,
                    Fail: x => default(MONADB).Fail(x)).Result;
            }
        } 

        [Pure]
        public TryAsync<A> Fail(object err) =>
            TryAsync<A>(() => { throw new BottomException(); });

        [Pure]
        public TryAsync<A> Fail(Exception err = null) =>
            TryAsync<A>(() => { throw err; });

        [Pure]
        public TryAsync<A> Plus(TryAsync<A> ma, TryAsync<A> mb) => async () =>
        {
            // Run in parallel
            var resA = ma.Try();
            var resB = mb.Try();

            for (int i = 0; i < 2; i++)
            {
                // Return first one that completes
                var completed = await Task.WhenAny(resA, resB);
                if (!completed.IsFaulted && !completed.Result.IsFaulted) return completed.Result;
            }
            if (!resA.IsFaulted) return resA.Result;
            if (!resB.IsFaulted) return resB.Result;
            throw new BottomException();
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryAsync<A> Return(Func<Unit, A> f) =>
            () => Task.FromResult(new Result<A>(f(unit)));

        [Pure]
        public TryAsync<A> Zero() => 
            none;

        [Pure]
        public bool IsNone(TryAsync<A> opt) =>
            !IsSome(opt);

        [Pure]
        public bool IsSome(TryAsync<A> opt) =>
            Match(opt, Some: _ => true, None: () => false);

        [Pure]
        public bool IsUnsafe(TryAsync<A> opt) =>
            true;

        [Pure]
        public B Match<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            try
            {
                var res = opt.Try().Result; // TODO: No asynchrony
                if (res.IsFaulted)
                    return None();
                else
                    return Some(res.Value);
            }
            catch
            {
                return None();
            }
        }

        public Unit Match(TryAsync<A> opt, Action<A> Some, Action None) =>
            Match(opt,
                x  => { Some(x); return unit; },
                () => { None(); return unit; });

        [Pure]
        public B MatchUnsafe<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            Match(opt, Some, None);

        [Pure]
        public Func<Unit, S> Fold<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryAsync<A>).Match(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, S> FoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryAsync<A>).Match(ma, x => f(state, x), () => state);

        [Pure]
        public S BiFold<S>(TryAsync<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb) =>
            Match(ma, x => fb(state, x), () => fa(state, unit));

        [Pure]
        public S BiFoldBack<S>(TryAsync<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb) =>
            Match(ma, x => fb(state, x), () => fa(state, unit));

        [Pure]
        public Func<Unit, int> Count(TryAsync<A> ma) => _ =>
            default(MTryAsync<A>).Match(ma, x => 1, () => 0);

        [Pure]
        public TryAsync<A> Some(A value) =>
            Return(_ => value);

        [Pure]
        public TryAsync<A> Optional(A value) =>
            Return(_ => value);

        [Pure]
        public TryAsync<A> Id(Func<Unit, TryAsync<A>> ma) =>
            ma(unit);

        [Pure]
        public TryAsync<A> BindReturn(Unit _, TryAsync<A> mb) =>
            mb;

        [Pure]
        public TryAsync<A> Return(A x) =>
            () => Task.FromResult(new Result<A>(x));
    }
}
