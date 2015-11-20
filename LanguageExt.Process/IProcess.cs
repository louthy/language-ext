using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Represents a process as an object rather than a function
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    public interface IProcess<in T>
    {
        /// <summary>
        /// Inbox message handler
        /// </summary>
        /// <param name="msg">Message</param>
        void OnMessage(T msg);

        /// <summary>
        /// Invoked when a watched Process terminates
        /// </summary>
        /// <param name="pid">Process ID of the Process that terminated</param>
        void OnTerminated(ProcessId pid);
    }
}
