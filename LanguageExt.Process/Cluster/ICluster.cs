using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface ICluster : IDisposable
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
        /// Role that this node plays in the cluster
        /// </summary>
        ProcessName Role
        {
            get;
        }

        /// <summary>
        /// Connect to cluster
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from cluster
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Publish data to a named channel
        /// </summary>
        int PublishToChannel(string channelName, object data);

        /// <summary>
        /// Subscribe to a named channel
        /// </summary>
        IObservable<Object> SubscribeToChannel(string channelName, System.Type type);

        /// <summary>
        /// Subscribe to a named channel
        /// </summary>
        IObservable<T> SubscribeToChannel<T>(string channelName);

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
        int Enqueue(string key, object value);

        /// <summary>
        /// Dequeue a message
        /// </summary>
        T Dequeue<T>(string key);

        /// <summary>
        /// Get queue by key
        /// </summary>
        IEnumerable<T> GetQueue<T>(string key);

        /// <summary>
        /// Remove a key
        /// </summary>
        /// <param name="key">Key</param>
        bool Delete(string key);

        /// <summary>
        /// Remove many keys
        /// </summary>
        /// <param name="keys">Keys</param>
        bool DeleteMany(params string[] keys);

        /// <summary>
        /// Look at the item at the head of the queue
        /// </summary>
        T Peek<T>(string key);

        /// <summary>
        /// Find the queue length
        /// </summary>
        /// <param name="key">Key</param>
        int QueueLength(string key);

        /// <summary>
        /// Finds all registered names in a role
        /// </summary>
        /// <param name="role">Role to limit search to</param>
        /// <param name="keyQuery">Key query.  * is a wildcard</param>
        /// <returns>Registered names</returns>
        IEnumerable<ProcessName> QueryRegistered(string role, string keyQuery);

        /// <summary>
        /// Finds all the processes based on the search pattern provided.  Note the returned
        /// ProcessIds may contain processes that aren't currently active.  You can still post
        /// to them however.
        /// </summary>
        /// <param name="keyQuery">Key query.  * is a wildcard</param>
        /// <returns>Matching ProcessIds</returns>
        IEnumerable<ProcessId> QueryProcesses(string keyQuery);

        /// <summary>
        /// Finds all the processes based on the search pattern provided and then returns the
        /// meta-data associated with them.
        /// </summary>
        /// <param name="keyQuery">Key query.  * is a wildcard</param>
        /// <returns>Map of ProcessId to ProcessMetaData</returns>
        Map<ProcessId, ProcessMetaData> QueryProcessMetaData(string keyQuery);

        /// <summary>
        /// Finds all session keys
        /// </summary>
        /// <returns>Session keys</returns>
        IEnumerable<string> QuerySessionKeys();

        // TODO: Docs

        bool HashFieldExists(string key, string field);
        void HashFieldAddOrUpdate<T>(string key, string field, T value);
        void HashFieldAddOrUpdate<T>(string key, Map<string, T> fields);
        bool DeleteHashField(string key, string field);
        int DeleteHashFields(string key, IEnumerable<string> fields);
        Map<string, object> GetHashFields(string key);
        Map<string, T> GetHashFields<T>(string key);
        Map<K, T> GetHashFields<K, T>(string key, Func<string, K> keyBuilder);
        Option<T> GetHashField<T>(string key, string field);
        Map<string, T> GetHashFields<T>(string key, IEnumerable<string> fields);
        void SetAddOrUpdate<T>(string key, T value);
        void SetRemove<T>(string key, T value);
        Set<T> GetSet<T>(string key);
        bool SetContains<T>(string key, T value);
        bool SetExpire(string key, TimeSpan time);
    }
}
