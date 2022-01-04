using System;

namespace LanguageExt
{
    /// <summary>
    /// Announces Ref change events
    /// </summary>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    public delegate void RefChangedEvent<in A>(A value);
}
