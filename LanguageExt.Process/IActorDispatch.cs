using System;

namespace LanguageExt
{
    internal interface IActorDispatch
    {
        IObservable<T> Observe<T>();
        IObservable<T> ObserveState<T>();
        Unit Tell(object message, ProcessId sender, string inbox, Message.Type type, Message.TagSpec tag);
        Unit Ask(object message, ProcessId sender, string inbox, Message.Type type);
        Map<string, ProcessId> GetChildren();
        Unit Publish(object message);
    }
}
