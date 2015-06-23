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
        public readonly bool IsBottom;

        internal WriterResult(T value, IEnumerable<Out> output, bool isBottom = false)
        {
            if (output == null) throw new ArgumentNullException("output");
            Value = value;
            Output = output;
            IsBottom = isBottom;
        }

        public static implicit operator WriterResult<Out, T>(T value) =>
           new WriterResult<Out, T>(value, new Out[0]);

        public static implicit operator T(WriterResult<Out, T> value) =>
           value.Value;
    }

    /// <summary>
    /// Writer extension methods
    /// </summary>
    public static class WriterExt
    {
        public static IEnumerable<T> AsEnumerable<Out, T>(this Writer<Out, T> self)
        {
            var res = self();
            if (!res.IsBottom)
            {
                yield return self().Value;
            }
        }

        public static Writer<Out,Unit> Iter<Out, T>(this Writer<Out, T> self, Action<T> action)
        {
            return () =>
            {
                var res = self();
                if (!res.IsBottom)
                {
                    action(res.Value);
                }
                return unit;
            };
        }

        public static Writer<Out,int> Count<Out, T>(this Writer<Out, T> self) => () =>
            bmap(self(), x => 1);

        public static Writer<Out, bool> ForAll<Out, T>(this Writer<Out, T> self, Func<T, bool> pred) => () =>
            bmap(self(), x => pred(x));

        public static Writer<Out,bool> Exists<Out, T>(this Writer<Out, T> self, Func<T, bool> pred) => () =>
            bmap(self(), x => pred(x));

        public static Writer<Out, S> Fold<Out, S, T>(this Writer<Out, T> self, S state, Func<S, T, S> folder) => () =>
            bmap(self(), x => folder(state, x));

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
                if (resT.IsBottom) return new WriterResult<W, U>(default(U), resT.Output, true);
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
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value).Invoke();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                var resV = project(resT.Value, resU.Value);
                return new WriterResult<W, V>(resV, resT.Output.Concat(resU.Output));
            };
        }

        public static Writer<W, T> Filter<W, T>(this Writer<W, T> self, Func<T, bool> pred) =>
            self.Where(pred);

        public static Writer<W, T> Where<W, T>(this Writer<W, T> self, Func<T, bool> pred)
        {
            return () =>
            {
                var res = self();
                return new WriterResult<W, T>(res.Value, res.Output, !pred(res.Value));
            };
        }

        public static Writer<W, int> Sum<W>(this Writer<W, int> self) =>
            () => bmap(self(), x => x);

        private static WriterResult<W, R> bmap<W, T, R>(WriterResult<W, T> r, Func<T, R> f) =>
            r.IsBottom
                ? new WriterResult<W, R>(default(R), r.Output, true)
                : new WriterResult<W, R>(f(r.Value), r.Output, false);

        private static WriterResult<W, Unit> bmap<W, T>(WriterResult<W, T> r, Action<T> f)
        {
            if (r.IsBottom)
            {
                return new WriterResult<W, Unit>(unit, r.Output, true);
            }
            else
            {
                f(r.Value);
                return new WriterResult<W, Unit>(unit, r.Output, false);
            }
        }

        /// <summary>
        /// Select Many - IEnumerable
        /// </summary>
        public static Writer<W, IEnumerable<V>> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, IEnumerable<U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, IEnumerable<V>>(new V[0], resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, IEnumerable<V>>(new V[0], resU.Output, true);
                var resV = resU.Value.Select(x => project(resT.Value, x));
                return new WriterResult<W, IEnumerable<V>>(resV, resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - Map
        /// </summary>
        public static Writer<W, Map<K,V>> SelectMany<W, K, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, Map<K,U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, Map<K, V>>(Map<K,V>(), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, Map<K, V>>(Map<K,V>(), resU.Output, true);
                var resV = resU.Value.Select(x => project(resT.Value, x));
                return new WriterResult<W, Map<K, V>>(resV, resT.Output.Concat(resU.Output));
            };
        }
        /// <summary>
        /// Select Many - Lst
        /// </summary>
        public static Writer<W, Lst<V>> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, Lst<U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, Lst<V>>(List<V>(), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, Lst<V>>(List<V>(), resU.Output, true);
                var resV = resU.Value.Select(x => project(resT.Value, x));
                return new WriterResult<W, Lst<V>>(List.createRange(resV), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - Option
        /// </summary>
        public static Writer<W, V> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, Option<U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom || resU.Value.IsNone) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, resU.Value.Value), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - OptionUnsafe
        /// </summary>
        public static Writer<W, V> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, OptionUnsafe<U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom || resU.Value.IsNone) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, resU.Value.Value), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - Try
        /// </summary>
        public static Writer<W, V> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, Try<U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                var resUU = resU.Value.Try();
                if (resUU.IsFaulted) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, resUU.Value), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - TryOption
        /// </summary>
        public static Writer<W, V> SelectMany<W, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, TryOption<U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                var resUU = resU.Value.Try();
                if (resUU.IsFaulted || resUU.Value.IsNone) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, resUU.Value.Value), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - Either
        /// </summary>
        public static Writer<W, V> SelectMany<W, L, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, Either<L, U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom || resU.Value.IsLeft) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, resU.Value.RightValue), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - EitherUnsafe
        /// </summary>
        public static Writer<W, V> SelectMany<W, L, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, EitherUnsafe<L, U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom || resU.Value.IsLeft) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, resU.Value.RightValue), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - State
        /// </summary>
        public static Writer<W, V> SelectMany<W, L, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, State<T, U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                var res = resU.Value(resT.Value);
                if (res.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, res.Value), resT.Output.Concat(resU.Output));
            };
        }

        /// <summary>
        /// Select Many - Reader
        /// </summary>
        public static Writer<W, V> SelectMany<W, L, T, U, V>(
            this Writer<W, T> self,
            Func<T, Writer<W, Reader<T, U>>> bind,
            Func<T, U, V> project
        )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return () =>
            {
                var resT = self();
                if (resT.IsBottom) return new WriterResult<W, V>(default(V), resT.Output, true);
                var resU = bind(resT.Value)();
                if (resU.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                var res = resU.Value(resT.Value);
                if (res.IsBottom) return new WriterResult<W, V>(default(V), resU.Output, true);
                return new WriterResult<W, V>(project(resT.Value, res.Value), resT.Output.Concat(resU.Output));
            };
        }
    }
}
