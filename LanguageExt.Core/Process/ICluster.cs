using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface ICluster
    {
        /// <summary>
        /// Cluster configuration
        /// </summary>
        ClusterConfig Config
        {
            get;
        }

        /// <summary>
        /// Name of this node in the cluster.  It must be unique in the 
        /// cluster.  It must also be a valid process-name as this will
        /// be used as the 'root' process identifier.
        /// </summary>
        ProcessName NodeName
        {
            get;
        }

        /// <summary>
        /// Connect to cluster
        /// </summary>
        Unit Connect();

        /// <summary>
        /// Disconnect from cluster
        /// </summary>
        Unit Disconnect();
    }
}
