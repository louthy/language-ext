using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using StackExchange.Redis;

namespace LanguageExt
{
    internal class RedisClusterImpl : ICluster
    {
        readonly ClusterConfig config;

        object sync = new object();
        int databaseNumber;
        ConnectionMultiplexer connection;

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
            connection != null;

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
                if (connection == null)
                {
                    this.connection = ConnectionMultiplexer.Connect(Config.ConnectionString);
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
                if (connection != null)
                {
                    connection.Close(true);
                    connection.Dispose();
                    connection = null;
                }
                return unit;
            }
        }
    }
}
