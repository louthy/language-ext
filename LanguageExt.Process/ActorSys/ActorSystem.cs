using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Threading;
using LanguageExt.Config;
using LanguageExt.Session;

namespace LanguageExt
{
    class ActorSystem : IDisposable
    {
        const string ClusterOnlineKey = "cluster-node-online";

        ActorRequestContext userContext;
        ClusterMonitor.State clusterState;
        IDisposable startupSubscription;
        Map<ProcessId, Set<ProcessId>> watchers;
        Map<ProcessId, Set<ProcessId>> watchings;
        Map<ProcessName, Set<ProcessId>> registeredProcessNames = Map<ProcessName, Set<ProcessId>>();
        Map<ProcessId, Set<ProcessName>> registeredProcessIds = Map<ProcessId, Set<ProcessName>>();
        ProcessSystemConfig settings;
        public AppProfile appProfile;
        ActorItem rootItem;

        readonly object sync = new object();
        readonly Option<ICluster> cluster;
        readonly long startupTimestamp;
        readonly object regsync = new object();
        readonly SessionManager sessionManager;
        public readonly SystemName SystemName;

        public AppProfile AppProfile => appProfile;
        public ProcessSystemConfig Settings => settings;

        public ActorSystem(SystemName systemName, Option<ICluster> cluster, AppProfile appProfile, ProcessSystemConfig settings)
        {
            var name = GetRootProcessName(cluster);
            if (name.Value == "root" && cluster.IsSome) throw new ArgumentException("Cluster node name cannot be 'root', it's reserved for local use only.");
            if (name.Value == "disp" && cluster.IsSome) throw new ArgumentException("Cluster node name cannot be 'disp', it's reserved for internal use.");
            if (name.Value == "js") throw new ArgumentException("Node name cannot be 'js', it's reserved for ProcessJS.");

            SystemName = systemName;
            this.appProfile = appProfile;
            this.settings = settings;
            this.cluster = cluster;
            startupTimestamp = DateTime.UtcNow.Ticks;
            sessionManager = new SessionManager(cluster, SystemName, appProfile.NodeName, VectorConflictStrategy.Branch);
            watchers = Map.empty<ProcessId, Set<ProcessId>>();
            watchings = Map.empty<ProcessId, Set<ProcessId>>();

            startupSubscription = NotifyCluster(cluster, startupTimestamp);

            Dispatch.init();
            Role.init(cluster.Map(r => r.Role).IfNone("local"));
            Reg.init();

            var root = ProcessId.Top.Child(GetRootProcessName(cluster));
            var rootInbox = new ActorInboxLocal<ActorSystemBootstrap, Unit>();
            var parent = new ActorItem(new NullProcess(SystemName), new NullInbox(), ProcessFlags.Default);

            var state = new ActorSystemBootstrap(
                SystemName,
                cluster,
                root, null,
                rootInbox,
                cluster.Map(x => x.NodeName).IfNone(ActorSystemConfig.Default.RootProcessName),
                ActorSystemConfig.Default,
                Settings,
                sessionManager.Sync
                );

            var rootProcess = state.RootProcess;
            state.Startup();
            userContext = new ActorRequestContext(
                this,
                rootProcess.Children["user"],
                ProcessId.NoSender,
                rootItem,
                null,
                null,
                ProcessFlags.Default,
                null,
                null);
            rootInbox.Startup(rootProcess, parent, cluster, settings.GetProcessMailboxSize(rootProcess.Id));
            rootItem = new ActorItem(rootProcess, rootInbox, ProcessFlags.Default);
        }

        public void Initialise()
        {
            ClusterWatch(cluster);
        }

        public SessionManager Sessions => sessionManager;
        public ActorRequestContext UserContext => userContext;

        class ClusterOnline
        {
            public string Name;
            public long Timestamp;
        }

        IDisposable NotifyCluster(Option<ICluster> cluster, long timestamp) =>
            cluster.Map(c =>
            {
                c.PublishToChannel(ClusterOnlineKey, new ClusterOnline { Name = c.NodeName.Value, Timestamp = timestamp });
                return c.SubscribeToChannel<ClusterOnline>(ClusterOnlineKey)
                        .Where(node => node.Name == c.NodeName.Value && node.Timestamp > timestamp)
                        .Take(1)
                        .Subscribe(_ => shutdownAll()); // Protects against multiple nodes with the same name running
            })
           .IfNone(() => Observable.Return(new ClusterOnline()).Take(1).Subscribe());

