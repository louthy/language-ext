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
        readonly bool transactionalIO;

        public ActorDispatchLocal(ActorItem actor, bool transactionalIO)
        {
            this.transactionalIO = transactionalIO;
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
            transactionalIO
                ? Inbox.Tell(Inbox.ValidateMessageType(message, sender), sender)
                : ProcessOp.IO(() => Inbox.Tell(Inbox.ValidateMessageType(message, sender), sender));

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            transactionalIO
                ? Inbox.TellSystem(message)
                : ProcessOp.IO(() => Inbox.TellSystem(message));

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            transactionalIO
                ? Inbox.TellUserControl(message)
                : ProcessOp.IO(() => Inbox.TellUserControl(message));

        public Unit Ask(object message, ProcessId sender) =>
            Inbox.Ask(message, sender);

        public Unit Kill() =>
            transactionalIO
                ? ShutdownProcess(false)
                : ProcessOp.IO(() => ShutdownProcess(false));

        public Unit Shutdown() =>
            transactionalIO
                ? ShutdownProcess(true)
                : ProcessOp.IO(() => ShutdownProcess(true));

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
            transactionalIO
                ? Actor.Publish(message)
                : ProcessOp.IO(() => Actor.Publish(message));

        public int GetInboxCount() =>
            Inbox.Count;

        public Unit Watch(ProcessId pid) =>
            transactionalIO
                ? Actor.AddWatcher(pid)
                : ProcessOp.IO(() => Actor.AddWatcher(pid));

        public Unit UnWatch(ProcessId pid) =>
            transactionalIO
                ? Actor.RemoveWatcher(pid)
                : ProcessOp.IO(() => Actor.RemoveWatcher(pid));

        public Unit DispatchWatch(ProcessId watching) =>
            transactionalIO
                ? Actor.DispatchWatch(watching)
                : ProcessOp.IO(() => Actor.DispatchWatch(watching));

        public Unit DispatchUnWatch(ProcessId watching) =>
            transactionalIO
                ? Actor.DispatchUnWatch(watching)
                : ProcessOp.IO(() => Actor.DispatchUnWatch(watching));

        public bool IsLocal => 
            true;

        public bool Exists =>
            true;
    }
}
