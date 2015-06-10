using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
