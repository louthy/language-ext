using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using static LanguageExt.List;

namespace LanguageExt
{
    internal class Actor<S, T> : IProcess, IProcessInternal<T>
    {
        Func<S, T, S> actorFn;
        Func<S> setupFn;
        S state;
        Map<string, ProcessId> children = Map.create<string, ProcessId>();
        Option<ICluster> cluster;
        object sync = new object();

        public Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Func<S, T, S> actor, Func<S> setup)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            this.cluster = cluster;
            actorFn = actor;
            setupFn = setup;
            Parent = parent;
            Name = name;
            Id = parent.MakeChildId(name);
        }

        public Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Func<T, Unit> actor)
            :
            this(cluster, parent, name,(s,t) => { actor(t); return default(S); }, () => default(S) )
            {}

        public Actor(Option<ICluster> cluster, ProcessId parent, ProcessName name, Action<T> actor)
            :
            this(cluster, parent, name, (s, t) => { actor(t); return default(S); }, () => default(S))
            {}

        /// <summary>
        /// Start up - creates the initial state
        /// </summary>
        /// <returns></returns>
        public Unit Startup()
        {
            ActorContext.TellSystem(Parent, SystemMessage.LinkChild(Id));

            ActorContext.WithContext(
                Id,
                ProcessId.NoSender,
                () => state = setupFn()
            );
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
        public ProcessId Parent { get; }

        /// <summary>
        /// Child processes
        /// </summary>
        public Map<string, ProcessId> Children =>
            children;

        /// <summary>
        /// Send the same message to all children
        /// </summary>
        private Unit TellChildren(object msg) =>
            iter(children.Values, child => Process.tell(child.Value, msg));

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        public Unit Restart()
        {
            state = setupFn();
            TellChildren(SystemMessage.Restart);
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
            TellChildren(SystemMessage.Shutdown);

            if (Parent.Value != "")
            {
                Process.tell(Parent, SystemMessage.UnLinkChild(Id));
            }
            Process.tell(ActorContext.Root, RootMessage.RemoveFromStore(Id));
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
                state = actorFn(state, message);
            }
            catch (SystemKillActorException)
            {
                Shutdown();
            }
            catch (Exception e)
            {
                /// TODO: Add extra strategy behaviours here
                Process.tell(ActorContext.Errors, e);
                Process.tell(ActorContext.DeadLetters, message);
                Restart();
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
