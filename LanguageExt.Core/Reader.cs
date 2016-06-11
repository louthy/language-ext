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

        [Pure]
        public static implicit operator ReaderResult<T>(T value) =>
           new ReaderResult<T>(value);

        [Pure]
        public static implicit operator T(ReaderResult<T> value) =>
           value.Value;

        [Pure]
        public T Value =>
            IsBottom
                ? default(T)
                : value;
    }

    internal static class ReaderResult
    {
        [Pure]
        public static ReaderResult<T> Bottom<T>() => new ReaderResult<T>(default(T), true);

        [Pure]
        public static ReaderResult<T> Return<T>(T value) => new ReaderResult<T>(value, false);
    }

    /// <summary>
    /// Reader monad extensions
    /// </summary>
    public static class ReaderExt
    {
        [Pure]
        public static Reader<Env, IEnumerable<T>> AsEnumerable<Env, T>(this Reader<Env, T> self) =>
            from x in self
            select (new T[1] { x }).AsEnumerable();

        [Pure]
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

        [Pure]
        public static Reader<Env, int> Count<Env, T>(this Reader<Env, T> self) =>
            env => bmap(self(env), x => 1);

        [Pure]
        public static Reader<Env, int> Sum<Env>(this Reader<Env, int> self) =>
            env => bmap(self(env), x => x);

        [Pure]
        public static Reader<Env, bool> ForAll<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => bmap(self(env), x => pred(x));

        [Pure]
        public static Reader<Env, bool> Exists<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => bmap(self(env), x => pred(x));

        [Pure]
        public static Reader<Env, S> Fold<Env, S, T>(this Reader<Env, T> self, S state, Func<S, T, S> folder) =>
            env => bmap(self(env), x => folder(state, x));

        [Pure]
        public static Reader<Env, R> Map<Env, T, R>(this Reader<Env, T> self, Func<T, R> mapper) =>
            env => bmap(self(env), mapper);

        [Pure]
        public static Reader<Env, T> Filter<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            self.Where(pred);

        [Pure]
        private static ReaderResult<R> bmap<T, R>(ReaderResult<T> r, Func<T, R> f) =>
            r.IsBottom
                ? Bottom<R>()
                : Return(f(r.Value));

        [Pure]
        private static ReaderResult<T> Return<T>(T value) =>
            ReaderResult.Return(value);

        [Pure]
        private static ReaderResult<T> Bottom<T>() =>
            ReaderResult.Bottom<T>();

        [Pure]
        private static ReaderResult<Unit> bmap<T>(ReaderResult<T> r, Action<T> f)
        {
            if (r.IsBottom)
            {
                return Bottom<Unit>();
            }
            else
            {
                f(r.Value);
                return Return(unit);
            }
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Reader<Env, T> Where<Env, T>(this Reader<Env, T> self, Func<T, bool> pred)
        {
            return env =>
            {
                var val = self(env);
                return val.IsBottom
                    ? Bottom<T>()
                    : pred(val.Value) 
                        ? Return(val.Value)
                        : Bottom<T>();
            };
        }

        [Pure]
        public static Reader<Env, R> Bind<Env, T, R>(this Reader<Env, T> self, Func<T, Reader<Env, R>> binder) =>
            env =>
            {
                var t = self(env);
                if (t.IsBottom) return Bottom<R>();
                return binder(t.Value)(env);
            };

        /// <summary>
        /// Select
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Reader<E, U> Select<E, T, U>(this Reader<E, T> self, Func<T, U> select)
        {
            if (select == null) throw new ArgumentNullException(nameof(select));
            return (E env) =>
            {
                var resT = self(env);
                return resT.IsBottom
                    ? Bottom<U>()
                    : Return<U>(select(resT.Value));
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Reader<E, V> SelectMany<E, T, U, V>(
            this Reader<E, T> self,
            Func<T, Reader<E, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException(nameof(bind));
            if (project == null) throw new ArgumentNullException(nameof(project));
            return (E env) =>
            {
                var resT = self(env);
                if (resT.IsBottom) return Bottom<V>();
                var resU = bind(resT.Value)(env);
                if (resU.IsBottom) return Bottom<V>();
                return Return(project(resT.Value, resU.Value));
            };
        }

        [Pure]
        public static Reader<Env, Writer<Out, V>> foldT<Env, Out, T, V>(Reader<Env, Writer<Out, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        [Pure]
        public static Reader<Env, State<S, V>> foldT<Env, S, T, V>(Reader<Env, State<S, T>> self, V state, Func<V, T, V> fold) =>
            self.FoldT(state, fold);

        [Pure]
        public static State<S, T> liftT<Env, S, T>(Reader<Env, State<S, T>> self, Env env) where T : struct =>
            self.LiftT(env);

        [Pure]
        public static Writer<Out, T> liftT<Env, Out, T>(Reader<Env, Writer<Out, T>> self, Env env) where T : struct =>
            self.LiftT(env);

        [Pure]
        public static State<S, T> liftUnsafeT<Env, S, T>(Reader<Env, State<S, T>> self, Env env) where T : class =>
            self.LiftUnsafeT(env);

        [Pure]
        public static Writer<Out, T> liftUnsafeT<Env, Out, T>(Reader<Env, Writer<Out, T>> self, Env env) where T : class =>
            self.LiftUnsafeT(env);

        [Pure]
        public static Reader<Env, Writer<Out, V>> FoldT<Env, Out, T, V>(this Reader<Env, Writer<Out, T>> self, V state, Func<V, T, V> fold)
        {
            return (Env env) =>
            {
                var inner = self(env);
                if (inner.IsBottom) return Bottom<Writer<Out, V>>();
                return Return(inner.Value.Fold(state, fold));
            };
        }

        [Pure]
        public static Reader<Env, State<S, V>> FoldT<Env, S, T, V>(this Reader<Env, State<S, T>> self, V state, Func<V, T, V> fold)
        {
            return (Env env) =>
            {
                var inner = self(env);
                if (inner.IsBottom) return Bottom<State<S, V>>();
                return Return(inner.Value.Fold(state, fold));
            };
        }

        [Pure]
        public static State<S, T> LiftT<Env, S, T>(this Reader<Env, State<S, T>> self, Env env) where T : struct
        {
            return state =>
            {
                var inner = self(env);
                if (inner.IsBottom) return StateResult.Bottom<S, T>(state);
                var res = inner.Value(state);
                if (res.IsBottom) return StateResult.Bottom<S, T>(state);
                return StateResult.Return(res.State, res.Value);
            };
        }

        [Pure]
        public static Writer<Out, T> LiftT<Env, Out, T>(this Reader<Env, Writer<Out, T>> self, Env env) where T : struct
        {
            return () =>
            {
                var inner = self(env);
                if (inner.IsBottom) return WriterResult.Bottom<Out, T>();
                var res = inner.Value();
                if (res.IsBottom) return WriterResult.Bottom<Out, T>();
                return WriterResult.Return(res.Value, res.Output);
            };
        }

        [Pure]
        public static State<S, T> LiftUnsafeT<Env, S, T>(this Reader<Env, State<S, T>> self, Env env) where T : class
        {
            return state =>
            {
                var inner = self(env);
                if (inner.IsBottom) return StateResult.Bottom<S, T>(state);
                var res = inner.Value(state);
                if (res.IsBottom) return StateResult.Bottom<S, T>(state);
                return StateResult.Return(res.State, res.Value);
            };
        }

        [Pure]
        public static Writer<Out, T> LiftUnsafeT<Env, Out, T>(this Reader<Env, Writer<Out, T>> self, Env env) where T : class
        {
            return () =>
            {
                var inner = self(env);
                if (inner.IsBottom) return WriterResult.Bottom<Out, T>();
                var res = inner.Value();
                if (res.IsBottom) return WriterResult.Bottom<Out, T>();
                return WriterResult.Return(res.Value, res.Output);
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        [Pure]
        public static Reader<E, Writer<Out, V>> SelectMany<E, Out, T, U, V>(
            this Reader<E, T> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException(nameof(bind));
            if (project == null) throw new ArgumentNullException(nameof(project));
            return (E env) =>
            {
                var resT = self(env);
                if (resT.IsBottom) return Bottom<Writer<Out, V>>();
                return Return<Writer<Out, V>>(() =>
                {
                    var resU = bind(resT.Value)();
                    if (resU.IsBottom) return WriterResult.Bottom<Out, V>(resU.Output);
                    return WriterResult.Return(project(resT.Value, resU.Value), resU.Output);
                });
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        [Pure]
        public static Reader<E, State<S, V>> SelectMany<E, S, T, U, V>(
            this Reader<E, T> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException(nameof(bind));
            if (project == null) throw new ArgumentNullException(nameof(project));
            return (E env) =>
            {
                var resT = self(env);
                if (resT.IsBottom) return Bottom<State<S, V>>();
                return Return<State<S, V>>(state =>
                {
                    var resU = bind(resT.Value)(state);
                    if (resU.IsBottom) return StateResult.Bottom<S, V>(state);
                    return StateResult.Return(resU.State, project(resT.Value, resU.Value));
                });
            };
        }
    }
}