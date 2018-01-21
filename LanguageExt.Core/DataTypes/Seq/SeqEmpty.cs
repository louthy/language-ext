using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// A unit type that represents `Seq.Empty`.  This type can be implicitly
    /// converted to `Seq<A>`.
    /// </summary>
    public struct SeqEmpty
    {
        public static SeqEmpty Default = new SeqEmpty();
    }

    /// <summary>
    /// Represents the Empty node at the end of a sequence
    /// </summary>
    internal class SeqEmpty<A> : Seq<A>
    {
        public static SeqEmpty<A> Default = new SeqEmpty<A>();

        public override int Count => 0;

        public override bool IsEmpty => 
            true;

        public override IEnumerable<A> AsEnumerable() =>
            new A[0];

        public override IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        public override S Fold<S>(S state, Func<S, A, S> f) =>
            state;

        public override S FoldBack<S>(S state, Func<S, A, S> f) =>
            state;

        public override bool Exists(Func<A, bool> f) =>
            false;

        public override bool ForAll(Func<A, bool> f) =>
            true;

        public override Seq<A> Skip(int count) =>
            Empty;

        public override Seq<A> Take(int count) =>
            Empty;

        public override Seq<A> TakeWhile(Func<A, bool> pred) =>
            Empty;

        public override Seq<A> TakeWhile(Func<A, int, bool> pred) =>
            Empty;

        public override A Head => 
            throw new InvalidOperationException("Can't call Head on an empty sequence");

        public override Seq<A> Tail => this;

        internal override bool IsTerminator => true;
    }
}
