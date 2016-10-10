﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Process;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    [Flags]
    public enum RouterOption
    {
        Default = 0,
        RemoveLocalWorkerWhenTerminated = 1,
        RemoveRemoteWorkerWhenTerminated = 2,
        RemoveWorkerWhenTerminated = 3
    }

    public static partial class Router
    {
        /// <summary>
        /// Spawn a router using the settings in the config
        /// </summary>
        /// <example>
        /// <para>
        ///     router broadcast1: 
        ///         pid:			/root/user/broadcast1
        ///         route:	        broadcast
        ///         worker-count:	10
        /// </para>
        /// <para>
        ///     router broadcast2: 
        ///         pid:			/root/user/broadcast2
        ///         route:	        broadcast
        ///         workers:		[hello, world]
        /// </para>
        /// <para>
        ///     router least: 
        ///         pid:			/role/user/least
        ///         route:	        least-busy
        ///         workers:		[one, two, three]
        /// </para>
        /// </example>
        /// <param name="name">Name of the child process that will be the router</param>
        /// <returns>ProcessId of the router</returns>
        public static ProcessId fromConfig<T>(ProcessName name)
        {
            var id       = Self[name];
            var type     = ActorContext.System(id).Settings.GetRouterDispatch(id);
            var workers  = ActorContext.System(id).Settings.GetRouterWorkers(id)
                                                 .Map(p => p.ProcessId.IfNone(ProcessId.None) )
                                                 .Filter(pid => pid != ProcessId.None);

            var flags    = ActorContext.System(id).Settings.GetProcessFlags(id);
            var mbs      = ActorContext.System(id).Settings.GetProcessMailboxSize(id);

            return type.Map(t =>
                {
                    // TODO: Consider the best approach to generalise this, so that bespoke router
                    //       types can make use of the config system too.
                    switch (t)
                    {
                        case "broadcast":
                            return broadcast<T>(name, workers, RouterOption.Default, flags, mbs);
                        case "hash":
                            return hash<T>(name, workers, null, RouterOption.Default, flags, mbs);
                        case "least-busy":
                            return leastBusy<T>(name, workers, RouterOption.Default, flags, mbs);
                        case "random":
                            return random<T>(name, workers, RouterOption.Default, flags, mbs);
                        case "round-robin":
                            return roundRobin<T>(name, workers, RouterOption.Default, flags, mbs);
                        default:
                            throw new Exception($"Unsupported router type (for config system setup): {t} ");
                    }
                })
               .IfNone(() => failwith<ProcessId>($"'route' not specified for {id}"));
        }

        /// <summary>
        /// Spawn a router using the settings in the config
        /// </summary>
        /// <example>
        /// <para>
        ///     router broadcast1: 
        ///         pid:			/root/user/broadcast1
        ///         route:	        broadcast
        ///         worker-count:	10
        /// </para>
        /// <para>
        ///     router broadcast2: 
        ///         pid:			/root/user/broadcast2
        ///         route:	        broadcast
        ///         workers:		[hello, world]
        /// </para>
        /// <para>
        ///     router least: 
        ///         pid:			/role/user/least
        ///         route:	        least-busy
        ///         workers:		[one, two, three]
        /// </para>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the child process that will be the router</param>
        /// <returns>ProcessId of the router</returns>
        public static ProcessId fromConfig<T>(ProcessName name, Func<T, Unit> Inbox) =>
            fromConfig<Unit, T>(name, () => unit, (_, m) => Inbox(m));

        /// <summary>
        /// Spawn a router using the settings in the config
        /// </summary>
        /// <example>
        /// <para>
        ///     router broadcast1: 
        ///         pid:			/root/user/broadcast1
        ///         route:	        broadcast
        ///         worker-count:	10
        /// </para>
        /// <para>
        ///     router broadcast2: 
        ///         pid:			/root/user/broadcast2
        ///         route:	        broadcast
        ///         workers:		[hello, world]
        /// </para>
        /// <para>
        ///     router least: 
        ///         pid:			/role/user/least
        ///         route:	        least-busy
        ///         workers:		[one, two, three]
        /// </para>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the child process that will be the router</param>
        /// <returns>ProcessId of the router</returns>
        public static ProcessId fromConfig<T>(ProcessName name, Action<T> Inbox) =>
            fromConfig<Unit, T>(name, () => unit, (_, m) => { Inbox(m); return unit; });

        /// <summary>
        /// Spawn a router using the settings in the config
        /// </summary>
        /// <example>
        /// <para>
        ///     router broadcast1: 
        ///         pid:			/root/user/broadcast1
        ///         route:	        broadcast
        ///         worker-count:	10
        /// </para>
        /// <para>
        ///     router broadcast2: 
        ///         pid:			/root/user/broadcast2
        ///         route:	        broadcast
        ///         workers:		[hello, world]
        /// </para>
        /// <para>
        ///     router least: 
        ///         pid:			/role/user/least
        ///         route:	        least-busy
        ///         workers:		[one, two, three]
        /// </para>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the child process that will be the router</param>
        /// <returns>ProcessId of the router</returns>
        public static ProcessId fromConfig<S, T>(ProcessName name, Func<S> Setup, Func<S,T,S> Inbox)
        {
            var id       = Self[name];
            var type     = ActorContext.System(id).Settings.GetRouterDispatch(id);
            var workers  = ActorContext.System(id).Settings.GetRouterWorkerCount(id);
            var flags    = ActorContext.System(id).Settings.GetProcessFlags(id);
            var mbs      = ActorContext.System(id).Settings.GetProcessMailboxSize(id);
            var strategy = ActorContext.System(id).Settings.GetProcessStrategy(id);
            var wrkrName = ActorContext.System(id).Settings.GetRouterWorkerName(id);

            return type.Map(t =>
                {
                    // TODO: Consider the best approach to generalise this, so that bespoke router
                    //       types can make use of the config system too.
                    switch (t)
                    {
                        case "broadcast":
                            return broadcast(name, workers.IfNone(2), Setup, Inbox, flags, strategy, mbs, wrkrName);
                        case "hash":
                            return hash(name, workers.IfNone(2), Setup, Inbox, null, flags, strategy, mbs, wrkrName);
                        case "least-busy":
                            return leastBusy(name, workers.IfNone(2), Setup, Inbox, flags, strategy, mbs, wrkrName);
                        case "random":
                            return random(name, workers.IfNone(2), Setup, Inbox, flags, strategy, mbs, wrkrName);
                        case "round-robin":
                            return roundRobin(name, workers.IfNone(2), Setup, Inbox, flags, strategy, mbs, wrkrName);
                        default:
                            throw new Exception($"Unsupported router type (for config system setup): {t} ");
                    }
                })
               .IfNone(() => failwith<ProcessId>($"'route' not specified for {id}"));
        }


        internal static ProcessId WatchWorkers(ProcessId router, IEnumerable<ProcessId> workers, RouterOption option)
        {
            if ((option & RouterOption.RemoveLocalWorkerWhenTerminated) == RouterOption.RemoveLocalWorkerWhenTerminated)
            {
                workers.Where(w => ActorContext.System(w).IsLocal(w)).Iter(w => watch(router, w));
            }
            if ((option & RouterOption.RemoveRemoteWorkerWhenTerminated) == RouterOption.RemoveRemoteWorkerWhenTerminated)
            {
                workers.Where(w => !ActorContext.System(w).IsLocal(w)).Iter(w => watch(router, w));
            }
            return router;
        }
    }
}
