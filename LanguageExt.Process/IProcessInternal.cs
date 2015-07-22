using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal interface IProcessInternal
    {
        Unit LinkChild(ProcessId pid);
        Unit UnlinkChild(ProcessId pid);
    }

    internal interface IProcessInternal<T> : IProcessInternal
    {
        Unit ProcessMessage(T message);
    }
}
