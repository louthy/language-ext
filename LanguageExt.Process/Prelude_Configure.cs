using System;
using System.Collections.Generic;
using System.IO;
#if !COREFX && !COREFX_45
using System.Web;
#endif
using LanguageExt.Config;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process system configuration
    /// 
    /// </summary>
    public static class ProcessConfig
    {
        static object sync = new object();

#if !COREFX && !COREFX_45

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for cluster.conf and process.conf files in the web application folder, you should call this
        /// function from within Application_BeginRequest of Global.asax.  It can run multiple times, once the config 
        /// has loaded the system won't re-load the config until you call ProcessConfig.unload() by 
        /// ProcessConfig.initialiseWeb(...)
        /// </summary>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialiseWeb(IEnumerable<FuncSpec> strategyFuncs = null) =>
            initialiseWeb(() => { }, strategyFuncs);

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for cluster.conf and process.conf files in the web application folder, you should call this
        /// function from within Application_BeginRequest of Global.asax.  It can run multiple times, once the config 
        /// has loaded the system won't re-load the config until you call ProcessConfig.unload() followed by 
        /// ProcessConfig.initialiseWeb(...)
        /// 
        /// NOTE: If a cluster is specified in the cluster.conf and its 'node-name' matches the host name of the web-
        /// application (i.e. www.example.com), then those settings will be used to connect to the cluster.  
        /// This allows for different staging environments to be setup.
        /// </summary>
        /// <param name="setup">A setup function to call on successful loading of the configuration files - this will
        /// happen once only.</param>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialiseWeb(Action setup, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            lock (sync)
            {
                if (HttpContext.Current == null) throw new NotSupportedException("There must be a valid HttpContext.Current to call ProcessConfig.initialiseWeb()");
                return initialiseFileSystem(hostName(HttpContext.Current), setup, strategyFuncs);
            }
        }

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for cluster.conf and process.conf files in the application folder.  It can run multiple times, 
        /// once the config has loaded the system won't re-load the config until you call ProcessConfig.unload() followed 
        /// by ProcessConfig.initialiseFileSystem(...), so it's safe to not surround it with ifs.
        /// 
        /// NOTE: If a cluster is specified in the cluster.conf and its 'node-name' matches nodeName, then those settings 
        /// will be used to connect to the cluster.  This allows for different staging environments to be setup.
        /// </summary>
        /// <param name="nodeName">If a cluster is specified in the cluster.conf and its 'node-name' matches nodeName, then 
        /// those settings will be used to connect to the cluster.  This allows for different staging environments to be 
        /// setup.</param>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialiseFileSystem(string nodeName, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            lock (sync)
            {
                return initialiseFileSystem(nodeName, () => { }, strategyFuncs);
            }
        }

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for process.conf files in the application folder.  It can run multiple times, 
        /// once the config has loaded the system won't re-load the config until you call ProcessConfig.unload() followed 
        /// by  ProcessConfig.initialiseFileSystem(...), so it's safe to not surround it with ifs.
        /// </summary>
        /// <param name="setup">A setup function to call on successful loading of the configuration files - this will
        /// happen once only.</param>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialiseFileSystem(Action setup, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            lock (sync)
            {
                return initialiseFileSystem(null, setup, strategyFuncs);
            }
        }

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for process.conf files in the application folder.  It can run multiple times, 
        /// once the config has loaded the system won't re-load the config until you call ProcessConfig.unload() followed 
        /// by ProcessConfig.initialiseFileSystem(...), so it's safe to not surround it with ifs.
        /// </summary>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialiseFileSystem(IEnumerable<FuncSpec> strategyFuncs = null)
        {
            lock (sync)
            {
                return initialiseFileSystem(null, () => { }, strategyFuncs);
            }
        }

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for cluster.conf and process.conf files in the application folder.  It can run multiple times, 
        /// but once the config has loaded the system won't re-load the config until you call ProcessConfig.unload() followed 
        /// by  ProcessConfig.initialiseFileSystem(...), so it's safe to not surround it with ifs.
        /// 
        /// NOTE: If a cluster is specified in the cluster.conf and its 'node-name' matches nodeName, then those settings 
        /// will be used to connect to the cluster.  This allows for different staging environments to be setup.
        /// </summary>
        /// <param name="nodeName">If a cluster is specified in the cluster.conf and its 'node-name' matches nodeName, then 
        /// those settings will be used to connect to the cluster.  This allows for different staging environments to be 
        /// setup.</param>
        /// <param name="setup">A setup function to call on successful loading of the configuration files - this will
        /// happen once only.</param>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialiseFileSystem(string nodeName, Action setup, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            lock (sync)
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var clusterPath = Path.Combine(appPath, "cluster.conf");
                var processPath = Path.Combine(appPath, "process.conf");

                var clusterText =
                    File.Exists(clusterPath)
                        ? File.ReadAllText(clusterPath)
                        : "";

                var processText = File.Exists(processPath)
                    ? File.ReadAllText(processPath)
                    : "";

                return initialise(clusterText + processText, nodeName, setup, strategyFuncs);
            }
        }
