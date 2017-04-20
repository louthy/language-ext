using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqArray<A> : Seq<A>
    {
        readonly A[] list;
        readonly int index;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqArray(A head, A[] list, int index) : base(head, list.Length - index + 1)
        {
            this.list = list;
            this.index = index;
        }

        public static Seq<A> New(A head, A[] tail) =>
            new SeqArray<A>(head, tail, 0);

        public static Seq<A> New(A[] seq) =>
            seq.Length == 0
                ? Empty
                : new SeqArray<A>(seq[0], seq, 1);

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
           index == list.Length
                ? Empty
                : new SeqArray<A>(list[index], list, index + 1);

        /// <summary>
        /// Skip count items
        /// </summary>
        public override Seq<A> Skip(int count)
        {
            if (count == 0) return this;
            if (count >= Count) return Empty;
            var index = this.index + count;
            return new SeqArray<A>(list[index - 1], list, index);
        }
    }
}
