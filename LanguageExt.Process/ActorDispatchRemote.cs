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

        public bool HasStateTypeOf<T>()
        {
            Try<bool> valid = () =>
            {
                if (Cluster.Exists(ActorInboxCommon.ClusterMetaDataKey(ProcessId)))
                {
                    var meta = Cluster.GetValue<ProcessMetaData>(ActorInboxCommon.ClusterMetaDataKey(ProcessId));

                    var rhsType = meta == null
                        ? null
                        : Type.GetType(meta.StateTypeName)?.GetTypeInfo();

                    return meta == null || rhsType == null
                        ? true
                        : typeof(T).GetTypeInfo().IsAssignableFrom(rhsType);
                }
                else
                {
                    return true;
                }
            };

            return valid.IfFail(true);
        }

        public bool CanAccept<T>()
        {
            Try<bool> valid = () =>
            {
                if (Cluster.Exists(ActorInboxCommon.ClusterMetaDataKey(ProcessId)))
                {
                    if (typeof(TerminatedMessage).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) || typeof(UserControlMessage).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) || typeof(SystemMessage).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
                    {
                        return true;
                    }
                    var meta = Cluster.GetValue<ProcessMetaData>(ActorInboxCommon.ClusterMetaDataKey(ProcessId));
                    return meta == null || meta.MsgTypeNames == null
                        ? true
                        : meta.MsgTypeNames.Fold(false, (value, typ) =>
                        {
                            var lhsType = Type.GetType(typ)?.GetTypeInfo();
                            if (lhsType == null) return false;

                            var rhsType = typeof(T).GetTypeInfo();

                            return value
                                ? true
                                : lhsType.IsAssignableFrom(rhsType);
                        });
                }
                else
                {
                    return true;
                }
            };

            return valid.IfFail(true);
        }

        void ValidateMessageType(object message, ProcessId sender)
        {
            if(message == null)
            {
                throw new ProcessException($"Invalid message.  Null is not allowed for Process ({ProcessId}).", ProcessId.Path, sender.Path, null);
            }
            if (message is TerminatedMessage || message is UserControlMessage || message is SystemMessage)
            {
                return;
            }
            if (Cluster.Exists(ActorInboxCommon.ClusterMetaDataKey(ProcessId)))
            {
                Try<bool> valid = () =>
                {
                    var meta = Cluster.GetValue<ProcessMetaData>(ActorInboxCommon.ClusterMetaDataKey(ProcessId));
                    return meta == null || meta.MsgTypeNames == null
                        ? true
                        : meta.MsgTypeNames.Fold(false, (value, typ) =>
                            {
                                var lhsType = Type.GetType(typ)?.GetTypeInfo();
                                if (lhsType == null) throw new ProcessException("Can't resolve message type: " + typ, ProcessId.Path, sender.Path);

                                var rhsType = message.GetType().GetTypeInfo();

                                return value
                                    ? true
                                    : lhsType.IsAssignableFrom(rhsType);
                            });
                };

                if( !valid.IfFail(true) )
                {
                    throw new ProcessException($"Invalid message-type ({message.GetType().Name}) for Process ({ProcessId}).", ProcessId.Path, sender.Path, null);
                }
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
