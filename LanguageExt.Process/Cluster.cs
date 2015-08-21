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
        /// <summary>
        /// Connect to a cluster
        /// </summary>
        /// <param name="providerName">Provider name is a unique name for the persistence layer 
        /// type, for example: "redis"</param>
        /// <param name="config">Cluster config</param>
        public static Unit connect(
            string providerName, 
            ClusterConfig config)
        {
            var cluster = ClusterFactory.CreateCluster(providerName, config);
            cluster.Connect();
            return ActorContext.RegisterCluster(cluster);
        }

        /// <summary>
        /// Connect to a cluster
        /// </summary>
        /// <param name="providerName">Provider name is a unique name for the persistence layer 
        /// type, for example: "redis"</param>
        /// <param name="nodeName">Unique name for this process.  It becomes the name of the root 
        /// node and allows other services on the cluster to discover you and communicate with you.
        /// </param>
        /// <param name="connectionString">Provider defined connection string</param>
        /// <param name="catalogueName">>Provider defined catalogue name</param>
        public static Unit connect(
            string providerName,
            ProcessName nodeName,
            string connectionString,
            string catalogueName
        )
        {
            var cluster = ClusterFactory.CreateCluster(providerName, config(nodeName,connectionString,catalogueName));
            cluster.Connect();
            return ActorContext.RegisterCluster(cluster);
        }

        /// <summary>
        /// Disconnect from a cluster
        /// </summary>
        /// <param name="cluster">Cluster to disconnect from</param>
        public static Unit disconnect() =>
            ActorContext.DeregisterCluster();

        /// <param name="nodeName">Unique name for this process.  It becomes the name of the root 
        /// node and allows other services on the cluster to discover you and communicate with you.
        /// </param>
        /// <param name="connectionString">Provider defined connection string</param>
        /// <param name="catalogueName">>Provider defined catalogue name</param>
        public static ClusterConfig config(
            ProcessName nodeName,
            string connectionString,
            string catalogueName
        ) => 
            new ClusterConfig(
                nodeName, 
                connectionString, 
                catalogueName
            );

    }
}
