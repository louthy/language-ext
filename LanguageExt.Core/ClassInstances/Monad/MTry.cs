using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTry<A> :
        Alternative<Try<A>, Unit, A>,
        Optional<Try<A>, A>,
        Monad<Try<A>, A>,
        BiFoldable<Try<A>, A, Unit>
    {
        public static readonly MTry<A> Inst = default(MTry<A>);

        static Try<A> none = () => new Result<A>(BottomException.Default);

        [Pure]
        public Try<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(Try<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B>
        {
            var mr = ma.Try();
            if (mr.IsFaulted) return default(MONADB).Fail(mr.Exception);
            return f(mr.Value);
        }

        [Pure]
        public Try<A> Fail(object err) =>
            Try<A>(BottomException.Default);

        [Pure]
        public Try<A> Fail(Exception err = null) =>
            Try<A>(err ?? BottomException.Default);

        [Pure]
        public Try<A> Plus(Try<A> ma, Try<A> mb) => () =>
        {
            var res = ma.Try();
            if (!res.IsFaulted) return res.Value;
            return mb().Value;
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Try<A> Return(Func<Unit, A> f) =>
            () => f(unit);

        [Pure]
        public Try<A> Zero() => 
            none;

        [Pure]
        public bool IsNone(Try<A> opt) =>
            opt.Match(
                Succ: __ => false, 
                Fail: ex => true);

        [Pure]
        public bool IsSome(Try<A> opt) =>
            opt.Match(
                Succ: __ => true,
                Fail: ex => false);

        [Pure]
        public bool IsUnsafe(Try<A> opt) =>
            false;

        [Pure]
        public B Match<B>(Try<A> opt, Func<A, B> Some, Func<B> None)
        {
            var res = opt.Try();
            if (res.IsFaulted)
                return None();
            else
                return Some(res.Value);
        }

        public Unit Match(Try<A> opt, Action<A> Some, Action None)
        {
            var res = opt.Try();
            if (res.IsFaulted) None(); else Some(res.Value);
            return unit;
        }

        [Pure]
        public B MatchUnsafe<B>(Try<A> opt, Func<A, B> Some, Func<B> None)
        {
            var res = opt.Try();
            if (res.IsFaulted)
                return None();
            else
                return Some(res.Value);
        }

        [Pure]
        public Func<Unit, S> Fold<S>(Try<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            var res = ma.Try();
            if (res.IsFaulted) return state;
            return f(state, res.Value);
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Try<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            var res = ma.Try();
            if (res.IsFaulted) return state;
            return f(state, res.Value);
        };

        [Pure]
        public S BiFold<S>(Try<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            var res = ma.Try();
            if (res.IsFaulted)
                return fb(state, unit);
            else
                return fa(state, res.Value);
        }

        [Pure]
        public S BiFoldBack<S>(Try<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            var res = ma.Try();
            if (res.IsFaulted)
                return fb(state, unit);
            else
                return fa(state, res.Value);
        }

        [Pure]
        public Func<Unit, int> Count(Try<A> ma) => _ =>
            ma.Try().IsFaulted
                ? 0
                : 1;

        [Pure]
        public Try<A> Some(A value) =>
            Return(value);

        [Pure]
        public Try<A> Optional(A value) =>
            Return(value);

        [Pure]
        public Try<A> Id(Func<Unit, Try<A>> ma) =>
            ma(unit);

        [Pure]
        public Try<A> BindReturn(Unit _, Try<A> mb) =>
            mb;

        [Pure]
        public Try<A> Return(A x) =>
            () => x;

        [Pure]
        public Try<A> IdAsync(Func<Unit, Task<Try<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Try<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(fa.Map(a => f(state, a)).IfFail(state));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Try<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.Map(a => f(state, a)).IfFail(Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Try<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(fa.Map(a => f(state, a)).IfFail(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Try<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.Map(a => f(state, a)).IfFail(Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<int>> CountAsync(Try<A> fa) => _ =>
            Task.FromResult(fa.Map(a => 1).IfFail(0));

        [Pure]
        public Try<A> Empty() =>
            none;

        [Pure]
        public Try<A> Append(Try<A> x, Try<A> y) =>
            Plus(x, y);
    }
}
