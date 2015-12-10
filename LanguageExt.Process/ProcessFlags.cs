using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    [Flags]
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
        PersistState = 1,

        /// <summary>
        /// All messages will be persisted for the duration of their time
        /// in the inbox.  The initial state of the inbox will come from the 
        /// persistent store if it exists.  
        /// </summary>
        /// <remarks>
        /// Messages are only dequeued from the inbox once they have been
        /// processed.  'Processed' means either:
        /// 
        ///     * the user process received the message and handled it successfully
        ///     * The user process received the message, and threw an error which
        ///       resulted in the message being redirected to DeadLettters.
        /// 
        /// Note: Use the Strategy system for more complex failure behaviours.
        /// </remarks>
        PersistInbox = 2,

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
        PersistAll = 7,

        /// <summary>
        /// Allows a process to listen for messages locally and remotely.  This is mainly
        /// for internal processes to do stuff like route replies.  But can also be used
        /// for regular processes where you want local messages to be sent direct and not
        /// persisted, but still make the process available to receive messages from remote
        /// sources.  
        /// </summary>
        /// <remarks>
        /// Note that the guarantees are weaker than the PersistInbox option, messages
        /// received locally are not persisted at all, and will be lost if the system dies,
        /// and messages received remotely are dequeued immediately and stored in the local
        /// inbox (and therefore will share the same fate if the system dies).  However
        /// messages received remotely whilst the system is down will persist until it wakes
        /// back up; so there's an element of safety.
        /// </remarks>
        ListenRemoteAndLocal = 8,

        /// <summary>
        /// When the state changes, publish for remote subscribers
        /// </summary>
        RemoteStatePublish = 16
    }
}