#endif

        /// <summary>
        /// Process system initialisation
        /// Initialises am in-memory only Process system
        /// </summary>
        public static Unit initialise() =>
            initialise("", None, () => { });

        /// <summary>
        /// Initialise without a config file or text
        /// </summary>
        /// <param name="systemName">Name of the system - this is most useful</param>
        /// <param name="roleName"></param>
        /// <param name="nodeName"></param>
        /// <param name="providerName"></param>
        /// <param name="connectionString"></param>
        /// <param name="catalogueName"></param>
        /// <returns></returns>
        public static Unit initialise(
            SystemName systemName,
            ProcessName roleName,
            ProcessName nodeName,
            string connectionString,
            string catalogueName,
            string providerName = "redis"
            )
        {
            lock (sync)
            {
                var types = new Types();

                StartFromConfig(new ProcessSystemConfig(
                    systemName,
                    nodeName.Value,
                    Map.empty<string, ValueToken>(),
                    Map.empty<ProcessId, ProcessToken>(),
                    Map.empty<string, State<StrategyContext, Unit>>(),
                    new ClusterToken(
                        List.create(
                            new NamedValueToken("node-name", new ValueToken(types.String, nodeName.Value)),
                            new NamedValueToken("role", new ValueToken(types.String, roleName.Value)),
                            new NamedValueToken("env", new ValueToken(types.String, systemName.Value)),
                            new NamedValueToken("connection", new ValueToken(types.String, connectionString)),
                            new NamedValueToken("database", new ValueToken(types.String, catalogueName)),
                            new NamedValueToken("provider", new ValueToken(types.String, providerName)))),
                    types
                ));
            }
            return unit;
        }

        /// <summary>
        /// Process system configuration initialisation
        /// This will parse the configuration text, It can run multiple times, once the config has loaded the system won't 
        /// re-load the config until you call ProcessConfig.unload() followed by ProcessConfig.initialise(...), so it's safe 
        /// to not surround it with ifs.
        /// 
        /// NOTE: If a cluster is specified in config text and its 'node-name' matches nodeName, then those settings 
        /// will be used to connect to the cluster.  This allows for different staging environments to be setup.
        /// </summary>
        /// <param name="nodeName">If a cluster is specified in the cluster.conf and its 'node-name' matches nodeName, then 
        /// those settings will be used to connect to the cluster.  This allows for different staging environments to be 
        /// setup.</param>
        /// <param name="setup">A setup function to call on successful loading of the configuration files - this will
        /// happen once only.</param>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static Unit initialise(string configText, Option<string> nodeName, Action setup = null, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            lock (sync)
            {
                var parser = new ProcessSystemConfigParser(nodeName.IfNone(""), new Types(), strategyFuncs);
                var configs = String.IsNullOrWhiteSpace(configText)
                    ? Map.create(Tuple(new SystemName("sys"), ProcessSystemConfig.Empty))
                    : parser.ParseConfigText(configText);

                configs.Iter(StartFromConfig);
                if(setup != null) setup();
                return unit;
            }
        }

        private static void StartFromConfig(ProcessSystemConfig config)
        {
            lock (sync)
            {
                config.Cluster.Match(
                    Some: _ =>
                    {
                        // Extract cluster settings
                        var provider = config.GetClusterSetting("provider", "value", "redis");
                            var role = config.GetClusterSetting("role", "value", name => clusterSettingMissing<string>(name));
                            var clusterConn = config.GetClusterSetting("connection", "value", "localhost");
                            var clusterDb = config.GetClusterSetting("database", "value", "0");
                            var env = config.SystemName;
                            var userEnv = config.GetClusterSetting<string>("user-env", "value");

                            var appProfile = new AppProfile(
                                config.NodeName,
                                role,
                                clusterConn,
                                clusterDb,
                                env,
                                userEnv
                                );

                        // Look for an existing actor-system with the same system name
                        var current = ActorContext.Systems.Filter(c => c.Value == env).HeadOrNone();

                        // Work out if something significant has changed that will cause us to restart
                        var restart = current.Map(ActorContext.System)
                                                 .Map(c => c.AppProfile.NodeName != appProfile.NodeName ||
                                                           c.AppProfile.Role != appProfile.Role ||
                                                           c.AppProfile.ClusterConn != appProfile.ClusterConn ||
                                                           c.AppProfile.ClusterDb != appProfile.ClusterDb);

                        // Either restart / update settings / or start new
                        restart.Match(
                            Some: r =>
                            {
                                if (r)
                                {
                                    // Restart
                                    try
                                    {
                                        ActorContext.StopSystem(env);
                                    }
                                    catch (Exception e)
                                    {
                                        logErr(e);
                                    }
                                    StartFromConfig(config);
                                }
                                else
                                {
                                    // Update settings
                                    ActorContext.System(env).UpdateSettings(config, appProfile);
                                        var cluster = from systm in current.Map(ActorContext.System)
                                                        from clstr in systm.Cluster
                                                        select clstr;
                                }
                            },
                            None: () =>
                            {
                                // Start new
                                ICluster cluster = Cluster.connect(
                                        provider,
                                        config.NodeName,
                                        clusterConn,
                                        clusterDb,
                                        role
                                        );

                                ActorContext.StartSystem(env, Optional(cluster), appProfile, config);
                                config.PostConnect();
                            });
                    },
                    None: () =>
                    {
                        ActorContext.StartSystem(new SystemName("sys"), None, AppProfile.NonClustered, config);
                    });
            }
        }

        /// <summary>
        /// Access a setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static T read<T>(string name, string prop, T defaultValue, SystemName system = default(SystemName)) =>
            InMessageLoop
                ? ActorContext.Request.Ops.Read(name, prop, ActorContext.Request.ProcessFlags, defaultValue)
                : ActorContext.System(system).Settings.GetRoleSetting(name, prop, defaultValue);

        /// <summary>
        /// Access a setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static T read<T>(string name, T defaultValue) =>
            read(name, "value", defaultValue);

        /// <summary>
        /// Access a list setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static Lst<T> readList<T>(string name, string prop = "value") =>
            read(name, prop, List.empty<T>());

        /// <summary>
        /// Access a map setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static Map<string, T> readMap<T>(string name, string prop = "value") =>
            read(name, prop, Map.empty<string, T>());

        /// <summary>
        /// Write a setting
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Value to set</param>
        public static Unit write(string name, object value) =>
            write(name, "value", value);

        /// <summary>
        /// Write a setting
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <param name="value">Value to set</param>
        public static Unit write(string name, string prop, object value, SystemName system = default(SystemName))
        {
            if (InMessageLoop)
            {
                var trans = ActorContext.Request.Ops;
                ActorContext.Request.SetOps(trans.Write(value, name, prop, ActorContext.Request.ProcessFlags));
                return unit;
            }
            else
            {
                return ActorContext.System(system).Settings.WriteSettingOverride($"role-{Role.Current.Value}@settings", value, name, prop, ProcessFlags.PersistState);
            }
        }

        /// <summary>
        /// Clear a setting
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        public static Unit clear(string name, string prop, SystemName system = default(SystemName))
        {
            if (InMessageLoop)
            {
                var trans = ActorContext.Request.Ops;
                ActorContext.Request.SetOps(trans.Clear(name, prop, ActorContext.Request.ProcessFlags));
                return unit;
            }
            else
            {
                return ActorContext.System(system).Settings.ClearSettingOverride($"role-{Role.Current.Value}@settings", name, prop, ProcessFlags.PersistState);
            }
        }

        /// <summary>
        /// Clear all settings for the process (or role if outside of the message-loop of a Process)
        /// </summary>
        public static Unit clear(SystemName system = default(SystemName))
        {
            if (InMessageLoop)
            {
                var trans = ActorContext.Request.Ops;
                ActorContext.Request.SetOps(trans.ClearAll(ActorContext.Request.ProcessFlags));
                return unit;
            }
            else
            {
                return ActorContext.System(system).Settings.ClearSettingsOverride($"role-{Role.Current.Value}@settings", ProcessFlags.PersistState);
            }
        }

        static T clusterSettingMissing<T>(string name) =>
            failwith<T>("Cluster setting missing: " + name);

#if !COREFX && !COREFX_45
        static string hostName(HttpContext context) =>
            context.Request.Url.Host == "localhost"
                ? System.Environment.MachineName
                : context.Request.Url.Host;
#endif
    }
}
