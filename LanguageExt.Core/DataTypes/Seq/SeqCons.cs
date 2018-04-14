using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt
{
    internal class SeqCons<A> : Seq<A>
    {
        readonly Seq<A> tail;
        readonly A head;
        int count = -1;

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        internal SeqCons(A head, Seq<A> tail)
        {
            this.head = head;
            this.tail = tail;
        }

        public override int Count =>
            count == -1
                ? count = GetCount()
                : count;

        public static Seq<A> New(A head, Seq<A> tail) =>
            new SeqCons<A>(head, tail);

        public override A Head =>
            head;

        public override Seq<A> Tail => 
            tail;

        public override bool IsEmpty =>
            false;

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

                var currentTail = current.Tail;

                if (currentTail.IsTerminator)
                {
                    foreach (var item in currentTail)
                    {
                        yield return item;
                    }
                    current = Empty;
                }
                else
                {
                    current = current.Tail;
                }
            }
        }

        public override S Fold<S>(S state, Func<S, A, S> f)
        {
            foreach (var item in AsEnumerable())
            {
                state = f(state, item);
            }
            return state;
        }

        public override S FoldBack<S>(S state, Func<S, A, S> f)
        {
            foreach (var item in AsEnumerable().Reverse())
            {
                state = f(state, item);
            }
            return state;
        }

        public override bool Exists(Func<A, bool> f)
        {
            foreach (var item in AsEnumerable())
            {
                if (f(item)) return true;
            }
            return false;
        }

        public override bool ForAll(Func<A, bool> f)
        {
            foreach(var item in AsEnumerable())
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
            while(!current.IsEmpty)
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
            foreach(var item in AsEnumerable())
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

        internal override bool IsTerminator => false;
    }
}
