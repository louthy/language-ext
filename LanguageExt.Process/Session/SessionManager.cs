using Newtonsoft.Json;
using System;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt.Session
{
    class SessionManager : IDisposable
    {
        const string SessionsNotify = "sys-sessions-notify";
        public readonly SessionSync Sync;

        readonly Option<ICluster> cluster;
        readonly SystemName system;
        readonly ProcessName nodeName;

        public SessionManager(Option<ICluster> cluster, SystemName system, ProcessName nodeName, VectorConflictStrategy strategy)
        {
            this.cluster = cluster;
            this.system = system;
            this.nodeName = nodeName;

            Sync = new SessionSync(cluster, system, nodeName, strategy);

            cluster.Iter(c =>
                c.SubscribeToChannel<SessionAction>(SessionsNotify).Subscribe(
                    act => Sync.Incoming(act)
                ));
        }

        public void Dispose()
        {
            cluster.Iter(c => c.UnsubscribeChannel(SessionsNotify));
        }

        public Option<SessionVector> GetSession(SessionId sessionId) =>
            Sync.GetSession(sessionId);

        public Unit Start(SessionId sessionId, int timeoutSeconds)
        {
            Sync.Start(sessionId, timeoutSeconds);
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Start(sessionId, timeoutSeconds, system, nodeName)));
        }

        public Unit Stop(SessionId sessionId)
        {
            Sync.Stop(sessionId);
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Stop(sessionId, system, nodeName)));
        }

        public Unit Touch(SessionId sessionId)
        {
            Sync.Touch(sessionId);
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Touch(sessionId, system, nodeName)));
        }

        public Unit SetData(long time, SessionId sessionId, string key, object value)
        {
            Sync.SetData(sessionId, key, value, time);

            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.SetData(
                time,
                sessionId,
                key,
                JsonConvert.SerializeObject(value, ActorSystemConfig.Default.JsonSerializerSettings),
                system,
                nodeName
            )));
        }

        public Unit ClearData(long time, SessionId sessionId, string key)
        {
            Sync.ClearData(sessionId, key, time);

            return cluster.Iter(c =>
                c.PublishToChannel(
                    SessionsNotify,
                    SessionAction.ClearData(time, sessionId, key, system, nodeName)));
        }
    }
}