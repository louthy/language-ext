using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace LanguageExt
{
    public static class Cluster
    {
        public static Unit connect(string providerName, ClusterConfig config)
        {
            var cluster = ClusterFactory.CreateCluster(providerName, config);
            cluster.Connect();
            return ActorContext.RegisterCluster(cluster);
        }

        public static Unit disconnect(ICluster cluster)
        {
            return cluster.Disconnect();
        }
    }
}
