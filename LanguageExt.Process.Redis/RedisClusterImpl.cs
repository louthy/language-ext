using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    class RedisClusterImpl : ICluster
    {
        readonly ClusterConfig config;

        object sync = new object();
        int databaseNumber;
        ConnectionMultiplexer redis;

        /// <summary>
        /// Ctor
        /// </summary>
        public RedisClusterImpl(ClusterConfig config)
        {
            this.config = config;
        }

        ~RedisClusterImpl()
        {
            Dispose();
        }

        public void Dispose()
        {
            var r = redis;
            if (r != null)
            {
                if (r.IsConnected)
                {
                    r.Close();
                }
                r.Dispose();
                redis = null;
            }
        }

        public ProcessName NodeName =>
            Config.NodeName;

        /// <summary>
        /// Return true if connected to cluster
        /// </summary>
        public bool Connected =>
            redis != null;

        /// <summary>
        /// Cluster configuration
        /// </summary>
        public ClusterConfig Config =>
            config;

        /// <summary>
        /// Connect to cluster
        /// </summary>
        public void Connect()
        {
            var databaseNumber = parseUInt(Config.CatalogueName).IfNone(() => raise<uint>(new ArgumentException("Parsing CatalogueName as a number that is 0 or greater, failed.")));

            if (databaseNumber < 0) throw new ArgumentException(nameof(databaseNumber) + " should be 0 or greater.", nameof(databaseNumber));

            lock (sync)
            {
                if (redis == null)
                {
                    this.redis = ConnectionMultiplexer.Connect(Config.ConnectionString);
                    this.databaseNumber = (int)databaseNumber;
                }
            }
        }

        /// <summary>
        /// Disconnect from cluster
        /// </summary>
        public void Disconnect()
        {
            lock (sync)
            {
                if (redis != null)
                {
                    redis.Close(true);
                    redis.Dispose();
                    redis = null;
                }
            }
        }

        /// <summary>
        /// Publish data to a named channel
        /// </summary>
        public int PublishToChannel(string channelName, object data) =>
            (int)redis.GetSubscriber().Publish(
                channelName,
                JsonConvert.SerializeObject(data)
                );

        /// <summary>
        /// Subscribe to a named channel
        /// </summary>
        public void SubscribeToChannel(string channelName, Type type, Action<object> handler)
        {
            UnsubscribeChannel(channelName);
            redis.GetSubscriber().Subscribe(
                channelName,
                (channel, value) =>
                {
                    if (channel == channelName && !value.IsNullOrEmpty)
                    {
                        try
                        {
                            handler(JsonConvert.DeserializeObject(value, type));
                        }
                        catch
                        {
                        }
                    }
                });
        }

        public void SubscribeToChannel<T>(string channelName, Action<T> handler)
        {
            UnsubscribeChannel(channelName);
            redis.GetSubscriber().Subscribe(
                channelName,
                (channel, value) =>
                {
                    if (channel == channelName && !value.IsNullOrEmpty)
                    {
                        try
                        {
                            handler(JsonConvert.DeserializeObject<T>(value));
                        }
                        catch
                        {
                        }
                    }
                });
        }

        public void UnsubscribeChannel(string channelName)
        {
            redis.GetSubscriber().Unsubscribe(channelName);
        }

        public void SetValue(string key, object value) =>
            Db.StringSet(key, JsonConvert.SerializeObject(value),TimeSpan.FromDays(RedisCluster.maxDaysToPersistProcessState));

        public T GetValue<T>(string key) =>
            JsonConvert.DeserializeObject<T>(Db.StringGet(key));

        public bool Exists(string key) =>
            Db.KeyExists(key);

        public bool Delete(string key) =>
            Db.KeyDelete(key);

        public int QueueLength(string key) =>
            (int)Db.ListLength(key);

        public int Enqueue(string key, object value) =>
            (int)Db.ListRightPush(key, JsonConvert.SerializeObject(value));

        public T Peek<T>(string key)
        {
            try
            {
                var val = Db.ListGetByIndex(key, 0);
                return JsonConvert.DeserializeObject<T>(val);
            }
            catch
            {
                return default(T);
            }
        }

        public T Dequeue<T>(string key)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Db.ListLeftPop(key));
            }
            catch 
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get queue by key
        /// </summary>
        public IEnumerable<T> GetQueue<T>(string key)
        {
            if (Exists(key))
            {
                // First get the entire queue
                return Db.ListRange(key)
                         .Select(x =>
                         {
                             try { return SomeUnsafe(JsonConvert.DeserializeObject<T>(x)); }
                             catch { return OptionUnsafe<T>.None; }
                         })
                         .Where( x => x.IsSome)
                         .Select( x => x.IfNoneUnsafe(null) )
                         .ToList();
            }
            else
            {
                return new T[0];
            }
        }

        public bool HashFieldExists(string key, string field) =>
            Db.HashExists(key, field);

        public void HashFieldAddOrUpdate<T>(string key, string field, T value) =>
            Db.HashSet(key, field, JsonConvert.SerializeObject(value));

        public void HashFieldAddOrUpdate<T>(string key, Map<string, T> fields) =>
            Db.HashSet(
                key, 
                fields.Map((k, v) => new HashEntry(k, JsonConvert.SerializeObject(v))).Values.ToArray()
                );

        public bool DeleteHashField(string key, string field) =>
            Db.HashDelete(key, field);

        public int DeleteHashFields(string key, IEnumerable<string> fields) =>
            (int)Db.HashDelete(key, fields.Map(x => (RedisValue)x).ToArray());

        public Map<string, T> GetHashFields<T>(string key) =>
            Db.HashGetAll(key)
              .Fold(
                Map.empty<string, T>(), 
                (m, e) => m.Add(e.Name, JsonConvert.DeserializeObject<T>(e.Value)));

        public Option<T> GetHashField<T>(string key, string field)
        {
            var res = Db.HashGet(key, field);
            if (res.IsNullOrEmpty) return None;
            return JsonConvert.DeserializeObject<T>(res);
        }

        public Map<string, T> GetHashFields<T>(string key, IEnumerable<string> fields) =>
            Db.HashGet(key, fields.Map(x => (RedisValue)x).ToArray())
              .Zip(fields)
              .Filter(x => !x.Item1.IsNullOrEmpty)
              .Fold(
                  Map.empty<string, T>(),
                  (m, e) => m.Add(e.Item2, JsonConvert.DeserializeObject<T>(e.Item1)));

        private IDatabase Db => 
            redis.GetDatabase(databaseNumber);
    }
}
