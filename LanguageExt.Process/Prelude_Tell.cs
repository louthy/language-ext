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
            message is SystemMessage
                ? ActorContext.TellSystem(pid, message as SystemMessage)
                : message is UserControlMessage
                    ? ActorContext.TellUserControl(pid, message as UserControlMessage)
                    : ActorContext.Tell(pid, message, sender);

        /// <summary>
        /// Send a message at a specified time in the future
        /// </summary>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <param name="delayFor">How long to delay sending for</param>
        public static IDisposable tell<T>(ProcessId pid, T message, TimeSpan delayFor, ProcessId sender = default(ProcessId)) =>
            delay(() => tell(pid, message, sender), delayFor).Subscribe();

        /// <summary>
        /// Send a message at a specified time in the future
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="pid">Process ID to send to</param>
        /// <param name="message">Message to send</param>
        /// <param name="sender">Optional sender override.  The sender is handled automatically if you do not provide one.</param>
        /// <param name="delayUntil">Date and time to send</param>
        public static IDisposable tell<T>(ProcessId pid, T message, DateTime delayUntil, ProcessId sender = default(ProcessId)) =>
            delay(() => tell(pid, message, sender), delayUntil).Subscribe();

        /// <summary>
        /// Tell children the same message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Unit tellChildren<T>(T message) =>
            iter(Children, child => tell(child, message));

        /// <summary>
        /// Tell children the same message, delayed.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, TimeSpan delayFor) =>
            delay(() => tellChildren(message), delayFor).Subscribe();

        /// <summary>
        /// Tell children the same message, delayed.
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, DateTime delayUntil) =>
            delay(() => tellChildren(message), delayUntil).Subscribe();

        /// <summary>
        /// Tell children the same message
        /// The list of children to send to are filtered by the predicate provided
        /// </summary>
        /// <param name="message">Message to send</param>
        public static Unit tellChildren<T>(T message, Func<ProcessId,bool> predicate) =>
            iter(filter(Children, predicate), child => tell(child, message));

        /// <summary>
        /// Tell children the same message, delayed.
        /// The list of children to send to are filtered by the predicate provided
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, TimeSpan delayFor, Func<ProcessId, bool> predicate) =>
            delay(() => tellChildren(message, predicate), delayFor).Subscribe();

        /// <summary>
        /// Tell children the same message, delayed.
        /// The list of children to send to are filtered by the predicate provided
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="message">Message to send</param>
        /// <param name="delayUntil">Date and time to send</param>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        public static IDisposable tellChildren<T>(T message, DateTime delayUntil, Func<ProcessId, bool> predicate) =>
            delay(() => tellChildren(message, predicate), delayUntil).Subscribe();

        /// <summary>
        /// Send a message to the parent process
        /// </summary>
        /// <param name="message">Message to send</param>
        public static Unit tellParent<T>(T message) =>
            tell(Parent, message);

        /// <summary>
        /// Send a message to the parent process at a specified time in the future
        /// </summary>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        public static IDisposable tellParent<T>(T message, TimeSpan delayFor) =>
            tell(Parent, message, delayFor);

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
        public static IDisposable tellParent<T>(T message, DateTime delayUntil) =>
            tell(Parent, message, delayUntil);


        /// <summary>
        /// Send a message to ourself
        /// </summary>
        /// <param name="message">Message to send</param>
        public static Unit tellSelf<T>(T message) =>
            tell(Self, message);

        /// <summary>
        /// Send a message to ourself at a specified time in the future
        /// </summary>
        /// <returns>IDisposable that you can use to cancel the operation if necessary.  You do not need to call Dispose 
        /// for any other reason.</returns>
        /// <param name="message">Message to send</param>
        /// <param name="delayFor">How long to delay sending for</param>
        public static IDisposable tellSelf<T>(T message, TimeSpan delayFor) =>
            tell(Self, message, delayFor);

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
        public static IDisposable tellSelf<T>(T message, DateTime delayUntil) =>
            tell(Self, message, delayUntil);
    }
}