        public ClusterMonitor.State ClusterState =>
            clusterState;

        private void ClusterWatch(Option<ICluster> cluster)
        {
            var monitor = System[ActorSystemConfig.Default.MonitorProcessName];

            cluster.IfSome(c =>
            {
                observe<NodeOnline>(monitor).Subscribe(x =>
                {
                    logInfo("Online: " + x.Name);
                });

                observe<NodeOffline>(monitor).Subscribe(x =>
                {
                    logInfo("Offline: " + x.Name);
                    RemoveWatchingOfRemote(x.Name);
                });

                observeState<ClusterMonitor.State>(monitor).Subscribe(x =>
                {
                    clusterState = x;
                });
            });

            tell(monitor, new ClusterMonitor.Msg(ClusterMonitor.MsgTag.Heartbeat));
        }

        public void Dispose()
        {
            lock (sync)
            {
                var ri = rootItem;
                rootItem = null;
                if (ri != null)
                {
                    startupSubscription?.Dispose();
                    startupSubscription = null;
                    var item = ri;
                    ri.Actor.Children.Iter(c => c.Actor.ShutdownProcess(true));
                    cluster.IfSome(c => c?.Dispose());
                }
            }
        }

        private Unit RemoveWatchingOfRemote(ProcessName node)
        {
            var root = ProcessId.Top[node];

            foreach (var watching in watchings)
            {
                if (watching.Key.Take(1) == root)
                {
                    RemoteDispatchTerminate(watching.Key);
                }
            }
            return unit;
        }

        public Unit AddWatcher(ProcessId watcher, ProcessId watching)
        {
            logInfo(watcher + " is watching " + watching);

            lock (sync)
            {
                watchers = watchers.AddOrUpdate(watching,
                    Some: set => set.AddOrUpdate(watcher),
                    None: () => Set(watcher)
                );

                watchings = watchings.AddOrUpdate(watcher,
                    Some: set => set.AddOrUpdate(watching),
                    None: () => Set(watching)
                );
            }
            return unit;
        }

        public Unit RemoveWatcher(ProcessId watcher, ProcessId watching)
        {
            logInfo(watcher + " stopped watching " + watching);

            lock (sync)
            {
                watchers = watchers.AddOrUpdate(watching,
                    Some: set => set.Remove(watcher),
                    None: () => Set.empty<ProcessId>()
                );

                if (watchers[watching].IsEmpty)
                {
                    watchers = watchers.Remove(watching);
                }

                watchings = watchings.AddOrUpdate(watcher,
                    Some: set => set.Remove(watching),
                    None: () => Set.empty<ProcessId>()
                );

                if (watchings[watcher].IsEmpty)
                {
                    watchers = watchers.Remove(watcher);
                }

            }
            return unit;
        }

        public Unit RemoteDispatchTerminate(ProcessId terminating)
        {
            watchings.Find(terminating).IfSome(ws =>
            {
                var term = new TerminatedMessage(terminating);
                ws.Iter(w =>
                {
                    try
                    {
                        TellUserControl(w, term);
                    }
                    catch (Exception e)
                    {
                        logErr(e);
                    }
                });
            });
            watchings.Remove(terminating);
            watchers = watchers.Remove(terminating);

            return unit;
        }

        public Unit DispatchTerminate(ProcessId terminating)
        {
            watchers.Find(terminating).IfSome(ws =>
            {
                var term = new TerminatedMessage(terminating);
                ws.Iter(w =>
                {
                    try
                    {
                        TellUserControl(w, term);
                    }
                    catch (Exception e)
                    {
                        logErr(e);
                    }
                });
            });

            watchers = watchers.Remove(terminating);

            watchings.Find(terminating).IfSome(ws => ws.Iter(w => GetDispatcher(w).UnWatch(terminating)));
            watchings.Remove(terminating);

            return unit;
        }

        public Option<ICluster> Cluster =>
            cluster;

        public IEnumerable<T> AskMany<T>(IEnumerable<ProcessId> pids, object message, int take)
        {
            take = Math.Min(take, pids.Count());

            var handle = new CountdownEvent(take);

            var responses = new List<AskActorRes>();
            foreach (var pid in pids)
            {
                var req = new AskActorReq(
                    message,
                    res =>
                    {
                        responses.Add(res);
                        handle.Signal();

                    }, pid.SetSystem(SystemName), Self);
                tell(AskId, req);
            }

            handle.Wait(ActorContext.System(pids.Head()).Settings.Timeout);

            return responses.Where(r => !r.IsFaulted).Map(r => (T)r.Response);
        }

