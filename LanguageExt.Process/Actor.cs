using System;
using System.Threading;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using LanguageExt.UnitsOfMeasure;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// Internal class that represents the state of a single process.
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="T">Message type</typeparam>
    class Actor<S, T> : IActor
    {
        readonly Func<S, T, S> actorFn;
        readonly Func<IActor, S> setupFn;
        readonly ProcessFlags flags;
        readonly Subject<object> publishSubject = new Subject<object>();
        readonly Subject<object> stateSubject = new Subject<object>();
        readonly Option<ICluster> cluster;
        Map<string, IDisposable> subs = Map.create<string, IDisposable>();
        Map<string, ActorItem> children = Map.create<string, ActorItem>();
        Option<S> state;
        StrategyState strategyState = StrategyState.Empty;
        EventWaitHandle request;
        volatile ActorResponse response;
        object sync = new object();
        bool remoteSubsAcquired;

        internal Actor(Option<ICluster> cluster, ActorItem parent, ProcessName name, Func<S, T, S> actor, Func<IActor, S> setup, State<StrategyContext, Unit> strategy, ProcessFlags flags)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            this.cluster = cluster;
            this.flags = flags;
            actorFn = actor;
            setupFn = setup;
            Parent = parent;
            Name = name;
            Strategy = strategy ?? Process.DefaultStrategy;
            Id = parent.Actor.Id[name];
            SetupRemoteSubscriptions(cluster, flags);
        }

        /// <summary>
        /// Start up - placeholder
        /// </summary>
        public Unit Startup()
        {
            lock(sync)
            {
                if (state.IsSome) return unit;

                var savedReq = ActorContext.CurrentRequest;
                var savedFlags = ActorContext.ProcessFlags;
                var savedMsg = ActorContext.CurrentMsg;

                try
                {
                    ActorContext.CurrentRequest = null;
                    ActorContext.ProcessFlags = flags;
                    ActorContext.CurrentMsg = null;

                    var stateValue = GetState();
                    try
                    {
                        if (notnull(stateValue))
                        {
                            stateSubject.OnNext(stateValue);
                        }
                    }
                    catch (Exception e)
                    {
                        logErr(e);
                    }

                }
                catch (Exception e)
                {
                    var directive = RunStrategy(
                        Id,
                        Parent.Actor.Id,
                        Sender,
                        Parent.Actor.Children.Keys.Map(n => new ProcessId(n)).Filter(x => x != Id),
                        e,
                        null,
                        Parent.Actor.Strategy
                    );
                    tell(ActorContext.Errors, e);
                }
                finally
                {
                    ActorContext.CurrentRequest = savedReq;
                    ActorContext.ProcessFlags = savedFlags;
                    ActorContext.CurrentMsg = savedMsg;
                }
                return unit;
            }
        }

        /// <summary>
        /// Failure strategy
        /// </summary>
        public State<StrategyContext, Unit> Strategy { get; }

        public Unit AddSubscription(ProcessId pid, IDisposable sub)
        {
            RemoveSubscription(pid);
            subs = subs.Add(pid.Path, sub);
            return unit;
        }

        public Unit RemoveSubscription(ProcessId pid)
        {
            subs.Find(pid.Path).IfSome(x => x.Dispose());
            subs = subs.Remove(pid.Path);
            return unit;
        }

        Unit RemoveAllSubscriptions()
        {
            subs.Iter(x => x.Dispose());
            subs = Map.empty<string, IDisposable>();
            return unit;
        }

        public ProcessFlags Flags => 
            flags;

        string StateKey => 
            Id.Path + "-state";

        void SetupRemoteSubscriptions(Option<ICluster> cluster, ProcessFlags flags)
        {
            if (remoteSubsAcquired) return;

            cluster.IfSome(c =>
            {
                // Watches for local state-changes and persists them
                if ((flags & ProcessFlags.PersistState) == ProcessFlags.PersistState)
                {
                    try
                    {
                        stateSubject.Subscribe(state => c.SetValue(StateKey, state));
                    }
                    catch (Exception e)
                    {
                        logSysErr(e);
                    }
                }

                // Watches for local state-changes and publishes them remotely
                if ((flags & ProcessFlags.RemoteStatePublish) == ProcessFlags.RemoteStatePublish)
                {
                    try
                    {
                        stateSubject.Subscribe(state => c.PublishToChannel(ActorInboxCommon.ClusterStatePubSubKey(Id), state));
                    }
                    catch (Exception e)
                    {
                        logSysErr(e);
                    }
                }

                // Watches for publish events and remotely publishes them
                if ((flags & ProcessFlags.RemotePublish) == ProcessFlags.RemotePublish)
                {
                    try
                    {
                        publishSubject.Subscribe(msg => c.PublishToChannel(ActorInboxCommon.ClusterPubSubKey(Id), msg));
                    }
                    catch (Exception e)
                    {
                        logSysErr(e);
                    }
                }
            });

            remoteSubsAcquired = true;
        }

        S GetState()
        {
            var res = state.IfNone(InitState);
            state = res;
            return res;
        }

        S InitState()
        {
            S state;

            try
            {
                SetupRemoteSubscriptions(cluster, flags);

                if (cluster.IsSome && ((flags & ProcessFlags.PersistState) == ProcessFlags.PersistState))
                {
                    try
                    {
                        logInfo($"Restoring state: {StateKey}");

                        state = cluster.LiftUnsafe().Exists(StateKey)
                            ? cluster.LiftUnsafe().GetValue<S>(StateKey)
                            : setupFn(this);
                    }
                    catch (Exception e)
                    {
                        logSysErr(e);
                        state = setupFn(this);
                    }
                }
                else
                {
                    state = setupFn(this);
                }
            }
            catch (Exception e)
            {
                throw new ProcessSetupException(Id.Path, e);
            }

            stateSubject.OnNext(state);
            return state;
        }

        /// <summary>
        /// Publish observable stream
        /// </summary>
        public IObservable<object> PublishStream => publishSubject;

        /// <summary>
        /// State observable stream
        /// </summary>
        public IObservable<object> StateStream   => stateSubject;

        /// <summary>
        /// Publish to the PublishStream
        /// </summary>
        public Unit Publish(object message)
        {
            publishSubject.OnNext(message);
            return unit;
        }

        /// <summary>
        /// Process path
        /// </summary>
        public ProcessId Id { get; }

        /// <summary>
        /// Process name
        /// </summary>
        public ProcessName Name { get; }

        /// <summary>
        /// Parent process
        /// </summary>
        public ActorItem Parent { get; }

        /// <summary>
        /// Child processes
        /// </summary>
        public Map<string, ActorItem> Children =>
            children;

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        public Unit Restart()
        {
            lock (sync)
            {
                RemoveAllSubscriptions();
                DisposeState();
                foreach (var kid in Children)
                {
                    kill(kid.Value.Actor.Id);
                }
            }
            tellSystem(Id, SystemMessage.StartupProcess);
            return unit;
        }

        /// <summary>
        /// Disowns a child process
        /// </summary>
        public Unit UnlinkChild(ProcessId pid)
        {
            lock(sync)
            {
                children = children.Remove(pid.GetName().Value);
            }
            return unit;
        }

        /// <summary>
        /// Gains a child process
        /// </summary>
        public Unit LinkChild(ActorItem item)
        {
            lock (sync)
            {
                children = children.AddOrUpdate(item.Actor.Id.GetName().Value, item);
            }
            return unit;
        }

        /// <summary>
        /// Shutdown everything from this node down
        /// </summary>
        public Unit Shutdown(bool maintainState)
        {
            lock(sync)
            {
                if (maintainState == false)
                {
                    cluster.IfSome(c =>
                    {
                        c.Delete(StateKey);
                        c.Delete(ActorInboxCommon.ClusterUserInboxKey(Id));
                        c.Delete(ActorInboxCommon.ClusterSystemInboxKey(Id));
                    });
                }

                RemoveAllSubscriptions();
                publishSubject.OnCompleted();
                stateSubject.OnCompleted();
                remoteSubsAcquired = false;
                strategyState = StrategyState.Empty;
                DisposeState();
                return unit;
            }
        }

        public Option<T> PreProcessMessageContent(object message)
        {
            if (message == null)
            {
                tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, $"Message is null for tell (expected {typeof(T)})", message));
                return None;
            }

            if (typeof(T) != typeof(string) && message is string)
            {
                try
                {
                    // This allows for messages to arrive from JS and be dealt with at the endpoint 
                    // (where the type is known) rather than the gateway (where it isn't)
                    return Some(JsonConvert.DeserializeObject<T>((string)message));
                }
                catch
                {
                    tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, $"Invalid message type for tell (expected {typeof(T)})", message));
                    return None;
                }
            }

            if (!typeof(T).IsAssignableFrom(message.GetType()))
            {
                tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, $"Invalid message type for tell (expected {typeof(T)})", message));
                return None;
            }

            return Some((T)message);
        }

        public R ProcessRequest<R>(ProcessId pid, object message)
        {
            try
            {
                if (request != null)
                {
                    throw new Exception("async ask not allowed");
                }

                response = null;
                request = new AutoResetEvent(false);
                ActorContext.Ask(pid, new ActorRequest(message, pid, Self, 0), Self);
                request.WaitOne(ActorConfig.Default.Timeout);

                if (response == null)
                {
                    throw new TimeoutException("Request timed out");
                }
                else
                {
                    if (response.IsFaulted)
                    {
                        var ex = (Exception)response.Message;
                        throw new ProcessException($"Process issue: {ex.Message}", pid.Path, Self.Path, ex);
                    }
                    else
                    {
                        return (R)response.Message;
                    }
                }
            }
            finally
            {
                if (request != null)
                {
                    request.Dispose();
                    request = null;
                }
            }
        }

        public Unit ProcessResponse(ActorResponse response)
        {
            if (request == null)
            {
                ProcessMessage(response);
            }
            else
            {
                this.response = response;
                request.Set();
            }
            return unit;
        }

        public InboxDirective ProcessAsk(ActorRequest request)
        {
            lock(sync)
            {
                var savedMsg = ActorContext.CurrentMsg;
                var savedFlags = ActorContext.ProcessFlags;
                var savedReq = ActorContext.CurrentRequest;

                try
                {
                    ActorContext.CurrentRequest = request;
                    ActorContext.ProcessFlags = flags;
                    ActorContext.CurrentMsg = request.Message;

                    //ActorContext.AssertSession();

                    if (typeof(T) != typeof(string) && request.Message is string)
                    {
                        state = PreProcessMessageContent(request.Message).Match(
                                    Some: tmsg =>
                                    {
                                        var stateIn = GetState();
                                        var stateOut = actorFn(stateIn, tmsg);
                                        try
                                        {
                                            if (notnull(stateOut) && !stateOut.Equals(stateIn))
                                            {
                                                stateSubject.OnNext(stateOut);
                                            }
                                        }
                                        catch (Exception ue)
                                        {
                                            logErr(ue);
                                        }
                                        return stateOut;
                                    },
                                    None: () => state
                                );
                    }
                    else if (request.Message is T)
                    {
                        var msg = (T)request.Message;
                        var stateIn = GetState();
                        var stateOut = actorFn(stateIn, msg);
                        try
                        {
                            if (notnull(stateOut) && !stateOut.Equals(stateIn))
                            {
                                stateSubject.OnNext(stateOut);
                            }
                        }
                        catch (Exception ue)
                        {
                            logErr(ue);
                        }
                        state = stateOut;
                    }
                    else if (request.Message is Message)
                    {
                        ProcessSystemMessage((Message)request.Message);
                    }
                    else
                    {
                        logErr($"ProcessAsk request.Message is not T {request.Message}");
                    }

                    strategyState = strategyState.With(
                        Failures: 0,
                        LastFailure: DateTime.MaxValue,
                        BackoffAmount: 0 * seconds
                        );
                }
                catch (Exception e)
                {
                    var directive = RunStrategy(
                        Id,
                        Parent.Actor.Id,
                        Sender,
                        Parent.Actor.Children.Keys.Map(n => new ProcessId(n)).Filter(x => x != Id),
                        e,
                        request,
                        Parent.Actor.Strategy
                    );

                    replyError(e);
                    tell(ActorContext.Errors, e);
                    return directive;
                }
                finally
                {
                    ActorContext.CurrentMsg = savedMsg;
                    ActorContext.ProcessFlags = savedFlags;
                    ActorContext.CurrentRequest = savedReq;
                }
                return InboxDirective.Default;
            }
        }

        void ProcessSystemMessage(Message message)
        {
            switch (message.Tag)
            {
                case Message.TagSpec.GetChildren:
                    replyIfAsked(Children);
                    break;
                case Message.TagSpec.ShutdownProcess:
                    replyIfAsked(ShutdownProcess(false));
                    break;
            }
        }

        /// <summary>
        /// Process an inbox message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public InboxDirective ProcessMessage(object message)
        {
            lock(sync)
            {
                var savedReq = ActorContext.CurrentRequest;
                var savedFlags = ActorContext.ProcessFlags;
                var savedMsg = ActorContext.CurrentMsg;

                try
                {
                    ActorContext.CurrentRequest = null;
                    ActorContext.ProcessFlags = flags;
                    ActorContext.CurrentMsg = message;

                    //ActorContext.AssertSession();

                    if (typeof(T) != typeof(string) && message is string)
                    {
                        state = PreProcessMessageContent(message).Match(
                                    Some: tmsg =>
                                    {
                                        var stateIn = GetState();
                                        var stateOut = actorFn(stateIn, tmsg);
                                        try
                                        {
                                            if (notnull(stateOut) && !stateOut.Equals(stateIn))
                                            {
                                                stateSubject.OnNext(stateOut);
                                            }
                                        }
                                        catch (Exception ue)
                                        {
                                            logErr(ue);
                                        }
                                        return stateOut;
                                    },
                                    None: () => state
                                );
                    }
                    else if (message is T)
                    {
                        var stateIn = GetState();
                        var stateOut = actorFn(GetState(), (T)message);
                        state = stateOut;
                        try
                        {
                            if (notnull(stateOut) && !state.Equals(stateIn))
                            {
                                stateSubject.OnNext(stateOut);
                            }
                        }
                        catch (Exception ue)
                        {
                            logErr(ue);
                        }
                    }
                    else if (message is Message)
                    {
                        ProcessSystemMessage((Message)message);
                    }
                    else
                    {
                        logErr($"ProcessMessage request.Message is not T {message}");
                    }

                    strategyState = strategyState.With(
                        Failures: 0,
                        LastFailure: DateTime.MaxValue,
                        BackoffAmount: 0*seconds
                        );
                }
                catch (Exception e)
                {
                    var directive = RunStrategy(
                        Id,
                        Parent.Actor.Id,
                        Sender,
                        Parent.Actor.Children.Keys.Map(n => new ProcessId(n)).Filter(x => x != Id),
                        e,
                        message,
                        Parent.Actor.Strategy
                    );
                    tell(ActorContext.Errors, e);
                    return directive;
                }
                finally
                {
                    ActorContext.CurrentRequest = savedReq;
                    ActorContext.ProcessFlags = savedFlags;
                    ActorContext.CurrentMsg = savedMsg;
                }
                return InboxDirective.Default;
            }
        }

        public InboxDirective RunStrategy(
            ProcessId pid,
            ProcessId parent,
            ProcessId sender,
            IEnumerable<ProcessId> siblings,
            Exception ex, 
            object message,
            State<StrategyContext, Unit> strategy
            )
        {
            // Build a strategy specifically for this event
            var failureStrategy = strategy.Failure(
                    pid,
                    parent,
                    sender,
                    siblings,
                    ex,
                    message
                );

            // Invoke the strategy with the running state
            var result = failureStrategy(strategyState);
            var decision = result.Value;

            // Save the strategy state back to the actor
            strategyState = result.State;

            if (decision.ProcessDirective.Type != DirectiveType.Stop && decision.Pause > 0*seconds)
            {
                decision.Affects.Iter(p => pause(p));
                delay(
                    () => RunProcessDirective(pid, sender, ex, message, decision), 
                    decision.Pause
                );
            }
            else
            {
                // Run the instruction for the Process (stop/restart/etc.)
                RunProcessDirective(pid, sender, ex, message, decision);
            }

            // Run the instruction for the message (dead-letters/send-to-self/...)
            return RunMessageDirective(pid, sender, decision, ex, message);
        }

        InboxDirective RunMessageDirective(
            ProcessId pid,
            ProcessId sender,
            StrategyDecision decision, 
            Exception e, 
            object message
            )
        {
            var directive = decision.MessageDirective;
            switch (directive.Type)
            {
                case MessageDirectiveType.ForwardToParent:
                    tell(pid.Parent(), message, sender);
                    return InboxDirective.Default;

                case MessageDirectiveType.ForwardToSelf:
                    tell(pid, message, sender);
                    return InboxDirective.Default;

                case MessageDirectiveType.ForwardToProcess:
                    tell((directive as ForwardToProcess).ProcessId, message, sender);
                    return InboxDirective.Default;

                case MessageDirectiveType.StayInQueue:
                    return InboxDirective.PushToFrontOfQueue;

                default:
                    tell(ActorContext.DeadLetters, DeadLetter.create(sender, pid, e, "Process error: ", message));
                    return InboxDirective.Default;
            }
        }

        void RunProcessDirective(
            ProcessId pid, 
            ProcessId sender, 
            Exception e, 
            object message, 
            StrategyDecision decision
            )
        {
            var directive = decision.ProcessDirective;

            // Find out the processes that this strategy affects and apply
            foreach (var cpid in decision.Affects.Filter(x => x != pid))
            {
                switch (directive.Type)
                {
                    case DirectiveType.Escalate:
                    case DirectiveType.Resume:
                        unpause(cpid);
                        break;
                    case DirectiveType.Restart:
                        restart(cpid);
                        break;
                    case DirectiveType.Stop:
                        kill(cpid);
                        break;
                }
            }

            unpause(pid);

            switch (directive.Type)
            {
                case DirectiveType.Escalate:
                    tellSystem(Parent.Actor.Id, SystemMessage.ChildFaulted(pid, sender, e, message), Self);
                    break;
                case DirectiveType.Resume:
                    // Do nothing
                    break;
                case DirectiveType.Restart:
                    Restart();
                    break;
                case DirectiveType.Stop:
                    ShutdownProcess(false);
                    break;
            }
        }

        public Unit ShutdownProcess(bool maintainState)
        {
            Parent.Actor.UnlinkChild(Id);
            ShutdownProcessRec(ActorContext.SelfProcess, ActorContext.GetInboxShutdownItem().Map(x => (ILocalActorInbox)x.Inbox), maintainState);

            children = Map.empty<string, ActorItem>();
            return unit;
        }

        void ShutdownProcessRec(ActorItem item, Option<ILocalActorInbox> inboxShutdown, bool maintainState)
        {
            var process = item.Actor;
            var inbox = item.Inbox;

            foreach (var child in process.Children.Values)
            {
                ShutdownProcessRec(child, inboxShutdown, maintainState);
            }

            inboxShutdown.Match(
                Some: ibs => ibs.Tell(inbox, ProcessId.NoSender),
                None: ()  => inbox.Dispose()
            );

            process.Shutdown(maintainState);
        }

        public void Dispose()
        {
            RemoveAllSubscriptions();
            DisposeState();
        }

        void DisposeState()
        {
            state.IfSome(s => (s as IDisposable)?.Dispose());
            state = None;
        }

        public InboxDirective ChildFaulted(ProcessId pid, ProcessId sender, Exception ex, object message)
        {
            return RunStrategy(
                pid,
                Parent.Actor.Id,
                sender,
                Parent.Actor.Children.Keys.Map(n => new ProcessId(n)).Filter(x => x != pid),
                ex,
                message,
                Parent.Actor.Strategy
            );
        }
    }
}
