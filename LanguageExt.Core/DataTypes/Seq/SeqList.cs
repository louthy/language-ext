using System;
using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqList<A> : Seq<A>
    {
        /*
         * These fields satisfy
         * 0 <= index <  list.Count
         * 0 <  count <= list.Count
         * 0 < index + count <= list.Count
         */
        readonly IList<A> list;
        readonly int index;
        readonly int count;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqList(IList<A> list, int index, int count)
        {
            this.list = list;
            this.index = index;
            this.count = count;
        }

        public override int Count =>
            count;

        public override A Head =>
            list[index];

        public override bool IsEmpty =>
            false;

        public static Seq<A> New(IList<A> list, int index, int count) =>
            count <= 0
                ? Empty
                : new SeqList<A>(list, index, count);

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
                : new SeqList<A>(list, index + 1, count - 1);

        /// <summary>
        /// Skip count items
        /// </summary>
        public override Seq<A> Skip(int skipCount)
        {
            if (skipCount <= 0) return this;
            if (skipCount >= count) return Empty;
            return new SeqList<A>(list, index + skipCount, count - skipCount);
        }

        public override S Fold<S>(S state, Func<S, A, S> f) =>
            AsEnumerable().Fold(state, f);

        public override S FoldBack<S>(S state, Func<S, A, S> f)
        {
            for (int i = index + count - 1; i >= index; i--)
            {
                state = f(state, list[i]);
            }
            return state;
        }

        public override bool Exists(Func<A, bool> f) =>
            AsEnumerable().Exists(f);

        public override bool ForAll(Func<A, bool> f)=>
            AsEnumerable().ForAll(f);

        public override Seq<A> Take(int takeCount)
        {
            if (takeCount <= 0) return Empty;
            if (takeCount >= count) return this;
            return new SeqList<A>(list, index, takeCount);
        }

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
