using System;

namespace LanguageExt
{
    internal interface IActorDispatch
    {
        IObservable<T> Observe<T>();
        IObservable<T> ObserveState<T>();
        Unit Ask(object message, ProcessId sender);
        Unit Tell(object message, ProcessId sender, Message.TagSpec tag);
        Unit TellSystem(SystemMessage message, ProcessId sender);
        Unit TellUserControl(UserControlMessage message, ProcessId sender);
        Map<string, ProcessId> GetChildren();
        Unit Publish(object message);
        Unit Kill();
        int GetInboxCount();
        Unit Watch(ProcessId pid);
        Unit UnWatch(ProcessId pid);
    }
}
