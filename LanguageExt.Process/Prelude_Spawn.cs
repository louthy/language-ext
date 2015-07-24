using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Spawn functions
    /// 
    ///     The spawn functions create a new process.  Processes are either simple message receivers that
    ///     don't manage state and therefore only need a messageHandler.  Or they manage state over time.
    /// 
    ///     If they manage state then you should also provide a 'setup' function that generates the initial
    ///     state.  This allows the process system to recover if a process crashes.  It can re-call the 
    ///     setup function to reset the state and continue processing the messages.
    /// 
    ///         Usage:
    /// 
    ///             // Simple process the simply prints messages to the console
    ///             var processId = spawn<string>("my-process", Console.WriteLine);
    /// 
    ///             // Thread safe caching system
    ///             var processId = spawn<Map<string,CacheItem>,CacheMsg>(
    ///                 "cache", 
    ///                 () => Map.empty<string,CacheItem>(),
    ///                 (state, msg) => 
    ///                 {
    ///                     switch(msg.Type)
    ///                     {
    ///                         case CacheInstr.Add:    return state.Add(msg.Key, msg.Value);
    ///                         case CacheInstr.Remove: return state.Remove(msg.Key);
    ///                         case CacheInstr.Get:    reply(state[msg.Key]);
    ///                     }
    ///                     return state;
    ///                 });
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Create a new process by name.  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<T>(ProcessName name, Action<T> messageHandler, ProcessFlags flags = ProcessFlags.Default) =>
            spawn<Unit, T>(name, () => unit, (state, msg) => { messageHandler(msg); return state; }, flags);

        /// <summary>
        /// Create a new process by name (accepts Unit as a return value instead of void).  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawnU<T>(ProcessName name, Func<T,Unit> messageHandler, ProcessFlags flags = ProcessFlags.Default) =>
            spawn<Unit, T>(name, () => unit, (state, msg) => { messageHandler(msg); return state; }, flags);

        /// <summary>
        /// Create a new process by name.  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<S, T>(ProcessName name, Func<S> setup, Func<S, T, S> messageHandler, ProcessFlags flags = ProcessFlags.Default) =>
            ActorContext.ActorCreate(ActorContext.Self, name, messageHandler, setup, flags);

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "<name>-<index>" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<T>(int count, ProcessName name, Action<T> messageHandler, ProcessFlags flags = ProcessFlags.Default) =>
            List.map(Range(0, count), n => spawn(name + "-" + n, messageHandler, flags)).ToList();

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "<name>-<index>" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="name">Name of the child-process</param>
        /// <param name="messageHandler">Function that is the process</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnN<S, T>(int count, ProcessName name, Func<S> setup, Func<S, T, S> messageHandler, ProcessFlags flags = ProcessFlags.Default) =>
            List.map(Range(0, count), n => ActorContext.ActorCreate(ActorContext.Self, name + "-" + n, messageHandler, setup, flags)).ToList();
    }
}
