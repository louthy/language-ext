﻿using System;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal class ActorDispatchNotExist : IActorDispatch
    {
        public readonly ProcessId ProcessId;

        public ActorDispatchNotExist(ProcessId pid)
        {
            ProcessId = pid;
        }

        private T Raise<T>(ProcessId sender) =>
            raise<T>(new ProcessException($"Doesn't exist ({ProcessId})", sender.Path, sender.Path, null));

        public Map<string, ProcessId> GetChildren() =>
            Map.empty<string, ProcessId>();

        public IObservable<T> Observe<T>() =>
            Observable.Empty<T>();

        public IObservable<T> ObserveState<T>() =>
            Observable.Empty<T>();

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            Raise<Unit>(sender);

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            unit;

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            Raise<Unit>(sender);

        public Unit Ask(object message, ProcessId sender) =>
            Raise<Unit>(sender);

        public Unit Publish(object message) =>
            Raise<Unit>(ProcessId.None);

        public Either<string, bool> CanAccept<T>() =>
            false;

        public Either<string, bool> HasStateTypeOf<T>() =>
            false;

        public Unit Kill() => 
            unit;

        public Unit Shutdown() =>
            unit;

        public int GetInboxCount() =>
            -1;

        public Unit Watch(ProcessId pid) =>
            Raise<Unit>(ProcessId.None);

        public Unit UnWatch(ProcessId pid) =>
            unit;

        public Unit DispatchWatch(ProcessId pid) =>
            Raise<Unit>(ProcessId.None);

        public Unit DispatchUnWatch(ProcessId pid) =>
            unit;

        public bool IsLocal => 
            false;

        public bool Exists =>
            false;
    }
}
