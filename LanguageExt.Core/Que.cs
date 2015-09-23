using System;
using System.Collections;
using System.Collections.Generic;

namespace LanguageExt
{
    public class Que<T> : IEnumerable<T>, IEnumerable
    {
        Lst<T> queue;

        internal Que()
        {
            queue = Lst<T>.Empty;
        }

        internal Que(IEnumerable<T> initial)
        {
            queue = new Lst<T>(initial);
        }

        internal Que(Lst<T> initial)
        {
            queue = initial;
        }

        public bool IsEmpty =>
            queue.Count == 0;

        public Que<T> Clear() =>
            new Que<T>();

        public IEnumerator<T> GetEnumerator() =>
            queue.GetEnumerator();

        public T Peek()
        {
            if (queue.Count > 0)
            {
                return queue[queue.Count - 1];
            }
            else
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        public Que<T> Dequeue()
        {
            if (queue.Count > 0)
            {
                return new Que<T>(queue.RemoveAt(0));
            }
            else
            {
                throw new InvalidOperationException("Queue is empty");
            }
        }

        public Que<T> Dequeue(out T outValue)
        {
            if (queue.Count > 0)
            {
                outValue = queue[0];
                return new Que<T>(queue.RemoveAt(0));
            }
            else
            {
                throw new InvalidOperationException("Queue is empty");
            }
        }

        public Tuple<Que<T>, Option<T>> TryDequeue()
        {
            if (queue.Count > 0)
            {
                var value = queue[queue.Count - 1];
                return Tuple.Create(Dequeue(), Prelude.Some(value));
            }
            else
            {
                return Tuple.Create<Que<T>, Option<T>>(this, Prelude.None);
            }
        }

        public Option<T> TryPeek()
        {
            if (queue.Count > 0)
            {
                return Peek();
            }
            else
            {
                return Prelude.None;
            }
        }

        public Que<T> Enqueue(T value) =>
            new Que<T>(queue.Add(value));

        IEnumerator IEnumerable.GetEnumerator() =>
            queue.GetEnumerator();
    }
}
