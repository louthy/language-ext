using System;
using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqLst<A> : Seq<A>
    {
        readonly IEnumerator<A> iter;
        readonly Lst<A> list;
        readonly int index;
        Seq<A> tail;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqLst(A head, IEnumerator<A> iter, Lst<A> list, int index) : base(head, list.Count - index + 1)
        {
            this.iter = iter;
            this.list = list;
            this.index = index;
        }

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        public override IEnumerable<A> AsEnumerable()
        {
            yield return Head;
            foreach(var item in list.Skip(index))
            {
                yield return item;
            }
        }

        public static Seq<A> New(A head, Lst<A> seq) =>
            new SeqLst<A>(head, seq.GetEnumerator(), seq, 0);

        public static Seq<A> New(Lst<A> seq)
        {
            var iter = seq.GetEnumerator();
            return iter.MoveNext()
                ? new SeqLst<A>(iter.Current, iter, seq, 1)
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
                    tail = new SeqLst<A>(iter.Current, iter, list, index + 1);
                }
                else
                {
                    tail = Empty;
                }
            }
            return tail;
        }

        /// <summary>
        /// Skip count items
        /// </summary>
        public override Seq<A> Skip(int count)
        {
            if (count == 0) return this;
            if (count >= Count) return Empty;

            switch(count)
            {
                case 0: return this;
                case 1: return Tail;
                case 2: return Tail.Tail;
                case 3: return Tail.Tail.Tail;

                default:
                    var index = this.index + count;

                    var iter = list.Skip(index - 1).GetEnumerator();
                    if (iter.MoveNext())
                    {
                        return new SeqLst<A>(iter.Current, iter, list, index);
                    }
                    else
                    {
                        return Empty;
                    }
            }
        }
    }
}
