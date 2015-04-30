using System;
using System.Collections.Generic;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Process' to your code.
    /// </summary>
    public static class Process
    {
        /// <summary>
        /// Registry of named processes for discovery
        /// </summary>
        public static IEnumerable<ProcessId> Registered =>
            ActorContext.Registered.Children;

        /// <summary>
        /// Current process ID
        /// </summary>
        public static ProcessId Self =>
            ActorContext.Self.Id;

        /// <summary>
        /// Parent process ID
        /// </summary>
        public static ProcessId Parent =>
            ActorContext.Self.Parent;

        /// <summary>
        /// User process ID
        /// The User process is the default entry process
        /// </summary>
        public static ProcessId User =>
            ActorContext.User.Id;

        /// <summary>
        /// Sender process ID
        /// Always valid even if there's not a sender (the 'NoSender' process ID will
        /// be provided).
        /// </summary>
        public static ProcessId Sender =>
            ActorContext.Sender;

        /// <summary>
        /// Get the child processes of the running process
        /// </summary>
        public static IEnumerable<ProcessId> Children =>
            children(ActorContext.Self.Id);

        /// <summary>
        /// Create a new child-process by name
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that can be passed around</returns>
        public static ProcessId spawn<T>(ProcessName name, Action<T> messageHandler) =>
            spawn<Unit, T>(name, () => unit, (state, msg) => { messageHandler(msg); return state; });

        /// <summary>
        /// Create a new child-process by name
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that can be passed around</returns>
        public static ProcessId spawn<S, T>(ProcessName name, Func<S> setup, Func<S, T, S> messageHandler) =>
            map((IProcessInternal)ActorContext.Self,
                self => match(self.GetChildProcess(name),
                            Some: _ => raise<IProcess>(new NamedProcessAlreadyExistsException()),
                            None: () => self.AddChildProcess(new Actor<S, T>(ActorContext.Self.Id, name, messageHandler, setup))).Id
            );

        /// <summary>
        /// Register self as a named process
        /// </summary>
        /// <param name="name">Name to register under</param>
        public static ProcessId reg(ProcessName name) =>
            ActorContext.Register(name, Self);

        /// <summary>
        /// Register the name with the process
        /// </summary>
        /// <param name="name">Name to register under</param>
        /// <param name="process">Process to be registered</param>
        public static ProcessId reg(ProcessName name, ProcessId process) =>
            ActorContext.Register(name, process);

        /// <summary>
        /// Un-register the process associated with the name
        /// </summary>
        /// <param name="name">Name of the process to un-register</param>
        public static Unit unreg(ProcessName name) =>
            ActorContext.UnRegister(name);

        /// <summary>
        /// Forces the current running process to shutdown.  The kill message 
        /// jumps ahead of any messages already in the queue.
        /// </summary>
        public static Unit kill() =>
            raise<Unit>(new SystemKillActorException());

        /// <summary>
        /// Forces the specified process to shutdown.  The kill message jumps 
        /// ahead of any messages already in the queue.
        /// </summary>
        public static Unit kill(ProcessId pid) =>
            tell(pid, SystemMessage.Shutdown);

        /// <summary>
        /// Shutdown the currently running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messages allowing any backlog
        /// to be processed first.
        /// </summary>
        public static Unit shutdown() =>
            shutdown(Self);

        /// <summary>
        /// Shutdown a specified running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messagesallowing any backlog
        /// to be processed first.
        /// </summary>
        public static Unit shutdown(ProcessId pid) =>
            tell(pid, UserControlMessage.Shutdown);

        /// <summary>
        /// Send a message to a process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="message">Message to send</param>
        public static Unit tell<T>(ProcessId pid, T message, ProcessId sender = default(ProcessId) ) =>
            map((IProcessInternal)ActorContext.GetProcess(pid),
                pi => 
                    message is SystemMessage
                        ? pi.TellSystem(message as SystemMessage)
                        : message is UserControlMessage
                            ? pi.TellUserControl(message as UserControlMessage)
                            : pi.Tell(message, sender) );

        /// <summary>
        /// Publish a message for any listening subscribers
        /// </summary>
        /// <param name="message">Message to publish</param>
        public static Unit pub<T>(T message) =>
            ObservableRouter.Publish(ActorContext.Self.Id, message);

        /// <summary>
        /// Get the child processes of the process provided
        /// </summary>
        public static IEnumerable<ProcessId> children(ProcessId pid) =>
            ActorContext.GetProcess(pid).Children;

        /// <summary>
        /// Shutdown all processes and restart
        /// </summary>
        public static Unit shutdownAll() =>
            ActorContext.Restart();

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, IObserver<T> observer) =>
            ObservableRouter.Subscribe(pid, observer);

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError, Action onComplete) =>
            ObservableRouter.Subscribe(pid, onNext, onError, onComplete);

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext, Action<Exception> onError) =>
            ObservableRouter.Subscribe(pid, onNext, onError, () => { });

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext) =>
            ObservableRouter.Subscribe(pid, onNext, ex => { }, () => { });

        /// <summary>
        /// Subscribe to the process's observable stream.
        /// NOTE: The process can publish any number of types, any published messages
        ///       not of type T will be ignored.
        /// </summary>
        public static IDisposable subs<T>(ProcessId pid, Action<T> onNext, Action onComplete) =>
            ObservableRouter.Subscribe(pid, onNext, ex => { }, onComplete);
    }
}
