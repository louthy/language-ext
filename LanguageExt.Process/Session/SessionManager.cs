using Newtonsoft.Json;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Reactive.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt.Session
{
    /// <summary>
    /// Session manager for a single actor-system
    /// Receives direct session actions locally and remote session action messages
    /// Routes them through to SessionSync to manage the local state
    /// </summary>
    class SessionManager : IDisposable
    {
        const string SessionsNotify = "sys-sessions-notify";
        public readonly SessionSync Sync;

        readonly Option<ICluster> cluster;
        readonly SystemName system;
        readonly ProcessName nodeName;
        IDisposable notify;
        IDisposable ended;
        IDisposable touch;

        public SessionManager(Option<ICluster> cluster, SystemName system, ProcessName nodeName, VectorConflictStrategy strategy)
        {
            this.cluster = cluster;
            this.system = system;
            this.nodeName = nodeName;

            Sync = new SessionSync(system, nodeName, strategy);

            cluster.Iter(c =>
            {
                notify = c.SubscribeToChannel<SessionAction>(SessionsNotify).Subscribe(act => Sync.Incoming(act));

                var now = DateTime.UtcNow;

                // Look for stranded sessions that haven't been removed properly.  This is done once only
                // on startup because the systems should be shutting down sessions on their own.  This just
                // catches the situation where an app-domain died without shutting down properly.
                c.QuerySessionKeys()
                 .Map(key =>
                    from ts in c.GetHashField<long>(key, LastAccessKey)
                    from to in c.GetHashField<int>(key, TimeoutKey)
                    where new DateTime(ts) < now.AddSeconds(to * 2) // Multiply by 2, just to catch genuine non-active sessions
                    select c.Delete(key))
                 .Iter(id => { });

                // Remove session keys when an in-memory session ends
                ended = SessionEnded.Subscribe(sid => Stop(sid));

                touch = Sync.Touched.Subscribe(tup =>
                {
                    try
                    {
                        c.HashFieldAddOrUpdate(SessionKey(tup.Item1), LastAccessKey, DateTime.UtcNow.Ticks);
                        c.PublishToChannel(SessionsNotify, SessionAction.Touch(tup.Item1, system, nodeName));
                    }
                    catch(Exception e)
                    {
                        logErr(e);
                    }
                });
            });
        }

        public void Dispose()
        {
            notify?.Dispose();
            ended?.Dispose();
            touch?.Dispose();
        }

        /// <summary>
        /// Attempts to get a SessionVector by ID.  If it doesn't exist then there's a chance that
        /// we're out-of-sync.  So go to the cluster and look for a session by that ID.  If one
        /// exists then start a local session with that ID and the details stored in the cluster.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public Option<SessionVector> GetSession(SessionId sessionId) =>
            Sync.GetSession(sessionId).IfNone( () =>
                Sync.GetSession(from c in cluster
                                from ts in c.GetHashField<long>(SessionKey(sessionId), LastAccessKey)
                                from to in c.GetHashField<int>(SessionKey(sessionId), TimeoutKey)
                                select Sync.Start(sessionId, to, LoadData(sessionId)))
                    .IfNone(() => failwith<SessionVector>("Session doesn't exist")));

        const string LastAccessKey = "__last-access";
        const string TimeoutKey = "__timeout";

        private static string SessionKey(SessionId sessionId) =>
            $"sys-session-{sessionId}";

        public Unit Start(SessionId sessionId, int timeoutSeconds)
        {
            Sync.Start(sessionId, timeoutSeconds, LoadData(sessionId));
            cluster.Iter(c => c.HashFieldAddOrUpdate(SessionKey(sessionId), TimeoutKey, timeoutSeconds));
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Start(sessionId, timeoutSeconds, system, nodeName)));
        }

        public Unit Stop(SessionId sessionId)
        {
            Sync.Stop(sessionId);
            cluster.Iter(c => c.Delete(SessionKey(sessionId)));
            return cluster.Iter(c => c.PublishToChannel(SessionsNotify, SessionAction.Stop(sessionId, system, nodeName)));
        }

        public Unit Touch(SessionId sessionId) =>
            Sync.Touch(sessionId);

        private Map<string, object> LoadData(SessionId sessionId) =>
            cluster.Map(c => c.GetHashFields(SessionKey(sessionId)))
                   .IfNone(Map.empty<string, object>());

        public Unit SetData(long time, SessionId sessionId, string key, object value)
        {
            Sync.SetData(sessionId, key, value, time);
            cluster.Iter(c => c.HashFieldAddOrUpdate(SessionKey(sessionId), key, value));

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
            cluster.Iter(c => c.Delete(SessionKey(sessionId)));

            return cluster.Iter(c =>
                c.PublishToChannel(
                    SessionsNotify,
                    SessionAction.ClearData(time, sessionId, key, system, nodeName)));
        }
    }
}