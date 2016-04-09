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
        readonly SessionSync sync;

        public SessionManager(Option<ICluster> cluster, SystemName system, VectorClockConflictStrategy strategy)
        {
            this.cluster = cluster;
            this.system = system;
            var self = Self;

            sync = new SessionSync(cluster, system, strategy);

            cluster.Iter(c =>
                c.SubscribeToChannel<SessionAction>(SessionsNotify).Subscribe(
                    act => sync.Incoming(act)
                ));
        }

        public void Dispose()
        {
            cluster.Iter(c => c.UnsubscribeChannel(SessionsNotify));
        }

        public Option<SessionVector> GetSession(string sessionId) =>
            sync.GetSession(sessionId);

        public Unit Start(string sessionId, int timeoutSeconds)
        {
            var action = SessionAction.Start(sessionId, timeoutSeconds);
            cluster.Map(c => c.PublishToChannel(SessionsNotify, action))
                   .IfNone(() => sync.Incoming(action));
            return unit;
        }

        public Unit Stop(string sessionId)
        {
            var action = SessionAction.Stop(sessionId);
            cluster.Map(c => c.PublishToChannel(SessionsNotify, action))
                   .IfNone(() => sync.Incoming(action));
            return unit;
        }

        public Unit Touch(string sessionId)
        {
            var action = SessionAction.Touch(sessionId);
            cluster.Map(c => c.PublishToChannel(SessionsNotify, action))
                   .IfNone(() => sync.Incoming(action));
            return unit;
        }

        public Unit SetData(long time, string sessionId, string key, object value)
        {
            var action = SessionAction.SetData(
                time,
                sessionId,
                key,
                JsonConvert.SerializeObject(value, ActorSystemConfig.Default.JsonSerializerSettings)
            );
            cluster.Map(c => c.PublishToChannel(SessionsNotify, action))
                   .IfNone(() => sync.Incoming(action));
            return unit;
        }

        public Unit ClearData(long time, string sessionId, string key)
        {
            cluster.Map(c =>
                c.PublishToChannel(
                    SessionsNotify,
                    SessionAction.ClearData(time, sessionId, key)));
            return unit;
        }
    }
}