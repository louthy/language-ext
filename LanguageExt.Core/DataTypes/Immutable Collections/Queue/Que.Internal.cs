using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    [Serializable]
    internal class QueInternal<T> : IEnumerable<T>, IEnumerable
    {
        public readonly static QueInternal<T> Empty = new QueInternal<T>();

        readonly StckInternal<T> forward;
        readonly StckInternal<T> backward;
        StckInternal<T> backwardRev;

        internal QueInternal()
        {
            forward = StckInternal<T>.Empty;
            backward = StckInternal<T>.Empty;
        }

        internal QueInternal(IEnumerable<T> items)
        {
            var q = new QueInternal<T>();
            foreach(var item in items)
            {
                q = q.Enqueue(item);
            }
            forward = q.forward;
            backward = q.backward;
            backwardRev = q.backwardRev;
        }

        private QueInternal(StckInternal<T> f, StckInternal<T> b)
        {
            forward = f;
            backward = b;
        }

        private StckInternal<T> BackwardRev
        {
            get
            {
                if (backwardRev == null)
                {
                    backwardRev = backward.Reverse();
                }
                return backwardRev;
            }
        }

        [Pure]
        public int Count =>
            forward.Count + backward.Count;

        [Pure]
        public bool IsEmpty =>
            forward.IsEmpty && backward.IsEmpty;

        [Pure]
        public QueInternal<T> Clear() =>
            Empty;

        [Pure]
        public T Peek() =>
            forward.Peek();

        [Pure]
        public QueInternal<T> Dequeue()
        {
            var f = forward.Pop();
            if (!f.IsEmpty)
            {
                return new QueInternal<T>(f, backward);
            }
            else if (backward.IsEmpty)
            {
                return Empty;
            }
            else
            {
                return new QueInternal<T>(BackwardRev, StckInternal<T>.Empty);
            }
        }

        [Pure]
        public QueInternal<T> Dequeue(out T outValue)
        {
            outValue = Peek();
            return Dequeue();
        }

        [Pure]
        public (QueInternal<T>, Option<T>) TryDequeue() =>
            forward.TryPeek().Match(
                Some: x => (Dequeue(), Some(x)),
                None: () => (this, Option<T>.None)
            );

        [Pure]
        public Option<T> TryPeek() =>
            forward.TryPeek();

        [Pure]
        public QueInternal<T> Enqueue(T value) =>
            IsEmpty
                ? new QueInternal<T>(StckInternal<T>.Empty.Push(value), StckInternal<T>.Empty)
                : new QueInternal<T>(forward, backward.Push(value));

        [Pure]
        public Seq<T> ToSeq() =>
            Seq(forward.AsEnumerable().ConcatFast(BackwardRev));

        [Pure]
        public IEnumerable<T> AsEnumerable() =>
            forward.AsEnumerable().ConcatFast(BackwardRev);

        [Pure]
        public IEnumerator<T> GetEnumerator() =>
            forward.AsEnumerable().ConcatFast(BackwardRev).GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            forward.AsEnumerable().ConcatFast(BackwardRev).GetEnumerator();

        [Pure]
        public static QueInternal<T> operator +(QueInternal<T> lhs, QueInternal<T> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public QueInternal<T> Append(QueInternal<T> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Enqueue(item);
            }
            return self;
        }
    }
}
