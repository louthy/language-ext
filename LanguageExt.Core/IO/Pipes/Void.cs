using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// The initial-object in a category  
    /// </summary>
    /// <remarks>
    /// Meant to represent `void`, but we can't construct a `System.Void`
    /// A void is the initial object in a category, equivalent to an empty set, and because there are no values in an
    /// empty set there's no way to construct a type of `void`.  We use this type in the pipes system to represent a
    /// a 'closed' path.
    /// </remarks>
    public class Void
    {
        [MethodImpl(Proxy.mops)]
        public Void() =>
            throw new BottomException();
    }
}
