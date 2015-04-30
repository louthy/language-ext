using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class ActorContext
    {
        static IProcess root;
        static IProcess user;
        static IProcess system;
        static IProcess deadLetters;
        static IProcess noSender;
        static IProcess registered;
        [ThreadStatic] static IProcess self;
        [ThreadStatic] static ProcessId sender;

        static object storeLock = new object();

        static ActorContext()
        {
            Restart();
        }

        public static Unit Restart()
        {
            ObservableRouter.Restart();

            ActorConfig config = ActorConfig.Default;

            if (system != null)
            {
                system.Shutdown();
            }

            Store = map(tuple("", (IProcess)null));

            // Root
            root        = new Actor<Unit, object>(new ProcessId(), config.RootProcessName, Process.pub);

            // Top tier
            system      = new Actor<Unit, object>(root.Id, config.SystemProcessName, Process.pub);
            self = user = new Actor<Unit, object>(root.Id, config.UserProcessName, Process.pub);

            // Second tier
            deadLetters = new Actor<Unit, object>(system.Id, config.DeadLettersProcessName, Process.pub);

            noSender    = new Actor<Unit, object>(system.Id, config.NoSenderProcessName, Process.pub);
            registered  = new Actor<Unit, object>(system.Id, config.RegisteredProcessName, Process.pub);

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

        public static IProcess Self =>
            self == null
                ? user
                : self;

        public static IProcess Root =>
            root;

        public static IProcess User =>
            user;

        public static IProcess System =>
            system;

        public static ProcessId Sender =>
            sender;

        public static ProcessId NoSender =>
            noSender.Id;

        public static ProcessId DeadLetters =>
            deadLetters.Id;

        public static IProcess Registered =>
            registered;

        public static ProcessId Register(ProcessName name, ProcessId process) =>
            map(registered as IProcessInternal,
                self => match(self.GetChildProcess(name),
                    Some: _ => failwith<IProcess>("Process '"+ name + "' already registered"),
                    None: () => self.AddChildProcess( new ActorProxy(
                                                        registered.Id,
                                                        name,
                                                        ActorProxyTemplate.Registered,
                                                        () => new ActorProxyConfig(process) ) ) ) ).Id;

        public static Unit UnRegister(ProcessName name) =>
            map(registered.Id + ProcessId.Sep.ToString() + name,
                id =>
                    Store.ContainsKey(id)
                        ? Process.kill(id)
                        : unit);

        public static void SetContext(IProcess self, ProcessId sender)
        {
            ActorContext.self = self;
            ActorContext.sender = sender;
        }

        public static IProcess GetProcess(ProcessId pid) =>
            map(ActorContext.Store, store =>
                store.ContainsKey(pid.Value)
                    ? store[pid.Value]
                    : store[ActorContext.DeadLetters.Value]
                );
    }
}
