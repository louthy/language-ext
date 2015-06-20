using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Writer monad
    /// </summary>
    /// <typeparam name="Out">Writer output</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    public delegate WriterResult<Out, T> Writer<Out, T>();

    public struct WriterResult<Out, T>
    {
        public readonly T Value;
        public readonly IEnumerable<Out> Output;

        internal WriterResult(T value, IEnumerable<Out> output)
        {
            if (output == null) throw new ArgumentNullException("output");
            Value = value;
            Output = output;
        }
    }

    /// <summary>
    /// Writer extension methods
    /// </summary>
    public static class WriterExt
    {
        public static Unit Iter<Out, T>(this Writer<Out, T> self, Action<T> action)
        {
            action(self().Value);
            return unit;
        }

        public static int Count<Out, T>(this Writer<Out, T> self) =>
            1;

        public static bool ForAll<Out, T>(this Writer<Out, T> self, Func<T, bool> pred) =>
            pred(self().Value);

        public static bool Exists<Out, T>(this Writer<Out, T> self, Func<T, bool> pred) =>
            pred(self().Value);

        public static S Fold<Out, S, T>(this Writer<Out, T> self, S state, Func<S, T, S> folder) =>
            folder(state, self().Value);

        public static Writer<Out, R> Map<Out, T, R>(this Writer<Out, T> self, Func<T, R> mapper) =>
            self.Select(mapper);

        public static Writer<Out, R> Bind<Out, T, R>(this Writer<Out, T> self, Func<T, Writer<Out, R>> binder) =>
            from x in self
            from y in binder(x)
            select y;

        /// <summary>
        /// Select
        /// </summary>
        public static Writer<W, U> Select<W, T, U>(this Writer<W, T> self, Func<T, U> select)
        {
            if (select == null) throw new ArgumentNullException("select");
            return () =>
            {
                var resT = self();
                var resU = select(resT.Value);
                return new WriterResult<W, U>(resU, resT.Output);
            };
        }

        /// <summary>
        /// Select Many
        /// </summary>
        public static Writer<W, V> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, U>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                var resU = bind(resT.Value).Invoke();
                var resV = project(resT.Value, resU.Value);

                return new WriterResult<W, V>(resV, resT.Output.Concat(resU.Output));
            };
        }
    }
}
