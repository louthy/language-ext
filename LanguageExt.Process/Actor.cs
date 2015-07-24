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

namespace LanguageExt
{
    internal class Actor<S, T> : IProcess, IProcess<T>
    {
        Func<S, T, S> actorFn;
        Func<IProcess, S> setupFn;
        S state;
        Map<string, ProcessId> children = Map.create<string, ProcessId>();
        Option<ICluster> cluster;
        Subject<object> publishSubject = new Subject<object>();
        Subject<object> stateSubject = new Subject<object>();
        ProcessFlags flags;

        internal Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Func<S, T, S> actor, Func<IProcess, S> setup, ProcessFlags flags)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            this.cluster = cluster;
            this.flags = flags;
            actorFn = actor;
            setupFn = setup;
            Parent = parent;
            Name = name;
            Id = parent.MakeChildId(name);

            SetupClusterStatePersist(cluster, flags);
        }

        public Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Func<S, T, S> actor, Func<S> setup, ProcessFlags flags)
            :
            this(cluster, parent, name, actor, _ => setup(), flags)
        { }

        public Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Func<T, Unit> actor, ProcessFlags flags)
            :
            this(cluster, parent, name, (s, t) => { actor(t); return default(S); }, () => default(S), flags)
        { }

        public Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Action<T> actor, ProcessFlags flags)
            :
            this(cluster, parent, name, (s, t) => { actor(t); return default(S); }, () => default(S), flags)
        { }

        /// <summary>
        /// Start up - creates the initial state
        /// </summary>
        /// <returns></returns>
        public Unit Startup()
        {
            ActorContext.WithContext(
                Id,
                Parent,
                Children,
                ProcessId.NoSender,
                () => InitState()
            );
            stateSubject.OnNext(state);
            return unit;
        }

        private string StateKey => Id.Value + "-state";

        private void SetupClusterStatePersist(Option<ICluster> cluster, ProcessFlags flags)
        {
            cluster.IfSome(c =>
            {
                if ((flags & ProcessFlags.PersistentState) == ProcessFlags.PersistentState)
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
            });
        }

        private void InitState()
        {
            if (cluster.IsSome && ((flags & ProcessFlags.PersistentState) == ProcessFlags.PersistentState))
            {
                try
                {
                    state = cluster.LiftUnsafe().Exists(StateKey)
                        ? cluster.LiftUnsafe().GetValue<S>(StateKey)
                        : setupFn(this);
                }
                catch (Exception e)
                {
                    state = setupFn(this);
                    logSysErr(e);
                }
            }
            else
            {
                state = setupFn(this);
            }
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
        /// Get state
        /// </summary>
        public object GetState()
        {
            return state;
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
        public ProcessId Parent { get; }

        /// <summary>
        /// Child processes
        /// </summary>
        public Map<string, ProcessId> Children =>
            children;

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        public Unit Restart()
        {
            InitState();
            stateSubject.OnNext(state);
            tellChildren(SystemMessage.Restart);
            return unit;
        }

        /// <summary>
        /// Disowns a child process
        /// </summary>
        public Unit UnlinkChild(ProcessId pid)
        {
            children = children.Remove(pid.Name.Value);
            return unit;
        }

        /// <summary>
        /// Gains a child process
        /// </summary>
        public Unit LinkChild(ProcessId pid)
        {
            children = children.AddOrUpdate(pid.Name.Value, pid);
            return unit;
        }

        /// <summary>
        /// Shutdown everything from this node down
        /// </summary>
        public Unit Shutdown()
        {
            publishSubject.OnCompleted();
            stateSubject.OnCompleted();
            return unit;
        }

        public Unit ProcessAsk(ActorRequest request)
        {
            try
            {
                ActorContext.ProcessFlags = flags;
                if (request.Message is T)
                {
                    ActorContext.CurrentRequestId = request.RequestId;
                    T msg = (T)request.Message;
                    state = actorFn(state, msg);
                    stateSubject.OnNext(state);
                }
            }
            catch (SystemKillActorException)
            {
                kill(Id);
            }
            catch (Exception e)
            {
                /// TODO: Add extra strategy behaviours here
                Restart();
                tell(ActorContext.Errors, e);
                tell(ActorContext.DeadLetters, request.Message);
            }
            return unit;
        }

        /// <summary>
        /// Process an inbox message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Unit ProcessMessage(T message)
        {
            try
            {
                ActorContext.CurrentRequestId = -1;
                ActorContext.ProcessFlags = flags;
                state = actorFn(state, message);
                stateSubject.OnNext(state);
            }
            catch (SystemKillActorException)
            {
                kill(Id);
            }
            catch (Exception e)
            {
                /// TODO: Add extra strategy behaviours here
                Restart();
                tell(ActorContext.Errors, e);
                tell(ActorContext.DeadLetters, message);
            }
            return unit;
        }

        public void Dispose()
        {
            if (state is IDisposable)
            {
                var s = state as IDisposable;
                if (s != null)
                {
                    s.Dispose();
                    state = default(S);
                }
            }
        }
    }
}
