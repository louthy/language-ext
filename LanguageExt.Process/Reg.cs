using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Build registered Process paths
    /// 
    /// </summary>
    public static class Reg
    {
        internal static Unit init()
        {
            // Triggers static ctor
            return unit;
        }

        /// <summary>
        /// Static ctor
        /// Sets up the default registered dispatcher
        /// </summary>
        static Reg()
        {
            ProcessName reg = "reg";

            var regs = fun((ProcessId leaf) => {
                var name = leaf.Head().GetName();
                var key  = ProcessId.Top["__registered"][name].Path;

                return ActorContext.Cluster
                                   .Map(x => x.GetSet<ProcessId>(key))
                                   .IfNone(Set.empty<ProcessId>())
                                   .Append(ActorContext.GetLocalRegistered(name))
                                   .Map(pid => pid.Append(leaf.Skip(1)))
                                   .AsEnumerable();
            });

            Dispatch.register(reg, leaf => regs(leaf));
        }
    }
}
