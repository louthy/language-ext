using System;

namespace LanguageExt
{
    internal class ActorDispatchJS : IActorDispatch
    {
        public readonly ProcessId ProcessId;
        public readonly ProcessId RelayId;
        public readonly ActorItem Js;

        public ActorDispatchJS(ProcessId pid, ProcessId relay, ActorItem js)
        {
            ProcessId = pid;
            RelayId = relay;
            Js = js;
        }

        public Map<string, ProcessId> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IObservable<T> Observe<T>()
        {
            throw new NotImplementedException();
        }

        public IObservable<T> ObserveState<T>()
        {
            throw new NotImplementedException();
        }

        public int GetInboxCount() => -1;

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            Tell(message, sender, "user", Message.Type.User, tag);

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            Tell(message, sender, "user", Message.Type.System, message.Tag);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            Tell(message, sender, "system", Message.Type.UserControl, message.Tag);

        Unit Tell(object message, ProcessId sender, string inbox, Message.Type type, Message.TagSpec tag)
        {
            var dto = RemoteMessageDTO.Create(message, ProcessId, sender, type, tag);
            // The standard structure for remote js relay paths are  "/root/js/{connection-id}/..."
            var connectionId = ProcessId.Skip(2).Take(1).GetName().Value;
            dto.To = ProcessId.Skip(3).Path;
            var relayMsg = new OutboundRelayMsg(connectionId, dto, dto.To, sender, dto.RequestId != -1);
            return Process.tell(RelayId, relayMsg, sender);
        }

        public Unit Ask(object message, ProcessId sender) =>
            Tell(message, sender, "user", Message.Type.User, Message.TagSpec.UserAsk);

        public Unit Publish(object message)
        {
            throw new NotSupportedException();
        }

        public bool CanAccept<T>() => true;
        public bool HasStateTypeOf<T>() => true;

        public Unit Kill() =>
            // TODO: Not yet implemented on the JS side
            ProcessId.Tell(SystemMessage.ShutdownProcess(false), ActorContext.Self);

        public Unit Shutdown() =>
            // TODO: Not yet implemented on the JS side
            ProcessId.Tell(SystemMessage.ShutdownProcess(true), ActorContext.Self);

        public Unit Watch(ProcessId pid)
        {
            throw new NotSupportedException();
        }

        public Unit UnWatch(ProcessId pid)
        {
            throw new NotSupportedException();
        }

        public Unit DispatchWatch(ProcessId pid)
        {
            throw new NotSupportedException();
        }

        public Unit DispatchUnWatch(ProcessId pid)
        {
            throw new NotSupportedException();
        }

        public bool IsLocal => false;
    }
}
