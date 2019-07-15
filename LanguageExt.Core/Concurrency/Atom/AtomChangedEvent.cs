using System;

namespace LanguageExt
{
    /// <summary>
    /// Announces Atom change events
    /// </summary>
    public delegate void AtomChangedEvent<A>(A value);
}
