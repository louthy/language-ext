using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Provides the name of a system that is shutting down, use Cancel to
    /// stop the shutdown from completing.
    /// </summary>
    public class ShutdownCancellationToken
    {
        public readonly SystemName System;

        public bool Cancelled
        {
            get;
            private set;
        }

        internal ShutdownCancellationToken(SystemName sys)
        {
            System = sys;
        }

        /// <summary>
        /// Call to stop the in-process shutting down of the Process system
        /// </summary>
        public void Cancel()
        {
            Cancelled = true;
        }
    }
}
