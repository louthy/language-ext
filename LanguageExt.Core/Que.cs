using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
#if !COREFX
    [Serializable]
#endif
    public class Que<T> : IEnumerable<T>, IEnumerable, IAppendable<Que<T>>
    {
        public readonly static Que<T> Empty = new Que<T>();

        readonly Stck<T> forward;
        readonly Stck<T> backward;
        Stck<T> backwardRev;

        internal Que()
        {
            forward = Stck<T>.Empty;
            backward = Stck<T>.Empty;
        }

        private Que(Stck<T> f, Stck<T> b)
        {
            forward = f;
            backward = b;
        }

        private Stck<T> BackwardRev
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
        public Que<T> Clear() =>
            Empty;

        [Pure]
        public T Peek() =>
            forward.Peek();

        [Pure]
        public Que<T> Dequeue()
        {
            var f = forward.Pop();
            if (!f.IsEmpty)
            {
                return new Que<T>(f, backward);
            }
            else if (backward.IsEmpty)
            {
                return Empty;
            }
            else
            {
                return new Que<T>(BackwardRev, Stck<T>.Empty);
            }
        }

        [Pure]
        public Que<T> Dequeue(out T outValue)
        {
            outValue = Peek();
            return Dequeue();
        }

        [Pure]
        public Tuple<Que<T>, Option<T>> TryDequeue() =>
            forward.TryPeek().Match(
                Some: x => Tuple(Dequeue(), Some(x)),
                None: () => Tuple(this, Option<T>.None)
            );

        [Pure]
        public Option<T> TryPeek() =>
            forward.TryPeek();

        [Pure]
        public Que<T> Enqueue(T value) =>
            IsEmpty
                ? new Que<T>(Stck<T>.Empty.Push(value), Stck<T>.Empty)
                : new Que<T>(forward, backward.Push(value));

        [Pure]
        public IEnumerable<T> AsEnumerable() =>
            forward.AsEnumerable().Concat(BackwardRev);

        [Pure]
        public IEnumerator<T> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        [Pure]
        public static Que<T> operator +(Que<T> lhs, Que<T> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Que<T> Append(Que<T> rhs)
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
