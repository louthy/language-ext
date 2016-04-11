using System;
using System.Linq;
using System.Reactive.Linq;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal class ActorDispatchLocal : IActorDispatch
    {
        public readonly ILocalActorInbox Inbox;
        public readonly IActor Actor;

        public ActorDispatchLocal(ActorItem actor)
        {
            Inbox = actor.Inbox as ILocalActorInbox;
            if (Inbox == null) throw new ArgumentException("Invalid (not local) ActorItem passed to LocalActorDispatch.");
            Actor = actor.Actor;
        }

        public IObservable<T> Observe<T>() =>
            from x in Actor.PublishStream
            where x is T
            select (T)x;

        public IObservable<T> ObserveState<T>() =>
            from x in Actor.StateStream
            where x is T
            select (T)x;

        public Either<string, bool> HasStateTypeOf<T>() =>
            Inbox.HasStateTypeOf<T>();

        public Either<string, bool> CanAccept<T>() =>
            Inbox.CanAcceptMessageType<T>();

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            ProcessOp.IO(() =>
            {
                message = Inbox.ValidateMessageType(message, sender);
                return Inbox.Tell(message, sender);
            });

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            ProcessOp.IO(() => Inbox.TellSystem(message));

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            ProcessOp.IO(() => Inbox.TellUserControl(message));

        public Unit Ask(object message, ProcessId sender) =>
            Inbox.Ask(message, sender);

        public Unit Kill() =>
            ProcessOp.IO(() => ShutdownProcess(false));

        public Unit Shutdown() =>
            ProcessOp.IO(() => ShutdownProcess(true));

        Unit ShutdownProcess(bool maintainState) =>
            ActorContext.System(Actor.Id).WithContext(
                new ActorItem(
                    Actor,
                    (IActorInbox)Inbox,
                    Actor.Flags
                    ),
                Actor.Parent,
                ProcessId.NoSender,
                null,
                SystemMessage.ShutdownProcess(maintainState),
                None,
                () => Actor.ShutdownProcess(maintainState)
            );

        public Map<string, ProcessId> GetChildren() =>
            Actor.Children.Map(a => a.Actor.Id);

        public Unit Publish(object message) =>
            ProcessOp.IO(() => Actor.Publish(message));

        public int GetInboxCount() =>
            Inbox.Count;

        public Unit Watch(ProcessId pid) =>
            ProcessOp.IO(() => Actor.AddWatcher(pid));

        public Unit UnWatch(ProcessId pid) =>
            ProcessOp.IO(() => Actor.RemoveWatcher(pid));

        public Unit DispatchWatch(ProcessId watching) =>
            ProcessOp.IO(() => Actor.DispatchWatch(watching));

        public Unit DispatchUnWatch(ProcessId watching) =>
            ProcessOp.IO(() => Actor.DispatchUnWatch(watching));

        public bool IsLocal => 
            true;

        public bool Exists =>
            true;
    }
}
