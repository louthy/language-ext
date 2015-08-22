using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt.Trans;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using static LanguageExt.List;
using System.Reactive.Subjects;
using Newtonsoft.Json;

namespace LanguageExt
{
    /// <summary>
    /// Internal class that represents the state of a single process.
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="T">Message type</typeparam>
    internal class Actor<S, T> : IActor
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
        EventWaitHandle request;
        volatile ActorResponse response;
        int roundRobinIndex = -1;

        internal Actor(Option<ICluster> cluster, ActorItem parent, ProcessName name, Func<S, T, S> actor, Func<IActor, S> setup, ProcessFlags flags)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            this.cluster = cluster;
            this.flags = flags;
            actorFn = actor;
            setupFn = setup;
            Parent = parent;
            Name = name;
            Id = parent.Actor.Id[name];

            SetupClusterStatePersist(cluster, flags);
        }

        public Actor(Option<ICluster> cluster, ActorItem parent, ProcessName name, Func<S, T, S> actor, Func<S> setup, ProcessFlags flags)
            :
            this(cluster, parent, name, actor, _ => setup(), flags)
        { }

        public Actor(Option<ICluster> cluster, ActorItem parent, ProcessName name, Func<T, Unit> actor, ProcessFlags flags)
            :
            this(cluster, parent, name, (s, t) => { actor(t); return default(S); }, () => default(S), flags)
        { }

        public Actor(Option<ICluster> cluster, ActorItem parent, ProcessName name, Action<T> actor, ProcessFlags flags)
            :
            this(cluster, parent, name, (s, t) => { actor(t); return default(S); }, () => default(S), flags)
        { }

        /// <summary>
        /// Start up - placeholder
        /// </summary>
        public Unit Startup()
        {
            return unit;
        }

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

        private Unit RemoveAllSubscriptions()
        {
            subs.Iter(x => x.Dispose());
            subs = Map.empty<string, IDisposable>();
            return unit;
        }

        public int GetNextRoundRobinIndex() =>
            Children.Count == 0
                ? 0 
                : roundRobinIndex = (roundRobinIndex + 1) % Children.Count;

        public ProcessFlags Flags => 
            flags;

        private string StateKey => 
            Id.Path + "-state";

        private void SetupClusterStatePersist(Option<ICluster> cluster, ProcessFlags flags)
        {
            cluster.IfSome(c =>
            {
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

                if ((flags & ProcessFlags.RemoteStatePublish) == ProcessFlags.RemoteStatePublish)
                {
                    try
                    {
                        stateSubject.Subscribe(state => c.PublishToChannel(ActorInboxCommon.ClusterPubSubKey(Id), state));
                    }
                    catch (Exception e)
                    {
                        logSysErr(e);
                    }
                }
            });
        }

        private S GetState()
        {
            var res = state.IfNone(InitState);
            state = res;
            return res;
        }

        private S InitState()
        {
            S state;

            if (cluster.IsSome && ((flags & ProcessFlags.PersistState) == ProcessFlags.PersistState))
            {
                try
                {
                    logInfo("Restoring state: " + StateKey);

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

            stateSubject.OnNext(state);
            return state;
        }

        public IObservable<object> PublishStream => publishSubject;
        public IObservable<object> StateStream => stateSubject;

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
            RemoveAllSubscriptions();
            DisposeState();
            tellChildren(SystemMessage.Restart);
            return unit;
        }

        /// <summary>
        /// Disowns a child process
        /// </summary>
        public Unit UnlinkChild(ActorItem item)
        {
            children = children.Remove(item.Actor.Id.GetName().Value);
            return unit;
        }

        /// <summary>
        /// Gains a child process
        /// </summary>
        public Unit LinkChild(ActorItem item)
        {
            children = children.AddOrUpdate(item.Actor.Id.GetName().Value, item);
            return unit;
        }

        /// <summary>
        /// Shutdown everything from this node down
        /// </summary>
        public Unit Shutdown()
        {
            RemoveAllSubscriptions();
            publishSubject.OnCompleted();
            stateSubject.OnCompleted();
            DisposeState();
            return unit;
        }

        public Option<T> PreProcessMessageContent(object message)
        {
            if (message == null)
            {
                tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, "Message is null for tell (expected " + typeof(T) + ")", message));
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
                    tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, "Invalid message type for tell (expected " + typeof(T) + ")", message));
                    return None;
                }
            }

            if (!typeof(T).IsAssignableFrom(message.GetType()))
            {
                tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, "Invalid message type for tell (expected " + typeof(T) + ")", message));
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
                        throw new ProcessException("Process issue: " + ex.Message, pid.Path, Self.Path, ex);
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

        public Unit ProcessAsk(ActorRequest request)
        {
            var savedMsg = ActorContext.CurrentMsg;
            var savedFlags = ActorContext.ProcessFlags;
            var savedReq = ActorContext.CurrentRequest;

            try
            {
                ActorContext.CurrentRequest = request;
                ActorContext.ProcessFlags = flags;
                ActorContext.CurrentMsg = request.Message;

                if (typeof(T) != typeof(string) && request.Message is string)
                {
                    state = PreProcessMessageContent(request.Message).Match(
                                Some: tmsg =>
                                {
                                    var stateIn = GetState();
                                    var stateOut = actorFn(stateIn, tmsg);
                                    try
                                    {
                                        if (stateOut != null && !stateOut.Equals(stateIn))
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
                    T msg = (T)request.Message;
                    var stateIn = GetState();
                    var stateOut = actorFn(stateIn, msg);
                    try
                    {
                        if (stateOut != null && !stateOut.Equals(stateIn))
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
                    logErr("ProcessAsk request.Message is not T " + request.Message);
                }
            }
            catch (SystemKillActorException)
            {
                kill(Id);
            }
            catch (Exception e)
            {
                // TODO: Add extra strategy behaviours here
                Restart();
                replyError(e);
                tell(ActorContext.Errors, e);
                tell(ActorContext.DeadLetters, DeadLetter.create(request.ReplyTo, request.To, e, "Process error (ask): ", request.Message));
            }
            finally
            {
                ActorContext.CurrentMsg = savedMsg;
                ActorContext.ProcessFlags = savedFlags;
                ActorContext.CurrentRequest = savedReq;
            }
            return unit;
        }

        private void ProcessSystemMessage(Message message)
        {
            switch (message.Tag)
            {
                case Message.TagSpec.GetChildren:
                    replyIfAsked(Children);
                    break;
                case Message.TagSpec.Shutdown:
                case Message.TagSpec.ShutdownProcess:
                    replyIfAsked(ShutdownProcess());
                    break;
            }
        }

        /// <summary>
        /// Process an inbox message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Unit ProcessMessage(object message)
        {
            var savedReq = ActorContext.CurrentRequest;
            var savedFlags = ActorContext.ProcessFlags;
            var savedMsg = ActorContext.CurrentMsg;

            try
            {
                ActorContext.CurrentRequest = null;
                ActorContext.ProcessFlags = flags;
                ActorContext.CurrentMsg = message;

                if (typeof(T) != typeof(string) && message is string)
                {
                    state = PreProcessMessageContent(message).Match(
                                Some: tmsg =>
                                {
                                    var stateIn = GetState();
                                    var stateOut = actorFn(stateIn, tmsg);
                                    try
                                    {
                                        if (stateOut != null && !stateOut.Equals(stateIn))
                                        {
                                            stateSubject.OnNext(state);
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
                    var s = actorFn(GetState(), (T)message);
                    state = s;
                    try
                    {
                        stateSubject.OnNext(state);
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
                    logErr("ProcessMessage request.Message is not T " + message);
                }
            }
            catch (SystemKillActorException)
            {
                logInfo("Process message - system kill " + Id);
                kill(Id);
            }
            catch (Exception e)
            {
                // TODO: Add extra strategy behaviours here
                Restart();
                tell(ActorContext.Errors, e);
                tell(ActorContext.DeadLetters, DeadLetter.create(Sender, Self, e, "Process error (tell): ", message));
            }
            finally
            {
                ActorContext.CurrentRequest = savedReq;
                ActorContext.ProcessFlags = savedFlags;
                ActorContext.CurrentMsg = savedMsg;
            }
            return unit;
        }

        public Unit ShutdownProcess()
        {
            Parent.Actor.UnlinkChild(ActorContext.SelfProcess);
            ShutdownProcessRec(ActorContext.SelfProcess, ActorContext.GetInboxShutdownItem().Map(x => (ILocalActorInbox)x.Inbox));
            children = Map.empty<string, ActorItem>();
            return unit;
        }

        private void ShutdownProcessRec(ActorItem item, Option<ILocalActorInbox> inboxShutdown)
        {
            var process = item.Actor;
            var inbox = item.Inbox;

            foreach (var child in process.Children.Values)
            {
                ShutdownProcessRec(child, inboxShutdown);
            }

            inboxShutdown.Match(
                Some: ibs => ibs.Tell(inbox, ProcessId.NoSender),
                None: ()  => inbox.Dispose()
            );

            process.Shutdown();
        }

        public void Dispose()
        {
            RemoveAllSubscriptions();
            DisposeState();
        }

        private void DisposeState()
        {
            state.IfSome(s =>
            {
               if (s is IDisposable)
               {
                   var sd = state as IDisposable;
                   if (sd != null)
                   {
                       sd.Dispose();
                   }
               }
            });
            state = None;
        }
    }
}
