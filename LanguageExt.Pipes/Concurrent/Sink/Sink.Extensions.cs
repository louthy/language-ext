using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public static class SinkExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static Sink<A> As<A>(this K<Sink, A> ma) =>
        (Sink<A>)ma;
}
