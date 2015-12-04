using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Role
    {
        public static ProcessId broadcast(ProcessName role) =>
            ProcessId.Top["role"][role]["broadcast"];

        public static ProcessId leastBusy(ProcessName role) =>
            ProcessId.Top["role"][role]["least-busy"];

        public static ProcessId random(ProcessName role) =>
            ProcessId.Top["role"][role]["random"];

        public static ProcessId roundRobin(ProcessName role) =>
            ProcessId.Top["role"][role]["round-robin"];
    }
}
