using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal class ActorDispatchRemote : IActorDispatch
    {
        public readonly ProcessId ProcessId;
        public readonly ICluster Cluster;

        public ActorDispatchRemote(ProcessId pid, ICluster cluster)
        {
            ProcessId = pid;
            Cluster = cluster;
        }

        public Map<string, ProcessId> GetChildren() =>
            ask<Map<string, ProcessId>>(ProcessId, ActorSystemMessage.GetChildren);

        public IObservable<T> Observe<T>()
        {
            var subject = new Subject<T>();
            Cluster.SubscribeToChannel<T>(ActorInboxCommon.ClusterPubSubKey(ProcessId), msg => subject.OnNext(msg));
            return subject.AsObservable();
        }

        public IObservable<T> ObserveState<T>()
        {
            var subject = new Subject<T>();
            Cluster.SubscribeToChannel<T>(ActorInboxCommon.ClusterStatePubSubKey(ProcessId), msg => subject.OnNext(msg));
            return subject.AsObservable();
        }

        public Unit Tell(object message, ProcessId sender, string inbox, Message.Type type, Message.TagSpec tag)
        {
            var dto = RemoteMessageDTO.Create(message, ProcessId, sender, type, tag);
            var inboxKey = ActorInboxCommon.ClusterInboxKey(ProcessId, inbox);
            var inboxNotifyKey = ActorInboxCommon.ClusterInboxNotifyKey(ProcessId, inbox);
            Cluster.Enqueue(inboxKey, dto);
            long clientsReached = Cluster.PublishToChannel(inboxNotifyKey, dto.MessageId);
            return unit;
        }

        public Unit Ask(object message, ProcessId sender, string inbox, Message.Type type) =>
            Tell(message, sender, inbox, type, Message.TagSpec.UserAsk);

        public Unit Publish(object message) =>
            ignore(Cluster.PublishToChannel(ActorInboxCommon.ClusterPubSubKey(ProcessId), message));
    }
}
