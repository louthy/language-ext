using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace LanguageExt.ProcessJS
{
    /// <summary>
    /// Process-log.  Provides a live view of the items being logged by the process
    /// system.  Can also be used directly by calling Tell to do user logging.
    /// Any process can Connect() to this process to get a stream of the log messages
    /// as they come through.  Your process must accept the LanguageExt.ProcessLogItem
    /// type.
    /// </summary>
    public static class ProcessLog
    {
        /// <summary>
        /// Message types this log supports
        /// </summary>
        public enum MsgTag
        {
            Connect,
            Disconnect,
            Tell,
            Ask
        }

        static object sync = new object();
        static IDisposable deadLetterSub;
        static IDisposable errorSub;
        static ProcessId processId;
        static ProcessName processName;

        /// <summary>
        /// Start up the process log
        /// </summary>
        /// <param name="processNameOverride">Override the default process name</param>
        /// <param name="logViewMax">Size of the log 'window'</param>
        public static Unit startup(Option<ProcessName> processNameOverride, int logViewMax = 200)
        {
            if(processId.IsValid) return unit;
            lock(sync)
            {
                if (processId.IsValid) return unit;

                processName   = processNameOverride.IfNone("process-log");
                processId     = spawn<State, ProcessLogItem>(processName, () => setup(logViewMax), inbox);

                deadLetterSub = subscribe<DeadLetter>(DeadLetters, msg => tellWarning(msg.ToString()));
                errorSub      = subscribe<Exception>(Errors, e => tellError(e));
            }

            return unit;
        }

        /// <summary>
        /// Shutdown the process log
        /// </summary>
        public static Unit shutdown()
        {
            if( processId.IsValid )
            {
                kill(processId);
                processId = ProcessId.None;
            }
            return unit;
        }

        /// <summary>
        /// Ask.  Gets a snapshot of the log.
        /// </summary>
        public static IEnumerable<ProcessLogItem> ask() =>
            ask<IEnumerable<ProcessLogItem>>(processId, new ProcessLogItem(ProcessLogItemType.Info,"ask"));

        /// <summary>
        /// General log function.  Look at TellInfo, TellWarning and TellError for more specific
        /// log functions.
        /// </summary>
        public static Unit tell(ProcessLogItem logItem) =>
            processId.IsValid
                ? Process.tell(processId, logItem)
                : unit;

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static Unit tellInfo(string message) =>
            processId.IsValid
                ? Process.tell(processId, new ProcessLogItem(ProcessLogItemType.Info, message, null))
                : unit;

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static Unit tellWarning(string message) =>
            processId.IsValid
                ? Process.tell(processId, new ProcessLogItem(ProcessLogItemType.Warning, message, null))
                : unit;

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static Unit tellError(string message) =>
            processId.IsValid
                ? Process.tell(processId, new ProcessLogItem(ProcessLogItemType.Error, message, null))
                : unit;

        /// <summary>
        /// Log an exception
        /// </summary>
        /// <param name="ex">Exception to log</param>
        public static Unit tellError(Exception ex) =>
            processId.IsValid
                ? Process.tell(processId, new ProcessLogItem(ProcessLogItemType.Error, null, ex))
                : unit;

        /// <summary>
        /// Log an exception and a message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="ex">Exception to log</param>
        public static Unit tellError(Exception ex, string message) =>
            processId.IsValid
                ? Process.tell(processId, new ProcessLogItem(ProcessLogItemType.Error, message, ex))
                : unit;

        /// <summary>
        /// Setup the process.  Subscribes to errors and dead-letters
        /// </summary>
        private static State setup(int logViewMax) => 
            new State(logViewMax);

        /// <summary>
        /// Process log inbox
        /// </summary>
        /// <param name="state">State</param>
        /// <param name="msg">Message</param>
        /// <returns>State</returns>
        private static State inbox(State state, ProcessLogItem msg)
        {
            try
            {
                if (isTell)
                {
                    publish(msg);
                    return state.EnqeueLogItem(msg);
                }
                else
                {
                    reply(state.GetLogSnapshot());
                    return state;
                }
            }
            catch
            {
                // Ignore.  We want the inbox to be robust and not throwing
                // errors that would just be reported back here.
                return state;
            }
        }

        /// <summary>
        /// State.  Manages the log items an the subscriptions.
        /// </summary>
        private class State
        {
            public readonly int LogViewMax;
            public readonly IImmutableQueue<ProcessLogItem> Log;

            public State(int logViewMax)
            {
                LogViewMax = logViewMax;
                Log        = Queue<ProcessLogItem>();
            }

            private State(
                int logViewMax,
                IImmutableQueue<ProcessLogItem> log
            )
            {
                LogViewMax = logViewMax;
                Log        = log;
            }

            public State EnqeueLogItem(ProcessLogItem logItem)
            {
                var log = Log.Enqueue(logItem);
                if (log.Count() > LogViewMax)
                {
                    log = log.Dequeue();
                }
                return new State(LogViewMax, log);
            }

            public IEnumerable<ProcessLogItem> GetLogSnapshot()
            {
                return Log;
            }
        }
    }
}
