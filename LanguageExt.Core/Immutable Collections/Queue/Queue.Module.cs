using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static class Queue
{
    [Pure]
    public static Que<A> singleton<A>(A item) =>
        [item];
    
    [Pure]
    public static Que<A> createRange<A>(IEnumerable<A> items) =>
        new (items);
    
    [Pure]
    public static Que<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.IsEmpty
            ? Que<A>.Empty
            : new (items);

    [Pure]
    public static Que<A> enq<A>(Que<A> queue, A value) =>
        queue.Enqueue(value);

    [Pure]
    public static A peekUnsafe<A>(Que<A> queue) =>
        queue.PeekUnsafe();

    [Pure]
    public static Option<A> peek<A>(Que<A> queue) =>
        queue.Peek();

    [Pure]
    public static bool tryPeek<A>(Que<A> queue, out A value) =>
        queue.TryPeek(out value);

    [Pure]
    public static Que<A> clear<A>(Que<A> queue) =>
        queue.Clear();

    [Pure]
    public static Que<B> map<A, B>(Que<A> queue, Func<A, long, B> map) =>
        queue.Map(map);

    [Pure]
    public static Que<A> filter<A>(Que<A> queue, Func<A, bool> predicate) =>
        queue.Filter(predicate);

    [Pure]
    public static Que<B> choose<A, B>(Que<A> queue, Func<A, Option<B>> selector) =>
        queue.Choose(selector);
    
    [Pure]
    public static Que<A> rev<A>(Que<A> queue) =>
        queue.Reverse();

    [Pure]
    public static Que<A> append<A>(Que<A> lhs, IEnumerable<A> rhs) =>
        lhs.Append(rhs);

    [Pure]
    public static S fold<S, A>(Que<A> queue, S state, Func<S, A, S> folder) =>
        Foldable.fold(folder, state, queue);

    [Pure]
    public static Que<C> zip<T, A, B, C>(Que<A> queue, K<T, B> other, Func<A, B, C> zipper) 
        where T : IterableK<T> =>
        toQueue(queue.ForwardIterator().Zip(other.ForwardIterator(), zipper));

    [Pure]
    public static Que<T> distinct<T>(Que<T> queue) =>
        queue.Distinct();

    [Pure]
    public static Que<T> distinct<EQ, T>(Que<T> queue) where EQ : Eq<T> =>
        queue.Distinct<EQ, T>();
}
