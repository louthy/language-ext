using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;
using LanguageExt.Config;
#if COREFX
using System.Threading;
#else
using System.Runtime.Remoting.Messaging;
#endif

namespace LanguageExt
{
    static class ActorContext
    {
        [ThreadStatic]
        static SystemName context;

        [ThreadStatic]
        static Option<SessionId> sessionId;

        [ThreadStatic]
        static ActorRequestContext request;

        static ConcurrentDictionary<SystemName, ActorSystem> systems = new ConcurrentDictionary<SystemName, ActorSystem>();

        public static Unit StartSystem(SystemName system, Option<ICluster> cluster, AppProfile appProfile, ProcessSystemConfig config)
        {
            if(systems.ContainsKey(system))
            {
                //throw new InvalidOperationException($"Process-system ({system}) already started");
                StopSystem(system);
            }

            var asystem = new ActorSystem(system, cluster, appProfile, config);
            systems.AddOrUpdate(system, asystem,(_,__) => asystem);
            try
            {
                asystem.Initialise();
            }
            catch
            {
                systems.TryRemove(system, out asystem);
                try
                {
                    asystem.Dispose();
                }
                catch { }
                throw;
            }
            return unit;
        }

        public static bool InMessageLoop =>
            request != null;

        public static Lst<SystemName> Systems =>
            systems.Keys.Freeze();

        public static Unit StopAllSystems()
        {
            while (systems.Any())
            {
                var head = systems.Map(x => x.Key).FirstOrDefault();
                StopSystem(head);
            }

            return unit;
        }

        public static Unit StopSystem(SystemName system)
        {
            ActorSystem asystem = null;
            var token = new ShutdownCancellationToken(system);
            try
            {
                Process.OnPreShutdown(token);
            }
            finally
            {
                if (!token.Cancelled)
                {
                    try
                    {
                        systems.TryGetValue(system, out asystem);
                        if (asystem != null)
                        {
                            asystem.Dispose();
                        }
                    }
                    finally
                    {
                        systems.TryRemove(system, out asystem);
                        Process.OnShutdown(system);
                    }
                }
            }
            return unit;
        }

        public static Unit SetSystem(SystemName system)
        {
            context = system;
            return unit;
        }

        public static Unit SetSystem(ActorSystem system)
        {
            context = system.SystemName;
            return unit;
        }

        public static Unit SetContext(ActorRequestContext requestContext)
        {
            request = requestContext;
            return unit;
        }

        public static ActorSystem System(ProcessId pid) =>
            System(pid.System);

        public static ActorSystem System(SystemName system)
        {
            if (!system.IsValid)
            {
                if (!context.IsValid)
                {
                    switch (systems.Count)
                    {
                        case 0: throw new ProcessConfigException("You must call one of the  ProcessConfig.initialiseXXX functions");
                        case 1: context = systems.Head().Key; break;
                        default: throw new Exception("There are multiple Process systems running, you must specify a context when calling-in from outside of the Process system");
                    }
                }
                ActorSystem asystem = null;
                return systems.TryGetValue(context, out asystem)
                    ? asystem
                    : failwith<ActorSystem>($"Process-system does not exist {context}");
            }
            else
            {
                ActorSystem asystem = null;
                return systems.TryGetValue(system, out asystem)
                    ? asystem
                    : failwith<ActorSystem>($"Process-system does not exist {system}");
            }
        }

        public static ActorSystem DefaultSystem
        {
            get
            {
                if (!context.IsValid)
                {
                    switch (systems.Count)
                    {
                        case 0: throw new ProcessConfigException("You must call one of the  ProcessConfig.initialiseXXX functions");
                        case 1: context = systems.Head().Key; break;
                        default: throw new Exception("There are multiple Process systems running, you must specify a context when calling-in from outside of the Process system");
                    }
                }
                return systems[context];
            }
        }

        public static ProcessId Self => 
            InMessageLoop
                ? Request.Self.Actor.Id
                : DefaultSystem.User;

        public static ActorItem SelfProcess =>
            InMessageLoop
                ? Request.Self
                : DefaultSystem.UserContext.Self;

        public static ActorRequestContext Request =>
            request;

        public static Option<SessionId> SessionId
        {
            get
            {
                return sessionId;
            }
            set
            {
                sessionId = value;
            }
        }

        public static Unit Publish(object message) =>
            InMessageLoop
                ? Request.Self.Actor.Publish(message)
                : failwith<Unit>("publish called outside of the message loop");

        public static ProcessId ResolvePID(ProcessId pid)
        {
            if (pid.Path == "/__special__/self" && Request == null) return System(context).User;
            if (pid.Path == "/__special__/self" && Request != null) return ActorContext.Self;
            if (pid.Path == "/__special__/sender" && Request == null) return ProcessId.NoSender;
            if (pid.Path == "/__special__/sender" && Request != null) return Request.Self.Actor.Id;
            if (pid.Path == "/__special__/parent" && Request == null) return System(context).User;
            if (pid.Path == "/__special__/parent" && Request != null) return Request.Parent.Actor.Id;
            if (pid.Path == "/__special__/user") return System(context).User;
            if (pid.Path == "/__special__/dead-letters") return System(context).DeadLetters;
            if (pid.Path == "/__special__/root") return System(context).Root;
            if (pid.Path == "/__special__/errors") return System(context).Errors;
            return pid;
        }
    }
}
