using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// Meant to represent `void`, but we can't construct a `System.Void`.
    /// 
    /// A `void` is the initial object in a category, equivalent to an empty set, and because there are no values in an
    /// empty set there's no way to construct a type of `void`.  We use this type in the pipes system to represent a
    /// a 'closed' path.
    /// </summary>
    public class Void
    {
        /// <summary>
        /// Voids can't be constructed, as they're the 'uninhabited type', i.e. an empty set, with no values.
        /// </summary>
        [MethodImpl(Proxy.mops)]
        Void() => throw new BottomException();
    }
}
