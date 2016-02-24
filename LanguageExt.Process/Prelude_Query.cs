using System.Collections.Generic;

namespace LanguageExt
{
    public partial class Process
    {
        /// <summary>
        /// Finds all *persistent* registered names in a role
        /// </summary>
        /// <param name="role">Role to limit search to</param>
        /// <param name="keyQuery">Key query.  * is a wildcard</param>
        /// <returns>Registered names</returns>
        public static IEnumerable<ProcessName> queryRegistered(ProcessName role, string keyQuery) =>
            ActorContext.Cluster
                        .Map(c => c.QueryRegistered(role.Value, keyQuery))
                        .IfNone(List.empty<ProcessName>());

        /// <summary>
        /// Finds all *persistent* processes based on the search pattern provided.  Note the returned
        /// ProcessIds may contain processes that aren't currently active.  You can still post
        /// to them however.
        /// </summary>
        /// <param name="keyQuery">Key query.  * is a wildcard</param>
        /// <returns>Matching ProcessIds</returns>
        public static IEnumerable<ProcessId> queryProcesses(string keyQuery) =>
            ActorContext.Cluster
                        .Map(c => c.QueryProcesses(keyQuery))
                        .IfNone(new ProcessId[0]);

        /// <summary>
        /// Finds all *persistent* processes based on the search pattern provided and then returns the
        /// meta-data associated with them.
        /// </summary>
        /// <param name="keyQuery">Key query.  * is a wildcard</param>
        /// <returns>Map of ProcessId to ProcessMetaData</returns>
        public static Map<ProcessId, ProcessMetaData> queryProcessMetaData(string keyQuery) =>
            ActorContext.Cluster
                        .Map(c => c.QueryProcessMetaData(keyQuery))
                        .IfNone(Map.empty<ProcessId,ProcessMetaData>());
    }
}
