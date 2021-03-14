using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    [Serializable]
    public struct Que<A> : 
        IEnumerable<A>, 
        IEnumerable,
        IEquatable<Que<A>>
    {
        public readonly static Que<A> Empty = new Que<A>(QueInternal<A>.Empty);

        int hashCode;
        readonly QueInternal<A> value;
        QueInternal<A> Value => value ?? QueInternal<A>.Empty;

        internal Que(QueInternal<A> value)
        {
            this.value = value;
            this.hashCode = 0;
        }

        public Que(IEnumerable<A> items)
        {
            this.value = new QueInternal<A>(items);
            this.hashCode = 0;
        }

        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public Que<A> Do(Action<A> f)
        {
            this.Iter(f);
            return this;
        }

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        /// <remarks>
        ///
        ///     Empty collection     = null
        ///     Singleton collection = A
        ///     More                 = (A, Seq<A>)   -- head and tail
        ///
        ///     var res = list.Case switch
        ///     {
        ///       
        ///        (var x, var xs) => ...,
        ///        A value         => ...,
        ///        _               => ...
        ///     }
        /// 
        /// </remarks>
        [Pure]
        public object Case =>
            IsEmpty
                ? null
                : Seq(Value).Case;

        /// <summary>
        /// Is the queue empty
        /// </summary>
        [Pure]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.IsEmpty ?? true;
        }

        /// <summary>
        /// Number of items in the queue
        /// </summary>
        [Pure]
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.Count ?? 0;
        }

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value?.Count ?? 0;
        }

        [Pure]
        public Que<A> Clear() =>
            Empty;

        [Pure]
        public A Peek() =>
            Value.Peek();

        [Pure]
        public Que<A> Dequeue() =>
            new Que<A>(Value.Dequeue());

        [Pure]
        public (Que<A> Queue, A Value) DequeueUnsafe() =>
            (Dequeue(), Peek());

        [Pure]
        public (Que<A> Queue, Option<A> Value) TryDequeue() =>
            Value.TryDequeue().MapFirst(qi => new Que<A>(qi));

        [Pure]
        public Option<A> TryPeek() =>
            Value.TryPeek();

        [Pure]
        public Que<A> Enqueue(A value) =>
            new Que<A>(Value.Enqueue(value));

        [Pure]
        public Seq<A> ToSeq() =>
            Value.ToSeq();

        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            Value.AsEnumerable();

        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        [Pure]
        public static Que<A> operator +(Que<A> lhs, Que<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Que<A> Append(Que<A> rhs) =>
            new Que<A>(Value.Append(rhs));

        [Pure]
        public static Que<A> operator -(Que<A> lhs, Que<A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public Que<A> Subtract(Que<A> rhs) =>
            new Que<A>(Enumerable.Except(Value.AsEnumerable(), rhs.Value.AsEnumerable()));

        [Pure]
        public static bool operator ==(Que<A> lhs, Que<A> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Que<A> lhs, Que<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public override int GetHashCode() =>
            hashCode == 0
                ? hashCode = FNV32.Hash<HashableDefault<A>, A>(this)
                : hashCode;

        [Pure]
        public override bool Equals(object obj) =>
            obj is Que<A> q && Equals(q);

        [Pure]
        public bool Equals(Que<A> other) =>
            GetHashCode() == other.GetHashCode() &&
            default(EqEnumerable<A>).Equals(this.Value, other.Value);

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        public static implicit operator Que<A>(SeqEmpty _) =>
            Empty;
    }
}
