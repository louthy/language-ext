using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryOptionAsync<A> :
        Alternative<TryOptionAsync<A>, Unit, A>,
        Optional<TryOptionAsync<A>, A>,
        OptionalAsync<TryOptionAsync<A>, A>,
        Monad<TryOptionAsync<A>, A>,
        BiFoldable<TryOptionAsync<A>, A, Unit>,
        BiFoldableAsync<TryOptionAsync<A>, A, Unit>
    {
        public static readonly MTryOptionAsync<A> Inst = default(MTryOptionAsync<A>);

        static TryOptionAsync<A> none = () => throw new BottomException();

        [Pure]
        public TryOptionAsync<A> None => none;

        [Pure]
        public TryOptionAsync<A> NoneAsync => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            default(MONADB).IdAsync(_ => ma.Try().ContinueWith(task =>
            {
                try
                {
                    return task.IsFaulted
                        ? default(MONADB).Fail(task.Exception)
                        : task.IsCanceled
                            ? default(MONADB).Fail()
                            : task.Result.IsFaulted
                                ? default(MONADB).Fail(task.Result.Exception)
                                : task.Result.Value.IsNone || task.Result.IsBottom
                                    ? default(MONADB).Fail()
                                    : f(task.Result.Value.Value);
                }
                catch(Exception e)
                {
                    return default(MONADB).Fail(e);
                }
            }));

        [Pure]
        public TryOptionAsync<A> Fail(object err) =>
            TryOptionAsync<A>(Option<A>.None);

        [Pure]
        public TryOptionAsync<A> Fail(Exception err = null) =>
            err == null
                ? TryOptionAsync<A>(Option<A>.None)
                : TryOptionAsync<A>(err);

        [Pure]
        public TryOptionAsync<A> Plus(TryOptionAsync<A> ma, TryOptionAsync<A> mb) => async () =>
        {
            // Run in parallel
            var resA = ma.Try();
            var resB = mb.Try();

            var tasks = Set<OrdTask<OptionalResult<A>>, Task<OptionalResult<A>>>(resA, resB);

            while (tasks.Count > 0)
            {
                // Return first one that completes
                var completed = await Task.WhenAny(tasks);
                if (!completed.IsFaulted && !completed.Result.IsFaulted) return completed.Result;
                tasks = tasks.Remove(completed);
            }
            if (!resA.IsFaulted) return resA.Result;
            if (!resB.IsFaulted) return resB.Result;
            return OptionalResult<A>.None;
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryOptionAsync<A> Return(Func<Unit, A> f) =>
            () => Task.FromResult(new OptionalResult<A>(f(unit)));

        [Pure]
        public TryOptionAsync<A> Zero() =>
            none;

        [Pure]
        public bool IsNone(TryOptionAsync<A> opt) =>
            !IsSome(opt);

        [Pure]
        public bool IsSome(TryOptionAsync<A> opt) =>
            Match(opt, Some: _ => true, None: () => false);

        [Pure]
        public bool IsUnsafe(TryOptionAsync<A> opt) =>
            true;

        [Pure]
        public B Match<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            try
            {
                var res = opt.Try().Result; // TODO: No asynchrony
                if (res.IsFaultedOrNone)
                    return None();
                else
                    return Some(res.Value.Value);
            }
            catch
            {
                return None();
            }
        }

        public Unit Match(TryOptionAsync<A> opt, Action<A> Some, Action None) =>
            Match(opt,
                x => { Some(x); return unit; },
                () => { None(); return unit; });

        [Pure]
        public B MatchUnsafe<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            Match(opt, Some, None);

        [Pure]
        public Func<Unit, S> Fold<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            Inst.Match(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, S> FoldBack<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            Inst.Match(ma, x => f(state, x), () => state);

        [Pure]
        public S BiFold<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            Match(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public S BiFoldBack<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            Match(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Func<Unit, int> Count(TryOptionAsync<A> ma) => _ =>
            Inst.Match(ma, x => 1, () => 0);

        [Pure]
        public TryOptionAsync<A> Some(A value) =>
            Return(_ => value);

        [Pure]
        public TryOptionAsync<A> Optional(A value) =>
            Return(_ => value);

        [Pure]
        public TryOptionAsync<A> Id(Func<Unit, TryOptionAsync<A>> ma) =>
            ma(unit);

        [Pure]
        public TryOptionAsync<A> IdAsync(Func<Unit, Task<TryOptionAsync<A>>> ma) =>
            new TryOptionAsync<A>(() =>
            {
                try
                {
                    return from a in ma(unit)
                           let b = a()
                           from c in b
                           select c;
                }
                catch (Exception e)
                {
                    return Task.FromResult(new OptionalResult<A>(e));
                }
            });

        [Pure]
        public TryOptionAsync<A> BindReturn(Unit _, TryOptionAsync<A> mb) =>
            mb;

        [Pure]
        public TryOptionAsync<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<int>> CountAsync(TryOptionAsync<A> ma) => _ =>
            ma.Map(a => 1).IfNoneOrFail(0);

        [Pure]
        public TryOptionAsync<A> Empty() =>
            none;

        [Pure]
        public TryOptionAsync<A> Append(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            Plus(x, y);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        public Task<bool> IsUnsafeAsync(TryOptionAsync<A> opt) =>
            Task.FromResult(default(MTryOptionAsync<A>).IsUnsafe(opt));

        public Task<bool> IsSomeAsync(TryOptionAsync<A> opt) =>
            opt.Map(x => true).IfNoneOrFail(false);

        public Task<bool> IsNoneAsync(TryOptionAsync<A> opt) =>
            opt.Map(x => false).IfNoneOrFail(true);

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _  => None());

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Action<A> Some, Action None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _  => None());

        public TryOptionAsync<A> SomeAsync(A value) =>
            TryOptionAsync(value);

        public TryOptionAsync<A> OptionalAsync(A value) =>
            TryOptionAsync(value);
    }
}
