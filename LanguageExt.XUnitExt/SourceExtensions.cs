using LanguageExt.Common;
using LanguageExt.Pipes.Concurrent;

namespace LanguageExt;

public static partial class AssertExt
{
    /// <summary>
    /// Asserts that the source is closed
    /// </summary>
    public static Unit SourceIsClosed<A>(SourceIterator<A> iter) =>
        Throws(Errors.SourceClosed, () => iter.Read().Run());
}
