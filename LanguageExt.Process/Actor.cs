using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt.Prelude;

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

        public Actor(ProcessId parent, ProcessName name, Func<S, T, S> actor, Func<S> setup )
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            actorFn = actor;
            setupFn = setup;
            Parent = parent;
            Name = name;
            Id = new ProcessId(parent.Value + "/" + name);

            with( FSHelper.StartUserMailbox(this, Parent, actor, setup), (q, mb) =>
            {
                userMailboxQuit = q;
                userMailbox = mb;
            });
            with( FSHelper.StartSystemMailbox(this, Parent, ProcessSystemMailbox, SystemMailboxInit), (q, mb) =>
            {
                systemMailboxQuit = q;
                systemMailbox = mb;
            });


            ActorContext.AddToStore(Id, this);
        }

        private Unit SystemMailboxInit()
        {
            return unit;
        }

        private Unit SendMessageToChildren(object msg)
        {
            foreach (var child in children)
            {
                Process.tell(child.Value.Id, msg);
            }
            return unit;
        }

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

            //Process.tell(childExMsg.ChildId, new Suspend());
            if (exceptionIs<SystemKillActorException>(msg.Exception))
            {
                Console.WriteLine(msg.ChildId + " user killed!");
                Process.tell(msg.ChildId, new SystemShutdownMessage());
            }
            else
            {
                Console.WriteLine(msg.ChildId + " faulted!");
                Process.tell(msg.ChildId, new SystemRestartMessage());
            }
            return unit;
        }

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        public Unit Restart()
        {
            lock(actorLock)
            {
                var msgs = new Queue<UserControlMessage>();
                while (userMailbox.CurrentQueueLength > 0)
                {
                    //if (userMailbox.CurrentQueueLength % 1000 == 0) Console.WriteLine("Q trans " + userMailbox.CurrentQueueLength);
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

                var oldMailbox = userMailbox;

                // We shutdown the children, because we're about to restart which will
                // recreate them.
                Shutdown();

                with( FSHelper.StartUserMailbox(this, Parent, actorFn, setupFn), (q, mb) =>
                {
                    userMailboxQuit = q;
                    userMailbox = mb;
                });

                while (msgs.Count > 0)
                {
                    userMailbox.Post(msgs.Dequeue());
                }

                with( FSHelper.StartSystemMailbox(this, Parent, ProcessSystemMailbox, SystemMailboxInit), (q, mb) =>
                {
                    systemMailboxQuit = q;
                    systemMailbox = mb;
                });
                return unit;
            }
        }

        private Unit ProcessSystemMailbox(Unit state, SystemMessage msg)
        {
            return unit;
        }

        // TODO: This doesn't do anything
        public void Suspend()
        {
            SendMessageToChildren(new SystemSuspendMessage());
        }

        public void Shutdown()
        {
            lock (actorLock)
            {
                foreach (var child in children)
                {
                    child.Value.Shutdown();
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
            }
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
        /// Get a child process by name
        /// </summary>
        public Option<IProcess> GetChildProcess(ProcessName name) =>
            children.TryGetValue(name.Value);

        /// <summary>
        /// Add a child process
        /// </summary>
        public IProcess AddChildProcess(Some<IProcess> process)
        {
            return children.AddOrUpdate(process.Value.Name.Value, process.Value,
                (n,p)=>
                {
                    p.Shutdown();
                    return process.Value;
                } );
        }

        public Unit Tell(object message)
        {
            if (message == null)
            {
                // TODO: Throw
                return unit;
            }

            if (!message.GetType().IsAssignableFrom(typeof(T)))
            {
                Process.tell(ActorContext.DeadLetters, message);
                return unit;
            }

            lock (actorLock)
            {
                userMailbox.Post(new UserMessage(
                    message, 
                    ActorContext.Self == null 
                        ? ActorContext.NoSender 
                        : ActorContext.Self.Id,
                    ActorContext.Self == null
                        ? ActorContext.NoSender
                        : ActorContext.Self.Id
                    ));
            }
            return unit;
        }

        public Unit TellUserControl(UserControlMessage message)
        {
            if (message == null)
            {
                return unit;
            }

            userMailbox.Post(message);
            return unit;
        }

        public Unit TellSystem(SystemMessage message)
        {
            if (message == null)
            {
                return unit;
            }

            systemMailbox.Post(message);
            return unit;
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
