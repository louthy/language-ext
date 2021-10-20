using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class Queue
    {
        [Pure]
        public static Que<T> enq<T>(Que<T> queue, T value) =>
            queue.Enqueue(value);

        [Pure]
        public static (Que<T> Queue, T Value) deqUnsafe<T>(Que<T> queue) =>
            queue.DequeueUnsafe();

        [Pure]
        public static (Que<T> Queue, Option<T> Value) deq<T>(Que<T> queue) =>
            queue.TryDequeue();

        [Pure]
        public static T peekUnsafe<T>(Que<T> queue) =>
            queue.Peek();

        [Pure]
        public static Option<T> peek<T>(Que<T> queue) =>
            queue.TryPeek();

        [Pure]
        public static Que<T> clear<T>(Que<T> queue) =>
            queue.Clear();

        [Pure]
        public static Que<R> map<T, R>(Que<T> queue, Func<int, T, R> map) =>
            queue.Map(map);

        [Pure]
        public static Que<T> filter<T>(Que<T> queue, Func<T, bool> predicate) =>
            queue.Filter(predicate);

        [Pure]
        public static Que<U> choose<T, U>(Que<T> queue, Func<T, Option<U>> selector) =>
            queue.Choose(selector);

        [Pure]
        public static Que<U> choose<T, U>(Que<T> queue, Func<int, T, Option<U>> selector) =>
            queue.Choose(selector);

        [Pure]
        public static Que<R> collect<T, R>(Que<T> queue, Func<T, IEnumerable<R>> map) =>
            queue.Collect(map);

        [Pure]
        public static Que<T> rev<T>(Que<T> queue) =>
            queue.Rev();

        [Pure]
        public static Que<T> append<T>(Que<T> lhs, IEnumerable<T> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Folds each value of the QueT into an S.
        /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
        /// </summary>
        /// <param name="queue">Queue to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.fold(queue, state, folder);

        /// <summary>
        /// Folds each value of the QueT into an S, but in reverse order.
        /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
        /// </summary>
        /// <param name="queue">Queue to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S foldBack<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.foldBack(queue, state, folder);

        [Pure]
        public static T reduce<T>(Que<T> queue, Func<T, T, T> reducer) =>
            List.reduce(queue, reducer);

        [Pure]
        public static T reduceBack<T>(Que<T> queue, Func<T, T, T> reducer) =>
            List.reduceBack(queue, reducer);

        [Pure]
        public static IEnumerable<S> scan<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.scan(queue, state, folder);

        [Pure]
        public static IEnumerable<S> scanBack<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.scanBack(queue, state, folder);

        [Pure]
        public static Option<T> find<T>(Que<T> queue, Func<T, bool> pred) =>
            List.find(queue, pred);

        [Pure]
        public static Que<V> zip<T, U, V>(Que<T> queue, IEnumerable<U> other, Func<T, U, V> zipper) =>
            toQueue(List.zip(queue, other, zipper));

        [Pure]
        public static int length<T>(Que<T> queue) =>
            List.length(queue);

        public static Unit iter<T>(Que<T> queue, Action<T> action) =>
            List.iter(queue, action);

        public static Unit iter<T>(Que<T> queue, Action<int, T> action) =>
            List.iter(queue, action);

        [Pure]
        public static bool forall<T>(Que<T> queue, Func<T, bool> pred) =>
            List.forall(queue, pred);

        [Pure]
        public static Que<T> distinct<T>(Que<T> queue) =>
            queue.Distinct();

        [Pure]
        public static Que<T> distinct<EQ, T>(Que<T> queue) where EQ : struct, Eq<T> =>
            queue.Distinct<EQ, T>();

        [Pure]
        public static IEnumerable<T> take<T>(Que<T> queue, int count) =>
            List.take(queue, count);

        [Pure]
        public static IEnumerable<T> takeWhile<T>(Que<T> queue, Func<T, bool> pred) =>
            List.takeWhile(queue, pred);

        [Pure]
        public static IEnumerable<T> takeWhile<T>(Que<T> queue, Func<T, int, bool> pred) =>
            List.takeWhile(queue, pred);

        [Pure]
        public static bool exists<T>(Que<T> queue, Func<T, bool> pred) =>
            List.exists(queue, pred);
    }

    public static class QueueExtensions
    {
        [Pure]
        public static (Que<T>, T) PopUnsafe<T>(this Que<T> queue) =>
            LanguageExt.Queue.deqUnsafe(queue);

        [Pure]
        public static (Que<T>, Option<T>) Pop<T>(this Que<T> queue) =>
            LanguageExt.Queue.deq(queue);

        [Pure]
        public static T PeekUnsafe<T>(this Que<T> queue) =>
            LanguageExt.Queue.peekUnsafe(queue);

        [Pure]
        public static Option<T> Peek<T>(this Que<T> queue) =>
            LanguageExt.Queue.peek(queue);

        [Pure]
        public static Que<R> Map<T, R>(this Que<T> queue, Func<T, R> map) =>
            toQueue(LanguageExt.List.map(queue, map));

        [Pure]
        public static Que<R> Map<T, R>(this Que<T> queue, Func<int, T, R> map) =>
            toQueue(LanguageExt.List.map(queue, map));

        [Pure]
        public static Que<T> Filter<T>(this Que<T> queue, Func<T, bool> predicate) =>
            toQueue(LanguageExt.List.filter(queue, predicate));

        [Pure]
        public static Que<U> Choose<T, U>(this Que<T> queue, Func<T, Option<U>> selector) =>
            toQueue(LanguageExt.List.choose(queue, selector));

        [Pure]
        public static Que<U> Choose<T, U>(this Que<T> queue, Func<int, T, Option<U>> selector) =>
            toQueue(LanguageExt.List.choose(queue, selector));

        [Pure]
        public static Que<R> Collect<T, R>(this Que<T> queue, Func<T, IEnumerable<R>> map) =>
            toQueue(LanguageExt.List.collect(queue, map));

        [Pure]
        public static Que<T> Rev<T>(this Que<T> queue) =>
            toQueue(LanguageExt.List.rev(queue));

        [Pure]
        public static Que<T> Append<T>(this Que<T> lhs, IEnumerable<T> rhs) =>
            toQueue(LanguageExt.List.append(lhs, rhs));

        [Pure]
        public static S Fold<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
            LanguageExt.List.fold(queue, state, folder);

        [Pure]
        public static S FoldBack<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
            LanguageExt.List.foldBack(queue, state, folder);

        [Pure]
        public static T ReduceBack<T>(this Que<T> queue, Func<T, T, T> reducer) =>
            LanguageExt.List.reduceBack(queue, reducer);

        [Pure]
        public static T Reduce<T>(this Que<T> queue, Func<T, T, T> reducer) =>
            LanguageExt.List.reduce(queue, reducer);

        [Pure]
        public static Que<S> Scan<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
            toQueue(LanguageExt.List.scan(queue, state, folder));

        [Pure]
        public static IEnumerable<S> ScanBack<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
            toQueue(LanguageExt.List.scanBack(queue, state, folder));

        [Pure]
        public static Option<T> Find<T>(this Que<T> queue, Func<T, bool> pred) =>
            LanguageExt.List.find(queue, pred);

        [Pure]
        public static int Length<T>(this Que<T> queue) =>
            LanguageExt.List.length(queue);

        public static Unit Iter<T>(this Que<T> queue, Action<T> action) =>
            LanguageExt.List.iter(queue, action);

        public static Unit Iter<T>(this Que<T> queue, Action<int, T> action) =>
            LanguageExt.List.iter(queue, action);

        [Pure]
        public static bool ForAll<T>(this Que<T> queue, Func<T, bool> pred) =>
            LanguageExt.List.forall(queue, pred);

        [Pure]
        public static Que<T> Distinct<T>(this Que<T> queue) =>
            toQueue(LanguageExt.List.distinct(queue));

        [Pure]
        public static Que<T> Distinct<EQ, T>(this Que<T> list) where EQ : struct, Eq<T> =>
            toQueue(LanguageExt.List.distinct<EQ, T>(list));

        [Pure]
        public static bool Exists<T>(this Que<T> queue, Func<T, bool> pred) =>
            LanguageExt.List.exists(queue, pred);
    }
}
