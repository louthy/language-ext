﻿using LanguageExt.UnitsOfMeasure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal interface IActor : IDisposable
    {
        /// <summary>
        /// Process path
        /// </summary>
        ProcessId Id { get; }

        /// <summary>
        /// Process name
        /// </summary>
        ProcessName Name { get; }

        /// <summary>
        /// Parent process
        /// </summary>
        ActorItem Parent { get; }

        /// <summary>
        /// Flags
        /// </summary>
        ProcessFlags Flags { get; }

        /// <summary>
        /// Failure strategy
        /// </summary>
        State<StrategyContext, Unit> Strategy { get; }

        /// <summary>
        /// Child processes
        /// </summary>
        Map<string, ActorItem> Children { get; }

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        Unit Restart();

        /// <summary>
        /// Startup
        /// </summary>
        Unit Startup();

        /// <summary>
        /// Shutdown
        /// </summary>
        Unit Shutdown(bool maintainState);

        /// <summary>
        /// Link child
        /// </summary>
        /// <param name="pid">Child to link</param>
        Unit LinkChild(ActorItem pid);

        /// <summary>
        /// Unlink child
        /// </summary>
        /// <param name="pid">Child to unlink</param>
        Unit UnlinkChild(ProcessId pid);

        /// <summary>
        /// Add a watcher of this Process
        /// </summary>
        /// <param name="pid">Id of the Process that will watch this Process</param>
        Unit AddWatcher(ProcessId pid);

        /// <summary>
        /// Remove a watcher of this Process
        /// </summary>
        /// <param name="pid">Id of the Process that will stop watching this Process</param>
        Unit RemoveWatcher(ProcessId pid);

        /// <summary>
        /// Publish to the PublishStream
        /// </summary>
        Unit Publish(object message);

        /// <summary>
        /// Publish stream - for calls to Process.pub
        /// </summary>
        IObservable<object> PublishStream { get; }

        /// <summary>
        /// State stream - sent after each message loop
        /// </summary>
        IObservable<object> StateStream { get; }

        InboxDirective ProcessMessage(object message);
        InboxDirective ProcessAsk(ActorRequest request);
        InboxDirective ProcessTerminated(ProcessId id);

        R ProcessRequest<R>(ProcessId pid, object message);
        Unit ProcessResponse(ActorResponse response);
        Unit ShutdownProcess(bool maintainState);

        Unit AddSubscription(ProcessId pid, IDisposable sub);
        Unit RemoveSubscription(ProcessId pid);

        Unit DispatchWatch(ProcessId pid);
        Unit DispatchUnWatch(ProcessId pid);
    }
}
