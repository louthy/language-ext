using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public static class SourceExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static Source<A> As<A>(this K<Source, A> ma) =>
        (Source<A>)ma;
}
