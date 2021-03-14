using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace LanguageExt
{
    /// <summary>
    /// Represents a *mutable* array type that can be sliced to return
    /// new versions of the array which represent subsections of the 
    /// original but without copying to a new buffer.  This means the
    /// underlying array is shared and vulnerable to data race issues 
    /// and the commonly known issues of mutable structures.  
    /// 
    /// It is primarily to facilitate super-fast algorithms that work 
    /// on collections.  The idea being that the algorithm knows about
    /// the issues of mutation and can avoid the problems whilst doing
    /// its thing, but also benefit from a more declarative style by
    /// using `Take`, `Skip`, `Slice`, `Head`, `Tail`, `Elem` etc.
    /// </summary>
    public readonly struct SpanArray<A> : IEnumerable<A>
    {
        public readonly A[] Data;
        public readonly int Count;
        readonly int Index;
        readonly int EndIndex;

        SpanArray(A[] data, int index, int count)
        {
            Data = data;
            Index = index;
            EndIndex = index + count;
            Count = count;
        }

        public Option<A> Elem(int index) =>
            index < 0 || index + Index >= EndIndex
                ? None
                : Some(Data[index + Index]);

        public IEnumerator<A> GetEnumerator()
        {
            for (int i = Index; i < EndIndex; i++)
            {
                yield return Data[i];
            }
        }

        public A this[int index]
        {
            get => index < 0 || index + Index >= EndIndex
                       ? throw new IndexOutOfRangeException()
                       : Data[index + Index];

            set => Data[index + Index] =
                       index < 0 || index + Index >= EndIndex
                           ? throw new IndexOutOfRangeException()
                           : value;
        }

        public static SpanArray<A> New(IEnumerable<A> sequence)
        {
            var data = sequence.ToArray();
            return new SpanArray<A>(data, 0, data.Length);
        }

        public static SpanArray<A> New(A[] data) =>
            new SpanArray<A>(data, 0, data.Length);

        public static SpanArray<A> New(int n) =>
            new SpanArray<A>(new A[n], 0, n);

        public SpanArray<A> Slice(int index, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            var newIndex = Index + index;
            if (newIndex > Index + Count) throw new ArgumentOutOfRangeException(nameof(index));
            if (newIndex + count > Index + Count) throw new ArgumentOutOfRangeException(nameof(count));
            return new SpanArray<A>(Data, newIndex, count);
        }

        public SpanArray<A> Take(int n) =>
            Slice(0, n);

        public SpanArray<A> Skip(int n) =>
            Slice(n, Count - n);

        public SpanArray<A> Tail =>
            Slice(1, Count - 1);

        public A Head =>
            this[0];

        public A Last =>
            this[Count - 1];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public Unit UnsafeCopy(SpanArray<A> dest)
        {
            System.Array.Copy(Data, Index, dest.Data, dest.Index, Count);
            return unit;
        }
    }
}
