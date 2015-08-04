using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal interface IActorInbox : IDisposable
    {
        Unit Startup(IActor process, ProcessId supervisor, Option<ICluster> cluster);
        Unit Shutdown();
    }
}
