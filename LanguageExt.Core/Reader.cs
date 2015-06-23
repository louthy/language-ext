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
        public readonly T Value;
        public readonly bool IsBottom;

        internal ReaderResult(T value, bool isBottom = false)
        {
            Value = value;
            IsBottom = isBottom;
        }

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
            yield return self(env).Value;
        }

        public static Reader<Env,Unit> Iter<Env, T>(this Reader<Env, T> self, Action<T> action)
        {
            return env =>
            {
                action(self(env).Value);
                return new ReaderResult<Unit>(unit);
            };
        }

        public static int Count<Env, T>(this Reader<Env, T> self) => 
            1;

        public static Reader<Env,int> Sum<Env>(this Reader<Env, int> self) =>
            env => self(env);

        public static Reader<Env, bool> ForAll<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => new ReaderResult<bool>(pred(self(env).Value));

        public static Reader<Env,bool> Exists<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => new ReaderResult<bool>(pred(self(env).Value));

        public static Reader<Env, S> Fold<Env, S, T>(this Reader<Env, T> self, S state, Func<S, T, S> folder) =>
            env => new ReaderResult<S>(folder(state, self(env).Value));

        public static Reader<Env, R> Map<Env, T, R>(this Reader<Env, T> self, Func<T, R> mapper) =>
            env => new ReaderResult<R>(mapper(self(env).Value));

        public static Reader<Env, T> Filter<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => failwith<ReaderResult<T>>("Reader doesn't support Where or Filter");

        public static Reader<Env, T> Where<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => failwith<ReaderResult<T>>("Reader doesn't support Where or Filter");

        public static Reader<Env, R> Bind<Env, T, R>(this Reader<Env, T> self, Func<T, Reader<Env, R>> binder) =>
            env => new ReaderResult<R>(binder(self(env).Value)(env).Value);

        /// <summary>
        /// Select
        /// </summary>
        public static Reader<E, U> Select<E, T, U>(this Reader<E, T> self, Func<T, U> select)
        {
            if (select == null) throw new ArgumentNullException("select");
            return (E env) => new ReaderResult<U>(select(self(env).Value));
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
                var resT = self(env).Value;
                var resU = bind(resT);
                var resV = project(resT, resU(env).Value);
                return new ReaderResult<V>(resV);
            };
        }
    }
}
