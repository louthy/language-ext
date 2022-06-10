using LanguageExt;
using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValueTaskExtensions
    {
        public static bool CompletedSuccessfully<A>(this ValueTask<A> ma) =>
            ma.IsCompleted && !ma.IsFaulted && !ma.IsCanceled;
        
        /// <summary>
        /// Use for pattern-matching the case of the target
        /// </summary>
        /// <remarks>
        ///
        ///     Task succeeds = result is A
        ///     Task fails    = result is LanguageExt.Common.Error
        ///
        /// </remarks>
        [Pure]
        public static async ValueTask<object> Case<A>(this ValueTask<A> ma)
        {
            try
            {
                return await ma.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Error.New(ex);
            }
        }

        [Pure]
        public static ValueTask<A> AsFailedValueTask<A>(this Exception ex) =>
            new ValueTask<A>(ex.AsFailedTask<A>());

        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        [Pure]
        public static ValueTask<A> AsValueTask<A>(this A self) =>
            new ValueTask<A>(self);

        /// <summary>
        /// Convert a Task to a ValueTask 
        /// </summary>
        [Pure]
        public static ValueTask<A> ToValue<A>(this Task<A> self) =>
            new ValueTask<A>(self);

        /// <summary>
        /// Convert the structure to an Aff
        /// </summary>
        /// <returns>An Aff representation of the structure</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Aff<A> ToAff<A>(this ValueTask<A> ma) =>
            Aff(() => ma);

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static async ValueTask<A> Flatten<A>(this ValueTask<ValueTask<A>> self)
        {
            var t = await self.ConfigureAwait(false);
            var u = await t.ConfigureAwait(false);
            return u;
        }

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static async ValueTask<A> Flatten<A>(this ValueTask<ValueTask<ValueTask<A>>> self)
        {
            var t = await self.ConfigureAwait(false);
            var u = await t.ConfigureAwait(false);
            var v = await u.ConfigureAwait(false);
            return v;
        }

        /// <summary>
        /// Standard LINQ Select implementation for Task
        /// </summary>
        [Pure]
        public static async ValueTask<U> Select<T, U>(this ValueTask<T> self, Func<T, U> map) =>
            map(await self.ConfigureAwait(false));

        /// <summary>
        /// Standard LINQ Where implementation for Task
        /// </summary>
        [Pure]
        public static async ValueTask<T> Where<T>(this ValueTask<T> self, Func<T, bool> pred)
        {
            var resT = await self.ConfigureAwait(false);
            var res = pred(resT);
            if (!res)
            {
                throw new TaskCanceledException();
            }

            return resT;
        }

        /// <summary>
        /// Standard LINQ SelectMany implementation for Task
        /// </summary>
        [Pure]
        public async static ValueTask<U> SelectMany<T, U>(this ValueTask<T> self,
            Func<T, ValueTask<U>> bind) =>
            await bind(await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Standard LINQ SelectMany implementation for Task
        /// </summary>
        [Pure]
        public static async ValueTask<V> SelectMany<T, U, V>(this ValueTask<T> self,
            Func<T, ValueTask<U>> bind,
            Func<T, U, V> project)
        {
            var resT = await self.ConfigureAwait(false);
            var resU = await bind(resT).ConfigureAwait(false);
            return project(resT, resU);
        }

        /// <summary>
        /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
        /// </summary>
        [Pure]
        public static async ValueTask<int> Count<T>(this ValueTask<T> self)
        {
            try
            {
                await self.ConfigureAwait(false);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Monadic bind operation for Task
        /// </summary>
        [Pure]
        public static ValueTask<U> Bind<T, U>(this ValueTask<T> self, Func<T, ValueTask<U>> bind) =>
            self.SelectMany(bind);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async ValueTask<bool> Exists<T>(this ValueTask<T> self, Func<T, bool> pred) =>
            pred(await self.ConfigureAwait(false));

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async ValueTask<bool> ExistsAsync<T>(this ValueTask<T> self, Func<T, ValueTask<bool>> pred) =>
            await pred(await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async ValueTask<bool> ForAll<T>(this ValueTask<T> self, Func<T, bool> pred) =>
            pred(await self.ConfigureAwait(false));

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async ValueTask<bool> ForAllAsync<T>(this ValueTask<T> self, Func<T, ValueTask<bool>> pred) =>
            await pred(await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Filters the task.  This throws a BottomException when pred(Result)
        /// returns false
        /// </summary>
        [Pure]
        public static ValueTask<T> Filter<T>(this ValueTask<T> self, Func<T, bool> pred) =>
            self.Where(pred);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static async ValueTask<S> Fold<T, S>(this ValueTask<T> self, S state, Func<S, T, S> folder) =>
            folder(state, await self.ConfigureAwait(false));

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static async ValueTask<S> FoldAsync<T, S>(this ValueTask<T> self, S state, Func<S, T, ValueTask<S>> folder) =>
            await folder(state, await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static async ValueTask<Unit> Iter<T>(this ValueTask<T> self, Action<T> f)
        {
            f(await self.ConfigureAwait(false));
            return unit;
        }

        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public static ValueTask<A> Do<A>(this ValueTask<A> ma, Action<A> f) =>
            ma.Map(x => {
                f(x);
                return x;
            });

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static async ValueTask<U> Map<T, U>(this ValueTask<T> self, Func<T, U> map) =>
            map(await self.ConfigureAwait(false));

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static async ValueTask<U> MapAsync<T, U>(this ValueTask<T> self, Func<T, ValueTask<U>> map) =>
            await map(await self.ConfigureAwait(false)).ConfigureAwait(false);

        [Pure]
        public static async ValueTask<V> Join<T, U, K, V>(this ValueTask<T> source,
            ValueTask<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project)
        {
            await Task.WhenAll(source.AsTask(), inner.AsTask()).ConfigureAwait(false);
            if (!default(EqDefault<K>).Equals(outerKeyMap(source.Result), innerKeyMap(inner.Result)))
            {
                throw new OperationCanceledException();
            }

            return project(source.Result, inner.Result);
        }

        [Pure]
        public static async ValueTask<V> GroupJoin<T, U, K, V>(this ValueTask<T> source,
            ValueTask<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, ValueTask<U>, V> project)
        {
            T t = await source.ConfigureAwait(false);
            return project(t, inner.Where(u => default(EqDefault<K>).Equals(outerKeyMap(t), innerKeyMap(u))));
        }

        [Pure]
        public static ValueTask<A> Plus<A>(this ValueTask<A> ma, ValueTask<A> mb) =>
            default(MValueTask<A>).Plus(ma, mb);

        class PropCache<T>
        {
            public static PropertyInfo Info = typeof(T).GetTypeInfo().DeclaredProperties.Where(p => p.Name == "Result").FirstOrDefault();
        }

        public static async ValueTask<A> Cast<A>(this ValueTask source)
        {
            await source.ConfigureAwait(false);
            var prop = PropCache<A>.Info;
            return prop != null
                ? (A) prop.GetValue(source)
                : default(A);
        }

        public static async ValueTask<Unit> ToUnit(this ValueTask source)
        {
            await source.ConfigureAwait(false);
            return unit;
        }

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `Sys.DefaultAsyncSequenceConcurrency` tasks is used, which by default means there are
        /// `Sys.DefaultAsyncSequenceConcurrency / 2` 'await streams'.  An await stream essentially awaits one
        /// task from the sequence, and on completion goes and gets the next task from the lazy sequence and
        /// awaits that too.  This continues until the end of the lazy sequence, or forever for infinite streams.
        /// </summary>
        public static ValueTask<Unit> WindowIter<A>(this IEnumerable<ValueTask<A>> ma, Action<A> f) =>
            WindowIter(ma, SysInfo.DefaultAsyncSequenceParallelism, f);

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `windowSize` tasks is used, which means there are `windowSize` 'await streams'.  An await stream 
        /// essentially awaits one task from the sequence, and on completion goes and gets the next task from 
        /// the lazy sequence and awaits that too.  This continues until the end of the lazy sequence, or forever 
        /// for infinite streams.  Therefore there are at most `windowSize` tasks running concurrently.
        /// </summary>
        public static async ValueTask<Unit> WindowIter<A>(this IEnumerable<ValueTask<A>> ma, int windowSize, Action<A> f)
        {
            var sync = new object();
            using var iter = ma.GetEnumerator();

            (bool Success, ValueTask<A> Task) GetNext()
            {
                lock (sync)
                {
                    return iter.MoveNext()
                        ? (true, iter.Current)
                        : default;
                }
            }

            var tasks = new List<ValueTask<Unit>>();
            for (var i = 0; i < windowSize; i++)
            {
                var (s, outerTask) = GetNext();
                if (!s) break;

                tasks.Add(outerTask.Bind(async oa => {
                    f(oa);

                    while (true)
                    {
                        var next = GetNext();
                        if (!next.Success) return unit;
                        var a = await next.Task.ConfigureAwait(false);
                        f(a);
                    }
                }));
            }

            await Task.WhenAll(tasks.Map(t => t.AsTask())).ConfigureAwait(false);
            return unit;
        }

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `Sys.DefaultAsyncSequenceConcurrency` tasks is used, which means there are `Environment.ProcessorCount / 2`
        /// 'await streams' (by default).  An await stream essentially awaits one task from the sequence, and on
        /// completion goes and gets the next task from the lazy sequence and awaits that too.  This continues until the
        /// end of the lazy sequence, or forever for infinite streams.
        /// </summary>
        internal static ValueTask<IList<B>> WindowMap<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f) =>
            WindowMap(ma, SysInfo.DefaultAsyncSequenceParallelism, f);

        /// <summary>
        /// Tasks a lazy sequence of tasks and maps them in a 'measured way'.  A default window size of
        /// `windowSize` tasks is used, which means there are `windowSize` 'await streams'.  An await stream 
        /// essentially awaits one task from the sequence, and on completion goes and gets the next task from 
        /// the lazy sequence and awaits that too.  This continues until the end of the lazy sequence, or forever 
        /// for infinite streams.  Therefore there are at most `windowSize` tasks running concurrently.
        /// </summary>
        internal static async ValueTask<IList<B>> WindowMap<A, B>(this IEnumerable<ValueTask<A>> ma, int windowSize, Func<A, B> f)
        {
            var sync = new object();
            using var iter = ma.GetEnumerator();

            (bool Success, ValueTask<A> Task) GetNext()
            {
                lock (sync)
                {
                    return iter.MoveNext()
                        ? (true, iter.Current)
                        : default;
                }
            }

            var tasks = new List<ValueTask<Unit>>();
            var results = new List<B>[windowSize];
            var errors = new List<AggregateException>[windowSize];

            for (var i = 0; i < windowSize; i++)
            {
                results[i] = new List<B>();
                errors[i] = new List<AggregateException>();
            }

            for (var i = 0; i < windowSize; i++)
            {
                var (s, outerTask) = GetNext();
                if (!s) break;

                var ix = i;
                tasks.Add(outerTask.Bind(async oa => {
                    results[ix].Add(f(oa));

                    while (true)
                    {
                        try
                        {
                            var next = GetNext();
                            if (!next.Success) return unit;
                            var a = await next.Task.ConfigureAwait(false);
                            if (next.Task.IsFaulted)
                            {
                                errors[ix].Add(next.Task.AsTask().Exception);
                                return unit;
                            }
                            else
                            {
                                results[ix].Add(f(a));
                            }
                        }
                        catch (Exception e)
                        {
                            errors[ix].Add(new AggregateException(e));
                            return unit;
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks.Map(t => t.AsTask())).ConfigureAwait(false);

            // Move all errors into one list
            for (var i = 1; i < windowSize; i++)
            {
                errors[0].AddRange(errors[i]);
            }

            if (errors[0].Count > 0)
            {
                // Throw an aggregate of all exceptions
                throw new AggregateException(errors[0].SelectMany(e => e.InnerExceptions));
            }

            // Move all results into one list
            for (var i = 1; i < windowSize; i++)
            {
                results[0].AddRange(results[i]);
            }

            return results[0];
        }
    }
}
