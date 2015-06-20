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
    public delegate T Reader<Env, T>(Env environment);

    /// <summary>
    /// Reader monad extensions
    /// </summary>
    public static class ReaderExt
    {
        public static Reader<Env,Unit> Iter<Env, T>(this Reader<Env, T> self, Action<T> action)
        {
            return env =>
            {
                action(self(env));
                return unit;
            };
        }

        public static int Count<Env, T>(this Reader<Env, T> self) => 
            1;

        public static Reader<Env, bool> ForAll<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env => pred(self(env));

        public static Reader<Env,bool> Exists<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            env =>pred(self(env));

        public static Reader<Env, S> Fold<Env, S, T>(this Reader<Env, T> self, S state, Func<S, T, S> folder) =>
            env => folder(state, self(env));

        public static Reader<Env, R> Map<Env, T, R>(this Reader<Env, T> self, Func<T, R> mapper) =>
            env => mapper(self(env));

        public static Reader<Env, R> Bind<Env, T, R>(this Reader<Env, T> self, Func<T, Reader<Env, R>> binder) =>
            env => binder(self(env))(env);

        /// <summary>
        /// Select
        /// </summary>
        public static Reader<E, U> Select<E, T, U>(this Reader<E, T> self, Func<T, U> select)
        {
            if (select == null) throw new ArgumentNullException("select");
            return (E env) => select(self(env));
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
                var resU = bind(resT);
                var resV = project(resT, resU(env));
                return resV;
            };
        }
    }
}
