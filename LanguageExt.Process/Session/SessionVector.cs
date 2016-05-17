using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Session
{
    /// <summary>
    /// Version vector conflict strategy
    /// </summary>
    public enum VectorConflictStrategy
    {
        /// <summary>
        /// Take the first item
        /// </summary>
        First,

        /// <summary>
        /// Take the last item
        /// </summary>
        Last,

        /// <summary>
        /// Keep all items of the same time
        /// </summary>
        Branch
    }

    /// <summary>
    /// Simple version vector.  There can be multiple values stored for the
    /// same event. The implementation will be replaced with Dotted Version 
    /// Vectors once I have implemented a general system for it in the Core.
    /// </summary>
    public class ValueVector
    {
        public readonly long Time;
        public readonly Lst<object> Vector;

        public ValueVector(long time, Lst<object> root)
        {
            Time = time;
            Vector = root;
        }

        public ValueVector AddValue(long time, object value, VectorConflictStrategy strategy)
        {
            if(Vector.Count == 0 || time > Time)
            {
                return new ValueVector(time, List(value));
            }

            if( time < Time)
            {
                // A value from the past has arrived, we're going to drop it because
                // we've already moved on.
                return this;
            }

            if (Vector.Exists(x => x.Equals(value)))
            {
                // There's already an entry at the same time with the
                // same value
                return this;
            }
            else
            {
                // Conflict!
                switch(strategy)
                {
                    case VectorConflictStrategy.First:  return this;
                    case VectorConflictStrategy.Last:   return new ValueVector(time, List(value));
                    case VectorConflictStrategy.Branch: return new ValueVector(Time, Vector.Add(value));
                    default: throw new ArgumentException("VectorConflictStrategy not supported: " + strategy);
                }
            }
        }
    }

    public class SessionVector
    {
        public readonly int TimeoutSeconds;

        Map<string, ValueVector> data;
        DateTime lastAccess;
        DateTime expires;
        object sync = new object();

        public static SessionVector Create(int timeout, VectorConflictStrategy strategy, Map<string,object> initialState) =>
            new SessionVector(Map.empty<string, ValueVector>(), DateTime.UtcNow, timeout, initialState);

        /// <summary>
        /// Ctor
        /// </summary>
        public SessionVector(Map<string, ValueVector> data, DateTime lastAccess, int timeoutSeconds, Map<string, object> initialState)
        {
            this.data = data;
            this.lastAccess = lastAccess;
            TimeoutSeconds = timeoutSeconds;
            data = initialState.Map(obj => new ValueVector(0, List(obj)));
        }

        /// <summary>
        /// Key/value store for the session
        /// </summary>
        public Map<string, ValueVector> Data => data;

        /// <summary>
        /// UTC date of last access
        /// </summary>
        public DateTime LastAccess => lastAccess;

        /// <summary>
        /// The date-time of expiry
        /// </summary>
        public DateTime Expires => expires;

        /// <summary>
        /// Invoke to keep the session alive
        /// </summary>
        public void Touch()
        {
            lastAccess = DateTime.UtcNow;
            expires = lastAccess.AddSeconds(TimeoutSeconds);
        }

        /// <summary>
        /// Remove a key from the session key/value store
        /// </summary>
        public void ClearKeyValue(long vector, string key)
        {
            lock (sync)
            {
                data = data.Remove(key);
            }
            Touch();
        }

        /// <summary>
        /// Add or update a key in the session key/value store
        /// </summary>
        public void SetKeyValue(long time, string key, object value, VectorConflictStrategy strategy)
        {
            lock (sync)
            {
                data = data.Find(key)
                           .Map(vector => data.AddOrUpdate(key, vector.AddValue(time, value, strategy)))
                           .IfNone(() => data.AddOrUpdate(key, new ValueVector(time, List(value))));
            }
            Touch();
        }
    }
}
