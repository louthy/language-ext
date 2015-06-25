using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Reader Writer State monad
    /// </summary>
    /// <typeparam name="R">Reader type</typeparam>
    /// <typeparam name="W">Writer type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    public delegate RwsResult<W, S, T> Rws<R, W, S, T>(Tuple<R,S> rws);

    /// <summary>
    /// RWS result.
    /// </summary>
    public struct RwsResult<W, S, T>
    {
        readonly T value;
        public readonly IEnumerable<W> Output;
        public readonly S State;
        public readonly bool IsBottom;

        internal RwsResult(IEnumerable<W> output, S state, T value, bool isBottom = false)
        {
            this.value = value;
            Output = output;
            State = state;
            IsBottom = isBottom;
        }

        public T Value =>
            IsBottom
                ? default(T)
                : value;

        public static implicit operator RwsResult<W,S,T>(T value) =>
           new RwsResult<W,S,T>(new W[0], default(S),value);        // TODO:  Not a good idea

        public static implicit operator T(RwsResult<W,S,T> value) =>
           value.Value;
    }

    public static class RwsExt
    {
        public static Rws<R, W, S, IEnumerable<T>> AsEnumerable<R, W, S, T>(this Rws<R, W, S, T> self) =>
            from x in self
            select (new T[1] { x }).AsEnumerable();

        public static IEnumerable<T> AsEnumerable<R, W, S, T>(this Rws<R, W, S, T> self, R env, S state)
        {
            var res = self(Tuple(env,state));
            if (!res.IsBottom)
            {
                yield return self(Tuple(env,res.State)).Value;
            }
        }

        public static Rws<R, W, S, Unit> Iter<R, W, S, T>(this Rws<R, W, S, T> self, Action<T> action) =>
            s => bmap(self(s), action);

        public static Rws<R, W, S, int> Count<R, W, S, T>(this Rws<R, W, S, T> self) =>
            s => self(s).IsBottom
                ? 0
                : 1;

        public static Rws<R, W, S, bool> ForAll<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        public static Rws<R, W, S, bool> Exists<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        public static Rws<R, W, S, FState> Fold<R, W, S, T, FState>(this Rws<R, W, S, T> self, FState state, Func<FState, T, FState> folder) =>
            s => bmap(self(s), x => folder(state, x));

        public static Rws<R, W, S, S> Fold<R, W, S, T>(this Rws<R, W, S, T> self, Func<S, T, S> folder) =>
            s => bmap(self(s), x => folder(s.Item2, x));

        public static Rws<R, W, S, Ret> Map<R, W, S, T, Ret>(this Rws<R, W, S, T> self, Func<T, Ret> mapper) =>
            self.Select(mapper);

        public static Rws<R, W, S, Ret> Bind<R, W, S, T, Ret>(this Rws<R, W, S, T> self, Func<T, Rws<R, W, S, Ret>> binder)
        {
            return state =>
            {
                var resT = self(state);
                if( resT.IsBottom )
                {
                    return new RwsResult<W, S, Ret>(resT.Output, resT.State, default(Ret), true);
                }
                return binder(resT.Value)(Tuple(state.Item1,resT.State));
            };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Rws<R, W, S, U> Select<R, W, S, T, U>(this Rws<R, W, S, T> self, Func<T, U> map)
        {
            if (map == null) throw new ArgumentNullException("map");
            return state =>
            {
                var resT = self(state);
                return resT.IsBottom
                    ? new RwsResult<W, S, U>(resT.Output, resT.State, default(U), true)
                    : new RwsResult<W, S, U>(resT.Output, resT.State, map(resT.Value));
            };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Rws<R, W, S, V> SelectMany<R, W, S, T, U, V>(
            this Rws<R, W, S, T> self,
            Func<T, Rws<R, W, S, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return state =>
            {
                var resT = self(state);
                if (resT.IsBottom) return new RwsResult<W, S, V>(resT.Output, resT.State, default(V), true);
                var resU = bind(resT.Value)(Tuple(state.Item1, resT.State));
                if (resU.IsBottom) return new RwsResult<W, S, V>(resU.Output, resU.State, default(V), true);
                var resV = project(resT.Value, resU.Value);
                return new RwsResult<W, S, V>(resT.Output.Concat(resU.Output), resU.State, resV);
            };
        }

        public static Rws<R, W, S, T> Filter<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
            from x in self
            where pred(x)
            select x;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Rws<R, W, S, T> Where<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred)
        {
            return state =>
            {
                var res = self(state);
                return new RwsResult<W, S, T>(res.Output, res.State, res.Value, !pred(res.Value));
            };
        }

        public static Rws<R, W, S, int> Sum<R, W, S>(this Rws<R, W, S, int> self) =>
            state => bmap(self(state), x => x);

        private static RwsResult<W, S, Ret> bmap<W, S, T, Ret>(RwsResult<W, S, T> r, Func<T, Ret> f) =>
            r.IsBottom
                ? new RwsResult<W, S, Ret>(r.Output, r.State, default(Ret), true)
                : new RwsResult<W, S, Ret>(r.Output, r.State, f(r.Value), false);

        private static RwsResult<W, S, Unit> bmap<W, S, T>(RwsResult<W, S, T> r, Action<T> f)
        {
            if (r.IsBottom)
            {
                return new RwsResult<W, S, Unit>(r.Output, r.State, unit, true);
            }
            else
            {
                f(r.Value);
                return new RwsResult<W, S, Unit>(r.Output, r.State, unit, false);
            }
        }

        /*
        public static State<S, Reader<Env, V>> foldT<S, Env, T, V>(State<S, Reader<Env, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        public static State<S, Writer<Out, V>> foldT<S, Out, T, V>(State<S, Writer<Out, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        public static State<S, V> foldT<S, T, V>(State<S, State<S, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        public static State<S, Reader<Env, V>> FoldT<S, Env, T, V>(this State<S, Reader<Env, T>> self, V state, Func<V, T, V> fold)
        {
            return (S s) =>
            {
                var inner = self(s);
                if (inner.IsBottom) return new StateResult<S, Reader<Env, V>>(s, default(Reader<Env, V>), true);
                return new StateResult<S, Reader<Env, V>>(inner.State, inner.Value.Fold(state, fold));
            };
        }

        public static State<S, Writer<Out, V>> FoldT<S, Out, T, V>(this State<S, Writer<Out, T>> self, V state, Func<V, T, V> fold)
        {
            return (S s) =>
            {
                var inner = self(s);
                if (inner.IsBottom) return new StateResult<S, Writer<Out, V>>(s, default(Writer<Out, V>), true);
                return new StateResult<S, Writer<Out, V>>(inner.State, inner.Value.Fold(state, fold));
            };
        }

        public static State<S, V> FoldT<S, T, V>(this State<S, State<S, T>> self, V state, Func<V, T, V> fold)
        {
            return (S s) =>
            {
                var inner = self(s);
                if (inner.IsBottom) return new StateResult<S, V>(s, default(V), true);
                return inner.Value.Fold(state, fold)(s);
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        public static State<S, Reader<E, V>> SelectMany<S, E, T, U, V>(
            this State<S, T> self,
            Func<T, Reader<E, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");
            return (S s) =>
            {
                var resT = self(s);
                if (resT.IsBottom) return new StateResult<S, Reader<E, V>>(resT.State, default(Reader<E, V>), true);
                return new StateResult<S, Reader<E, V>>(resT.State, envInner =>
                {
                    var resU = bind(resT.Value)(envInner);
                    if (resU.IsBottom) return new ReaderResult<V>(default(V), true);
                    return project(resT, resU.Value);
                });
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        public static State<S, Writer<Out, V>> SelectMany<S, Out, T, U, V>(
            this State<S, T> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");
            return (S s) =>
            {
                var resT = self(s);
                if (resT.IsBottom) return new StateResult<S, Writer<Out, V>>(s, default(Writer<Out, V>), true);
                return new StateResult<S, Writer<Out, V>>(s, () =>
                {
                    var resU = bind(resT.Value)();
                    if (resU.IsBottom) return new WriterResult<Out, V>(default(V), resU.Output, true);
                    return project(resT, resU.Value);
                });
            };
        }*/
    }
}
