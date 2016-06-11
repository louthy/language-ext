using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    public delegate StateResult<S, T> State<S, T>(S state);

    /// <summary>
    /// State result.
    /// </summary>
    public struct StateResult<S, T>
    {
        readonly T value;
        public readonly S State;
        public readonly bool IsBottom;

        internal StateResult(S state, T value, bool isBottom = false)
        {
            this.value = value;
            State = state;
            IsBottom = isBottom;
        }

        [Pure]
        public T Value =>
            IsBottom
                ? default(T)
                : value;

        [Pure]
        public static implicit operator StateResult<S, T>(T value) =>
           new StateResult<S, T>(default(S), value);        // TODO:  Not a good idea

        [Pure]
        public static implicit operator T(StateResult<S, T> value) =>
           value.Value;
    }

    public static class StateResult
    {
        [Pure]
        public static StateResult<S, T> Bottom<S, T>(S state) =>
            new StateResult<S, T>(state, default(T), true);

        [Pure]
        public static StateResult<S, T> Return<S, T>(S state, T value) =>
            new StateResult<S, T>(state, value, false);
    }

    public static class StateExt
    {
        [Pure]
        public static State<S, IEnumerable<T>> AsEnumerable<S, T>(this State<S, T> self) =>
            from x in self
            select (new T[1] { x }).AsEnumerable();

        [Pure]
        public static IEnumerable<T> AsEnumerable<S, T>(this State<S, T> self, S state)
        {
            var res = self(state);
            if (!res.IsBottom)
            {
                yield return self(state).Value;
            }
        }

        public static State<S, Unit> Iter<S, T>(this State<S, T> self, Action<T> action) =>
            s => bmap(self(s), action);

        [Pure]
        public static State<S, int> Count<S, T>(this State<S, T> self) =>
            s =>
            {
                var res = self(s);
                return res.IsBottom
                  ? StateResult.Bottom<S, int>(s)
                  : StateResult.Return(res.State, 1);
            };

        [Pure]
        public static State<S, bool> ForAll<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        [Pure]
        public static State<S, bool> Exists<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        [Pure]
        public static State<S, FState> Fold<S, T, FState>(this State<S, T> self, FState state, Func<FState, T, FState> folder) =>
            s => bmap(self(s), x => folder(state, x));

        [Pure]
        public static State<S, S> Fold<S, T>(this State<S, T> self, Func<S, T, S> folder) =>
            s => bmap(self(s), x => folder(s, x));

        [Pure]
        public static State<S, R> Map<S, T, R>(this State<S, T> self, Func<T, R> mapper) =>
            self.Select(mapper);

        [Pure]
        public static State<S, T> Modify<S, T>(this State<S, T> self, Func<S, S> f)
        {
            if (f == null) throw new ArgumentNullException(nameof(map));
            return (S state) =>
            {
                var resT = self(state);
                return resT.IsBottom
                    ? StateResult.Bottom<S, T>(state)
                    : StateResult.Return(f(resT.State), resT.Value);
            };
        }

        [Pure]
        public static State<S, R> Bind<S, T, R>(this State<S, T> self, Func<T, State<S, R>> binder)
        {
            return state =>
            {
                var resT = self(state);
                if( resT.IsBottom )
                {
                    return StateResult.Bottom<S, R>(state);
                }
                return binder(resT.Value)(resT.State);
            };
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static State<S, U> Select<S, T, U>(this State<S, T> self, Func<T, U> map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            return (S state) =>
            {
                var resT = self(state);
                return resT.IsBottom
                    ? StateResult.Bottom<S, U>(state)
                    : StateResult.Return(resT.State, map(resT.Value));
            };
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static State<S, V> SelectMany<S, T, U, V>(
            this State<S, T> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException(nameof(bind));
            if (project == null) throw new ArgumentNullException(nameof(project));

            return (S state) =>
            {
                var resT = self(state);
                if (resT.IsBottom) return StateResult.Bottom<S, V>(state);
                var resU = bind(resT.Value)(resT.State);
                if (resU.IsBottom) return StateResult.Bottom<S, V>(resT.State);
                var resV = project(resT.Value, resU.Value);
                return StateResult.Return(resU.State, resV);
            };
        }

        [Pure]
        public static State<S, T> Filter<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            where pred(x)
            select x;

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static State<S, T> Where<S, T>(this State<S, T> self, Func<T, bool> pred)
        {
            return state =>
            {
                var res = self(state);
                return pred(res.Value)
                    ? StateResult.Return(res.State, res.Value)
                    : StateResult.Bottom<S, T>(state);
            };
        }

        [Pure]
        public static State<S, int> Sum<S>(this State<S, int> self) =>
            state => bmap(self(state), x => x);

        [Pure]
        static StateResult<S, R> bmap<S, T, R>(StateResult<S, T> r, Func<T, R> f) =>
            r.IsBottom
                ? StateResult.Bottom<S, R>(r.State)
                : StateResult.Return(r.State, f(r.Value));

        [Pure]
        static StateResult<S, Unit> bmap<S, T>(StateResult<S, T> r, Action<T> f)
        {
            if (r.IsBottom)
            {
                return StateResult.Bottom<S, Unit>(r.State);
            }
            else
            {
                f(r.Value);
                return StateResult.Return(r.State, unit);
            }
        }

        [Pure]
        public static State<S, Reader<Env, V>> foldT<S, Env, T, V>(State<S, Reader<Env, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        [Pure]
        public static State<S, Writer<Out, V>> foldT<S, Out, T, V>(State<S, Writer<Out, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        [Pure]
        public static State<S, Reader<Env, V>> FoldT<S, Env, T, V>(this State<S, Reader<Env, T>> self, V state, Func<V, T, V> fold)
        {
            return (S s) =>
            {
                var inner = self(s);
                if (inner.IsBottom) return StateResult.Bottom<S, Reader<Env, V>>(s);
                return StateResult.Return(inner.State, inner.Value.Fold(state, fold));
            };
        }

        [Pure]
        public static State<S, Writer<Out, V>> FoldT<S, Out, T, V>(this State<S, Writer<Out, T>> self, V state, Func<V, T, V> fold)
        {
            return (S s) =>
            {
                var inner = self(s);
                if (inner.IsBottom) return StateResult.Bottom<S, Writer<Out, V>>(s);
                return StateResult.Return(inner.State, inner.Value.Fold(state, fold));
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static State<S, Reader<E, V>> SelectMany<S, E, T, U, V>(
            this State<S, T> self,
            Func<T, Reader<E, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException(nameof(bind));
            if (project == null) throw new ArgumentNullException(nameof(project));
            return (S s) =>
            {
                var resT = self(s);
                if (resT.IsBottom) return StateResult.Bottom<S, Reader<E, V>>(s);
                return StateResult.Return<S, Reader<E, V>>(resT.State, envInner =>
                {
                    var resU = bind(resT.Value)(envInner);
                    if (resU.IsBottom) return new ReaderResult<V>(default(V), true);
                    return ReaderResult.Return(project(resT.Value, resU.Value));
                });
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static State<S, Writer<Out, V>> SelectMany<S, Out, T, U, V>(
            this State<S, T> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException(nameof(bind));
            if (project == null) throw new ArgumentNullException(nameof(project));
            return (S s) =>
            {
                var resT = self(s);
                if (resT.IsBottom) return StateResult.Bottom<S, Writer<Out, V>>(s);
                return StateResult.Return<S, Writer<Out, V>>(resT.State, () =>
                {
                    var resU = bind(resT.Value)();
                    if (resU.IsBottom) return new WriterResult<Out, V>(default(V), resU.Output, true);
                    return WriterResult.Return(project(resT.Value, resU.Value),resU.Output);
                });
            };
        }
    }
}
