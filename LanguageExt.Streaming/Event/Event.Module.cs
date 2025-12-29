using System;

namespace LanguageExt;

public static class Event
{
    /// <summary>
    /// Create an `Event` from an event delegate
    /// </summary>
    /// <param name="eventDelegate">Delegate to which the event will listen for values</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Event that can be passed around in a first-class manner</returns>
    public static Event<A> from<A>(ref Action<A> eventDelegate) =>
        new (ref eventDelegate);
}
