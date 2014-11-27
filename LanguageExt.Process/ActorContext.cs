using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt.Prelude;
using System.Collections.Immutable;
using System.Reactive.Subjects;

namespace LanguageExt
{
    internal static class ActorContext
    {
        static Subject<object> deadLetterSubject = new Subject<object>();

        static IProcess system;
        static IProcess deadLetters;
        static IProcess noSender;
        [ThreadStatic] static IProcess self;
        [ThreadStatic] static ProcessId sender;

        static object processesLock = new object();
        static object storeLock = new object();

        static ActorContext()
        {
            Restart();
        }

        public static Unit Restart()
        {
            if (system != null)
            {
                system.Shutdown();
            }

            Processes = map<string, ProcessId>();
            Store = map(tuple("", (IProcess)null));
            self = system = new Actor<Unit, string>(new ProcessId(), new ProcessName("system"), (Unit state, string msg) => state, () => unit);

            deadLetters = new Actor<Unit, object>(self.Id, "dead-letters", (state, msg) => { deadLetterSubject.OnNext(msg); return state; }, () => unit);
            noSender = new Actor<Unit, object>(self.Id, "no-sender", (state, msg) => state, () => unit);

            return unit;
        }

        public static Unit AddToStore(ProcessId id, IProcess process)
        {
            lock (storeLock)
            {
                var path = id.Value;
                if (Store.ContainsKey(path))
                {
                    Store[path].Dispose();
                    Store = Store.Remove(path);
                }
                // TODO: Create a Exception type 
                Store = Store.Add(path, process);
            }
            return unit;
        }

        public static Unit RemoveFromStore(ProcessId id)
        {
            lock (storeLock)
            {
                var path = id.Value;
                if (Store.ContainsKey(path))
                {
                    Store = Store.Remove(path);
                }
            }
            return unit;
        }

        public static IImmutableDictionary<string, IProcess> Store
        {
            get;
            private set;
        }

        public static IImmutableDictionary<string, ProcessId> Processes
        {
            get;
            private set;
        }

        public static IProcess Self
        {
            get
            {
                return self == null
                    ? system
                    : self;
            }
        }

        public static IProcess System
        {
            get
            {
                return system;
            }
        }

        public static ProcessId Sender
        {
            get
            {
                return sender;
            }
        }

        public static ProcessId NoSender
        {
            get
            {
                return noSender.Id;
            }
        }

        public static ProcessId DeadLetters
        {
            get
            {
                return deadLetters.Id;
            }
        }

        public static Unit Register(string name, ProcessId process)
        {
            lock(processesLock)
            {
                if (Processes.ContainsKey(name))
                {
                    Processes = Processes.Add(name, process);
                }
                else
                {
                    // TODO: Create a Exception type 
                    throw new Exception("Process already registered");
                }
            }
            return unit;
        }

        public static void SetContext(IProcess self, ProcessId sender)
        {
            ActorContext.self = self;
            ActorContext.sender = sender;
        }

        public static IProcess GetProcess(ProcessId pid) =>
            pid.Value == "/system"
                ? ActorContext.System
                : ActorContext.Store.ContainsKey(pid.Value)
                    ? ActorContext.Store[pid.Value]
                    : ActorContext.Store[ActorContext.DeadLetters.Value]; // TODO:
    }
}
