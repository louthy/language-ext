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
    
    extension<A>(Que<A> queue)
    {
        [Pure]
        public Option<A> Peek() =>
            Queue.peek(queue);

        [Pure]
        public Que<B> Map<B>(Func<A, B> map) =>
            toQueue(queue.ForwardIterator().Map(map));

        [Pure]
        public Que<B> Map<B>(Func<A, long, B> map) =>
            toQueue(queue.ForwardIterator().Map(map));

        [Pure]
        public Que<A> Filter(Func<A, bool> predicate) =>
            toQueue(queue.ForwardIterator().Filter(predicate));

        [Pure]
        public Que<B> Choose<B>(Func<A, Option<B>> selector) =>
            toQueue(queue.ForwardIterator().Choose(selector));

        [Pure]
        public Que<A> Reverse() =>
            toQueue(toArr(queue).Reverse());

        [Pure]
        public Que<A> Append(IEnumerable<A> rhs) =>
            toQueue(queue.Concat(rhs));
    }

    [Pure]
    public static Que<A> Distinct<A>(this Que<A> queue) =>
        toQueue(queue.ForwardIterator().Distinct());

    [Pure]
    public static Que<A> Distinct<EqA, A>(this Que<A> list) 
        where EqA : Eq<A> =>
        toQueue(list.ForwardIterator().Distinct<EqA>());
}
