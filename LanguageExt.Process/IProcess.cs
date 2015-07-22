using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IProcess : IDisposable
    {
        /// <summary>
        /// Process path
        /// </summary>
        ProcessId Id { get; }

        /// <summary>
        /// Process name
        /// </summary>
        ProcessName Name { get; }

        /// <summary>
        /// Parent process
        /// </summary>
        ProcessId Parent { get; }

        /// <summary>
        /// Child processes
        /// </summary>
        Map<string, ProcessId> Children { get; }

        /// <summary>
        /// Clears the state (keeps the mailbox items)
        /// </summary>
        Unit Restart();

        /// <summary>
        /// Startup
        /// </summary>
        Unit Startup();

        /// <summary>
        /// Shutdown
        /// </summary>
        Unit Shutdown();
    }
}
