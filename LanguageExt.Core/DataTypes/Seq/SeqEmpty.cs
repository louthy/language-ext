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

        public SeqEmpty() : base(default(A), 0)
        {
        }

        public override IEnumerable<A> AsEnumerable() =>
            new A[0];

        public override IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();
        
        public override A Head => 
            throw new InvalidOperationException("Can't call Head on an empty sequence.  Use matching or test for IsEmpty to avoid.");

        public override Seq<A> Tail => this;
    }
}
