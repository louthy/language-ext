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
        static AppProfile appProfile;
        static ProcessSystemConfig config = new ProcessSystemConfig("");
        static Map<string, object> processSettings = Map.empty<string, object>();

        /// <summary>
        /// Resets the configuration system to all default settings (i.e. empty).  Use this call followed by
        /// one of the ProcessConfig.initialise(...) variants to reload new configuration settings live.
        /// </summary>
        public static Unit unload()
        {
            lock(sync)
            {
                appProfile = null;
                config = new ProcessSystemConfig("");
                processSettings = Map.empty<string, object>();
                return unit;
            }
        }

#if !COREFX && !COREFX_45

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for cluster.conf and process.conf files in the web application folder, you should call this
        /// function from within Application_BeginRequest of Global.asax.  It can run multiple times, once the config 
        /// has loaded the system won't re-load the config until you call ProcessConfig.unload() by 
        /// ProcessConfig.initialiseWeb(...)
        /// </summary>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static AppProfile initialiseWeb(IEnumerable<FuncSpec> strategyFuncs = null) =>
            initialiseWeb(_ => { }, strategyFuncs);

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
        public static AppProfile initialiseWeb(Action<AppProfile> setup, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            if (appProfile != null) return appProfile;
            if (HttpContext.Current == null) throw new NotSupportedException("There must be a valid HttpContext.Current to call ProcessConfig.initialiseWeb()");
            return initialiseFileSystem(hostName(HttpContext.Current), setup, strategyFuncs);
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
        public static AppProfile initialiseFileSystem(string nodeName, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            if (appProfile != null) return appProfile;
            return initialiseFileSystem(nodeName, _ => { }, strategyFuncs);
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
        public static AppProfile initialiseFileSystem(Action<AppProfile> setup, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            if (appProfile != null) return appProfile;
            return initialiseFileSystem(null, setup, strategyFuncs);
        }

        /// <summary>
        /// Process system configuration initialisation
        /// This will look for process.conf files in the application folder.  It can run multiple times, 
        /// once the config has loaded the system won't re-load the config until you call ProcessConfig.unload() followed 
        /// by ProcessConfig.initialiseFileSystem(...), so it's safe to not surround it with ifs.
        /// </summary>
        /// <param name="strategyFuncs">Plugin extra strategy behaviours by passing in a list of FuncSpecs.</param>
        public static AppProfile initialiseFileSystem(IEnumerable<FuncSpec> strategyFuncs = null)
        {
            if (appProfile != null) return appProfile;
            return initialiseFileSystem(null, _ => { }, strategyFuncs);
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
        public static AppProfile initialiseFileSystem(string nodeName, Action<AppProfile> setup, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            if (appProfile != null) return appProfile;

            lock(sync)
            {
                if (appProfile != null) return appProfile;

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
        /// Process system configuration initialisation
        /// This will parse the configuration text, you should call this
        /// function from within Application_BeginRequest of Global.asax.  It can run multiple times, once the config 
        /// has loaded the system won't re-load the config until you call ProcessConfig.unload() followed by 
        /// ProcessConfig.initialiseFileSystem(...), so it's safe to not surround it with ifs.
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
        public static AppProfile initialise(string configText, Option<string> nodeName, Action<AppProfile> setup = null, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            if (appProfile != null) return appProfile;
            lock (sync)
            {
                if (appProfile != null) return appProfile;

                config = new ProcessSystemConfig(nodeName.IfNone(""), strategyFuncs);
                config.ParseConfigText(configText);

                appProfile = AppProfile.NonClustered;

                nodeName.Iter(node =>
                {
                    var provider = config.GetClusterSetting("provider", "value", "redis");
                    var role = config.GetClusterSetting("role", "value", name => clusterSettingMissing<string>(name));
                    var clusterConn = config.GetClusterSetting("connection", "value", "localhost");
                    var clusterDb = config.GetClusterSetting("database", "value", "0");
                    var env = config.GetClusterSetting<string>("env", "value");
                    var userEnv = config.GetClusterSetting<string>("user-env", "value");

                    appProfile = new AppProfile(
                        node,
                        role,
                        clusterConn,
                        clusterDb,
                        env,
                        userEnv
                        );

                    Cluster.connect(provider, node, clusterConn, clusterDb, role);
                });

                config.PostConnect();

                setup(appProfile);
                return appProfile;
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
        public static T read<T>(string name, string prop, T defaultValue) =>
            InMessageLoop
                ? ActorContext.Context.Ops.Read(name, prop, ActorContext.Context.ProcessFlags, defaultValue)
                : config.GetRoleSetting(name, prop, defaultValue);

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
        public static Unit write(string name, string prop, object value)
        {
            if (InMessageLoop)
            {
                var trans = ActorContext.Context.Ops;
                ActorContext.Context = ActorContext.Context.SetOps(trans.Write(value, name, prop, ActorContext.Context.ProcessFlags));
                return unit;
            }
            else
            {
                return config.WriteSettingOverride($"role-{Role.Current.Value}@settings", value, name, prop, ProcessFlags.PersistState);
            }
        }

        /// <summary>
        /// Clear a setting
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        public static Unit clear(string name, string prop)
        {
            if (InMessageLoop)
            {
                var trans = ActorContext.Context.Ops;
                ActorContext.Context = ActorContext.Context.SetOps(trans.Clear(name, prop, ActorContext.Context.ProcessFlags));
                return unit;
            }
            else
            {
                return config.ClearSettingOverride($"role-{Role.Current.Value}@settings", name, prop, ProcessFlags.PersistState);
            }
        }

        /// <summary>
        /// Clear all settings for the process (or role if outside of the message-loop of a Process)
        /// </summary>
        public static Unit clear()
        {
            if (InMessageLoop)
            {
                var trans = ActorContext.Context.Ops;
                ActorContext.Context = ActorContext.Context.SetOps(trans.ClearAll(ActorContext.Context.ProcessFlags));
                return unit;
            }
            else
            {
                return config.ClearSettingsOverride($"role-{Role.Current.Value}@settings", ProcessFlags.PersistState);
            }
        }

        internal static ProcessSystemConfig Settings =>
            config;

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
