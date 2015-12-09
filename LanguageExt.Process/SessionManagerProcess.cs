using Newtonsoft.Json;
using System;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
#if (NETFX_CORE || DNXCORE50)
using System.Threading;
#else
using System.Runtime.Remoting.Messaging;
#endif

namespace LanguageExt
{
    class SessionManager
    {
        static object sync = new object();
        static IDisposable updated;
        static ProcessId pid => ActorContext.System["sessions"];
        static SessionManagerProcess.State sessions = SessionManagerProcess.State.Empty;

        public static Unit Touch(string sessionId) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.TouchSession, sessionId, false));

        public static Unit Start(string sessionId, int timeoutSeconds) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.StartSession, sessionId, false, timeoutSeconds));

        public static Unit Stop(string sessionId) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.StopSession, sessionId, false));

        public static Unit CheckExpired() =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.CheckExpired, null, false));

        public static Unit SetSessionData(string sessionId, object sessionData) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.SetSessionMetadata, sessionId, false, 0, sessionData));

        public static Unit ClearSessionData(string sessionId) =>
            tell(pid, new SessionManagerProcess.Msg(SessionManagerProcess.MsgTag.ClearSessionMetadata, sessionId, false));

        public static void Init(Option<ICluster> cluster)
        {
            lock(sync)
            {
                Shutdown(cluster);
                updated = observeState<SessionManagerProcess.State>(pid).Subscribe(SessionsUpdated);
                CheckExpired();

                // Forward remote messages on
                cluster.IfSome(c =>
                {
                    c.UnsubscribeChannel(SessionManagerProcess.SessionsNotify);
                    c.SubscribeToChannel<SessionManagerProcess.Msg>(SessionManagerProcess.SessionsNotify)
                        .Subscribe(msg => tell(pid, msg.MakeRemote()));
                });
            }
        }

        public static void Shutdown(Option<ICluster> cluster)
        {
            lock(sync)
            {
                updated?.Dispose();
                updated = null;
                cluster.IfSome(c => c.UnsubscribeChannel(SessionManagerProcess.SessionsNotify));
            }
        }

        static void SessionsUpdated(SessionManagerProcess.State sessions)
        {
            SessionManager.sessions = sessions;
            SessionId.IfSome(sid => SessionId = sessions.Sessions.Find(sid).Map(s => s.Id));
        }

#if (NETFX_CORE || DNXCORE50)
        static AsyncLocal<string> sessionAsyncLocal = new AsyncLocal<string>();
