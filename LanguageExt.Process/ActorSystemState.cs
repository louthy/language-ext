using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using LanguageExt.Trans;
using System.Threading;

namespace LanguageExt
{
    /// <summary>
    /// Represents the state of the whole actor system.  Mostly it holds the store of
    /// processes (actors) and their inboxes.
    /// </summary>
    internal class ActorSystemState
    {
        public readonly Option<ICluster> Cluster;
        public readonly IActorInbox RootInbox;
        public readonly IActor RootProcess;
        public readonly ActorSystemConfig Config;
        public readonly ProcessName RootProcessName;

        ActorItem root;
        ActorItem sessions;
        ActorItem user;
        ActorItem system;
        ActorItem deadLetters;
        ActorItem errors;
        ActorItem inboxShutdown;
        ActorItem ask;
        ActorItem js;
        ActorItem reply;
        ActorItem monitor;

        public ActorSystemState(Option<ICluster> cluster, ProcessId rootId, IActor rootProcess, IActorInbox rootInbox, ProcessName rootProcessName, ActorSystemConfig config)
        {
            var parent = new ActorItem(new NullProcess(), new NullInbox(), ProcessFlags.Default);

            rootProcess = new Actor<ActorSystemState, Unit>(
                cluster,
                parent,
                rootProcessName,
                ActorSystem.Inbox,
                _ => this,
                null,
                Process.DefaultStrategy,
                ProcessFlags.Default
            );

            root = new ActorItem(rootProcess, RootInbox, rootProcess.Flags);
            Config = config;
            Cluster = cluster;
            RootProcess = rootProcess;
            RootInbox = rootInbox;
            RootProcessName = rootProcessName;
        }

        private Option<ActorItem> GetItem(ProcessId pid) =>
            pid.IsValid
                ? pid.Head() == RootProcess.Id
                    ? GetItem(pid.Tail(), root)
                    : failwith<Option<ActorItem>>("Can't get non-local items from within ActorSystemState")
                : None;

        private Option<ActorItem> GetItem(ProcessId pid, ActorItem current)
        {
            if (pid == ProcessId.Top)
            {
                return current;
            }
            else
            {
                var child = pid.Head().GetName().Value;
                if (current.Actor.Children.ContainsKey(child))
                {
                    var process = current.Actor.Children[child];
                    return GetItem(pid.Tail(), process);
                }
                else
                {
                    return None;
                }
            }
        }

        private ProcessName NodeName =>
            Cluster.Map(c => c.NodeName).IfNone("user");

        public ActorSystemState Startup()
        {
            logInfo("Process system starting up");

            // Top tier
            system          = ActorCreate<object>(root, Config.SystemProcessName, publish, null, ProcessFlags.Default);
            user            = ActorCreate<object>(root, Config.UserProcessName, publish, null, ProcessFlags.Default);
            js              = ActorCreate<ProcessId, RelayMsg>(root, "js", RelayActor.Inbox, () => User["process-hub-js"], null, ProcessFlags.Default);

            // Second tier
            sessions        = ActorCreate<SessionManagerProcess.State, SessionManagerProcess.Msg>(
                                system, 
                                ActorSystemConfig.Default.Sessions,
                                SessionManagerProcess.Inbox,
                                SessionManagerProcess.Setup,
                                null,
                                ProcessFlags.Default,
                                100000
                                );

            deadLetters     = ActorCreate<DeadLetter>(system, Config.DeadLettersProcessName, publish, null, ProcessFlags.Default);
            errors          = ActorCreate<Exception>(system, Config.ErrorsProcessName, publish, null, ProcessFlags.Default);
            monitor         = ActorCreate<ClusterMonitor.State, ClusterMonitor.Msg>(system, Config.MonitorProcessName, ClusterMonitor.Inbox, ClusterMonitor.Setup, null, ProcessFlags.Default);

            inboxShutdown   = ActorCreate<IActorInbox>(system, Config.InboxShutdownProcessName, inbox => inbox.Shutdown(), null, ProcessFlags.Default, 100000);

            reply = ask     = ActorCreate<Tuple<long, Dictionary<long, AskActorReq>>, object>(system, Config.AskProcessName, AskActor.Inbox, AskActor.Setup, null, ProcessFlags.ListenRemoteAndLocal);

            logInfo("Process system startup complete");

            return this;
        }

        public Unit Shutdown()
        {
            logInfo("Process system shutting down");

            user?.Actor?.ShutdownProcess(true);
            user = ActorCreate<object>(root, Config.UserProcessName, publish, null, ProcessFlags.Default);

            if (ActorContext.CurrentRequest != null && ActorContext.CurrentRequest.RequestId != -1)
            {
                tell(ActorContext.CurrentRequest.ReplyTo, new ActorResponse(unit, unit.GetType().AssemblyQualifiedName, ActorContext.CurrentRequest.ReplyTo, root.Actor.Id, ActorContext.CurrentRequest.RequestId), ActorContext.Root);
            }

            logInfo("Process system shutdown complete");

            return unit;
        }

        public ActorItem ActorCreate<T>(ActorItem parent, ProcessName name, Func<T, Unit> actorFn, Func<Unit, ProcessId, Unit> termFn, ProcessFlags flags, int maxMailboxSize = -1)
        {
            return ActorCreate<Unit, T>(parent, name, (s, t) => { actorFn(t); return unit; }, () => unit, termFn, flags, maxMailboxSize);
        }

        public ActorItem ActorCreate<T>(ActorItem parent, ProcessName name, Action<T> actorFn, Func<Unit, ProcessId, Unit> termFn, ProcessFlags flags, int maxMailboxSize = -1)
        {
            return ActorCreate<Unit, T>(parent, name, (s, t) => { actorFn(t); return unit; }, () => unit, termFn, flags, maxMailboxSize);
        }

        public ActorItem ActorCreate<S, T>(ActorItem parent, ProcessName name, Func<S, T, S> actorFn, Func<S> setupFn, Func<S, ProcessId, S> termFn, ProcessFlags flags, int maxMailboxSize = -1)
        {
            if (ProcessDoesNotExist(nameof(ActorCreate), parent.Actor.Id)) return null;

            var actor = new Actor<S, T>(Cluster, parent, name, actorFn, _ => setupFn(), termFn, Process.DefaultStrategy, flags);

            IActorInbox inbox = null;
            if ((flags & ProcessFlags.ListenRemoteAndLocal) == ProcessFlags.ListenRemoteAndLocal && Cluster.IsSome)
            {
                inbox = new ActorInboxDual<S, T>();
            }
            else if ((flags & ProcessFlags.PersistInbox) == ProcessFlags.PersistInbox && Cluster.IsSome)
            {
                inbox = new ActorInboxRemote<S, T>();
            }
            else
            {
                inbox = new ActorInboxLocal<S, T>();
            }

            var item = new ActorItem(actor, inbox, flags);
            try
            {
                parent.Actor.LinkChild(item);
                inbox.Startup(actor, actor.Parent, Cluster, 
                    maxMailboxSize == -1 
                        ? ActorContext.Config.GetProcessMailboxSize(actor.Id)
                        : maxMailboxSize
                    );
            }
            catch (Exception e)
            {
                item.Actor.ShutdownProcess(false);
                logSysErr(new ProcessException($"Process failed starting up: {e.Message}", actor.Id.Path, actor.Parent.Actor.Id.Path, e));
            }

            return item;
        }

        private bool ProcessDoesNotExist(string func, ProcessId pid)
        {
            if (GetItem(pid))
            {
                return false;
            }
            else
            {
                logErr($"{func}: process doesn't exist: {pid}");
                return true;
            }
        }
    }
}
