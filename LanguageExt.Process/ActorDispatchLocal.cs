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

        public bool HasStateTypeOf<T>() =>
            Inbox.HasStateTypeOf<T>();

        public bool CanAccept<T>() =>
            Inbox.CanAcceptMessageType<T>();

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag)
        {
            Inbox.ValidateMessageType(message, sender);
            return Inbox.Tell(message, sender);
        }

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            Inbox.TellSystem(message);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            Inbox.TellUserControl(message);

        public Unit Ask(object message, ProcessId sender) =>
            Inbox.Ask(message, sender);

        public Unit Kill() =>
            ShutdownProcess(false);

        public Unit Shutdown() =>
            ShutdownProcess(true);

        Unit ShutdownProcess(bool maintainState) =>
            ActorContext.WithContext(
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
            Actor.Publish(message);

        public int GetInboxCount() =>
            Inbox.Count;

        public Unit Watch(ProcessId pid) =>
            Actor.AddWatcher(pid);

        public Unit UnWatch(ProcessId pid) =>
            Actor.RemoveWatcher(pid);

        public Unit DispatchWatch(ProcessId watching) =>
            Actor.DispatchWatch(watching);

        public Unit DispatchUnWatch(ProcessId watching) =>
            Actor.DispatchUnWatch(watching);

        public bool IsLocal => true;
    }
}
