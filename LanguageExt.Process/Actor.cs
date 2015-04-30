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
    internal class Actor<S, T> : IProcess, IProcessInternal
    {
        FSharpMailboxProcessor<UserControlMessage> userMailbox;
        FSharpMailboxProcessor<SystemMessage> systemMailbox;
        Action userMailboxQuit;
        Action systemMailboxQuit;
        Func<S, T, S> actorFn;
        Func<S> setupFn;
        ConcurrentDictionary<string, IProcess> children = new ConcurrentDictionary<string, IProcess>();
        object actorLock = new object();

        public Actor(ProcessId parent, ProcessName name, Func<S, T, S> actor, Func<S> setup)
        {
            if (setup == null) throw new ArgumentNullException(nameof(setup));
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            actorFn = actor;
            setupFn = setup;
            Parent = parent;
            Name = name;
            Id = new ProcessId(parent.Value + ProcessId.Sep + name);

            StartMailboxes();

            ActorContext.AddToStore(Id, this);
        }

        public Actor(ProcessId parent, ProcessName name, Func<T, Unit> actor)
            :
            this(parent,name,(s,t) => { actor(t); return default(S); }, () => default(S) )
            {}

        public Actor(ProcessId parent, ProcessName name, Action<T> actor)
            :
            this(parent, name, (s, t) => { actor(t); return default(S); }, () => default(S))
        { }

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
        public IEnumerable<ProcessId> Children =>
            from c in children
            select c.Value.Id;

        /// <summary>
        /// Creates a user and system mailbox
        /// </summary>
        private void StartMailboxes()
        {
            map(FSHelper.StartUserMailbox(this, Parent, actorFn, setupFn), (q, mb) =>
            {
                userMailboxQuit = q;
                userMailbox = mb;
            });

            map(FSHelper.StartSystemMailbox(this, Parent), (q, mb) =>
            {
                systemMailboxQuit = q;
                systemMailbox = mb;
            });
        }

        /// <summary>
        /// Send the same message to all children
        /// </summary>
        private Unit SendMessageToChildren(object msg) =>
            iter(children, child => Process.tell(child.Value.Id, msg));

        /// <summary>
        /// An exception has happened in a child process.
        /// Restart it (and all its children) with fresh state unless it's a 
        /// kill message.
        /// 
        /// TODO: Add extra strategy behaviours here
        /// TODO: Need better strategy for lost messages on children, at the
        ///       moment we just shut them down.
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Unit HandleFaultedChild(SystemChildIsFaultedMessage msg)
        {
            if (exceptionIs<SystemKillActorException>(msg.Exception))
            {
                Process.tell(msg.ChildId, SystemMessage.Shutdown);
                PurgeChild(msg.ChildId);
            }
            else
            {
                Process.tell(msg.ChildId, SystemMessage.Restart);
            }
            return unit;
        }

        /// <summary>
        /// Remove the child from our child list, and from the ActorContext.Store
        /// </summary>
        /// <param name="childName"></param>
        private Unit PurgeChild(ProcessId child)
        {
            IProcess temp;
            children.TryRemove(child.Name.Value, out temp);
            return ActorContext.RemoveFromStore(child);
        }

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        public Unit Restart()
        {
            lock(actorLock)
            {
                // Take a copy of the messages from the dead mailbox
                var msgs = new Queue<UserControlMessage>(userMailbox.CurrentQueueLength);
                while (userMailbox.CurrentQueueLength > 0)
                {
                    UserControlMessage userMessage = FSharpAsync.StartAsTask(
                                                        userMailbox.Receive(FSharpOption<int>.None),
                                                        FSharpOption<TaskCreationOptions>.None,
                                                        FSharpOption<CancellationToken>.None
                                                        ).Result;

                    if (userMessage != null)
                    {
                        msgs.Enqueue(userMessage);
                    }
                }

                // We shutdown the children, because we're about to restart which will
                // recreate them.
                Shutdown(false);

                // Start new mailboxes
                StartMailboxes();

                // Copy the old messages 
                while (msgs.Count > 0)
                {
                    userMailbox.Post(msgs.Dequeue());
                }

                return unit;
            }
        }

        /// <summary>
        /// Disowns a child processes
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Unit UnlinkChild(ProcessId pid) =>
            PurgeChild(pid);

        /// <summary>
        /// Shutdown everything from this node down
        /// </summary>
        /// <param name="unlinkFromParent">Flag if we should tell the parent we're leaving</param>
        public Unit Shutdown(bool unlinkFromParent = true)
        {
            lock (actorLock)
            {
                foreach (var child in children)
                {
                    child.Value.Shutdown();
                    PurgeChild(child.Value.Id);
                }
                children.Clear();

                if (userMailbox != null)
                {
                    userMailboxQuit();
                    userMailbox.Post(null);
                }
                if (systemMailbox != null)
                {
                    systemMailboxQuit();
                    systemMailbox.Post(null);
                }

                if (unlinkFromParent && Parent.Value != "")
                {
                    Process.tell(Parent, SystemMessage.UnLinkChild(Id));
                }
            }
            return unit;
        }

        /// <summary>
        /// Get a child process by name
        /// </summary>
        public Option<IProcess> GetChildProcess(ProcessName name) =>
            children.TryGetValue(name.Value);

        /// <summary>
        /// Add a child process
        /// </summary>
        public IProcess AddChildProcess(Some<IProcess> process) =>
            children.AddOrUpdate(process.Value.Name.Value, process.Value,
                (n,p)=>
                {
                    p.Shutdown();
                    return process.Value;
                } );

        public Unit Tell(object message, ProcessId sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!typeof(T).IsAssignableFrom(message.GetType()))
            {
                Process.tell(ActorContext.DeadLetters, message);
                return unit;
            }

            lock (actorLock)
            {
                sender = sender.IsValid
                    ? sender
                    : ActorContext.Self == null
                        ? ActorContext.NoSender
                        : ActorContext.Self.Id;

                userMailbox.Post(new UserMessage( message, sender, sender ));
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            userMailbox.Post(message);
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            systemMailbox.Post(message);
            return unit;
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
