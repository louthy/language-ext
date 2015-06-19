using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class NullProcess : IProcess
    {
        public IEnumerable<ProcessId> Children
        {
            get
            {
                return new ProcessId[0];
            }
        }

        public ProcessId Id
        {
            get
            {
                return ProcessId.None;
            }
        }

        public ProcessName Name
        {
            get
            {
                return "$";
            }
        }

        public ProcessId Parent
        {
            get
            {
                return ProcessId.None;
            }
        }

        public void Dispose()
        {
        }

        public Unit Restart()
        {
            return Unit.Default;
        }

        public Unit Shutdown(bool unlinkFromParent = true)
        {
            return Unit.Default;
        }
    }
}
