using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Application profile
    /// It provides the core settings used when initialising the Process
    /// system.  You don't create it yourself, it is passed to you once
    /// the config system has successfully parsed the config files.
    /// </summary>
    public class AppProfile
    {
        /// <summary>
        /// The name of the running process system node
        /// Comes from the 'node-name' attribute in the configuration file.
        /// </summary>
        public readonly ProcessName NodeName;

        /// <summary>
        /// The role of the running process system
        /// Comes from the 'role' attribute in the configuration file.
        /// </summary>
        public readonly ProcessName Role;

        /// <summary>
        /// The cluster connection string 
        /// Comes from the 'connection' attribute in the configuration file.
        /// </summary>
        public readonly string ClusterConn;

        /// <summary>
        /// The cluster db name (in Redis this is "0"-"15").  Comes from
        /// the 'database' attribute in the configuration file.
        /// </summary>
        public readonly string ClusterDb;

        /// <summary>
        /// The environment name (this comes from the cluster variable 
        /// name in the config file).  Use this to build staging 
        /// environments.
        /// 
        ///     i.e  cluster dev:
        ///             node-name:     ms-1
        ///             role:          mail-system
        ///             string sql-db: "dev.example.com"
        ///             ..
        ///             
        ///          cluster production:
        ///             node-name: ms-1
        ///             role:      mail-system
        ///             string sql-db: "live.example.com"
        ///             
        /// </summary>
        public readonly Option<string> Env;

        /// <summary>
        /// A user environment 
        /// </summary>
        public readonly Option<string> UserEnv;

        /// <summary>
        /// Ctor
        /// </summary>
        internal AppProfile(
            string nodeName,
            string role,
            string clusterConn,
            string clusterDb,
            Option<string> env,
            Option<string> userEnv)
        {
            NodeName = nodeName;
            Role = role;
            ClusterConn = clusterConn;
            ClusterDb = clusterDb;
            Env = env;
            UserEnv = userEnv;
        }

        internal readonly static AppProfile NonClustered = 
            new AppProfile("root", "local", "", "", None, None);
    }
}
