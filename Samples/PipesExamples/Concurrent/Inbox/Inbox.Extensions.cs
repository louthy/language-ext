using LanguageExt.Traits;

namespace LanguageExt.Pipes2.Concurrent;

public static class InboxExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static Inbox<A> As<A>(this K<Inbox, A> ma) =>
        (Inbox<A>)ma;
}
