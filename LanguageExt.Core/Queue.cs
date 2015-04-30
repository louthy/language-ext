using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Queue
    {
        public static IImmutableQueue<T> enq<T>(IImmutableQueue<T> queue, T value) =>
            queue.Enqueue(value);

        public static Tuple<IImmutableQueue<T>, T> deqUnsafe<T>(IImmutableQueue<T> queue)
        {
            T value;
            var newqueue = queue.Dequeue(out value);
            return tuple(newqueue, value);
        }

        public static Tuple<IImmutableQueue<T>, Option<T>> deq<T>(IImmutableQueue<T> queue)
        {
            try
            {
                T value;
                var newqueue = queue.Dequeue(out value);
                return tuple(newqueue, Some(value));
            }
            catch (InvalidOperationException)
            {
                return tuple(queue, Option<T>.None);
            }
        }

        public static T peekUnsafe<T>(IImmutableQueue<T> queue) =>
            queue.Peek();

        public static Option<T> peek<T>(IImmutableQueue<T> queue)
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

        public static IImmutableQueue<T> clear<T>(IImmutableQueue<T> queue) =>
            queue.Clear();

        public static IEnumerable<R> map<T, R>(IImmutableQueue<T> queue, Func<int, T, R> map) =>
            List.map(queue, map);

        public static IEnumerable<T> filter<T>(IImmutableQueue<T> queue, Func<T, bool> predicate) =>
            List.filter(queue, predicate);

        public static IEnumerable<T> choose<T>(IImmutableQueue<T> queue, Func<T, Option<T>> selector) =>
            List.choose(queue, selector);

        public static IEnumerable<T> choose<T>(IImmutableQueue<T> queue, Func<int, T, Option<T>> selector) =>
            List.choose(queue, selector);

        public static IEnumerable<R> collect<T, R>(IImmutableQueue<T> queue, Func<T, IEnumerable<R>> map) =>
            List.collect(queue, map);

        public static IEnumerable<T> rev<T>(IImmutableQueue<T> queue) =>
            List.rev(queue);

        public static IEnumerable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            List.append(lhs, rhs);

        public static S fold<S, T>(IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
            List.fold(queue, state, folder);

        public static S foldBack<S, T>(IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
            List.foldBack(queue, state, folder);

        public static T reduce<T>(IImmutableQueue<T> queue, Func<T, T, T> reducer) =>
            List.reduce(queue, reducer);

        public static T reduceBack<T>(IImmutableQueue<T> queue, Func<T, T, T> reducer) =>
            List.reduceBack(queue, reducer);

        public static IEnumerable<S> scan<S, T>(IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
            List.scan(queue, state, folder);

        public static IEnumerable<S> scanBack<S, T>(IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
            List.scanBack(queue, state, folder);

        public static Option<T> find<T>(IImmutableQueue<T> queue, Func<T, bool> pred) =>
            List.find(queue, pred);

        public static IEnumerable<V> zip<T, U, V>(IImmutableQueue<T> queue, IEnumerable<U> other, Func<T, U, V> zipper) =>
            List.zip(queue, other, zipper);

        public static int length<T>(IImmutableQueue<T> queue) =>
            List.length(queue);

        public static Unit iter<T>(IImmutableQueue<T> queue, Action<T> action) =>
            List.iter(queue, action);

        public static Unit iter<T>(IImmutableQueue<T> queue, Action<int, T> action) =>
            List.iter(queue, action);

        public static bool forall<T>(IImmutableQueue<T> queue, Func<T, bool> pred) =>
            List.forall(queue, pred);

        public static IEnumerable<T> distinct<T>(IImmutableQueue<T> queue) =>
            List.distinct(queue);

        public static IEnumerable<T> distinct<T>(IImmutableQueue<T> queue, Func<T, T, bool> compare) =>
            List.distinct(queue, compare);

        public static IEnumerable<T> take<T>(IImmutableQueue<T> queue, int count) =>
            List.take(queue, count);

        public static IEnumerable<T> takeWhile<T>(IImmutableQueue<T> queue, Func<T, bool> pred) =>
            List.takeWhile(queue, pred);

        public static IEnumerable<T> takeWhile<T>(IImmutableQueue<T> queue, Func<T, int, bool> pred) =>
            List.takeWhile(queue, pred);

        public static bool exists<T>(IImmutableQueue<T> queue, Func<T, bool> pred) =>
            List.exists(queue, pred);
    }
}

public static class __QueueExt
{
    public static Tuple<IImmutableQueue<T>, T> PopUnsafe<T>(this IImmutableQueue<T> queue) =>
        Queue.deqUnsafe(queue);

    public static Tuple<IImmutableQueue<T>, Option<T>> Pop<T>(this IImmutableQueue<T> queue) =>
        Queue.deq(queue);

    public static T PeekUnsafe<T>(this IImmutableQueue<T> queue) =>
        Queue.peekUnsafe(queue);

    public static Option<T> Peek<T>(this IImmutableQueue<T> queue) =>
        Queue.peek(queue);

    public static IEnumerable<R> Map<T, R>(this IImmutableQueue<T> queue, Func<T, R> map) =>
        List.map(queue, map);

    public static IEnumerable<R> Map<T, R>(this IImmutableQueue<T> queue, Func<int, T, R> map) =>
        List.map(queue, map);

    public static IEnumerable<T> Filter<T>(this IImmutableQueue<T> queue, Func<T, bool> predicate) =>
        List.filter(queue, predicate);

    public static IEnumerable<T> Choose<T>(this IImmutableQueue<T> queue, Func<T, Option<T>> selector) =>
        List.choose(queue, selector);

    public static IEnumerable<T> Choose<T>(this IImmutableQueue<T> queue, Func<int, T, Option<T>> selector) =>
        List.choose(queue, selector);

    public static IEnumerable<R> Collect<T, R>(this IImmutableQueue<T> queue, Func<T, IEnumerable<R>> map) =>
        List.collect(queue, map);

    public static IEnumerable<T> Rev<T>(this IImmutableQueue<T> queue) =>
        List.rev(queue);

    public static IEnumerable<T> Append<T>(this IImmutableQueue<T> lhs, IEnumerable<T> rhs) =>
        List.append(lhs, rhs);

    public static S Fold<S, T>(this IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
        List.fold(queue, state, folder);

    public static S FoldBack<S, T>(this IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
        List.foldBack(queue, state, folder);

    public static T ReduceBack<T>(this IImmutableQueue<T> queue, Func<T, T, T> reducer) =>
        List.reduceBack(queue, reducer);

    public static T Reduce<T>(this IImmutableQueue<T> queue, Func<T, T, T> reducer) =>
        List.reduce(queue, reducer);

    public static IEnumerable<S> Scan<S, T>(this IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
        List.scan(queue, state, folder);

    public static IEnumerable<S> ScanBack<S, T>(this IImmutableQueue<T> queue, S state, Func<T, S, S> folder) =>
        List.scanBack(queue, state, folder);

    public static Option<T> Find<T>(this IImmutableQueue<T> queue, Func<T, bool> pred) =>
        List.find(queue, pred);

    public static int Length<T>(this IImmutableQueue<T> queue) =>
        List.length(queue);

    public static Unit Iter<T>(this IImmutableQueue<T> queue, Action<T> action) =>
        List.iter(queue, action);

    public static Unit Iter<T>(this IImmutableQueue<T> queue, Action<int, T> action) =>
        List.iter(queue, action);

    public static bool ForAll<T>(this IImmutableQueue<T> queue, Func<T, bool> pred) =>
        List.forall(queue, pred);

    public static IEnumerable<T> Distinct<T>(IImmutableQueue<T> queue, Func<T, T, bool> compare) =>
        List.distinct(queue, compare);

    public static bool Exists<T>(IImmutableQueue<T> queue, Func<T, bool> pred) =>
        List.exists(queue, pred);
}
