using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static class Queue
{
    [Pure]
    public static Que<T> singleton<T>(T item) =>
        [item];
    
    [Pure]
    public static Que<T> createRange<T>(IEnumerable<T> items) =>
        new (items);
    
    [Pure]
    public static Que<T> createRange<T>(ReadOnlySpan<T> items) =>
        items.IsEmpty
            ? Que<T>.Empty
            : new (items);

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
    public static Que<T> rev<T>(Que<T> queue) =>
        queue.Reverse();

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
        Foldable.fold(folder, state, queue);

    [Pure]
    public static Que<V> zip<T, U, V>(Que<T> queue, IEnumerable<U> other, Func<T, U, V> zipper) =>
        toQueue(Iterable.zip(queue, other, zipper));

    [Pure]
    public static Que<T> distinct<T>(Que<T> queue) =>
        queue.Distinct();

    [Pure]
    public static Que<T> distinct<EQ, T>(Que<T> queue) where EQ : Eq<T> =>
        queue.Distinct<EQ, T>();
}
