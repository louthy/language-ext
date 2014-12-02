using System;
using System.Collections.Generic;

using LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Process' to your code.
    /// </summary>
    public static partial class Process
    {
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
            with((IProcessInternal)ActorContext.Self,
                self => match(self.GetChildProcess(name),
                            Some: _ => raise<IProcess>(new NamedProcessAlreadyExistsException()),
                            None: () => self.AddChildProcess(new Actor<S, T>(ActorContext.Self.Id, name, messageHandler, setup))).Id
            );

        /// <summary>
        /// Register the name with the process
        /// </summary>
        /// <param name="name">Name to register under</param>
        /// <param name="process">Process to be registered</param>
        public static ProcessId register(ProcessName name, ProcessId process) =>
            ActorContext.Register(name, process);

        /// <summary>
        /// Un-register the process associated with the name
        /// </summary>
        /// <param name="name">Name of the process to un-register</param>
        public static Unit unregister(ProcessName name) =>
            ActorContext.UnRegister(name);

        /// <summary>
        /// Registry of named processes for discovery
        /// </summary>
        public static IEnumerable<ProcessId> registered() =>
            ActorContext.Registered.Children;

        /// <summary>
        /// Forces the current running process to shutdown.  The kill message 
        /// jumps ahead of any messages already in the queue.
        /// </summary>
        public static Unit kill() =>
            raise<Unit>(new SystemKillActorException());

        /// <summary>
        /// Forces the process to shutdown.  The kill message jumps ahead
        /// of any messages already in the queue.
        /// </summary>
        public static Unit kill(ProcessId pid) =>
            tell(pid, SystemMessage.Shutdown);

        /// <summary>
        /// Shutdown a running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messages.
        /// </summary>
        public static Unit shutdown() =>
            shutdown(self());

        /// <summary>
        /// Shutdown a running process.
        /// This differs from kill() in that the shutdown message just joins
        /// the back of the queue like all other messages.
        /// </summary>
        /// <returns></returns>
        public static Unit shutdown(ProcessId pid) =>
            tell(pid, UserControlMessage.Shutdown);

        /// <summary>
        /// Current process ID
        /// </summary>
        /// <returns></returns>
        public static ProcessId self() =>
            ActorContext.Self.Id;

        /// <summary>
        /// Parent process ID
        /// </summary>
        /// <returns></returns>
        public static ProcessId parent() =>
            ActorContext.Self.Parent;

        /// <summary>
        /// User process ID
        /// </summary>
        /// <returns></returns>
        public static ProcessId user() =>
            ActorContext.User.Id;

        /// <summary>
        /// Sender process ID
        /// Always valid even if there's not a sender (the 'NoSender' process ID will
        /// be provided).
        /// </summary>
        /// <returns></returns>
        public static ProcessId sender() =>
            ActorContext.Sender;

        /// <summary>
        /// Send a message to a process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="message">Message to send</param>
        public static Unit tell<T>(ProcessId pid, T message, ProcessId sender = default(ProcessId) ) =>
            with((IProcessInternal)ActorContext.GetProcess(pid),
                pi => 
                    message is SystemMessage
                        ? pi.TellSystem(message as SystemMessage)
                        : message is UserControlMessage
                            ? pi.TellUserControl(message as UserControlMessage)
                            : pi.Tell(message, sender) );

        /// <summary>
        /// Get the child processes of the process provided
        /// </summary>
        public static IEnumerable<ProcessId> children(ProcessId pid) =>
            ActorContext.GetProcess(pid).Children;

        /// <summary>
        /// Get the child processes of the running process
        /// </summary>
        public static IEnumerable<ProcessId> children() =>
            children(ActorContext.Self.Id);

        /// <summary>
        /// Shutdown all processes and restart
        /// </summary>
        public static Unit restart() =>
            ActorContext.Restart();
    }
}
