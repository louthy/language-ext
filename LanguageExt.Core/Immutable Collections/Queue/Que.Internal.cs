#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    [Serializable]
    internal class QueInternal<A> : IEnumerable<A>
    {
        public static readonly QueInternal<A> Empty = new ();

        readonly StckInternal<A> forward;
        readonly StckInternal<A> backward;
        StckInternal<A>? backwardRev;
        int hashCode;

        internal QueInternal()
        {
            forward = StckInternal<A>.Empty;
            backward = StckInternal<A>.Empty;
        }

        internal QueInternal(IEnumerable<A> items)
        {
            var q = new QueInternal<A>();
            foreach(var item in items)
            {
                q = q.Enqueue(item);
            }
            forward = q.forward;
            backward = q.backward;
            backwardRev = q.backwardRev;
        }

        private QueInternal(StckInternal<A> f, StckInternal<A> b)
        {
            forward = f;
            backward = b;
        }

        private StckInternal<A> BackwardRev
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
        public QueInternal<A> Clear() =>
            Empty;

        [Pure]
        public A Peek() =>
            forward.Peek();

        [Pure]
        public QueInternal<A> Dequeue()
        {
            var f = forward.Pop();
            if (!f.IsEmpty)
            {
                return new QueInternal<A>(f, backward);
            }
            if (backward.IsEmpty)
            {
                return Empty;
            }
            return new QueInternal<A>(BackwardRev, StckInternal<A>.Empty);
        }

        [Pure]
        public QueInternal<A> Dequeue(out A outValue)
        {
            outValue = Peek();
            return Dequeue();
        }

        [Pure]
        public (QueInternal<A>, Option<A>) TryDequeue() =>
            forward.TryPeek().Match(
                Some: x => (Dequeue(), Some(x)),
                None: () => (this, Option<A>.None)
            );

        [Pure]
        public Option<A> TryPeek() =>
            forward.TryPeek();

        [Pure]
        public QueInternal<A> Enqueue(A value) =>
            IsEmpty
                ? new QueInternal<A>(StckInternal<A>.Empty.Push(value), StckInternal<A>.Empty)
                : new QueInternal<A>(forward, backward.Push(value));

        [Pure]
        public Seq<A> ToSeq() =>
            toSeq(forward.AsEnumerable().ConcatFast(BackwardRev));

        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            forward.AsEnumerable().ConcatFast(BackwardRev);

        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            forward.AsEnumerable().ConcatFast(BackwardRev).GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            forward.AsEnumerable().ConcatFast(BackwardRev).GetEnumerator();

        [Pure]
        public static QueInternal<A> operator +(QueInternal<A> lhs, QueInternal<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public QueInternal<A> Append(QueInternal<A> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Enqueue(item);
            }
            return self;
        }

        [Pure]
        public override int GetHashCode() =>
            hashCode == 0
                ? hashCode = FNV32.Hash<HashableDefault<A>, A>(this)
                : hashCode;
    }
}