#endif

        public static Option<string> SessionId
        {
            get
            {
#if (NETFX_CORE || DNXCORE50)
                return Optional(sessionAsyncLocal.Value);
#else
                return Optional(CallContext.LogicalGetData("lang-ext-session") as string);
#endif
            }
            set
            {
#if (NETFX_CORE || DNXCORE50)
                sessionAsyncLocal.Value = value.IfNoneUnsafe((string)null);
#else
                value.Match(
                    Some: x  => CallContext.LogicalSetData("lang-ext-session", x),
                    None: () => CallContext.FreeNamedDataSlot("lang-ext-session"));
#endif
            }
        }

        /// <summary>
        /// Get session meta data
        /// </summary>
        public static Option<T> GetSessionData<T>(string sid) =>
            SessionManagerProcess.GetSessionMetadata<T>(sessions, sid);

        /// <summary>
        /// Asserts that the current session ID is valid
        /// Checks the cached state first.  If the session has expired locally it 
        /// then checks to see if it's been updated remotely.  Otherwise it throws.
        /// </summary>
        /// <returns></returns>
        public static Unit AssertSession() =>
            SessionId.IfSome(sid =>
                SessionManagerProcess.GetSession(sessions.Sessions, sid).IfNone(() =>
                {
                    SessionManager.Stop(sid); // Make sure it's gone
                    SessionId = None;
                    throw new ProcessSessionExpired();
                }));
    }

    class SessionManagerProcess
    {
        public enum MsgTag
        {
            TouchSession,
            StartSession,
            StopSession,
            CheckExpired,
            SetSessionMetadata,
            ClearSessionMetadata
        }

        public class Msg
        {
            public readonly MsgTag Tag;
            public readonly string SessionId;
            public readonly int TimeoutSeconds;
            public readonly object Data;
            public readonly bool Remote;

            [JsonConstructor]
            public Msg(MsgTag tag, string sessionId, bool remote, int timeoutSeconds = 0, object data = null)
            {
                Tag = tag;
                SessionId = sessionId;
                Remote = remote;
                TimeoutSeconds = timeoutSeconds;
                Data = data;
            }

            public virtual Msg MakeRemote() =>
                new Msg(Tag, SessionId, true, TimeoutSeconds, Data);
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

            public State ClearSessionMetadatum(string sid) =>
                new State(Sessions, Metadata.Remove(sid));

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

        public const string SessionsKey = "sys-sessions";
        public const string SessionsNotify = "sys-sessions-notify";

        public static State Setup() =>
            ActorContext.Cluster.Map(
                c => 
                {
                    try
                    {
                        return CheckExpired(State.Empty.SetSessions(c.GetHashFields<Session>(SessionsKey)));
                    }
                    catch
                    {
                        return State.Empty;
                    }
                })
            .IfNone(State.Empty);

        public static State RemoteInbox(State state, Msg msg)
        {
            switch (msg.Tag)
            {
                case MsgTag.StartSession:
                    return state.SetSession(msg.SessionId, new Session(msg.SessionId, msg.TimeoutSeconds, DateTime.UtcNow));

                case MsgTag.StopSession:
                    return state.ClearSession(msg.SessionId);

                case MsgTag.TouchSession:
                    return state.MapSession(msg.SessionId, s => s.Touch());

                case MsgTag.SetSessionMetadata:
                    return state.SetMetadatum(msg.SessionId, msg.Data);

                case MsgTag.ClearSessionMetadata:
                    return state.ClearSessionMetadatum(msg.SessionId);

                default:
                    return state;
            }
        }

        public static State Inbox(State state, Msg msg)
        {
            if (msg.Remote)
            {
                return RemoteInbox(state, msg);
            }
            else
            {
                switch (msg.Tag)
                {
                    case MsgTag.StartSession:
                        return StartSession(state, msg);

                    case MsgTag.StopSession:
                        return StopSession(state, msg);

                    case MsgTag.TouchSession:
                        return TouchSession(state, msg.SessionId);

                    case MsgTag.CheckExpired:
                        state = CheckExpired(state);
                        tellSelf(new Msg(MsgTag.CheckExpired, null, false), ActorConfig.Default.SessionTimeoutCheckFrequency);
                        return state;

                    case MsgTag.SetSessionMetadata:
                        return SetSessionMetadata(state, msg);

                    case MsgTag.ClearSessionMetadata:
                        return ClearSessionMetadata(state, msg);

                    default:
                        return state;
                }
            }
        }

        static string GetMetaKey(string sid) =>
            "SessionMeta_" + sid;

        static State SetSessionMetadata(State state, Msg msg) =>
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

        static State ClearSessionMetadata(State state, Msg msg) =>
            GetSession(state.Sessions, msg.SessionId).Match(
                Some: s =>
                {
                    ActorContext.Cluster.IfSome(c => c.Delete(GetMetaKey(msg.SessionId)));
                    return TouchSession(state.ClearSessionMetadatum(msg.SessionId), msg.SessionId);
                },
                None: () => TouchSession(state.ClearSessionMetadatum(msg.SessionId), msg.SessionId)
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
        static State CheckExpired(State state) =>
            state.Sessions
                 .Filter(s => GetSession(state.Sessions, s.Id).IsNone)
                 .Fold(state, (s, x) => StopSession(s, new Msg(MsgTag.StopSession, x.Id, false)));

        /// <summary>
        /// If the session is valid (not expired) then return Some(session) else None
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>Optional session</returns>
        static Option<Session> SomeIfValid(Session session) =>
            (((DateTime.UtcNow - session.LastAccess).TotalSeconds > session.TimeoutSeconds))
                ? None
                : Some(session);

        /// <summary>
        /// Load a session from the cluster
        /// </summary>
        /// <param name="sessionId">Session ID to load</param>
        /// <returns>Optional session</returns>
        static Option<Session> GetClusterSession(string sessionId) =>
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
                             Some: s  => Some(s),
                             None: () => GetClusterSession(sessionId)))
                 .IfNone(None);

        /// <summary>
        /// Update the timestamp on the session
        /// </summary>
        /// <param name="state">Sessions</param>
        /// <param name="sessionId">Session to update</param>
        /// <returns>New state</returns>
        static State TouchSession(State state, string sessionId)
        {
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
        static State StartSession(State state, Msg msg)
        {
            var sessionId = msg.SessionId;
            var timeoutSeconds = msg.TimeoutSeconds;

            return GetSession(state.Sessions, sessionId).Match(
                Some: session => TouchSession(state, sessionId),
                None: () =>
                {
                    var session = new Session(sessionId, timeoutSeconds, DateTime.UtcNow);
                    ActorContext.Cluster.IfSome(c =>
                    {
                        c.HashFieldAddOrUpdate(SessionsKey, sessionId, session);
                        c.PublishToChannel(SessionsNotify, msg.MakeRemote());
                    });
                    return state.SetSession(sessionId, session);
                });
        }

        /// <summary>
        /// Stop a session
        /// </summary>
        /// <param name="state">Sessions</param>
        /// <param name="sessionId">Session to stop</param>
        /// <returns>Update sessions state</returns>
        static State StopSession(State state, Msg msg)
        {
            var sessionId = msg.SessionId;

            GetSession(state.Sessions, sessionId).IfSome(
                session =>
                {
                    ActorContext.Cluster.IfSome(c =>
                    {
                        c.DeleteHashField(SessionsKey, sessionId);
                        c.Delete(GetMetaKey(sessionId));
                        c.PublishToChannel(SessionsNotify, msg.MakeRemote());
                    });
                }
            );

            return state.ClearSession(sessionId);
        }
    }
}
