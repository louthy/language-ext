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

        /// <summary>
        /// Publish data to a named channel
        /// </summary>
        long PublishToChannel(string channelName, object data);

        /// <summary>
        /// Subscribe to a named channel
        /// </summary>
        IObservable<object> SubscribeToChannel(string channelName, System.Type type);

        void SetValue(string key, object value);

        T GetValue<T>(string key);

        bool Exists(string key);
    }
}
