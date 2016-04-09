using Newtonsoft.Json;
using System;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt.Session
{
    class SessionManager : IDisposable
    {
        const string SessionsNotify = "sys-sessions-notify";
        readonly Option<ICluster> cluster;
        readonly SystemName system;
        readonly ProcessName nodeName;
        readonly SessionSync sync;

        public SessionManager(Option<ICluster> cluster, SystemName system, ProcessName nodeName, VectorConflictStrategy strategy)
        {
            this.cluster = cluster;
            this.system = system;
            this.nodeName = nodeName;

            sync = new SessionSync(cluster, system, nodeName, strategy);

            cluster.Iter(c =>
                c.SubscribeToChannel<SessionAction>(SessionsNotify).Subscribe(
                    act => sync.Incoming(act)
                ));
        }

        public void Dispose()
        {
            lock (sync)
            {
                cluster.Iter(c => c.UnsubscribeChannel(SessionsNotify));
            }
        }

        public Option<SessionVector> GetSession(SessionId sessionId) =>
            sync.GetSession(sessionId);

        public Unit Start(SessionId sessionId, int timeoutSeconds)
        {
            sync.Start(sessionId, timeoutSeconds);
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Start(sessionId, timeoutSeconds, system, nodeName)));
        }

        public Unit Stop(SessionId sessionId)
        {
            sync.Stop(sessionId);
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Stop(sessionId, system, nodeName)));
        }

        public Unit Touch(SessionId sessionId)
        {
            sync.Touch(sessionId);
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Touch(sessionId, system, nodeName)));
        }

        public Unit SetData(long time, SessionId sessionId, string key, object value)
        {
            sync.SetData(sessionId, key, value, time);

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
            sync.ClearData(sessionId, key, time);

            return cluster.Iter(c =>
                c.PublishToChannel(
                    SessionsNotify,
                    SessionAction.ClearData(time, sessionId, key, system, nodeName)));
        }
    }
}