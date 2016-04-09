using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.Trans;

namespace LanguageExt.Session
{
    public enum VectorClockConflictStrategy
    {
        First,
        Last,
        Branch
    }

    class ValueVector
    {
        public readonly long Time;
        public readonly Lst<object> Vector;

        public ValueVector(long time, Lst<object> root)
        {
            Time = time;
            Vector = root;
        }

        public ValueVector AddValue(long time, object value, VectorClockConflictStrategy strategy)
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

            if (Vector.Exists(x => x == value))
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
                    case VectorClockConflictStrategy.First:  return this;
                    case VectorClockConflictStrategy.Last:   return new ValueVector(time, List(value));
                    case VectorClockConflictStrategy.Branch: return new ValueVector(Time, Vector.Add(value));
                    default: throw new ArgumentException("VectorClockConflictStrategy not supported: " + strategy);
                }
            }
        }
    }

    class SessionVector
    {
        public readonly int TimeoutSeconds;
        public readonly Map<string, Versioned<object>> Data;
        public DateTime LastAccess;

        public static SessionVector Create(int timeout) =>
            new SessionVector(Map.empty<string, Versioned<object>>(), DateTime.UtcNow, timeout);

        public SessionVector(Map<string, Versioned<object>> data, DateTime lastAccess, int timeoutSeconds)
        {
            Data = data;
            LastAccess = lastAccess;
            TimeoutSeconds = timeoutSeconds;
        }

        public void Touch() =>
            LastAccess = DateTime.UtcNow;

        public SessionVector ClearKeyValue(long vector, string key) =>
            new SessionVector(Data.Remove(key), DateTime.UtcNow, TimeoutSeconds);

        public SessionVector SetKeyValue(long time, int nodeId, string key, object value)
        {
            Data.Find(key)
                .Map( version =>
                {
                    var newVersion= version.Version.Incr()

                })


            //Versioned<object> v = new Versioned<object>(value, new VectorClock() )

            //var data = Data.Find(key)
            //               .Map(vector => Data.AddOrUpdate(key, vector.AddValue(time, value, strategy)))
            //               .IfNone(() => Data.AddOrUpdate(key, new ValueVector(time, List(value))));

            //return new SessionVector(data, DateTime.UtcNow, TimeoutSeconds);
        }
    }
}
