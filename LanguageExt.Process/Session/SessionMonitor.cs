using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt.Session
{
    /// <summary>
    /// Very simple process that tells the Session Manager to synchronise with
    /// the cluster and to update its internal map of sessions.
    /// </summary>
    class SessionMonitor
    {
        /// <summary>
        /// Setup
        /// </summary>
        /// <param name="sessionManager">Process ID of the session manager</param>
        /// <param name="checkFreq">Frequency to check</param>
        public static Tuple<ProcessId, Time> Setup(ProcessId sessionManager, Time checkFreq) =>
            Tuple(sessionManager, checkFreq);

        /// <summary>
        /// Inbox
        /// </summary>
        public static Tuple<ProcessId, Time> Inbox(Tuple<ProcessId, Time> state, Unit _)
        {
            proxy<ISessionSync>(state.Item1).Sync();
            tellSelf(unit, state.Item2);
            return state;
        }
    }
}
