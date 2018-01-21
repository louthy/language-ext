using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Fully lazy enumerable, the Head isn't loaded until accessed, unlike
    /// SeqEnumerable which always takes the first item immediately.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    internal class SeqEnumerable2<A> : Seq<A>
    {
        readonly IEnumerable<A> seq;
        Seq<A> cached;
        object sync = new object();

        /// <summary>
        /// Construct a new sequence
        /// </summary>
        SeqEnumerable2(IEnumerable<A> seq) =>
            this.seq = seq;

        public static Seq<A> New(IEnumerable<A> seq) =>
            new SeqEnumerable2<A>(seq);

        public override int Count =>
            Run().Count;

        public override bool IsEmpty =>
            Run().IsEmpty;

        Seq<A> Run()
        {
            if (cached == null)
            {
                lock (sync)
                {
                    if (cached == null)
                    {
                        var iter = seq.GetEnumerator();
                        if (iter.MoveNext())
                        {
                            cached = new SeqEnumerable<A>(iter.Current, iter);
                        }
                        else
                        {
                            cached = Empty;
                        }
                    }
                }
                return cached;
            }
            else
            {
                return cached;
            }
        }

        public override A Head =>
            Run().Head;

        /// <summary>
        /// Get an enumerator for the sequence
        /// </summary>
        /// <returns>An IEnumerator of As</returns>
        public override IEnumerator<A> GetEnumerator() =>
            Run().GetEnumerator();

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Seq<A> Tail =>
            Run().Tail;

        internal override bool IsTerminator => 
            true;

        public override S Fold<S>(S state, Func<S, A, S> f) =>
            Run().Fold(state, f);

        public override S FoldBack<S>(S state, Func<S, A, S> f) =>
            Run().FoldBack(state, f);

        public override bool Exists(Func<A, bool> f) =>
            Run().Exists(f);

        public override bool ForAll(Func<A, bool> f) =>
            Run().ForAll(f);

        public override Seq<A> Skip(int count) =>
            Run().Skip(count);

        public override Seq<A> Take(int count) =>
            Run().Take(count);

        public override Seq<A> TakeWhile(Func<A, bool> pred) =>
            Run().TakeWhile(pred);

        public override Seq<A> TakeWhile(Func<A, int, bool> pred) =>
            Run().TakeWhile(pred);
    }
}
