using System;
using System.Reflection;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    class ActorDispatchRemote : IActorDispatch
    {
        public readonly ProcessId ProcessId;
        public readonly ICluster Cluster;

        public ActorDispatchRemote(ProcessId pid, ICluster cluster)
        {
            ProcessId = pid;
            Cluster = cluster;
        }

        public Map<string, ProcessId> GetChildren() =>
            ask<Map<string, ProcessId>>(ProcessId, UserControlMessage.GetChildren);

        public IObservable<T> Observe<T>() =>
            Cluster.SubscribeToChannel<T>(ActorInboxCommon.ClusterPubSubKey(ProcessId));

        public IObservable<T> ObserveState<T>() =>
            Cluster.SubscribeToChannel<T>(ActorInboxCommon.ClusterStatePubSubKey(ProcessId));

        public Either<string, bool> HasStateTypeOf<T>()
        {
            if (Cluster.Exists(ActorInboxCommon.ClusterMetaDataKey(ProcessId)))
            {
                var meta = Cluster.GetValue<ProcessMetaData>(ActorInboxCommon.ClusterMetaDataKey(ProcessId));
                if(meta == null)
                {
                    return true;
                }

                return TypeHelper.HasStateTypeOf(typeof(T), meta.StateTypeInterfaces);
            }
            else
            {
                return true;
            }
        }

        public Either<string, bool> CanAccept<T>()
        {
            if (Cluster.Exists(ActorInboxCommon.ClusterMetaDataKey(ProcessId)))
            {
                var meta = Cluster.GetValue<ProcessMetaData>(ActorInboxCommon.ClusterMetaDataKey(ProcessId));
                return meta == null
                    ? true
                    : TypeHelper.IsMessageValidForProcess(typeof(T), meta.MsgTypeNames).Map(_ => true);
            }
            else
            {
                return true;
            }
        }

        void ValidateMessageType(object message, ProcessId sender)
        {
            if (Cluster.Exists(ActorInboxCommon.ClusterMetaDataKey(ProcessId)))
            {
                var meta = Cluster.GetValue<ProcessMetaData>(ActorInboxCommon.ClusterMetaDataKey(ProcessId));
                if( meta == null )
                {
                    return;
                }

                TypeHelper.IsMessageValidForProcess(message, meta.MsgTypeNames).IfLeft((string err) =>
                {
                    throw new ProcessException($"{err} for Process ({ProcessId}).", ProcessId.Path, sender.Path, null);
                });
            }
        }

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            Tell(message, sender, "user", Message.Type.User, tag);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            Tell(message, sender, "user", Message.Type.UserControl, message.Tag);

        public Unit TellSystem(SystemMessage message, ProcessId sender)
        {
            var dto = RemoteMessageDTO.Create(message, ProcessId, sender, Message.Type.System, message.Tag);
            var clientsReached = Cluster.PublishToChannel(ActorInboxCommon.ClusterSystemInboxNotifyKey(ProcessId), dto);
            return unit;
        }

        public Unit Tell(object message, ProcessId sender, string inbox, Message.Type type, Message.TagSpec tag)
        {
            ValidateMessageType(message, sender);
            var dto = RemoteMessageDTO.Create(message, ProcessId, sender, type, tag);
            var inboxKey = ActorInboxCommon.ClusterInboxKey(ProcessId, inbox);
            var inboxNotifyKey = ActorInboxCommon.ClusterInboxNotifyKey(ProcessId, inbox);
            Cluster.Enqueue(inboxKey, dto);
            var clientsReached = Cluster.PublishToChannel(inboxNotifyKey, dto.MessageId);
            return unit;
        }

        public Unit Ask(object message, ProcessId sender) =>
            Tell(message, sender, Message.TagSpec.UserAsk);

        public Unit Publish(object message) =>
            ignore(Cluster.PublishToChannel(ActorInboxCommon.ClusterPubSubKey(ProcessId), message));

        public Unit Kill() =>
            ProcessId.Tell(SystemMessage.ShutdownProcess(false), ActorContext.Self);

        public Unit Shutdown() =>
            ProcessId.Tell(SystemMessage.ShutdownProcess(true), ActorContext.Self);

        public int GetInboxCount() =>
            Cluster.QueueLength(ActorInboxCommon.ClusterUserInboxKey(ProcessId));

        public Unit Watch(ProcessId pid) =>
            TellSystem(SystemMessage.Watch(pid), pid);

        public Unit UnWatch(ProcessId pid) =>
            TellSystem(SystemMessage.UnWatch(pid), pid);

        public Unit DispatchWatch(ProcessId watching) =>
            TellSystem(SystemMessage.DispatchWatch(watching), watching);

        public Unit DispatchUnWatch(ProcessId watching) =>
            TellSystem(SystemMessage.DispatchUnWatch(watching), watching);

        public bool IsLocal => false;
    }
}
