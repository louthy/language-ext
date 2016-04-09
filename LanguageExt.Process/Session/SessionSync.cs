using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt.Session
{
    class SessionSync
    {
        const string SessionsNotify = "sys-sessions-notify";

        Option<ICluster> cluster;
        SystemName system;
        Map<string, SessionVector> sessions = Map.empty<string, SessionVector>();
        VectorClockConflictStrategy strategy;
        object sync = new object();

        public SessionSync(Option<ICluster> cluster, SystemName system, VectorClockConflictStrategy strategy)
        {
            this.cluster = cluster;
            this.system = system;
            this.strategy = strategy;
        }

        public int Incoming(SessionAction incoming)
        {
            if (incoming is SessionTouchAction)
            {
                Touch(incoming.SessionId, false);
            }
            else if (incoming is SessionStartAction)
            {
                Start(incoming.SessionId, (incoming as SessionStartAction).TimeoutSeconds);
            }
            else if (incoming is SessionStopAction)
            {
                Stop(incoming.SessionId, incoming.Time);
            }
            else if (incoming is SessionSetDataAction)
            {
                var setData = (SessionSetDataAction)incoming;
                SetData(incoming.SessionId, setData.Key, JsonConvert.DeserializeObject(setData.Value), incoming.Time);
            }
            else if (incoming is SessionClearDataAction)
            {
                var clearData = (SessionClearDataAction)incoming;
                ClearData(incoming.SessionId, clearData.Key, incoming.Time);
            }
            else
            {
                return 0;
            }
            return 1;
        }

        public Option<SessionVector> GetSession(string sessionId) =>
            sessions.Find(sessionId);

        public void Start(string sessionId, int timeoutSeconds)
        {
            lock (sync)
            {
                if (sessions.ContainsKey(sessionId))
                {
                    return;
                }
                var session = SessionVector.Create(timeoutSeconds);
                sessions = sessions.Add(sessionId, session);
            }
        }

        public void Stop(string sessionId, long vector)
        {
            lock (sync)
            {
                if (!sessions.ContainsKey(sessionId))
                {
                    return;
                }

                var session = sessions[sessionId];

                // When it comes to stopping sessions we just get rid of the whole history
                // regardless of whether something happened in the future.  Session IDs
                // are randomly generated, so any future event with the same session ID
                // is a deleted future.
                sessions = sessions.Remove(sessionId);
            }
        }

        /// <summary>
        /// This is mutable for speed and to remove the need for a lock
        /// It really doesn't matter if it's slightly out
        /// </summary>
        public void Touch(string sessionId, bool notify) =>
            sessions.Find(sessionId).IfSome(s => s.Touch());

        public void SetData(string sessionId, string key, object value, long vector)
        {
            lock (sync)
            {
                if (!sessions.ContainsKey(sessionId))
                {
                    return;
                }

                var session = sessions[sessionId];
                session = session.SetKeyValue(vector, key, value, strategy);
            }
        }

        public void ClearData(string sessionId, string key, long vector)
        {
            lock (sync)
            {
                if (!sessions.ContainsKey(sessionId))
                {
                    return;
                }

                var session = sessions[sessionId];
                session = session.ClearKeyValue(vector, key);
            }
        }
    }
}