using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    internal class SeqEnumerable<A> : Seq<A>
    {
        readonly IEnumerator<A> iter;
        readonly A head;
        Seq<A> tail;
        int count = -1;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        internal SeqEnumerable(A head, IEnumerator<A> iter)
        {
            this.head = head;
            this.iter = iter;
        }

        public override int Count =>
            count == -1
                ? count = GetCount()
                : count;

        public override bool IsEmpty => 
            false;

        public override A Head =>
            head;

        public static Seq<A> New(A head, IEnumerable<A> seq) =>
            new SeqEnumerable<A>(head, seq.GetEnumerator());

        public static Seq<A> New(IEnumerable<A> seq) =>
            SeqEnumerable2<A>.New(seq);

        /// <summary>
        /// Get an enumerator for the sequence
        /// </summary>
        /// <returns>An IEnumerator of As</returns>
        public override IEnumerator<A> GetEnumerator()
        {
            Seq<A> current = this;
            while (!current.IsEmpty)
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

        internal override bool IsTerminator => 
            true;

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

        public override S Fold<S>(S state, Func<S, A, S> f)
        {
            foreach (var item in this)
            {
                state = f(state, item);
            }
            return state;
        }

        public override S FoldBack<S>(S state, Func<S, A, S> f)
        {
            foreach (var item in this.Reverse())
            {
                state = f(state, item);
            }
            return state;
        }

        public override bool Exists(Func<A, bool> f)
        {
            foreach (var item in this)
            {
                if (f(item)) return true;
            }
            return false;
        }

        public override bool ForAll(Func<A, bool> f)
        {
            foreach (var item in this)
            {
                if (!f(item)) return false;
            }
            return true;
        }

        public override Seq<A> Skip(int count)
        {
            if (count == 0) return this;
            if (count == 1) return Tail;

            Seq<A> current = this;
            while (!current.IsEmpty)
            {
                if (current.IsTerminator && !ReferenceEquals(current, this)) return current.Skip(count);
                current = current.Tail;
                count--;
                if (count == 0) return current;
            }
            return Empty;
        }

        public override Seq<A> Take(int count)
        {
            int taken = 0;
            var list = new List<A>();
            foreach (var item in AsEnumerable())
            {
                taken++;
                list.Add(item);
                if (taken == count) break;
            }
            return SeqList<A>.New(list, 0, taken);
        }

        public override Seq<A> TakeWhile(Func<A, bool> pred)
        {
            int taken = 0;
            var list = new List<A>();
            foreach (var item in AsEnumerable())
            {
                if (!pred(item)) break;
                taken++;
                list.Add(item);
                if (taken == count) break;
            }
            return SeqList<A>.New(list, 0, taken);
        }

        public override Seq<A> TakeWhile(Func<A, int, bool> pred)
        {
            int taken = 0;
            var list = new List<A>();
            foreach (var item in AsEnumerable())
            {
                if (!pred(item, taken)) break;
                taken++;
                list.Add(item);
                if (taken == count) break;
            }
            return SeqList<A>.New(list, 0, taken);
        }
    }
}
