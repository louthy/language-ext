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
    public static class TaskExtensions
    {
        public static bool CompletedSuccessfully<A>(this Task<A> ma) =>
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
        public static async Task<object> Case<A>(this Task<A> ma)
        {
            if (ma == null) return Common.Errors.Bottom;
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
        public static Task<A> AsFailedTask<A>(this Exception ex)
        {
            var tcs = new TaskCompletionSource<A>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        [Pure]
        public static Task<A> AsTask<A>(this A self) =>
            Task.FromResult(self);

        /// <summary>
        /// Convert a ValueTask to a Task 
        /// </summary>
        [Pure]
        public static Task<A> ToRef<A>(this ValueTask<A> self) =>
            self.AsTask();
        
        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static async Task<A> Flatten<A>(this Task<Task<A>> self)
        {
            var t = await self.ConfigureAwait(false);
            var u = await t.ConfigureAwait(false);
            return u;
        }

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static async Task<A> Flatten<A>(this Task<Task<Task<A>>> self)
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
        public static async Task<U> Select<T, U>(this Task<T> self, Func<T, U> map) =>
            map(await self.ConfigureAwait(false));

        /// <summary>
        /// Standard LINQ Where implementation for Task
        /// </summary>
        [Pure]
        public static async Task<T> Where<T>(this Task<T> self, Func<T, bool> pred)
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
        public async static Task<U> SelectMany<T, U>(this Task<T> self,
            Func<T, Task<U>> bind) =>
            await bind(await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Standard LINQ SelectMany implementation for Task
        /// </summary>
        [Pure]
        public static async Task<V> SelectMany<T, U, V>(this Task<T> self,
            Func<T, Task<U>> bind,
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
        public static async Task<int> Count<T>(this Task<T> self)
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
        public static Task<U> Bind<T, U>(this Task<T> self, Func<T, Task<U>> bind) =>
            self.SelectMany(bind);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> Exists<T>(this Task<T> self, Func<T, bool> pred) =>
            pred(await self.ConfigureAwait(false));

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> ExistsAsync<T>(this Task<T> self, Func<T, Task<bool>> pred) =>
            await pred(await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> ForAll<T>(this Task<T> self, Func<T, bool> pred) =>
            pred(await self.ConfigureAwait(false));

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> ForAllAsync<T>(this Task<T> self, Func<T, Task<bool>> pred) =>
            await pred(await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Filters the task.  This throws a BottomException when pred(Result)
        /// returns false
        /// </summary>
        [Pure]
        public static Task<T> Filter<T>(this Task<T> self, Func<T, bool> pred) =>
            self.Where(pred);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static async Task<S> Fold<T, S>(this Task<T> self, S state, Func<S, T, S> folder) =>
            folder(state, await self.ConfigureAwait(false));

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static async Task<S> FoldAsync<T, S>(this Task<T> self, S state, Func<S, T, Task<S>> folder) =>
            await folder(state, await self.ConfigureAwait(false)).ConfigureAwait(false);

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static async Task<Unit> Iter<T>(this Task<T> self, Action<T> f)
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
        public static Task<A> Do<A>(this Task<A> ma, Action<A> f) =>
            ma.Map(x => {
                f(x);
                return x;
            });

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static async Task<U> Map<T, U>(this Task<T> self, Func<T, U> map) =>
            map(await self.ConfigureAwait(false));

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static async Task<U> MapAsync<T, U>(this Task<T> self, Func<T, Task<U>> map) =>
            await map(await self.ConfigureAwait(false)).ConfigureAwait(false);

        [Pure]
        public static async Task<V> Join<T, U, K, V>(this Task<T> source,
            Task<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project)
        {
            await Task.WhenAll(source, inner).ConfigureAwait(false);
            if (!default(EqDefault<K>).Equals(outerKeyMap(source.Result), innerKeyMap(inner.Result)))
            {
                throw new OperationCanceledException();
            }

            return project(source.Result, inner.Result);
        }

        [Pure]
        public static async Task<V> GroupJoin<T, U, K, V>(this Task<T> source,
            Task<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, Task<U>, V> project)
        {
            T t = await source.ConfigureAwait(false);
            return project(t, inner.Where(u => default(EqDefault<K>).Equals(outerKeyMap(t), innerKeyMap(u))));
        }

        [Pure]
        public static Task<A> Plus<A>(this Task<A> ma, Task<A> mb) =>
            default(MTask<A>).Plus(ma, mb);

        [Pure]
        public static Task<A> PlusFirst<A>(this Task<A> ma, Task<A> mb) =>
            default(MTaskFirst<A>).Plus(ma, mb);

        class PropCache<T>
        {
            public static PropertyInfo Info = typeof(T).GetTypeInfo().DeclaredProperties.Where(p => p.Name == "Result").FirstOrDefault();
        }

        public static async Task<A> Cast<A>(this Task source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            await source.ConfigureAwait(false);
            var prop = PropCache<A>.Info;
            return prop != null
                ? (A) prop.GetValue(source)
                : default(A);
        }

        public static async Task<Unit> ToUnit(this Task source)
        {
            await source.ConfigureAwait(false);
            return unit;
        }

        /// <summary>
        /// Convert the structure to an Aff
        /// </summary>
        /// <returns>An Aff representation of the structure</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Aff<A> ToAff<A>(this Task<A> ma) =>
            Aff(async () => await ma);

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `Sys.DefaultAsyncSequenceConcurrency` tasks is used, which by default means there are
        /// `Sys.DefaultAsyncSequenceConcurrency / 2` 'await streams'.  An await stream essentially awaits one
        /// task from the sequence, and on completion goes and gets the next task from the lazy sequence and
        /// awaits that too.  This continues until the end of the lazy sequence, or forever for infinite streams.
        /// </summary>
        public static Task<Unit> WindowIter<A>(this IEnumerable<Task<A>> ma, Action<A> f) =>
            WindowIter(ma, SysInfo.DefaultAsyncSequenceParallelism, f);

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `windowSize` tasks is used, which means there are `windowSize` 'await streams'.  An await stream 
        /// essentially awaits one task from the sequence, and on completion goes and gets the next task from 
        /// the lazy sequence and awaits that too.  This continues until the end of the lazy sequence, or forever 
        /// for infinite streams.  Therefore there are at most `windowSize` tasks running concurrently.
        /// </summary>
        public static async Task<Unit> WindowIter<A>(this IEnumerable<Task<A>> ma, int windowSize, Action<A> f)
        {
            var sync = new object();
            using var iter = ma.GetEnumerator();

            (bool Success, Task<A> Task) GetNext()
            {
                lock (sync)
                {
                    return iter.MoveNext()
                        ? (true, iter.Current)
                        : default;
                }
            }
            
            var tasks = new List<Task<Unit>>();
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

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return unit;
        }

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `Sys.DefaultAsyncSequenceConcurrency` tasks is used, which means there are `Environment.ProcessorCount / 2`
        /// 'await streams' (by default).  An await stream essentially awaits one task from the sequence, and on
        /// completion goes and gets the next task from the lazy sequence and awaits that too.  This continues until the
        /// end of the lazy sequence, or forever for infinite streams.
        /// </summary>
        internal static Task<IList<B>> WindowMap<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
            WindowMap(ma, SysInfo.DefaultAsyncSequenceParallelism, f);

        /// <summary>
        /// Tasks a lazy sequence of tasks and maps them in a 'measured way'.  A default window size of
        /// `windowSize` tasks is used, which means there are `windowSize` 'await streams'.  An await stream 
        /// essentially awaits one task from the sequence, and on completion goes and gets the next task from 
        /// the lazy sequence and awaits that too.  This continues until the end of the lazy sequence, or forever 
        /// for infinite streams.  Therefore there are at most `windowSize` tasks running concurrently.
        /// </summary>
        internal static async Task<IList<B>> WindowMap<A, B>(this IEnumerable<Task<A>> ma, int windowSize, Func<A, B> f)
        {
            var sync = new object();
            using var iter = ma.GetEnumerator();

            (bool Success, Task<A> Task) GetNext()
            {
                lock (sync)
                {
                    return iter.MoveNext()
                        ? (true, iter.Current)
                        : default;
                }
            }

            var tasks = new List<Task<Unit>>();
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
                                errors[ix].Add(next.Task.Exception);
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

            await Task.WhenAll(tasks).ConfigureAwait(false);

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
