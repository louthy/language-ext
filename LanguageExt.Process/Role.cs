using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Role
    {
        public static readonly ProcessId broadcast =
            ProcessId.Top["role"]["broadcast"];

        public static readonly ProcessId leastBusy =
            ProcessId.Top["role"]["least-busy"];

        public static readonly ProcessId random =
            ProcessId.Top["role"]["random"];

        public static readonly ProcessId roundRobin =
            ProcessId.Top["role"]["round-robin"];
    }
}
