using System;
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
        internal static ProcessId WatchWorkers(ProcessId router, IEnumerable<ProcessId> workers, RouterOption option)
        {
            if ((option & RouterOption.RemoveLocalWorkerWhenTerminated) == RouterOption.RemoveLocalWorkerWhenTerminated)
            {
                workers.Where(w => ActorContext.IsLocal(w)).Iter(w => watch(router, w));
            }
            if ((option & RouterOption.RemoveRemoteWorkerWhenTerminated) == RouterOption.RemoveRemoteWorkerWhenTerminated)
            {
                workers.Where(w => !ActorContext.IsLocal(w)).Iter(w => watch(router, w));
            }
            return router;
        }
    }
}
