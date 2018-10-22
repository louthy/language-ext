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
}