        public T Ask<T>(ProcessId pid, object message)
        {
            return (T)Ask(pid, message);
        }

        public object Ask(ProcessId pid, object message)
        {
            if (false) //Process.InMessageLoop)
            {
                //return SelfProcess.Actor.ProcessRequest<T>(pid, message);
            }
            else
            {
                AskActorRes response = null;
                var handle = new AutoResetEvent(false);

                var req = new AskActorReq(
                    message,
                    res =>
                    {
                        response = res;
                        handle.Set();
                    },
                    pid,
                    Self
                );

                var askItem = GetAskItem();

                askItem.IfSome(
                    ask =>
                    {
                        var inbox = ask.Inbox as ILocalActorInbox;
                        inbox.Tell(req, Self);
                        handle.WaitOne(ActorContext.System(pid).Settings.Timeout);
                    });

                if (askItem)
                {
                    if (response == null)
                    {
                        throw new TimeoutException("Request timed out");
                    }
                    else
                    {
                        if (response.IsFaulted)
                        {
                            throw response.Exception;
                        }
                        else
                        {
                            return response.Response;
                        }
                    }
                }
                else
                {
                    throw new Exception("Ask process doesn't exist");
                }
            }
        }

        public static ProcessName GetRootProcessName(Option<ICluster> cluster) =>
            cluster.Map(x => x.NodeName)
                   .IfNone(ActorSystemConfig.Default.RootProcessName);

        public ProcessId ActorCreate<S, T>(
            ActorItem parent,
            ProcessName name,
            Func<T, Unit> actorFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy,
            ProcessFlags flags,
            int maxMailboxSize,
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), termFn, strategy, flags, maxMailboxSize, lazy);

        public Unit UpdateSettings(ProcessSystemConfig settings, AppProfile profile)
        {
            this.settings = settings;
            this.appProfile = profile;
            // TODO: Consider notification system for Processes
            return unit;
        }

