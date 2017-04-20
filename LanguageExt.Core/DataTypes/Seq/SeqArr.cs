using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqArr<A> : Seq<A>
    {
        readonly Arr<A> list;
        readonly int index;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqArr(A head, Arr<A> list, int index) : base(head, list.Count - index + 1)
        {
            this.list = list;
            this.index = index;
        }

        public static Seq<A> New(A head, Arr<A> tail) =>
            new SeqArr<A>(head, tail, 0);

        public static Seq<A> New(Arr<A> seq) =>
            seq.Count == 0
                ? Empty
                : new SeqArr<A>(seq[0], seq, 1);

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        public override IEnumerable<A> AsEnumerable()
        {
            yield return Head;
            var to = index + (Count - 1 /* removes the head */);
            for (var i = index; i < to; i++)
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
           index == list.Count
                ? Empty
                : new SeqArr<A>(list[index], list, index + 1);

        /// <summary>
        /// Skip count items
        /// </summary>
        public override Seq<A> Skip(int count)
        {
            if (count == 0) return this;
            if (count >= Count) return Empty;
            var index = this.index + count;
            return new SeqArr<A>(list[index - 1], list, index);
        }
    }
}
