using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageExt
{
    internal class SeqTake<A> : Seq<A>
    {
        readonly Seq<A> seq;
        readonly int taken;

        public SeqTake(Seq<A> seq, int taken)
        {
            this.seq = seq;
            this.taken = taken;
        }

        public override A Head => 
            seq.Head;

        public override Seq<A> Tail => 
            seq.Tail;

        public override bool IsEmpty =>
            taken == 0;

        public override int Count =>
            taken;

        internal override bool IsTerminator => 
            true;

        public override IEnumerable<A> AsEnumerable()
        {
            var iter = seq.GetEnumerator();
            for (int i = 0; i < taken && iter.MoveNext(); i++)
            {
                yield return iter.Current;
            }
        }

        public override bool Exists(Func<A, bool> f) =>
            AsEnumerable().Exists(f);

        public override S Fold<S>(S state, Func<S, A, S> f) =>
            AsEnumerable().Fold(state, f);

        public override S FoldBack<S>(S state, Func<S, A, S> f) =>
            AsEnumerable().FoldBack(state, f);

        public override bool ForAll(Func<A, bool> f) =>
            AsEnumerable().ForAll(f);

        public override Seq<A> Skip(int count) =>
            SeqEnumerable2<A>.New(AsEnumerable().Skip(count));

        public override Seq<A> Take(int count) =>
            count >= taken
                ? this
                : new SeqTake<A>(seq, count);

        public override Seq<A> TakeWhile(Func<A, bool> pred)
        {
            var take = 0;
            foreach(var item in AsEnumerable())
            {
                if (!pred(item)) return Take(take);
                take++;
            }
            return this;
        }

        public override Seq<A> TakeWhile(Func<A, int, bool> pred)
        {
            var take = 0;
            foreach (var item in AsEnumerable())
            {
                if (!pred(item, take)) return Take(take);
                take++;
            }
            return this;
        }
    }
}
