using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Config
{
    class Manager
    {
        public Map<ProcessId, ActorConfig> Configs;

        public Manager()
        {
            Configs = Map.empty<ProcessId, ActorConfig>();
        }

        string FindLocalConfig() =>
            map(Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, "process.config"),
                (string path1) => File.Exists(path1)
                    ? path1
                    : map(Path.Combine(Directory.GetParent(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName).FullName, "process.config"),
                        (string path2) => File.Exists(path2)
                            ? path2
                            : ""));

        public void Initialise(Option<string> configFilename)
        {
            Configs = (from p in configFilename.Map(Some: x => File.Exists(x) ? x : "", None: () => FindLocalConfig())
                       where p != ""
                       select LoadConfigFile(p))
                      .IfNone(Map.empty<ProcessId, ActorConfig>());
        }

        Map<ProcessId, ActorConfig> LoadConfigFile(string p)
        {
            return null;
        }
    }
}
