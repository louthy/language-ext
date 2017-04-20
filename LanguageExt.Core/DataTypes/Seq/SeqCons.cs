using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqCons<A> : Seq<A>
    {
        readonly Seq<A> tail;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        internal SeqCons(A head, Seq<A> tail) : base(head, tail.count == -1 ? -1 : tail.Count + 1) =>
            this.tail = tail;

        public static Seq<A> New(A head, Seq<A> tail) =>
            new SeqCons<A>(head, tail);

        public override Seq<A> Tail => tail;

        /// <summary>
        /// Get an enumerator for the sequence
        /// </summary>
        /// <returns>An IEnumerator of As</returns>
        public override IEnumerator<A> GetEnumerator()
        {
            Seq<A> current = this;
            while (current != Empty)
            {
                yield return current.Head;
                current = current.Tail;
            }
        }
    }
}
