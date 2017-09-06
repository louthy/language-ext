using System;
using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqArray<A> : Seq<A>
    {
        readonly A[] list;
        readonly int index;
        readonly int count;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqArray(A[] list, int index, int count)
        {
            this.list = list;
            this.index = index;
            this.count = count == -1
                ? list.Length - index
                : count;
        }

        public override int Count =>
            count;

        public override A Head =>
            list[index];

        public override bool IsEmpty =>
            false;

        public static Seq<A> New(A[] seq, int index = 0, int count = -1) =>
            seq.Length == 0
                ? Empty
                : new SeqArray<A>(seq, index, count);

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        public override IEnumerable<A> AsEnumerable()
        {
            for (int i = index; i < index + count; i++)
            {
                yield return list[i];
            }
        }

        /// <summary>
        /// Get an enumerator for the sequence
        /// </summary>
        /// <returns>An IEnumerator of As</returns>
        public override IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Seq<A> Tail =>
            count == 1
                ? Empty
                : new SeqArray<A>(list, index + 1, count - 1);

        /// <summary>
        /// Skip count items
        /// </summary>
        public override Seq<A> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            if (skipCount >= count) return Empty;
            return new SeqArray<A>(list, index + skipCount, count - skipCount);
        }

        public override S Fold<S>(S state, Func<S, A, S> f)
        {
            for (int i = index; i < index + count; i++)
            {
                state = f(state, list[i]);
            }
            return state;
        }

        public override S FoldBack<S>(S state, Func<S, A, S> f)
        {
            for (int i = index + count - 1; i >= index; i--)
            {
                state = f(state, list[i]);
            }
            return state;
        }

        public override bool Exists(Func<A, bool> f)
        {
            for (int i = index; i < index + count; i++)
            {
                if (f(list[i])) return true;
            }
            return false;
        }

        public override bool ForAll(Func<A, bool> f)
        {
            for (int i = index; i < index + count; i++)
            {
                if (!f(list[i])) return false;
            }
            return true;
        }

        public override Seq<A> Take(int takeCount) =>
            takeCount > 0 && takeCount < count
                ? new SeqArray<A>(list, index, takeCount)
                : this;

        public override Seq<A> TakeWhile(Func<A, bool> pred)
        {
            for (int i = index; i < index + count; i++)
            {
                if (!pred(list[i]))
                {
                    return Take(i - index);
                }
            }
            return this;
        }

        public override Seq<A> TakeWhile(Func<A, int, bool> pred)
        {
            for (int i = index, zeroIndex = 0; i < index + count; i++, zeroIndex++)
            {
                if (!pred(list[i], zeroIndex))
                {
                    return Take(i - index);
                }
            }
            return this;
        }

        internal override bool IsTerminator => true;
    }
}
