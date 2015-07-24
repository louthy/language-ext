using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class ClusterConfig
    {
        public readonly ProcessName NodeName;
        public readonly string ConnectionString;
        public readonly string CatalogueName;
        public readonly Map<string, string> Metadata;

        public ClusterConfig(
            ProcessName nodeName,
            string connectionString,
            string catalogueName,
            Map<string, string> metadata = null
        )
        {
            NodeName = nodeName;
            ConnectionString = connectionString;
            CatalogueName = catalogueName;
            Metadata = metadata ?? Map.empty<string,string>();
        }
    }
}
