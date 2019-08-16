using LanguageExt;
using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class TaskExtensions
    {
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
        public static Task<T> AsTask<T>(this T self) =>
            Task.FromResult(self);

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static async Task<A> Flatten<A>(this Task<Task<A>> self)
        {
            var t = await self;
            var u = await t;
            return u;
        }

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static async Task<A> Flatten<A>(this Task<Task<Task<A>>> self)
        {
            var t = await self;
            var u = await t;
            var v = await u;
            return v;
        }

        /// <summary>
        /// Standard LINQ Select implementation for Task
        /// </summary>
        [Pure]
        public static async Task<U> Select<T, U>(this Task<T> self, Func<T, U> map) =>
            map(await self);

        /// <summary>
        /// Standard LINQ Where implementation for Task
        /// </summary>
        [Pure]
        public static async Task<T> Where<T>(this Task<T> self, Func<T, bool> pred)
        {
            var resT = await self;
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
        public async static Task<U> SelectMany<T, U>(
            this Task<T> self,
            Func<T, Task<U>> bind
            ) =>
            await bind(await self);

        /// <summary>
        /// Standard LINQ SelectMany implementation for Task
        /// </summary>
        [Pure]
        public static async Task<V> SelectMany<T, U, V>(
            this Task<T> self,
            Func<T, Task<U>> bind,
            Func<T, U, V> project
            )
        {
            var resT = await self;
            var resU = await bind(resT);
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
                await self;
                return 1;
            }
            catch(Exception)
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
            pred(await self);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> ExistsAsync<T>(this Task<T> self, Func<T, Task<bool>> pred) =>
            await pred(await self);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> ForAll<T>(this Task<T> self, Func<T, bool> pred) =>
            pred(await self);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static async Task<bool> ForAllAsync<T>(this Task<T> self, Func<T, Task<bool>> pred) =>
            await pred(await self);

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
            folder(state, await self);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static async Task<S> FoldAsync<T, S>(this Task<T> self, S state, Func<S, T, Task<S>> folder) =>
            await folder(state, await self);

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static async Task<Unit> Iter<T>(this Task<T> self, Action<T> f)
        {
            f(await self);
            return unit;
        }

        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public static Task<A> Do<A>(this Task<A> ma, Action<A> f) =>
            ma.Map(x => { f(x); return x; });

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static async Task<U> Map<T, U>(this Task<T> self, Func<T, U> map) =>
            map(await self);

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static async Task<U> MapAsync<T, U>(this Task<T> self, Func<T, Task<U>> map) =>
            await map(await self);

        [Pure]
        public static async Task<V> Join<T, U, K, V>(
            this Task<T> source,
            Task<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project)
        {
            await Task.WhenAll(source, inner);
            if (!EqualityComparer<K>.Default.Equals(outerKeyMap(source.Result), innerKeyMap(inner.Result)))
            {
                throw new OperationCanceledException();
            }
            return project(source.Result, inner.Result);
        }

        [Pure]
        public static async Task<V> GroupJoin<T, U, K, V>(
            this Task<T> source,
            Task<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, Task<U>, V> project)
        {
            T t = await source;
            return project(t, inner.Where(u => EqualityComparer<K>.Default.Equals(outerKeyMap(t), innerKeyMap(u))));
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
            await source;
            var prop = PropCache<A>.Info;
            return prop != null
                ? (A)prop.GetValue(source)
                : default(A);
        }

        public static async Task<Unit> ToUnit(this Task source)
        {
            await source;
            return unit;
        }

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `Environment.ProcessorCount / 2` tasks is used, which means there are `Environment.ProcessorCount / 2`
        /// 'await streams'.  An await stream essentially awaits one task from the sequence, and on completion
        /// goes and gets the next task from the lazy sequence and awaits that too.  This continues until the end
        /// of the lazy sequence, or forever for infinite streams.
        /// </summary>
        public static Task<Unit> WindowIter<A>(this IEnumerable<Task<A>> ma, Action<A> f) =>
            WindowIter(ma, Math.Max(1, Environment.ProcessorCount >> 1), f);

        /// <summary>
        /// Tasks a lazy sequence of tasks and iterates them in a 'measured way'.  A default window size of
        /// `windowSize` tasks is used, which means there are `windowSize` 'await streams'.  An await stream 
        /// essentially awaits one task from the sequence, and on completion goes and gets the next task from 
        /// the lazy sequence and awaits that too.  This continues until the end of the lazy sequence, or forever 
        /// for infinite streams.
        /// </summary>
        public static async Task<Unit> WindowIter<A>(this IEnumerable<Task<A>> ma, int windowSize, Action<A> f)
        {
            object sync = new object();
            var iter = ma.GetEnumerator();

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

                tasks.Add(outerTask.Iter(async oa =>
                {
                    f(oa);

                    while (true)
                    {
                        var next = GetNext();
                        if (!next.Success) return;
                        var a = await next.Task;
                        f(a);
                    }
                }));
            }
            await Task.WhenAll(tasks);
            return unit;
        }
    }
}
