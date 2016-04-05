using System;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Map;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process:  Tell functions
    /// 
    ///     'Tell' is used to send a message from one process to another (or from outside a process to a process).
    ///     The messages are sent to the process asynchronously and join the process' inbox.  The process will 
    ///     deal with one message from its inbox at a time.  It cannot start the next message until it's finished
    ///     with a previous message.
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Send a message to a process
        /// </summary>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static Unit tell<T>(ProcessId pid, T message, ProcessId sender = default(ProcessId)) =>
            message is UserControlMessage
                ? ActorContext.System(pid).TellUserControl(pid, message as UserControlMessage)
                : ActorContext.System(pid).Tell(pid, message, sender);

        /// <summary>
        /// Send a message to a process
        /// </summary>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        internal static Unit tellSystem<T>(ProcessId pid, T message, ProcessId sender = default(ProcessId)) =>
            ActorContext.System(pid).TellSystem(pid, message as SystemMessage);

        /// <summary>
        /// Send a message at a specified time in the future
        /// </summary>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static IDisposable tell<T>(ProcessId pid, T message, TimeSpan delayFor, ProcessId sender = default(ProcessId)) =>
            safedelay(() => tell(pid, message, sender), delayFor);

        /// <summary>
        /// Send a message at a specified time in the future
        /// </summary>
        /// <remarks>
        /// It is advised to use the variant that takes a TimeSpan, this will fail to be accurate across a Daylight Saving 
        /// Time boundary or if you use non-UTC dates
        /// </remarks>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static IDisposable tell<T>(ProcessId pid, T message, DateTime delayUntil, ProcessId sender = default(ProcessId)) =>
            safedelay(() => tell(pid, message, sender), delayUntil);

        /// <summary>
        /// Tell children the same message
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns></returns>
        public static Unit tellChildren<T>(T message, ProcessId sender = new ProcessId()) =>
            iter(Children, child => tell(child, message, sender));

        /// <summary>
        /// Tell children the same message
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns></returns>
        internal static Unit tellSystemChildren<T>(T message, ProcessId sender = new ProcessId()) =>
            iter(Children, child => tellSystem(child, message, sender));

        /// <summary>
        /// Tell children the same message, delayed.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, TimeSpan delayFor, ProcessId sender = new ProcessId()) =>
            safedelay(() => tellChildren(message, sender), delayFor);

        /// <summary>
        /// Tell children the same message, delayed.
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, DateTime delayUntil, ProcessId sender = new ProcessId()) =>
            safedelay(() => tellChildren(message, sender), delayUntil);

        /// <summary>
        /// Tell children the same message
        /// The list of children to send to are filtered by the predicate provided
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="predicate">The list of children to send to are filtered by the predicate provided</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static Unit tellChildren<T>(T message, Func<ProcessId, bool> predicate, ProcessId sender = new ProcessId()) =>
            iter(filter(Children, predicate), child => tell(child, message, sender));

        /// <summary>
        /// Tell children the same message, delayed.
        /// The list of children to send to are filtered by the predicate provided
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="predicate">The list of children to send to are filtered by the predicate provided</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, TimeSpan delayFor, Func<ProcessId, bool> predicate, ProcessId sender = new ProcessId()) =>
            safedelay(() => tellChildren(message, predicate, sender), delayFor);

        /// <summary>
        /// Tell children the same message, delayed.
        /// The list of children to send to are filtered by the predicate provided
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="predicate">The list of children to send to are filtered by the predicate provided</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, DateTime delayUntil, Func<ProcessId, bool> predicate, ProcessId sender = new ProcessId()) =>
            safedelay(() => tellChildren(message, predicate, sender), delayUntil);

        /// <summary>
        /// Send a message to the parent process
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static Unit tellParent<T>(T message, ProcessId sender = new ProcessId()) =>
            tell(Parent, message, sender);

        /// <summary>
        /// Send a message to the parent process at a specified time in the future
        /// </summary>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static IDisposable tellParent<T>(T message, TimeSpan delayFor, ProcessId sender = new ProcessId()) =>
            tell(Parent, message, delayFor, sender);

        /// <summary>
        /// Send a message to the parent process at a specified time in the future
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static IDisposable tellParent<T>(T message, DateTime delayUntil, ProcessId sender = new ProcessId()) =>
            tell(Parent, message, delayUntil, sender);


        /// <summary>
        /// Send a message to ourself
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static Unit tellSelf<T>(T message, ProcessId sender = new ProcessId()) =>
            tell(Self, message, sender);

        /// <summary>
        /// Send a message to ourself at a specified time in the future
        /// </summary>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static IDisposable tellSelf<T>(T message, TimeSpan delayFor, ProcessId sender = new ProcessId()) =>
            tell(Self, message, delayFor, sender);

        /// <summary>
        /// Send a message to ourself at a specified time in the future
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static IDisposable tellSelf<T>(T message, DateTime delayUntil, ProcessId sender = new ProcessId()) =>
            tell(Self, message, delayUntil, sender);

        /// <summary>
        /// Send a message to a named child process
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="name">Name of the child process</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static Unit tellChild<T>(ProcessName name, T message, ProcessId sender = new ProcessId()) =>
            tell(Self.Child(name), message, sender);

        /// <summary>
        /// Send a message to a child process (found by index)
        /// </summary>
        /// <remarks>
        /// Because of the potential changeable nature of child nodes, this will
        /// take the index and mod it by the number of children.  We expect this 
        /// call will mostly be used for load balancing, and round-robin type 
        /// behaviour, so feel that's acceptable.  
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="index">Index of the child process (see remarks)</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        public static Unit tellChild<T>(int index, T message, ProcessId sender = new ProcessId()) =>
            tell(child(index), message, sender);
    }
}
