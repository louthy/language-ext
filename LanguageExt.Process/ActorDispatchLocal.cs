using System;
using System.Linq;
using System.Reactive.Linq;

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

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            Inbox.Tell(message, sender);

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            Inbox.TellSystem(message);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            Inbox.TellUserControl(message);

        public Unit Ask(object message, ProcessId sender) =>
            Inbox.Ask(message, sender);

        public Unit Kill() =>
            ActorContext.WithContext(new ActorItem(Actor, (IActorInbox)Inbox, Actor.Flags), Actor.Parent, ProcessId.NoSender, null, SystemMessage.ShutdownProcess, () =>
              Actor.ShutdownProcess(false)
            );

        public Map<string, ProcessId> GetChildren() =>
            Actor.Children.Map(a => a.Actor.Id);

        public Unit Publish(object message) =>
            Actor.Publish(message);
    }
}
