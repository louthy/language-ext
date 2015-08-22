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
        Unit Shutdown();

        /// <summary>
        /// Link child
        /// </summary>
        /// <param name="pid">Child to link</param>
        Unit LinkChild(ActorItem pid);

        /// <summary>
        /// Unlink child
        /// </summary>
        /// <param name="pid">Child to unlink</param>
        Unit UnlinkChild(ActorItem pid);

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

        Unit ProcessMessage(object message);
        Unit ProcessAsk(ActorRequest request);
        R ProcessRequest<R>(ProcessId pid, object message);
        Unit ProcessResponse(ActorResponse response);
        Unit ShutdownProcess();

        Unit AddSubscription(ProcessId pid, IDisposable sub);
        Unit RemoveSubscription(ProcessId pid);
        int GetNextRoundRobinIndex();
    }
}
