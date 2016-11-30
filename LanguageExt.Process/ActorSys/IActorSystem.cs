using LanguageExt.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal interface IActorSystem
    {
        /// <summary>
        /// Name of the system
        /// </summary>
        SystemName Name { get; }

        /// <summary>
        /// Cluster
        /// </summary>
        Option<ICluster> Cluster { get; }

        /// <summary>
        /// Return the ActorItem that is used to shut down ActorInboxes
        /// </summary>
        /// <remarks>ActorInboxes are slow to shutdown, so they're queued and done separately to
        /// the main shutdown of a process</remarks>
        Option<ActorItem> GetInboxShutdownItem();

        /// <summary>
        /// Root process
        /// </summary>
        ProcessId Root { get; }

        /// <summary>
        /// User process
        /// </summary>
        ProcessId User { get; }

        /// <summary>
        /// Errors process
        /// </summary>
        ProcessId Errors { get; }

        /// <summary>
        /// Dead-letters process
        /// </summary>
        ProcessId DeadLetters { get; }

        /// <summary>
        /// Process system settings
        /// </summary>
        ProcessSystemConfig Settings { get; }

        /// <summary>
        /// Get an IActorDisptach for a Process - this works out how to
        /// communicate with other processes (remote, local, JS, etc.)
        /// </summary>
        IActorDispatch GetDispatcher(ProcessId pid);

        /// <summary>
        /// Sets up a watcher for 'watching'
        /// </summary>
        Unit AddWatcher(ProcessId watcher, ProcessId watching);

        /// <summary>
        /// Removes a watcher that's watching 'watching'
        /// </summary>
        Unit RemoveWatcher(ProcessId watcher, ProcessId watching);

        /// <summary>
        /// Remove the named registrations for any names that point to 'pid'
        /// </summary>
        Unit DeregisterById(ProcessId pid);

        /// <summary>
        /// Remove all processes registered as 'name'
        /// </summary>
        Unit DeregisterByName(ProcessName name);

        /// <summary>
        /// Tell any watchers that a 'terminating' is shutting down
        /// </summary>
        Unit DispatchTerminate(ProcessId terminating);

        /// <summary>
        /// Ask process 'pid' a question (message)
        /// </summary>
        Unit Ask(ProcessId pid, object message, ProcessId sender);
    }
}
