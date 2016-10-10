﻿using System;

namespace LanguageExt
{
    interface IActorDispatch
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
        Unit Shutdown();
        int GetInboxCount();
        Unit Watch(ProcessId pid);
        Unit UnWatch(ProcessId pid);
        Unit DispatchWatch(ProcessId pid);
        Unit DispatchUnWatch(ProcessId pid);
        bool IsLocal { get; }
        bool Exists { get; }
        Either<string, bool> CanAccept<T>();
        Either<string, bool> HasStateTypeOf<T>();
    }
}
