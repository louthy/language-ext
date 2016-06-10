using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Reactive.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Reactive.Concurrency;

namespace LanguageExt.Session
{
    /// <summary>
    /// Manages the in-memory view of the sessions
    /// </summary>
    class SessionSync
    {
        SystemName system;
        ProcessName nodeName;
        Map<SessionId, SessionVector> sessions = Map.empty<SessionId, SessionVector>();
        Map<SessionId, Subject<Tuple<SessionId, DateTime>>> sessionTouched = Map.empty<SessionId, Subject<Tuple<SessionId, DateTime>>>();
        Subject<Tuple<SessionId, DateTime>> touched = new Subject<Tuple<SessionId, DateTime>>();

        VectorConflictStrategy strategy;
        object sync = new object();

        public SessionSync(SystemName system, ProcessName nodeName, VectorConflictStrategy strategy)
        {
            this.system = system;
            this.nodeName = nodeName;
            this.strategy = strategy;
        }

        public IObservable<Tuple<SessionId, DateTime>> Touched => touched;

        public void ExpiredCheck()
        {
            var now = DateTime.UtcNow;
            sessions.Filter(s => s.Expires < now)
                    .Iter((sid,_) =>
                    {
                        try
                        {
                            sessionEnded.OnNext(sid);
                        }
                        catch(Exception e)
                        {
                            logErr(e);
                        }
                    });
        }

        public int Incoming(SessionAction incoming)
        {
            if (incoming.SystemName == system.Value && incoming.NodeName == nodeName.Value) return 0;

            switch(incoming.Tag)
            { 
                case SessionActionTag.Touch:
                    Touch(incoming.SessionId);
                    break;
                case SessionActionTag.Start:
                    Start(incoming.SessionId, incoming.Timeout, Map.empty<string,object>());
                    break;
                case SessionActionTag.Stop:
                    Stop(incoming.SessionId);
                    break;
                case SessionActionTag.SetData:
                    var type = Type.GetType(incoming.Type);
                    if (type == null)
                    {
                        logErr("Session-value type not found: " + incoming.Type);
                    }
                    else
                    {
                        var value = Deserialise.Object(incoming.Value, type);
                        if (value == null)
                        {
                            logErr("Session-value is null or failed to deserialise: " + incoming.Value);
                        }
                        else
                        {
                            SetData(incoming.SessionId, incoming.Key, value, incoming.Time);
                        }
                    }
                    break;
                case SessionActionTag.ClearData:
                    ClearData(incoming.SessionId, incoming.Key, incoming.Time);
                    break;
                default:
                    return 0;
            }
            return 1;
        }

        /// <summary>
        /// Get an active session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public Option<SessionVector> GetSession(SessionId sessionId) =>
            sessions.Find(sessionId);

        internal Option<SessionVector> GetSession(Option<SessionId> sessionId) =>
            from sid in sessionId
            from ses in sessions.Find(sid)
            select ses;

        /// <summary>
        /// Start a new session
        /// </summary>
        public SessionId Start(SessionId sessionId, int timeoutSeconds, Map<string,object> initialState)
        {
            lock (sync)
            {
                if (sessions.ContainsKey(sessionId))
                {
                    return sessionId;
                }
                var session = SessionVector.Create(timeoutSeconds, VectorConflictStrategy.First, initialState);
                sessions = sessions.Add(sessionId, session);

                // Create a subject per session that will buffer touches so we don't push too
                // much over the net when not needed.  This routes to a single stream that isn't
                // buffered.
                var touch = new Subject<Tuple<SessionId, DateTime>>();
                touch.Sample(TimeSpan.FromSeconds(1))
                     .ObserveOn(TaskPoolScheduler.Default)
                     .Subscribe(touched.OnNext);
                sessionTouched = sessionTouched.Add(sessionId, touch);
            }
            return sessionId;
        }

        /// <summary>
        /// Remove a session and its state
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="vector"></param>
        public Unit Stop(SessionId sessionId)
        {
            lock (sync)
            {
                // When it comes to stopping sessions we just get rid of the whole history
                // regardless of whether something happened in the future.  Session IDs
                // are randomly generated, so any future event with the same session ID
                // is a deleted future.
                sessions = sessions.Remove(sessionId);
                sessionTouched.Find(sessionId).Iter(sub => sub.OnCompleted());
                sessionTouched = sessionTouched.Remove(sessionId);
            }
            return unit;
        }

        /// <summary>
        /// Timestamp a sessions to keep it alive
        /// </summary>
        public Unit Touch(SessionId sessionId) =>
            sessions.Find(sessionId).IfSome(s =>
            {
                s.Touch();
                sessionTouched.Find(sessionId).Iter(sub => sub.OnNext(Tuple(sessionId, DateTime.UtcNow)));
            });

        /// <summary>
        /// Set data on the session key/value store
        /// </summary>
        public Unit SetData(SessionId sessionId, string key, object value, long vector) =>
            sessions.Find(sessionId).Iter(s => s.SetKeyValue(vector, key, value, strategy));

        /// <summary>
        /// Clear a session key
        /// </summary>
        public Unit ClearData(SessionId sessionId, string key, long vector) =>
            sessions.Find(sessionId).Iter(s => s.ClearKeyValue(vector, key));
    }
}