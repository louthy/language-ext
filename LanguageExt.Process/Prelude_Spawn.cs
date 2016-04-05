using Newtonsoft.Json;
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
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminated">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<T>(
            ProcessName Name,
            Action<T> Inbox,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            Action<ProcessId> Terminated = null
            ) =>
            spawn<Unit, T>(
                Name,
                () => unit,
                (state, msg) => {
                    Inbox(msg);
                    return state;
                },
                Flags,
                Strategy,
                MaxMailboxSize,
                (state, pid) => {
                    Terminated(pid);
                    return state;
                }
            );

        /// <summary>
        /// Create a new process by name (accepts Unit as a return value instead of void).  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminated">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawnUnit<T>(
            ProcessName Name,
            Func<T, Unit> Inbox,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            Func<ProcessId, Unit> Terminated = null
            ) =>
            spawn<Unit, T>(Name, () => unit, (state, msg) => { Inbox(msg); return state; }, Flags, Strategy, MaxMailboxSize, (state, pid) => { Terminated(pid); return state; });

        /// <summary>
        /// Create a new process by name.  
        /// If this is called from within a process' message loop 
        /// then the new process will be a child of the current process.  If it is called from
        /// outside of a process, then it will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Setup">Startup and restart function</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminated">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>A ProcessId that identifies the child</returns>
        public static ProcessId spawn<S, T>(
            ProcessName Name,
            Func<S> Setup,
            Func<S, T, S> Inbox,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            Func<S, ProcessId, S> Terminated = null,
            SystemName System = default(SystemName)
            )
        {
            if (System.IsValid && ActorContext.Request != null) throw new ProcessException("When spawning you can only specify a System from outside of a Process", ActorContext.Self[Name].Path, "");

            var sys = System.IsValid
                ? ActorContext.System(System)
                : ActorContext.DefaultSystem;

            var parent = System.IsValid
                ? sys.UserContext.Self
                : ActorContext.SelfProcess;

            return sys.ActorCreate(parent, Name, Inbox, Setup, Terminated, Strategy, Flags, MaxMailboxSize, false);
        }

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Count">Number of processes to spawn</param>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminated">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnMany<T>(
            int Count, 
            ProcessName Name, 
            Action<T> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            Action<ProcessId> Terminated = null
            ) =>
            List.map(Range(0, Count), n => spawn($"{Name}-{n}", Inbox, Flags, Strategy, MaxMailboxSize, Terminated)).ToList();

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Count">Number of processes to spawn</param>
        /// <param name="Setup">Startup and restart function</param>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminated">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnMany<S, T>(
            int Count, 
            ProcessName Name, 
            Func<S> Setup, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            Func<S, ProcessId, S> Terminated = null
            ) =>
            List.map(Range(0, Count), n => ActorContext.System(default(SystemName)).ActorCreate(ActorContext.SelfProcess, $"{Name}-{n}", Inbox, Setup, Terminated, Strategy, Flags, MaxMailboxSize, false)).ToList();

        /// <summary>
        /// Create N child processes.
        /// The name provided will be used as a basis to generate the child names.  Each child will
        /// be named "name-index" where index starts at zero.  
        /// If this is called from within a process' message loop 
        /// then the new processes will be a children of the current process.  If it is called from
        /// outside of a process, then they will be made a child of the root 'user' process.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Type of messages that the child-process can accept</typeparam>
        /// <param name="Spec">Map of IDs and State for generating child workers</param>
        /// <param name="Name">Name of the child-process</param>
        /// <param name="Inbox">Function that is the process</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminated">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>ProcessId IEnumerable</returns>
        public static IEnumerable<ProcessId> spawnMany<S, T>(
            ProcessName Name, 
            Map<int, Func<S>> Spec, 
            Func<S, T, S> Inbox, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize,
            Func<S, ProcessId, S> Terminated = null
            ) =>
            Map.map(Spec, (id,state) => ActorContext.System(default(SystemName)).ActorCreate(ActorContext.SelfProcess, $"{Name}-{id}", Inbox, state, Terminated, Strategy, Flags, MaxMailboxSize, false)).Values.ToList();

        /// <summary>
        /// Spawn by type
        /// </summary>
        /// <typeparam name="TProcess">Process type</typeparam>
        /// <typeparam name="TMsg">Message type</typeparam>
        /// <param name="Name">Name of process to spawn</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <param name="Terminate">Message function to call when a Process [that this Process
        /// watches] terminates</param>
        /// <returns>ProcessId</returns>
        public static ProcessId spawn<TProcess,TMsg>(
            ProcessName Name, 
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
            where TProcess : IProcess<TMsg>, new()
        {
            return spawn<IProcess<TMsg>, TMsg>(Name, () => new TProcess(),
              (process, msg) => {
                  process.OnMessage(msg);
                  return process;
              },
              Flags,
              Strategy,
              MaxMailboxSize,
              (process, pid) => {
                  process.OnTerminated(pid);
                  return process;
              }
            );
        }

#if !COREFX

        /// <summary>
        /// Proxy based system for processes that are defined by a single type: 
        /// their state type.  The state type holds state in its member variables
        /// and its message handlers are its member functions.  This function builds
        /// a proxy from the interface passed so that you can invoke the message-
        /// handlers in a strongly typed way.
        /// </summary>
        /// <typeparam name="TProcess">Interface type that represents the Process
        /// </typeparam>
        /// <param name="pid">ProcessId of the Process to proxy</param>
        /// <returns>An interface that automatically tells and asks depending on
        /// method arguments and return types.</returns>
        public static TProcess proxy<TProcess>(ProcessId pid) =>
            ProxyBuilder.Build<TProcess>(pid);

        /// <summary>
        /// Spawn by iterface type.  You must provide a Setup function that returns
        /// the concrete implementation of TProcessInterface. 
        /// Communication is via a generated proxy: the returned TProcessInterface.
        /// </summary>
        /// <typeparam name="TProcess">Process type</typeparam>
        /// <param name="Name">Name of process to spawn</param>
        /// <param name="Setup">You must provide a Setup function that returns
        /// the concrete implementation of TProcessInterface</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>TProcessInterface - The proxy for communicating with the Process</returns>
        public static TProcessInterface spawn<TProcessInterface>(
            ProcessName Name,
            Func<TProcessInterface> Setup,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            ) =>
            proxy<TProcessInterface>(spawn<TProcessInterface, ProxyMsg>(
              Name, 
              Setup,
              ProxyMsgInbox,
              Flags,
              Strategy,
              MaxMailboxSize,
              ProxyTerminatedInbox
            ));

        /// <summary>
        /// Spawn by type.  
        /// Communication is via proxy - call Process.proxy(pid) on the resulting
        /// ProcessId to send tells and asks in a strongly typed fashion.
        /// </summary>
        /// <typeparam name="TProcess">Process type</typeparam>
        /// <param name="Name">Name of process to spawn</param>
        /// <param name="Flags">Process flags</param>
        /// <param name="Strategy">Failure supervision strategy</param>
        /// <returns>TProcessInterface - The proxy for communicating with the Process</returns>
        public static ProcessId spawn<TProcess>(
            ProcessName Name,
            ProcessFlags Flags = ProcessFlags.Default,
            State<StrategyContext, Unit> Strategy = null,
            int MaxMailboxSize = ProcessSetting.DefaultMailboxSize
            )
            where TProcess : new() =>
            spawn<TProcess, ProxyMsg>(
              Name, 
              () => new TProcess(),
              ProxyMsgInbox,
              Flags,
              Strategy,
              MaxMailboxSize,
              ProxyTerminatedInbox
            );


        static S ProxyTerminatedInbox<S>(S process, ProcessId termpid)
        {
            var method = process.GetType().GetMethod("OnTerminated");
            if (method != null)
            {
                method.Invoke(process, new object[] { termpid });
            }
            return process;
        }

        static S ProxyMsgInbox<S>(S process, ProxyMsg msg)
        {
            var types = msg.ArgTypes.Map(Type.GetType).ToArray();
            var args = msg.Args.Map((i, x) => JsonConvert.DeserializeObject(x, types[i])).ToArray();
            var method = process.GetType().GetMethod(msg.Method, types);

            var result = method.Invoke(process, args);

            if (msg.ReturnType != "System.Void" && notnull(result))
            {
                replyOrTellSender(result);
            }
            return process;
        }
#endif
    }
}
