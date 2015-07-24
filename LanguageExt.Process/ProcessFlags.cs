using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public enum ProcessFlags
    {
        /// <summary>
        /// Default.  No persistence state, inbox, or remote channel routing.
        /// </summary>
        Default = 0,

        /// <summary>
        /// All state changes will be persisted at the end of a message
        /// loop.  The initial state of the process will come from the 
        /// persistent store if it exists.
        /// </summary>
        PersistentState = 1,

        /// <summary>
        /// All messages will be persisted for the duration of their time
        /// in the inbox.  The initial state of the inbox will come from the 
        /// persistent store if it exists.
        /// </summary>
        PersistentInbox = 2,

        /// <summary>
        /// Any messages published by calling 'Process.publish()' will be
        /// sent via a persistent channel named after the process ID.  
        /// This can be subscribed to be any other process within or without
        /// the application.
        /// </summary>
        RemotePublish = 4,

        /// <summary>
        /// Combines PersistentState | PersistentInbox | RemotePublish to persist
        /// all data relating to the process.
        /// </summary>
        PersistAll = 7
    }
}
