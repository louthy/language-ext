using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class __TaskExt
    {
        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        public static Task<T> AsTask<T>(this T self) =>
            Task.FromResult(self);

        /// <summary>
        /// Standard LINQ Select implementation for Task
        /// </summary>
        public static async Task<U> Select<T, U>(this Task<T> self, Func<T, U> map) =>
            map(await self);

        /// <summary>
        /// Standard LINQ Where implementation for Task
        /// </summary>
        public static async Task<T> Where<T>(this Task<T> self, Func<T, bool> pred)
        {
            var resT = await self;
            var res = pred(resT);
            if(!res)
            {
                throw new OperationCanceledException();
            }
            return resT;
        }

        /// <summary>
        /// Standard LINQ SelectMany implementation for Task
        /// </summary>
        public async static Task<U> SelectMany<T, U>(
            this Task<T> self,
            Func<T, Task<U>> bind
            ) =>
            await bind(await self);

        /// <summary>
        /// Standard LINQ SelectMany implementation for Task
        /// </summary>
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
        /// Get the Sum of a Task int.  Returns either the wrapped value or 0 if cancelled or faulted.
        /// </summary>
        public static int Sum(this Task<int> self)
        {
            if (self.IsFaulted || self.IsCanceled) return 0;
            return self.Result;
        }

        /// <summary>
        /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
        /// </summary>
        public static int Count<T>(this Task<T> self)
        {
            if (self.IsFaulted || self.IsCanceled) return 0;
            self.Wait();
            if (self.IsFaulted || self.IsCanceled) return 0;
            return 1;
        }

        /// <summary>
        /// Monadic bind operation for Task
        /// </summary>
        public static Task<U> Bind<T, U>(this Task<T> self, Func<T, Task<U>> bind) =>
            self.SelectMany(bind);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        public static bool Exists<T>(this Task<T> self, Func<T,bool> pred)
        {
            if (self.IsFaulted || self.IsCanceled) return false;
            self.Wait();
            if (self.IsFaulted || self.IsCanceled) return false;
            return pred(self.Result);
        }

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        public static bool ForAll<T>(this Task<T> self, Func<T, bool> pred)
        {
            if (self.IsFaulted || self.IsCanceled) return false;
            self.Wait();
            if (self.IsFaulted || self.IsCanceled) return false;
            return pred(self.Result);
        }

        /// <summary>
        /// Filters the task.  This throws a BottomException when pred(Result)
        /// returns false
        /// </summary>
        public static Task<T> Filter<T>(this Task<T> self, Func<T, bool> pred) =>
            self.Where(pred);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        public static S Fold<T, S>(this Task<T> self, S state, Func<S, T, S> folder)
        {
            if (self.IsFaulted || self.IsCanceled) return state;
            self.Wait();
            if (self.IsFaulted || self.IsCanceled) return state;
            return folder(state, self.Result);
        }

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static Unit Iter<T>(this Task<T> self, Action<T> f)
        {
            if (self.IsFaulted || self.IsCanceled) return unit;
            self.ContinueWith(t => f(t.Result));
            return unit;
        }

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        public static Task<U> Map<T, U>(this Task<T> self, Func<T, U> map) =>
            self.Select(map);

        /// <summary>
        /// Extracts the value from the Task - here for completeness so that
        /// the HKT work.
        /// </summary>
        public static T LiftUnsafe<T>(this Task<T> self) where T : class
        {
            if (self.IsFaulted || self.IsCanceled) return self.Result;
            self.Wait();
            return self.Result;
        }

        /// <summary>
        /// Extracts the value from the Task - here for completeness so that
        /// the HKT work.
        /// </summary>
        public static T Lift<T>(this Task<T> self) where T : struct
        {
            if (self.IsFaulted || self.IsCanceled) return self.Result;
            self.Wait();
            return self.Result;
        }

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

        public static async Task<V> GroupJoin<T, U, K, V>(
            this Task<T> source, 
            Task<U> inner,
            Func<T, K> outerKeyMap, 
            Func<U, K> innerKeyMap,
            Func<T, Task<U>, V> project)
        {
            T t = await source;
            return project(t,inner.Where(u => EqualityComparer<K>.Default.Equals(outerKeyMap(t), innerKeyMap(u))));
        }
#if !COREFX
        public static async Task<T> Cast<T>(this Task source)
        {
            await source;
            return (T)((dynamic)source).Result;
        }
#endif
    }
}
