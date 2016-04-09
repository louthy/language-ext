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
        public static Tuple<SessionSync, Time> Setup(SessionSync sync, Time checkFreq) =>
            Tuple(sync, checkFreq);

        /// <summary>
        /// Inbox
        /// </summary>
        public static Tuple<SessionSync, Time> Inbox(Tuple<SessionSync, Time> state, Unit _)
        {
            try
            {
                state.Item1.ExpiredCheck();
            }
            catch(Exception e)
            {
                logErr(e);
            }
            tellSelf(unit, state.Item2);
            return state;
        }
    }
}
