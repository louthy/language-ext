using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Queue
    {
        public static Que<T> enq<T>(Que<T> queue, T value) =>
            queue.Enqueue(value);

        public static Tuple<Que<T>, T> deqUnsafe<T>(Que<T> queue)
        {
            T value;
            var newqueue = queue.Dequeue(out value);
            return Tuple(newqueue, value);
        }

        public static Tuple<Que<T>, Option<T>> deq<T>(Que<T> queue)
        {
            try
            {
                T value;
                var newqueue = queue.Dequeue(out value);
                return Tuple(newqueue, Some(value));
            }
            catch (InvalidOperationException)
            {
                return Tuple(queue, Option<T>.None);
            }
        }

        public static T peekUnsafe<T>(Que<T> queue) =>
            queue.Peek();

        public static Option<T> peek<T>(Que<T> queue)
        {
            try
            {
                return Some(queue.Peek());
            }
            catch (InvalidOperationException)
            {
                return None;
            }
        }

        public static Que<T> clear<T>(Que<T> queue) =>
            queue.Clear();

        public static IEnumerable<R> map<T, R>(Que<T> queue, Func<int, T, R> map) =>
            List.map(queue, map);

        public static IEnumerable<T> filter<T>(Que<T> queue, Func<T, bool> predicate) =>
            List.filter(queue, predicate);

        public static IEnumerable<T> choose<T>(Que<T> queue, Func<T, Option<T>> selector) =>
            List.choose(queue, selector);

        public static IEnumerable<T> choose<T>(Que<T> queue, Func<int, T, Option<T>> selector) =>
            List.choose(queue, selector);

        public static IEnumerable<R> collect<T, R>(Que<T> queue, Func<T, IEnumerable<R>> map) =>
            List.collect(queue, map);

        public static IEnumerable<T> rev<T>(Que<T> queue) =>
            List.rev(queue);

        public static IEnumerable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            List.append(lhs, rhs);

        /// <summary>
        /// Folds each value of the QueT into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="queue">Queue to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public static S fold<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.fold(queue, state, folder);

        /// <summary>
        /// Folds each value of the QueT into an S, but in reverse order.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="queue">Queue to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        public static S foldBack<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.foldBack(queue, state, folder);

        public static T reduce<T>(Que<T> queue, Func<T, T, T> reducer) =>
            List.reduce(queue, reducer);

        public static T reduceBack<T>(Que<T> queue, Func<T, T, T> reducer) =>
            List.reduceBack(queue, reducer);

        public static IEnumerable<S> scan<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.scan(queue, state, folder);

        public static IEnumerable<S> scanBack<S, T>(Que<T> queue, S state, Func<S, T, S> folder) =>
            List.scanBack(queue, state, folder);

        public static Option<T> find<T>(Que<T> queue, Func<T, bool> pred) =>
            List.find(queue, pred);

        public static IEnumerable<V> zip<T, U, V>(Que<T> queue, IEnumerable<U> other, Func<T, U, V> zipper) =>
            List.zip(queue, other, zipper);

        public static int length<T>(Que<T> queue) =>
            List.length(queue);

        public static Unit iter<T>(Que<T> queue, Action<T> action) =>
            List.iter(queue, action);

        public static Unit iter<T>(Que<T> queue, Action<int, T> action) =>
            List.iter(queue, action);

        public static bool forall<T>(Que<T> queue, Func<T, bool> pred) =>
            List.forall(queue, pred);

        public static IEnumerable<T> distinct<T>(Que<T> queue) =>
            List.distinct(queue);

        public static IEnumerable<T> distinct<T>(Que<T> queue, Func<T, T, bool> compare) =>
            List.distinct(queue, compare);

        public static IEnumerable<T> take<T>(Que<T> queue, int count) =>
            List.take(queue, count);

        public static IEnumerable<T> takeWhile<T>(Que<T> queue, Func<T, bool> pred) =>
            List.takeWhile(queue, pred);

        public static IEnumerable<T> takeWhile<T>(Que<T> queue, Func<T, int, bool> pred) =>
            List.takeWhile(queue, pred);

        public static bool exists<T>(Que<T> queue, Func<T, bool> pred) =>
            List.exists(queue, pred);
    }
}

public static class __QueueExt
{
    public static Tuple<Que<T>, T> PopUnsafe<T>(this Que<T> queue) =>
        LanguageExt.Queue.deqUnsafe(queue);

    public static Tuple<Que<T>, Option<T>> Pop<T>(this Que<T> queue) =>
        LanguageExt.Queue.deq(queue);

    public static T PeekUnsafe<T>(this Que<T> queue) =>
        LanguageExt.Queue.peekUnsafe(queue);

    public static Option<T> Peek<T>(this Que<T> queue) =>
        LanguageExt.Queue.peek(queue);

    public static IEnumerable<R> Map<T, R>(this Que<T> queue, Func<T, R> map) =>
        LanguageExt.List.map(queue, map);

    public static IEnumerable<R> Map<T, R>(this Que<T> queue, Func<int, T, R> map) =>
        LanguageExt.List.map(queue, map);

    public static IEnumerable<T> Filter<T>(this Que<T> queue, Func<T, bool> predicate) =>
        LanguageExt.List.filter(queue, predicate);

    public static IEnumerable<T> Choose<T>(this Que<T> queue, Func<T, Option<T>> selector) =>
        LanguageExt.List.choose(queue, selector);

    public static IEnumerable<T> Choose<T>(this Que<T> queue, Func<int, T, Option<T>> selector) =>
        LanguageExt.List.choose(queue, selector);

    public static IEnumerable<R> Collect<T, R>(this Que<T> queue, Func<T, IEnumerable<R>> map) =>
        LanguageExt.List.collect(queue, map);

    public static IEnumerable<T> Rev<T>(this Que<T> queue) =>
        LanguageExt.List.rev(queue);

    public static IEnumerable<T> Append<T>(this Que<T> lhs, IEnumerable<T> rhs) =>
        LanguageExt.List.append(lhs, rhs);

    public static S Fold<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
        LanguageExt.List.fold(queue, state, folder);

    public static S FoldBack<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
        LanguageExt.List.foldBack(queue, state, folder);

    public static T ReduceBack<T>(this Que<T> queue, Func<T, T, T> reducer) =>
        LanguageExt.List.reduceBack(queue, reducer);

    public static T Reduce<T>(this Que<T> queue, Func<T, T, T> reducer) =>
        LanguageExt.List.reduce(queue, reducer);

    public static IEnumerable<S> Scan<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scan(queue, state, folder);

    public static IEnumerable<S> ScanBack<S, T>(this Que<T> queue, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scanBack(queue, state, folder);

    public static Option<T> Find<T>(this Que<T> queue, Func<T, bool> pred) =>
        LanguageExt.List.find(queue, pred);

    public static int Length<T>(this Que<T> queue) =>
        LanguageExt.List.length(queue);

    public static Unit Iter<T>(this Que<T> queue, Action<T> action) =>
        LanguageExt.List.iter(queue, action);

    public static Unit Iter<T>(this Que<T> queue, Action<int, T> action) =>
        LanguageExt.List.iter(queue, action);

    public static bool ForAll<T>(this Que<T> queue, Func<T, bool> pred) =>
        LanguageExt.List.forall(queue, pred);

    public static IEnumerable<T> Distinct<T>(Que<T> queue, Func<T, T, bool> compare) =>
        LanguageExt.List.distinct(queue, compare);

    public static bool Exists<T>(Que<T> queue, Func<T, bool> pred) =>
        LanguageExt.List.exists(queue, pred);
}
