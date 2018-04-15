using System;
using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqLst<A> : Seq<A>
    {
        readonly Lst<A> list;
        readonly int index;
        readonly int count;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqLst(Lst<A> list, int index, int count)
        {
            this.list = list;
            this.index = index;
            this.count = count == -1
                ? list.Count - index
                : count;
        }

        public override int Count =>
            count;

        public override A Head =>
            list[index];

        public override bool IsEmpty =>
            false;

        public static Seq<A> New(Lst<A> seq, int index = 0, int count = -1) =>
            seq.Count == 0
                ? Empty
                : new SeqLst<A>(seq, index, count);

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        public override IEnumerable<A> AsEnumerable() =>
            list.FindRange(index, count);

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
                : new SeqLst<A>(list, index + 1, count - 1);

        /// <summary>
        /// Skip count items
        /// </summary>
        public override Seq<A> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            if (skipCount >= count) return Empty;
            return new SeqLst<A>(list, index + skipCount, count - skipCount);
        }

        public override S Fold<S>(S state, Func<S, A, S> f) =>
            AsEnumerable().Fold(state, f);

        public override S FoldBack<S>(S state, Func<S, A, S> f)
        {
            foreach (var item in list.Reverse().FindRange(index, count))
            {
                state = f(state, item);
            }
            return state;
        }

        public override bool Exists(Func<A, bool> f) =>
            AsEnumerable().Exists(f);

        public override bool ForAll(Func<A, bool> f) =>
            AsEnumerable().ForAll(f);

        public override Seq<A> Take(int takeCount) =>
            takeCount > 0 && takeCount < count
                ? new SeqLst<A>(list, index, takeCount)
                : this;

        public override Seq<A> TakeWhile(Func<A, bool> pred)
        {
            int takeCount = 0;
            foreach (var item in AsEnumerable())
            {
                if (!pred(item))
                {
                    return Take(takeCount);
                }
                takeCount++;
            }
            return this;
        }

        public override Seq<A> TakeWhile(Func<A, int, bool> pred)
        {
            int takeCount = 0;
            foreach (var item in AsEnumerable())
            {
                if (!pred(item, takeCount))
                {
                    return Take(takeCount);
                }
                takeCount++;
            }
            return this;
        }

        internal override bool IsTerminator => true;
    }
}
