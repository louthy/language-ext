using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    class SessionManager
    {
        static ProcessId pid => ActorContext.System["sessions"];

        public static Unit Touch(string sessionId) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.TouchSession, sessionId));

        public static Unit Start(string sessionId, int timeoutSeconds) =>
            tell(pid, new SessionManagerProcess.StartSessionMsg(sessionId, timeoutSeconds));

        public static Unit Stop(string sessionId) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.StopSession, sessionId));

        public static Unit CheckExpired() =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.CheckExpired, null));

        public static Unit SetSessionData(string sessionId, object sessionData) =>
            tell(pid, new SessionManagerProcess.SetSessionMetadataMsg(sessionId, sessionData));
    }

    class SessionManagerProcess
    {
        public enum MsgTag
        {
            TouchSession,
            StartSession,
            StopSession,
            CheckExpired,
            SetSessionMetadata
        }

        public class Msg
        {
            public readonly MsgTag Tag;
            public readonly string SessionId;

            public Msg(MsgTag tag, string sessionId)
            {
                Tag = tag;
                SessionId = sessionId;
            }
        }

        public class StartSessionMsg : Msg
        {
            public readonly int TimeoutSeconds;

            public StartSessionMsg(string sessionId, int timeoutSeconds)
                :
                base(MsgTag.StartSession,sessionId)
            {
                TimeoutSeconds = timeoutSeconds;
            }
        }

        public class SetSessionMetadataMsg : Msg
        {
            public readonly object Data;

            public SetSessionMetadataMsg(string sessionId, object data)
                :
                base(MsgTag.SetSessionMetadata, sessionId)
            {
                Data = data;
            }
        }

        public class Session
        {
            public readonly string Id;
            public readonly int TimeoutSeconds;
            public readonly DateTime LastAccess;

            [JsonConstructor]
            public Session(
                string id,
                int timeoutSeconds,
                DateTime lastAccess
                )
            {
                Id = id;
                TimeoutSeconds = timeoutSeconds;
                LastAccess = lastAccess;
            }

            public Session Touch() =>
                new Session(Id, TimeoutSeconds, DateTime.UtcNow);
        }

        public class State
        {
            public readonly Map<string, Session> Sessions;
            public readonly Map<string, object> Metadata;

            public static readonly State Empty = 
                new State(Map.empty<string, Session>(), Map.empty<string, object>());

            public State()
            {
                Sessions = Map.empty<string, Session>();
                Metadata = Map.empty<string, object>();
            }

            public State(Map<string, Session> sessions, Map<string, object> metadata)
            {
                Sessions = sessions;
                Metadata = metadata;
            }

            public State SetSessions(Map<string, Session> sessions) =>
                new State(sessions, Metadata);

            public State SetMetadata(Map<string, object> metadata) =>
                new State(Sessions, metadata);

            public State SetSession(string sid, Session session) =>
                SetSessions(Sessions.AddOrUpdate(sid, session));

            public State SetMetadatum(string sid, object metadatum) =>
                SetMetadata(Metadata.AddOrUpdate(sid, metadatum));

            public State ClearSession(string sid) =>
                new State(Sessions.Remove(sid), Metadata.Remove(sid));

            public State MapSessions(Func<Map<string, Session>, Map<string, Session>> map) =>
                SetSessions(map(Sessions));

            public State MapMetadata(Func<Map<string, object>, Map<string, object>> map) =>
                SetMetadata(map(Metadata));

            public State MapSession(string sid, Func<Session, Session> map) =>
                Sessions.Find(sid,
                    Some: s => SetSession(sid, map(s)),
                    None: () => this
                );

            public State MapMetadatum(string sid, Func<object, object> map) =>
                Metadata.Find(sid,
                    Some: s => SetMetadatum(sid, map(s)),
                    None: () => this
                );
        }

        const string SessionsKey = "ProcessSessions";

        public static State Setup() =>
            ActorContext.Cluster.Map(
                c => 
                {
                    try
                    {
                        var state = CheckExpired(State.Empty.SetSessions(c.GetHashFields<Session>(SessionsKey)));
                        state.Sessions.Iter(s => logWarn("Reloading session: " + s.Id));
                        return state;
                    }
                    catch
                    {
                        return State.Empty;
                    }
                })
            .IfNone(State.Empty);

        public static State Inbox(State state, Msg msg)
        {
            switch (msg.Tag)
            {
                case MsgTag.StartSession:
                    return StartSession(state, msg.SessionId, (msg as StartSessionMsg).TimeoutSeconds);

                case MsgTag.StopSession:
                    return StopSession(state, msg.SessionId);

                case MsgTag.TouchSession:
                    return TouchSession(state, msg.SessionId);

                case MsgTag.CheckExpired:
                    logWarn("Checking for expired sessions");
                    state = CheckExpired(state);
                    tellSelf(new Msg(MsgTag.CheckExpired, null), ProcessSetting.SessionTimeoutCheckFrequency);
                    return state;

                case MsgTag.SetSessionMetadata:
                    logWarn("Session data set: " + msg.SessionId);
                    return SetSessionMetadata(state, msg as SetSessionMetadataMsg);

                default:
                    return state;
            }
        }

        private static string GetMetaKey(string sid) =>
            "SessionMeta_" + sid;

        private static State SetSessionMetadata(State state, SetSessionMetadataMsg msg) =>
            GetSession(state.Sessions, msg.SessionId).Match(
                Some: s =>
                {
                    ActorContext.Cluster.IfSome(c =>
                       c.SetValue(
                           GetMetaKey(msg.SessionId),
                           msg.Data
                       ));

                    return TouchSession(
                        state.SetMetadatum(
                            msg.SessionId,
                            msg.Data), msg.SessionId);
                },
                None: () => state
            );

        public static Option<T> GetSessionMetadata<T>(State state, string sessionId) =>
            GetSession(TouchSession(state,sessionId).Sessions, sessionId).Match(
                Some: s =>
                    ActorContext.Cluster.Match(
                        Some: c => c.GetValue<T>(GetMetaKey(sessionId)),
                        None: () => state.Metadata.Find(sessionId).Map(x => (T)x)
                        ),
                None: () => None
            );

        /// <summary>
        /// Find all the expired sessions and stop them
        /// TODO: This isn't particularly efficient.  We could do a mass get of the latest state
        ///       and do a mass update.  Also it would make sense if this process was outside of 
        ///       the main loop.
        /// </summary>
        /// <param name="state">Sessions</param>
        /// <returns>Updated sessions</returns>
        private static State CheckExpired(State state) =>
            state.Sessions
                .Filter(s => GetSession(state.Sessions, s.Id).IsNone)
                .Fold(state, (s, x) => StopSession(s, x.Id));

        /// <summary>
        /// If the session is valid (not expired) then return Some(session) else None
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>Optional session</returns>
        private static Option<Session> SomeIfValid(Session session) =>
            (((DateTime.UtcNow - session.LastAccess).TotalSeconds > session.TimeoutSeconds))
                ? None
                : Some(session);

        /// <summary>
        /// Load a session from the cluster
        /// </summary>
        /// <param name="sessionId">Session ID to load</param>
        /// <returns>Optional session</returns>
        private static Option<Session> GetClusterSession(string sessionId) =>
            ActorContext.Cluster.Match(
                Some: cluster => cluster.GetHashField<Session>(SessionsKey, sessionId)
                                        .Match( Some: SomeIfValid,
                                                None: () => None),
                None: () => None);

        /// <summary>
        /// Get a session from local state or from the cluster.  If it's expired locally
        /// it will double check if it's been updated remotely.
        /// </summary>
        /// <param name="state">State</param>
        /// <param name="sessionId">Session to get</param>
        /// <returns>Optional session</returns>
        public static Option<Session> GetSession(Map<string, Session> state, string sessionId) =>
            state.Find(sessionId)
                 .Map(
                     session =>
                         SomeIfValid(session).Match(
                             Some: s => Some(s),
                             None: () => GetClusterSession(sessionId)))
                 .IfNone(None);

        /// <summary>
        /// Update the timestamp on the session
        /// </summary>
        /// <param name="state">Sessions</param>
        /// <param name="sessionId">Session to update</param>
        /// <returns>New state</returns>
        private static State TouchSession(State state, string sessionId)
        {
            logWarn("Session touched: " + sessionId);

            return state.MapSessions(
                sessions =>
                    GetSession(sessions, sessionId).Match(
                        Some: session =>
                        {
                            session = session.Touch();
                            ActorContext.Cluster.IfSome(c => c.HashFieldAddOrUpdate(SessionsKey, sessionId, session));
                            return sessions.AddOrUpdate(sessionId, session);
                        },
                        None: () =>
                        {
                            ActorContext.Cluster.IfSome(c => c.DeleteHashField(SessionsKey, sessionId));
                            return sessions.Remove(sessionId);
                        }));
        }

        /// <summary>
        /// Start a new session
        /// </summary>
        /// <param name="state">Sessions</param>
        /// <param name="sessionId">Id of session to start</param>
        /// <param name="timeoutSeconds">Expiry</param>
        /// <returns>Updated sessions state</returns>
        private static State StartSession(State state, string sessionId, int timeoutSeconds)
        {
            logWarn("Session started: " + sessionId);
            
            return GetSession(state.Sessions, sessionId).Match(
                Some: session => TouchSession(state, sessionId),
                None: () =>
                {
                    var session = new Session(sessionId, timeoutSeconds, DateTime.UtcNow);
                    ActorContext.Cluster.IfSome(c => c.HashFieldAddOrUpdate(SessionsKey, sessionId, session));
                    return state.SetSession(sessionId, session);
                });
        }

        /// <summary>
        /// Stop a session
        /// </summary>
        /// <param name="state">Sessions</param>
        /// <param name="sessionId">Session to stop</param>
        /// <returns>Update sessions state</returns>
        private static State StopSession(State state, string sessionId)
        {
            logWarn("Session stopping: " + sessionId);

            GetSession(state.Sessions, sessionId).IfSome(
                session =>
                {
                    ActorContext.Cluster.IfSome(c =>
                    {
                        c.DeleteHashField(SessionsKey, sessionId);
                        c.Delete(GetMetaKey(sessionId));
                    });
                }
            );

            return state.ClearSession(sessionId);
        }
    }
}
