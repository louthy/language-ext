using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ClusterFactory
    {
        static object sync = new object();
        static Map<string, Func<ClusterConfig, ICluster>> providers = Map.create<string, Func<ClusterConfig, ICluster>>();

        /// <summary>
        /// Provider registration
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="provider">Function that generates a new cluster based on provided config</param>
        /// <returns>Unit</returns>
        public static Unit RegisterProvider(string name, Func<ClusterConfig, ICluster> provider)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            lock (sync)
            {
                providers = providers.AddOrUpdate(name, provider);
                return unit;
            }
        }

        /// <summary>
        /// Create a process cluster
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="config"></param>
        /// <returns>ICluster</returns>
        public static ICluster CreateCluster(string providerName, ClusterConfig config)
        {
            if (providerName == null) throw new ArgumentNullException(nameof(providerName));
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (providers.ContainsKey(providerName))
            {
                return providers[providerName](config);
            }
            else
            {
                throw new ArgumentException($"'{providerName}' isn't a registered provider",nameof(providerName));
            }
        }
    }
}
