using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// A unit type that represents `Option.None`.  This type can be implicitly
    /// converted to Option or OptionUnsafe.
    /// </summary>
    public struct OptionNone
    {
        public static OptionNone Default = new OptionNone();
    }
}
