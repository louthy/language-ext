using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class QueExtensions
{
    extension<A>(K<Que, A> queue)
    {
        [Pure]
        public Que<A> As() => 
            (Que<A>)queue;
    }
    
    extension<T>(Que<T> queue)
    {
        [Pure]
        public (Que<T>, T) PopUnsafe() =>
            Queue.deqUnsafe(queue);

        [Pure]
        public (Que<T>, Option<T>) Pop() =>
            Queue.deq(queue);

        [Pure]
        public T PeekUnsafe() =>
            Queue.peekUnsafe(queue);

        [Pure]
        public Option<T> Peek() =>
            Queue.peek(queue);

        [Pure]
        public Que<R> Map<R>(Func<T, R> map) =>
            toQueue(Iterable.map(queue, map));

        [Pure]
        public Que<R> Map<R>(Func<int, T, R> map) =>
            toQueue(Iterable.map(queue, map));

        [Pure]
        public Que<T> Filter(Func<T, bool> predicate) =>
            toQueue(Iterable.filter(queue, predicate));

        [Pure]
        public Que<U> Choose<U>(Func<T, Option<U>> selector) =>
            toQueue(Iterable.choose(queue, selector));

        [Pure]
        public Que<U> Choose<U>(Func<int, T, Option<U>> selector) =>
            toQueue(Iterable.choose(queue, selector));

        [Pure]
        public Que<T> Reverse() =>
            toQueue(toArray(queue).Reverse());

        [Pure]
        public Que<T> Append(IEnumerable<T> rhs) =>
            toQueue(queue.Concat(rhs));
    }

    [Pure]
    public static Que<T> Distinct<T>(this Que<T> queue) =>
        toQueue(Iterable.distinct(queue));

    [Pure]
    public static Que<T> Distinct<EQ, T>(this Que<T> list) where EQ : Eq<T> =>
        toQueue(Iterable.distinct<EQ, T>(list));
}