        public ProcessId ActorCreate<S, T>(
            ActorItem parent,
            ProcessName name,
            Action<T> actorFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy,
            ProcessFlags flags,
            int maxMailboxSize,
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), termFn, strategy, flags, maxMailboxSize, lazy);

        public ProcessId ActorCreate<S, T>(
            ActorItem parent,
            ProcessName name,
            Func<S, T, S> actorFn,
            Func<S> setupFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy,
            ProcessFlags flags,
            int maxMailboxSize,
            bool lazy
            ) =>
            ActorCreate(parent, name, actorFn, _ => setupFn(), termFn, strategy, flags, maxMailboxSize, lazy);

        public ProcessId ActorCreate<S, T>(
            ActorItem parent,
            ProcessName name,
            Func<S, T, S> actorFn,
            Func<IActor, S> setupFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy,
            ProcessFlags flags,
            int maxMailboxSize,
            bool lazy)
        {
            var actor = new Actor<S, T>(cluster, parent, name, actorFn, setupFn, termFn, strategy, flags, ActorContext.System(parent.Actor.Id).Settings);

            IActorInbox inbox = null;
            if ((actor.Flags & ProcessFlags.ListenRemoteAndLocal) == ProcessFlags.ListenRemoteAndLocal && cluster.IsSome)
            {
                inbox = new ActorInboxDual<S, T>();
            }
            else if ((actor.Flags & ProcessFlags.PersistInbox) == ProcessFlags.PersistInbox && cluster.IsSome)
            {
                inbox = new ActorInboxRemote<S, T>();
            }
            else
            {
                inbox = new ActorInboxLocal<S, T>();
            }

            var item = new ActorItem(actor, inbox, actor.Flags);

            parent.Actor.LinkChild(item);

            // Auto register if there are config settings and we
            // have the variable name it was assigned to.
            ActorContext.System(actor.Id).Settings.GetProcessRegisteredName(actor.Id).Iter(regName =>
            {
                // Also check if 'dispatch' is specified in the config, if so we will
                // register the Process as a role dispatcher PID instead of just its
                // PID.  
                ActorContext.System(actor.Id).Settings.GetProcessDispatch(actor.Id)
                      .Match(
                        Some: disp => Process.register(regName, Disp[$"role-{disp}"][Role.Current].Append(actor.Id.Skip(1))),
                        None: () => Process.register(regName, actor.Id)
                      );
            });

            try
            {
                inbox.Startup(actor, parent, cluster, maxMailboxSize);
                if (!lazy)
                {
                    TellSystem(actor.Id, SystemMessage.StartupProcess);
                }
            }
            catch
            {
                item?.Actor?.ShutdownProcess(false);
                throw;
            }
            return item.Actor.Id;
        }

        public ILocalActorInbox LocalRoot =>
            (ILocalActorInbox)rootItem.Inbox;

        public IActorInbox RootInbox =>
            rootItem.Inbox;

        public ProcessId Root =>
            rootItem.Actor.Id;

        public readonly ProcessId Disp =
            ProcessId.Top["disp"];

        public ProcessId System =>
            Root[ActorSystemConfig.Default.SystemProcessName];

        public ProcessId User =>
            Root[ActorSystemConfig.Default.UserProcessName];

        public ProcessId Errors =>
            System[ActorSystemConfig.Default.ErrorsProcessName];

        public ProcessId DeadLetters =>
            System[ActorSystemConfig.Default.DeadLettersProcessName];

        private ProcessName NodeName =>
            cluster.Map(c => c.NodeName).IfNone("user");

        internal ProcessId AskId =>
            System[ActorSystemConfig.Default.AskProcessName];

        public Option<ActorItem> GetJsItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey("js"))
            {
                return Some(children["js"]);
            }
            else
            {
                return None;
            }
        }

        public Option<ActorItem> GetAskItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey(ActorSystemConfig.Default.SystemProcessName.Value))
            {
                var sys = children[ActorSystemConfig.Default.SystemProcessName.Value];
                children = sys.Actor.Children;
                if (children.ContainsKey(ActorSystemConfig.Default.AskProcessName.Value))
                {
                    return Some(children[ActorSystemConfig.Default.AskProcessName.Value]);
                }
                else
                {
                    return None;
                }
            }
            else
            {
                return None;
            }
        }

        public Option<ActorItem> GetInboxShutdownItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey(ActorSystemConfig.Default.SystemProcessName.Value))
            {
                var sys = children[ActorSystemConfig.Default.SystemProcessName.Value];
                children = sys.Actor.Children;
                if (children.ContainsKey(ActorSystemConfig.Default.InboxShutdownProcessName.Value))
                {
                    return Some(children[ActorSystemConfig.Default.InboxShutdownProcessName.Value]);
                }
                else
                {
                    return None;
                }
            }
            else
            {
                return None;
            }
        }

        public Set<ProcessId> GetLocalRegistered(ProcessName name) =>
            registeredProcessNames
                .Find(name)
                .IfNone(Set.empty<ProcessId>());

        public ProcessId Register(ProcessName name, ProcessId pid)
        {
            if (!pid.IsValid)
            {
                throw new InvalidProcessIdException();
            }

            Cluster.Match(
                Some: c =>
                {
                    if (IsLocal(pid) && GetDispatcher(pid).IsLocal)
                    {
                        AddLocalRegistered(name, pid.SetSystem(SystemName));
                    }
                    else
                    {
                        // TODO - Make this transactional
                        // {
                        c.SetAddOrUpdate(ProcessId.Top["__registered"][name].Path, pid.Path);
                        c.SetAddOrUpdate(pid.Path + "-registered", name.Value);
                        // }
                    }
                },
                None: () => AddLocalRegistered(name, pid)
            );
            return Disp["reg"][name];
        }

        void AddLocalRegistered(ProcessName name, ProcessId pid)
        {
            lock (regsync)
            {
                registeredProcessNames = registeredProcessNames.AddOrUpdate(name,
                    Some: set => set.AddOrUpdate(pid.SetSystem(SystemName)),
                    None: () => Set(pid)
                );
                registeredProcessIds = registeredProcessIds.AddOrUpdate(pid.SetSystem(SystemName),
                    Some: set => set.AddOrUpdate(name),
                    None: () => Set(name)
                );
            }
        }

        public Unit DeregisterByName(ProcessName name)
        {
            Cluster.Match(
                Some: c =>
                {
                    RemoveLocalRegisteredByName(name);
                    var regpath = (ProcessId.Top["__registered"][name]).Path;

                    // TODO - Make this transactional
                    // {
                    var pids = c.GetSet<string>(regpath);
                    pids.Iter(pid =>
                    {
                        c.SetRemove(pid + "-registered", name.Value);
                    });
                    c.Delete(regpath);
                    // }
                },
                None: () =>
                {
                    RemoveLocalRegisteredByName(name);
                }
            );
            return unit;
        }

        public Unit DeregisterById(ProcessId pid)
        {
            if (!pid.IsValid) throw new InvalidProcessIdException();
            if (pid.Take(2) == Disp["reg"])
            {
                throw new InvalidProcessIdException(@"
When de-registering a Process, you should use its actual ProcessId, not its registered
ProcessId.  Multiple Processes can be registered with the same name, and therefore share
the same registered ProcessId.  The de-register system can only know for sure which Process
to de-register if you pass in the actual ProcessId.  If you want to deregister all Processes
by name then use Process.deregisterByName(name).");
            }

            Cluster.Match(
                Some: c =>
                {
                    if (IsLocal(pid) && GetDispatcher(pid).IsLocal)
                    {
                        RemoveLocalRegisteredById(pid);
                    }
                    else
                    {
                        var path = pid.Path;
                        var regpath = path + "-registered";

                        // TODO - Make this transactional
                        // {
                        var names = c.GetSet<string>(regpath);
                        names.Iter(name =>
                        {
                            c.SetRemove(ProcessId.Top["__registered"][name].Path, path);
                        });
                        c.Delete(regpath);
                        // }
                    }
                },
                None: () =>
                {
                    RemoveLocalRegisteredById(pid);
                }
            );
            return unit;
        }

        void RemoveLocalRegisteredById(ProcessId pid)
        {
            lock (regsync)
            {
                var names = registeredProcessIds.Find(pid).IfNone(Set.empty<ProcessName>());
                names.Iter(name =>
                    registeredProcessNames = registeredProcessNames.SetItem(name, registeredProcessNames[name].Remove(pid))
                );
                registeredProcessIds = registeredProcessIds.Remove(pid);
            }
        }

        void RemoveLocalRegisteredByName(ProcessName name)
        {
            lock (regsync)
            {
                var pids = registeredProcessNames.Find(name).IfNone(Set.empty<ProcessId>());

                pids.Iter(pid =>
                    registeredProcessIds = registeredProcessIds.SetItem(pid, registeredProcessIds[pid].Remove(name))
                );
                registeredProcessNames = registeredProcessNames.Remove(name);
            }
        }

        public R WithContext<R>(ActorItem self, ActorItem parent, ProcessId sender, ActorRequest request, object msg, Option<SessionId> sessionId, Func<R> f)
        {
            var savedContext = ActorContext.Request;
            var savedSession = ActorContext.SessionId;

            try
            {
                ActorContext.SessionId = sessionId;

                ActorContext.SetContext(new ActorRequestContext(
                    this,
                    self,
                    sender,
                    parent,
                    msg,
                    request,
                    ProcessFlags.Default,
                    ProcessOpTransaction.Start(self.Actor.Id),
                    (from sid in sessionId
                     from ses in ActorContext.System(self.Actor.Id).Sessions.GetSession(sid)
                     select ses)
                    .IfNoneUnsafe((SessionVector)null)
                ));
                return f();
            }
            finally
            {
                ActorContext.SessionId = savedSession;
                ActorContext.SetContext(savedContext);
            }
        }

        public Unit WithContext(ActorItem self, ActorItem parent, ProcessId sender, ActorRequest request, object msg, Option<SessionId> sessionId, Action f) =>
            WithContext(self, parent, sender, request, msg, sessionId, fun(f));

        internal IObservable<T> Observe<T>(ProcessId pid) =>
            GetDispatcher(pid).Observe<T>();

        internal IObservable<T> ObserveState<T>(ProcessId pid) =>
            GetDispatcher(pid).ObserveState<T>();

        internal ProcessId SenderOrDefault(ProcessId sender) =>
            sender.IsValid
                ? sender
                : Self;

        internal IActorDispatch GetJsDispatcher(ProcessId pid)
        {
            switch (pid.Count())
            {
                case 0:
                case 1:
                    return new ActorDispatchNotExist(pid);

                //  /root/js                            <-- relay
                case 2:
                    return GetDispatcher(pid.Skip(1), rootItem, pid);

                //  /root/js/{connection id}            <-- relay
                case 3:
                    return GetDispatcher(pid.Skip(1), rootItem, pid);

                // /root/js/{connection id}/js-root/..  --> back to JS
                default:
                    return new ActorDispatchJS(pid, pid.Take(3), rootItem.Actor.Children["js"]);
            }
        }

        internal IActorDispatch GetLocalDispatcher(ProcessId pid) =>
            pid.Take(2) == Root["js"]
                ? GetJsDispatcher(pid)
                : GetDispatcher(pid.Tail(), rootItem, pid);

        internal IActorDispatch GetRemoteDispatcher(ProcessId pid) =>
            cluster.Match<IActorDispatch>(
                Some: c => new ActorDispatchRemote(pid, c),
                None: () => new ActorDispatchNotExist(pid));

        internal Option<Func<ProcessId, IEnumerable<ProcessId>>> GetProcessSelector(ProcessId pid)
        {
            if (pid.Count() < 3) throw new InvalidProcessIdException("Invalid role Process ID");
            var type = pid.Skip(1).Take(1).GetName();
            return Dispatch.getFunc(type);
        }

        internal IEnumerable<ProcessId> ResolveProcessIdSelection(ProcessId pid) =>
            GetProcessSelector(pid)
                .Map(selector => selector(pid.Skip(2)))
                .IfNone(() => new ProcessId[0]);

        internal IActorDispatch GetPluginDispatcher(ProcessId pid) =>
            GetProcessSelector(pid)
                .Map(selector => new ActorDispatchGroup(selector(pid.Skip(2))) as IActorDispatch)
                .IfNone(() => new ActorDispatchNotExist(pid));

        internal bool IsLocal(ProcessId pid) =>
            pid.Head() == Root;

        internal bool IsDisp(ProcessId pid) =>
            pid.Head() == Disp;

        internal IActorDispatch GetDispatcher(ProcessId pid) =>
            pid.IsValid
                ? pid.IsSelection
                    ? new ActorDispatchGroup(pid.GetSelection())
                    : IsDisp(pid)
                        ? GetPluginDispatcher(pid)
                        : IsLocal(pid)
                            ? GetLocalDispatcher(pid)
                            : GetRemoteDispatcher(pid)
                : new ActorDispatchNotExist(pid);

        IActorDispatch GetDispatcher(ProcessId pid, ActorItem current, ProcessId orig)
        {
            if (pid == ProcessId.Top)
            {
                if (current.Inbox is ILocalActorInbox)
                {
                    return new ActorDispatchLocal(current);
                }
                else
                {
                    return cluster.Match<IActorDispatch>(
                            Some: c => new ActorDispatchRemote(orig, c),
                            None: () => new ActorDispatchNotExist(orig));
                }
            }
            else
            {
                var child = pid.Head().GetName().Value;
                if (current.Actor.Children.ContainsKey(child))
                {
                    var process = current.Actor.Children[child];
                    return GetDispatcher(pid.Tail(), process, orig);
                }
                else
                {
                    return new ActorDispatchNotExist(orig);
                }
            }
        }

        public Unit Ask(ProcessId pid, object message, ProcessId sender) =>
            GetDispatcher(pid).Ask(message, sender);

        public Unit Tell(ProcessId pid, object message, ProcessId sender) =>
            GetDispatcher(pid).Tell(message, sender, message is ActorRequest ? Message.TagSpec.UserAsk : Message.TagSpec.User);

        public Unit TellUserControl(ProcessId pid, UserControlMessage message) =>
            GetDispatcher(pid).TellUserControl(message, Self);

        public Unit TellSystem(ProcessId pid, SystemMessage message) =>
            GetDispatcher(pid).TellSystem(message, Self);

        public Map<string, ProcessId> GetChildren(ProcessId pid) =>
            GetDispatcher(pid).GetChildren();

        public Unit Kill(ProcessId pid, bool maintainState) =>
            maintainState
                ? GetDispatcher(pid).Shutdown()
                : GetDispatcher(pid).Kill();

        public Option<ActorItem> GetLocalActor(ProcessId pid)
        {
            if (pid.System != SystemName) return None;
            return GetLocalActor(rootItem, pid.Skip(1), pid);
        }

        Option<ActorItem> GetLocalActor(ActorItem current, ProcessId walk, ProcessId pid)
        {
            if (current.Actor.Id == pid) return current;
            var name = walk.Take(1).GetName();
            return from child in current.Actor.Children.Find(walk.Take(1).GetName().Value)
                   from result in GetLocalActor(child, walk.Skip(1), pid)
                   select result;
        }

    }
}
