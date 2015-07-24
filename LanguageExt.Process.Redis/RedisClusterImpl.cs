using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LanguageExt
{
    internal class RedisClusterImpl : ICluster
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
        public Unit Connect()
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
                return unit;
            }
        }

        /// <summary>
        /// Disconnect from cluster
        /// </summary>
        public Unit Disconnect()
        {
            lock(sync)
            {
                if (redis != null)
                {
                    redis.Close(true);
                    redis.Dispose();
                    redis = null;
                }
                return unit;
            }
        }

        /// <summary>
        /// Publish data to a named channel
        /// </summary>
        public long PublishToChannel(string channelName, object data) =>
            redis.GetSubscriber().Publish(
                channelName, 
                JsonConvert.SerializeObject(data)
                );

        /// <summary>
        /// Subscribe to a named channel
        /// </summary>
        public IObservable<object> SubscribeToChannel(string channelName, Type type)
        {
            var subject = new Subject<object>();
            return subject.PostSubscribeAction(() =>
                redis.GetSubscriber().Subscribe(
                    channelName,
                    (channel, value) =>
                    {
                        if (channel == channelName && !value.IsNullOrEmpty)
                        {
                            try
                            {
                                subject.OnNext(JsonConvert.DeserializeObject(value, type));
                            }
                            catch
                            {
                            }
                        }
                    }));
        }

        public void SetValue(string key, object value) =>
            Db.StringSet(key, JsonConvert.SerializeObject(value));

        public T GetValue<T>(string key) =>
            JsonConvert.DeserializeObject<T>(Db.StringGet(key));

        public bool Exists(string key) =>
            Db.KeyExists(key);

        private IDatabase Db => redis.GetDatabase(databaseNumber);
    }
}
