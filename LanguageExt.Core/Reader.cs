using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// The reader monad
    /// Allows for an 'environment' value to be carried through bind functions
    /// </summary>
    /// <typeparam name="Env">Environment</typeparam>
    /// <typeparam name="T">The wrapped type</typeparam>
    public delegate ReaderResult<T> Reader<Env, T>(Env environment);

    /// <summary>
    /// State result.
    /// </summary>
    public struct ReaderResult<T>
    {
        readonly T value;
        public readonly bool IsBottom;

        internal ReaderResult(T value, bool isBottom = false)
        {
            this.value = value;
            IsBottom = isBottom;
        }

        public T Value =>
            IsBottom
                ? default(T)
                : value;

        public static implicit operator ReaderResult<T>(T value) =>
           new ReaderResult<T>(value);

        public static implicit operator T(ReaderResult<T> value) =>
           value.Value;
    }

    /// <summary>
    /// Reader monad extensions
    /// </summary>
    public static class ReaderExt
    {
        public static Reader<Env, IEnumerable<T>> AsEnumerable<Env, T>(this Reader<Env, T> self) =>
            from x in self
            select (new T[1] { x }).AsEnumerable();

        public static IEnumerable<T> AsEnumerable<Env, T>(this Reader<Env, T> self, Env env)
        {
            var res = self(env);
            if (!res.IsBottom)
            {
                yield return res.Value;
            }
        }

        public static Reader<Env, Unit> Iter<Env, T>(this Reader<Env, T> self, Action<T> action) =>
            env => bmap(self(env), x => action(x));

        public static Reader<Env, int> Count<Env, T>(this Reader<Env, T> self) =>
            env => bmap(self(env), x => 1);

        public static Reader<Env, int> Sum<Env>(this Reader<Env, int> self) =>
            env => bmap(self(env), x => x);

        public static Reader<Env, bool> ForAll<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => bmap(self(env), x => pred(x));

        public static Reader<Env, bool> Exists<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => bmap(self(env), x => pred(x));

        public static Reader<Env, S> Fold<Env, S, T>(this Reader<Env, T> self, S state, Func<S, T, S> folder) =>
            env => bmap(self(env), x => folder(state, x));

        public static Reader<Env, R> Map<Env, T, R>(this Reader<Env, T> self, Func<T, R> mapper) =>
            env => new ReaderResult<R>(bmap(self(env), mapper));

        public static Reader<Env, T> Filter<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            self.Where(pred);

        private static ReaderResult<R> bmap<T, R>(ReaderResult<T> r, Func<T, R> f) =>
            r.IsBottom
                ? new ReaderResult<R>(default(R), true)
                : new ReaderResult<R>(f(r.Value), false);

        private static ReaderResult<Unit> bmap<T>(ReaderResult<T> r, Action<T> f)
        {
            if (r.IsBottom)
            {
                return new ReaderResult<Unit>(unit, true);
            }
            else
            {
                f(r.Value);
                return new ReaderResult<Unit>(unit, false);
            }
        }

        public static Reader<Env, T> Where<Env, T>(this Reader<Env, T> self, Func<T, bool> pred)
        {
            return env =>
            {
                var val = self(env);
                return new ReaderResult<T>(val, !pred(val));
            };
        }

        public static Reader<Env, R> Bind<Env, T, R>(this Reader<Env, T> self, Func<T, Reader<Env, R>> binder) =>
            env => self.Select(t => binder(t)(env))(env);

        /// <summary>
        /// Select
        /// </summary>
        public static Reader<E, U> Select<E, T, U>(this Reader<E, T> self, Func<T, U> select)
        {
            if (select == null) throw new ArgumentNullException("select");
            return (E env) =>
            {
                var resT = self(env);
                return resT.IsBottom
                    ? new ReaderResult<U>(default(U), true)
                    : new ReaderResult<U>(select(resT.Value));
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        public static Reader<E, V> SelectMany<E, T, U, V>(
            this Reader<E, T> self,
            Func<T, Reader<E, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");
            return (E env) =>
            {
                var resT = self(env);
                if (resT.IsBottom) return new ReaderResult<V>(default(V), true);
                var resU = bind(resT.Value)(env);
                if (resU.IsBottom) return new ReaderResult<V>(default(V), true);
                var resV = project(resT, resU.Value);
                return new ReaderResult<V>(resV);
            };
        }

        public static Reader<Env, V> foldT<Env, T, V>(Reader<Env, Reader<Env, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        public static Reader<Env, Writer<Out, V>> foldT<Env, Out, T, V>(Reader<Env, Writer<Out, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        public static Reader<Env, State<S, V>> foldT<Env, S, T, V>(Reader<Env, State<S, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        public static Reader<Env, V> FoldT<Env, T, V>(this Reader<Env, Reader<Env, T>> self, V state, Func<V, T, V> fold)
        {
            return (Env env) =>
            {
                var inner = self(env);
                if (inner.IsBottom) return new ReaderResult<V>(default(V), true);
                return inner.Value.Fold(state, fold)(env);
            };
        }

        public static Reader<Env, Writer<Out, V>> FoldT<Env, Out, T, V>(this Reader<Env, Writer<Out, T>> self, V state, Func<V, T, V> fold)
        {
            return (Env outerArgs) =>
            {
                var inner = self(outerArgs);
                if (inner.IsBottom) return new ReaderResult<Writer<Out, V>>(default(Writer<Out, V>), true);
                return new ReaderResult<Writer<Out, V>>(inner.Value.Fold(state, fold));
            };
        }

        public static Reader<Env, State<S, V>> FoldT<Env, S, T, V>(this Reader<Env, State<S, T>> self, V state, Func<V, T, V> fold)
        {
            return (Env outerArgs) =>
            {
                var inner = self(outerArgs);
                if (inner.IsBottom) return new ReaderResult<State<S, V>>(default(State<S, V>), true);
                return new ReaderResult<State<S, V>>(inner.Value.Fold(state, fold));
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        public static Reader<E, Writer<Out, V>> SelectMany<E, Out, T, U, V>(
            this Reader<E, T> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");
            return (E env) =>
            {
                var resT = self(env);
                if (resT.IsBottom) return new ReaderResult<Writer<Out, V>>(default(Writer<Out, V>), true);
                return new ReaderResult<Writer<Out, V>>(() =>
                {
                    var resU = bind(resT.Value)();
                    if (resU.IsBottom) return new WriterResult<Out, V>(default(V), resU.Output, true);
                    return project(resT, resU.Value);
                });
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        public static Reader<E, State<S, V>> SelectMany<E, S, T, U, V>(
            this Reader<E, T> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");
            return (E env) =>
            {
                var resT = self(env);
                if (resT.IsBottom) return new ReaderResult<State<S, V>>(default(State<S, V>), true);
                return new ReaderResult<State<S, V>>(state =>
                {
                    var resU = bind(resT.Value)(state);
                    if (resU.IsBottom) return new StateResult<S, V>(state, default(V), true);
                    return project(resT, resU.Value);
                });
            };
        }
    }
}