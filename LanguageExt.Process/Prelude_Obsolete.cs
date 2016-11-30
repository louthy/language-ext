using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Process
    {
        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("spawnRoundRobin is obsolete, use Router.roundRobin instead")]
        public static ProcessId spawnRoundRobin<T>(
            ProcessName Name,
            int Count,
            Action<T> Inbox,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            Router.roundRobin(Name, Count, Inbox, Flags, Strategy, MaxMailboxSize);

        /// <summary>
        /// Spawns a new process with N worker processes, each message is sent to one worker
        /// process in a round robin fashion.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="Name">Delegator process name</param>
        /// <param name="Count">Number of worker processes</param>
        /// <param name="Inbox">Worker message handler</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>Process ID of the delegator process</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("spawnRoundRobin is obsolete, use Router.roundRobin instead")]
        public static ProcessId spawnRoundRobin<S, T>(
            ProcessName Name, 
            int Count, 
            Func<S> Setup, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            Router.roundRobin(Name, Count, Setup, Inbox, Flags, Strategy, MaxMailboxSize);
    }
}
