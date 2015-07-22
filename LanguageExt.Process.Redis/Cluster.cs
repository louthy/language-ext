using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Process.Redis
{
    public static class Cluster
    {
        static Cluster()
        {
            // We get the static ctor to do the actual registration of the factory
            // method.  Cluster.Register merely triggers the CLR to invoke this 
            // ctor.  
            ClusterFactory.RegisterProvider("redis", Factory);
        }

        /// <summary>
        /// Call to register the Redis cluster provider with Language-Ext cluster
        /// process factory.
        /// </summary>
        public static Unit Register() => unit;

        /// <summary>
        /// ICluster factory provider
        /// </summary>
        /// <param name="config">ClusterConfig</param>
        /// <returns>ICluster</returns>
        private static ICluster Factory(ClusterConfig config)
        {
            return new RedisClusterImpl(config);
        }
    }
}
