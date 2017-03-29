using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    /// <summary>
    /// Delegate data-type that represents an optional value.  This is used internally
    /// by Option and OptionUnsafe to represent its state.  Being a delegate it allows
    /// for lazy and strict behaviours.
    /// </summary>
    internal delegate (bool IsSome, A Value) OptionLazy<A>();
}
