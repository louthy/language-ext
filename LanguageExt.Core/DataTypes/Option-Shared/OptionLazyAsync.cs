using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Delegate data-type that represents an optional value.  This is used internally
    /// by OptionAsync and OptionUnsafeAsync to represent its state.  Being a delegate it allows
    /// for lazy and strict behaviours.
    /// </summary>
    internal delegate Task<Result<A>> OptionLazyAsync<A>();
}
