using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Session
{
    public enum SessionActionTag
    {
        Touch,
        Stop,
        Start,
        ClearData,
        SetData
    }

    public class SessionAction
    {
        public readonly SessionActionTag Tag;
        public readonly long Time;
        public readonly SessionId SessionId;
        public readonly string SystemName;
        public readonly string NodeName;
        public readonly int Timeout;
        public readonly string Key;
        public readonly string Value;
        public readonly string Type;

        [JsonConstructor]
        public SessionAction(SessionActionTag tag, long time, SessionId sessionId, string systemName, string nodeName, int timeout, string key, string value, string type)
        {
            Tag = tag;
            Time = time;
            SessionId = sessionId;
            SystemName = systemName;
            NodeName = nodeName;
            Timeout = timeout;
            Key = key;
            Value = value;
            Type = type;
        }

        public static SessionAction Touch(SessionId sessionId, SystemName systemName, ProcessName nodeName) =>
            new SessionAction(SessionActionTag.Touch, 0L, sessionId, systemName.Value, nodeName.Value, 0, null, null, null);

        public static SessionAction Stop(SessionId sessionId, SystemName systemName, ProcessName nodeName) =>
            new SessionAction(SessionActionTag.Stop, 0L, sessionId, systemName.Value, nodeName.Value, 0, null, null, null);

        public static SessionAction Start(SessionId sessionId, int timeoutSeconds, SystemName systemName, ProcessName nodeName) =>
            new SessionAction(SessionActionTag.Start, 0L, sessionId, systemName.Value, nodeName.Value, timeoutSeconds, null, null, null);

        public static SessionAction ClearData(long time, SessionId sessionId, string key, SystemName systemName, ProcessName nodeName) =>
            new SessionAction(SessionActionTag.ClearData, time, sessionId, systemName.Value, nodeName.Value, 0, key, null, null);

        public static SessionAction SetData(long time, SessionId sessionId, string key, string data, SystemName systemName, ProcessName nodeName) =>
            new SessionAction(SessionActionTag.SetData, time, sessionId, systemName.Value, nodeName.Value, 0, key, data, data.GetType().AssemblyQualifiedName);
    }
}
