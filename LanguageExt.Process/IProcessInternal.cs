using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal interface IProcessInternal
    {
        Option<IProcess> GetChildProcess(ProcessName name);
        IProcess AddChildProcess(Some<IProcess> process);
        Unit HandleFaultedChild(SystemChildIsFaultedMessage message);
        Unit TellSystem(SystemMessage message);
        Unit TellUserControl(UserControlMessage message);
        Unit Tell(object message);
        Unit UnlinkChild(ProcessId pid);
    }
}
