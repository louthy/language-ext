using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public static class OutboxExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static Outbox<A> As<A>(this K<Outbox, A> ma) =>
        (Outbox<A>)ma;
}
