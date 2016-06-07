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
        public readonly DateTime LastHeartbeat;
        public readonly ProcessName Role;

        public ClusterNode(ProcessName nodeName, DateTime lastHeartbeat, ProcessName role)
        {
            NodeName = nodeName;
            LastHeartbeat = lastHeartbeat;
            Role = role;
        }
    }
}
