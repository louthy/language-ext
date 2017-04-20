using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    [Serializable]
    public struct Que<T> : 
        IEnumerable<T>, 
        IEnumerable,
        IEquatable<Que<T>>
    {
        public readonly static Que<T> Empty = new Que<T>(QueInternal<T>.Empty);

        int hashCode;
        readonly QueInternal<T> value;
        QueInternal<T> Value => value ?? QueInternal<T>.Empty;

        internal Que(QueInternal<T> value)
        {
            this.value = value;
            this.hashCode = 0;
        }

        public Que(IEnumerable<T> items)
        {
            this.value = new QueInternal<T>(items);
            this.hashCode = 0;
        }

        [Pure]
        public int Count =>
            Value.Count;

        [Pure]
        public bool IsEmpty =>
            Value.IsEmpty;

        [Pure]
        public Que<T> Clear() =>
            Empty;

        [Pure]
        public T Peek() =>
            Value.Peek();

        [Pure]
        public Que<T> Dequeue() =>
            new Que<T>(Value.Dequeue());

        [Pure]
        public (Que<T> Queue, T Value) DequeueUnsafe() =>
            (Dequeue(), Peek());

        [Pure]
        public (Que<T> Queue, Option<T> Value) TryDequeue() =>
            Value.TryDequeue().MapFirst(qi => new Que<T>(qi));

        [Pure]
        public Option<T> TryPeek() =>
            Value.TryPeek();

        [Pure]
        public Que<T> Enqueue(T value) =>
            new Que<T>(Value.Enqueue(value));

        [Pure]
        public Seq<T> ToSeq() =>
            Seq(Value);

        [Pure]
        public IEnumerable<T> AsEnumerable() =>
            Value;

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
        public Que<T> Append(Que<T> rhs) =>
            new Que<T>(Value.Append(rhs));

        [Pure]
        public static Que<T> operator -(Que<T> lhs, Que<T> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public Que<T> Subtract(Que<T> rhs) =>
            new Que<T>(Enumerable.Except(Value.AsEnumerable(), rhs.Value.AsEnumerable()));

        [Pure]
        public static bool operator ==(Que<T> lhs, Que<T> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Que<T> lhs, Que<T> rhs) =>
            !(lhs == rhs);

        [Pure]
        public override int GetHashCode() =>
            hashCode == 0
                ? hashCode = hash(this)
                : hashCode;

        [Pure]
        public override bool Equals(object obj) =>
            obj is Que<T> && Equals((Que<T>)obj);

        [Pure]
        public bool Equals(Que<T> other) =>
            hashCode == other.hashCode && Enumerable.Equals(this.Value, other.Value);
    }
}
