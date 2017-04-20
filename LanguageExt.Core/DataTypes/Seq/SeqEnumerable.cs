using System;
using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqEnumerable<A> : Seq<A>
    {
        readonly IEnumerator<A> iter;
        Seq<A> tail;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqEnumerable(A head, IEnumerator<A> iter) : base(head, -1) =>
            this.iter = iter;

        public static Seq<A> New(A head, IEnumerable<A> seq) =>
            new SeqEnumerable<A>(head, seq.GetEnumerator());

        public static Seq<A> New(IEnumerable<A> seq)
        {
            var iter = seq.GetEnumerator();
            return iter.MoveNext()
                ? new SeqEnumerable<A>(iter.Current, iter)
                : Empty;
        }

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

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Seq<A> Tail =>
           tail ?? TailLazy();

        /// <summary>
        /// Lazily 
        /// </summary>
        /// <returns></returns>
        Seq<A> TailLazy()
        {
            lock (iter)
            {
                if (tail != null) return tail;
                if (iter.MoveNext())
                {
                    tail = new SeqEnumerable<A>(iter.Current, iter);
                }
                else
                {
                    tail = Empty;
                    count = 1;
                }
            }
            return tail;
        }
    }
}
