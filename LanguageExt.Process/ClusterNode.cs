using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class ClusterNode
    {
        public readonly ProcessName NodeName;
        public readonly ProcessName Role;

        internal ClusterNode(ProcessName nodeName, ProcessName role)
        {
            NodeName = nodeName;
            Role = role;
        }
    }
}
