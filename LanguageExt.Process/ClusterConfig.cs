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

        public ClusterConfig(
            ProcessName nodeName,
            string connectionString,
            string catalogueName
        )
        {
            NodeName = nodeName;
            ConnectionString = connectionString;
            CatalogueName = catalogueName;
        }
    }
}
