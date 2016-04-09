using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Session
{
    public abstract class SessionAction
    {
        public readonly long Time;
        public readonly string SessionId;

        public SessionAction(long vector, string sessionId)
        {
            Time = vector;
            SessionId = sessionId;
        }

        public static SessionAction Touch(string sessionId) => 
            new SessionTouchAction(sessionId);

        public static SessionAction Stop(string sessionId) => 
            new SessionStopAction(sessionId);

        public static SessionAction Start(string sessionId, int timeoutSeconds) => 
            new SessionStartAction(sessionId, timeoutSeconds);

        public static SessionAction ClearData(long time, string sessionId, string key) =>
            new SessionClearDataAction(time, sessionId, key);

        public static SessionAction SetData(long time, string sessionId, string key, string data) => 
            new SessionSetDataAction(time, sessionId, key, data);
    }

    class SessionTouchAction : SessionAction
    {
        public SessionTouchAction(string sessionId)
            :
            base(0, sessionId)
        {
        }
    }

    class SessionStartAction : SessionAction
    {
        public readonly int TimeoutSeconds;

        public SessionStartAction(string sessionId, int timeoutSeconds)
            :
            base(0, sessionId)
        {
            TimeoutSeconds = timeoutSeconds;
        }
    }

    class SessionStopAction : SessionAction
    {
        public SessionStopAction(string sessionId)
            :
            base(0, sessionId)
        {
        }
    }

    class SessionSetDataAction : SessionAction
    {
        public readonly string Key;
        public readonly string Value;

        public SessionSetDataAction(long time, string sessionId, string key, string value)
            :
            base(time, sessionId)
        {
            Key = key;
            Value = value;
        }
    }

    class SessionClearDataAction : SessionAction
    {
        public readonly string Key;

        public SessionClearDataAction(long time, string sessionId, string key)
            :
            base(time, sessionId)
        {
            Key = key;
        }
    }
}
