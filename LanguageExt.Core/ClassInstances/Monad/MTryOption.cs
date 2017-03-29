using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryOption<A> :
        Alternative<TryOption<A>, Unit, A>,
        Optional<TryOption<A>, A>,
        Monad<TryOption<A>, A>,
        BiFoldable<TryOption<A>, A, Unit>
    {
        public static readonly MTryOption<A> Inst = default(MTryOption<A>);

        static TryOption<A> none = () => Option<A>.None;

        [Pure]
        public TryOption<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryOption<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B>
        {
            var mr = ma.Try();
            if (mr.IsFaulted) return default(MONADB).Fail(mr.Exception);
            if (mr.Value.IsNone) return default(MONADB).Fail(default(A));
            return f(mr.Value.Value);
        }

        [Pure]
        public TryOption<A> Fail(object err) => 
            none;

        [Pure]
        public TryOption<A> Fail(Exception err = null) => 
            err == null 
                ? TryOption<A>(Option<A>.None)
                : TryOption<A>(err);

        [Pure]
        public TryOption<A> Plus(TryOption<A> ma, TryOption<A> mb) => () =>
        {
            var res = ma.Try();
            if (!res.IsFaulted && res.Value.IsSome) return res.Value;
            return mb();
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryOption<A> Return(Func<Unit, A> f) =>
            () => f(unit);

        [Pure]
        public TryOption<A> Zero() => 
            none;

        [Pure]
        public bool IsNone(TryOption<A> opt) =>
            opt.Match(
                Some: __ => false, 
                None: () => true, 
                Fail: ex => true);

        [Pure]
        public bool IsSome(TryOption<A> opt) =>
            opt.Match(
                Some: __ => true,
                None: () => false,
                Fail: ex => false);

        [Pure]
        public bool IsUnsafe(TryOption<A> opt) =>
            false;

        [Pure]
        public B Match<B>(TryOption<A> opt, Func<A, B> Some, Func<B> None)
        {
            var res = opt.Try();
            if (res.IsFaulted || res.Value.IsNone)
                return None();
            else
                return Some(res.Value.Value);
        }

        public Unit Match(TryOption<A> opt, Action<A> Some, Action None)
        {
            var res = opt.Try();
            if (res.IsFaulted || res.Value.IsNone) None(); else Some(res.Value.Value);
            return unit;
        }

        [Pure]
        public B MatchUnsafe<B>(TryOption<A> opt, Func<A, B> Some, Func<B> None)
        {
            var res = opt.Try();
            if (res.IsFaulted || res.Value.IsNone)
                return None();
            else
                return Some(res.Value.Value);
        }

        [Pure]
        public Func<Unit, S> Fold<S>(TryOption<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            var res = ma.Try();
            if (res.IsFaulted || res.Value.IsNone) return state;
            return f(state, res.Value.Value);
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(TryOption<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            var res = ma.Try();
            if (res.IsFaulted || res.Value.IsNone) return state;
            return f(state, res.Value.Value);
        };

        [Pure]
        public S BiFold<S>(TryOption<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            var res = ma.Try();
            if (res.IsFaulted || res.Value.IsNone)
                return fb(state, unit);
            else
                return fa(state, res.Value.Value);
        }

        [Pure]
        public S BiFoldBack<S>(TryOption<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            var res = ma.Try();
            if (res.IsFaulted || res.Value.IsNone)
                return fb(state, unit);
            else
                return fa(state, res.Value.Value);
        }

        [Pure]
        public Func<Unit, int> Count(TryOption<A> ma) => _ =>
            ma.Try().Value.IsSome
                ? 1
                : 0;

        [Pure]
        public TryOption<A> Some(A value) =>
            Return(value);

        [Pure]
        public TryOption<A> Optional(A value) =>
            Return(value);

        [Pure]
        public TryOption<A> Id(Func<Unit, TryOption<A>> ma) =>
            ma(unit);

        [Pure]
        public TryOption<A> BindReturn(Unit _, TryOption<A> mb) =>
            mb;

        [Pure]
        public TryOption<A> Return(A x) =>
            () => x;

        [Pure]
        public TryOption<A> IdAsync(Func<Unit, Task<TryOption<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOption<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(fa.Map(a => f(state, a)).IfNoneOrFail(state));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOption<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.Map(a => f(state, a)).IfNoneOrFail(Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOption<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(fa.Map(a => f(state, a)).IfNoneOrFail(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOption<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.Map(a => f(state, a)).IfNoneOrFail(Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<int>> CountAsync(TryOption<A> fa) => _ =>
            Task.FromResult(fa.Map(a => 1).IfNoneOrFail(0));

        [Pure]
        public TryOption<A> Empty() =>
            none;

        [Pure]
        public TryOption<A> Append(TryOption<A> x, TryOption<A> y) =>
            Plus(x, y);
    }
}
