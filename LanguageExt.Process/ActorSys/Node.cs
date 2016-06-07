using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class NodeOffline
    {
        public readonly ProcessName Name;

        internal NodeOffline(ProcessName name)
        {
            Name = name;
        }
    }

    public class NodeOnline
    {
        public readonly ProcessName Name;

        internal NodeOnline(ProcessName name)
        {
            Name = name;
        }
    }
}
