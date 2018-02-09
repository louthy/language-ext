﻿using LanguageExt;
using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static Task<A> asFailedTask<A>(Exception ex)
        {
            var tcs = new TaskCompletionSource<A>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        [Pure]
        public static Task<T> asTask<T>(T self) =>
            Task.FromResult(self);

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static Task<A> flatten<A>(Task<Task<A>> self) =>
            self.Flatten();

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static Task<A> flatten<A>(Task<Task<Task<A>>> self) =>
            self.Flatten();

        /// <summary>
        /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
        /// </summary>
        [Pure]
        public static Task<int> count<T>(Task<T> self) =>
            self.Count();

        /// <summary>
        /// Monadic bind operation for Task
        /// </summary>
        [Pure]
        public static Task<U> bind<T, U>(Task<T> self, Func<T, Task<U>> bind) =>
            self.Bind(bind);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static Task<bool> exists<T>(Task<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static Task<bool> existsAsync<T>(Task<T> self, Func<T, Task<bool>> pred) =>
            self.ExistsAsync(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static Task<bool> forall<T>(Task<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static Task<bool> forallAsync<T>(Task<T> self, Func<T, Task<bool>> pred) =>
            self.ForAllAsync(pred);

        /// <summary>
        /// Filters the task.  This throws a BottomException when pred(Result)
        /// returns false
        /// </summary>
        [Pure]
        public static Task<T> filter<T>(Task<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static Task<S> fold<T, S>(Task<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static Task<S> foldAsync<T, S>(Task<T> self, S state, Func<S, T, Task<S>> folder) =>
            self.FoldAsync(state, folder);

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static Task<Unit> iter<T>(Task<T> self, Action<T> f) =>
            self.Iter(f);

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static Task<U> map<T, U>(Task<T> self, Func<T, U> map) =>
            self.Map(map);

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static Task<U> mapAsync<T, U>(Task<T> self, Func<T, Task<U>> map) =>
            self.MapAsync(map);

        [Pure]
        public static Task<A> plus<A>(this Task<A> ma, Task<A> mb) =>
            default(MTask<A>).Plus(ma, mb);

        [Pure]
        public static Task<A> plusFirst<A>(this Task<A> ma, Task<A> mb) =>
            default(MTaskFirst<A>).Plus(ma, mb);

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="ma">The first computation to run</param>
        /// <param name="tail">The rest of the computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static Task<A> choice<A>(Task<A> ma, params Task<A>[] tail) =>
            choice(Cons(ma, tail));

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="xs">Sequence of computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static async Task<A> choice<A>(Seq<Task<A>> xs) =>
            xs.IsEmpty
                ? await BottomException.Default.AsFailedTask<A>()
                : await default(MTask<A>).MatchAsync(
                    xs.Head,
                    SomeAsync: async x  => await xs.Head,
                    NoneAsync: async () => await choice(xs.Tail));

    }
}