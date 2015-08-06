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
        void SubscribeToChannel(string channelName, System.Type type, Action<object> handler);

        /// <summary>
        /// Subscribe to a named channel
        /// </summary>
        void SubscribeToChannel<T>(string channelName, Action<T> handler);

        /// <summary>
        /// Unsubscribe from a channel (removes all subscribers from a channel)
        /// </summary>
        void UnsubscribeChannel(string channelName);

        /// <summary>
        /// Set a value by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(string key, object value);

        /// <summary>
        /// Get a value by key
        /// </summary>
        T GetValue<T>(string key);

        /// <summary>
        /// Check if a key exists
        /// </summary>
        bool Exists(string key);

        /// <summary>
        /// Enqueue a message
        /// </summary>
        void Enqueue(string key, object value);

        /// <summary>
        /// Dequeue a message
        /// </summary>
        T Dequeue<T>(string key);

        /// <summary>
        /// Get queue by key
        /// </summary>
        IEnumerable<T> GetQueue<T>(string key);

        T Peek<T>(string key);

        int QueueLength(string key);
    }
}
