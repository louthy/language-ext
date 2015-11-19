using System;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Map;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process:  Dead letter functions
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Forward a message to dead-letters (and wrap it in a contextual dead-letter
        /// structre)
        /// </summary>
        /// <param name="message">Dead letter message</param>
        /// <param name="reason">Reason for the dead-letter</param>
        public static Unit dead(object message, string reason) =>
            tell(
                ActorContext.DeadLetters, 
                DeadLetter.create(
                    Sender, 
                    Self,
                    reason, 
                    message
            ));

        /// <summary>
        /// Forward a message to dead-letters (and wrap it in a contextual dead-letter
        /// structre)
        /// </summary>
        /// <param name="message">Dead letter message</param>
        /// <param name="ex">Exception that caused the dead-letter</param>
        public static Unit dead(object message, Exception ex) =>
            tell(
                ActorContext.DeadLetters,
                DeadLetter.create(
                    Sender,
                    Self,
                    ex,
                    message
            ));

        /// <summary>
        /// Forward a message to dead-letters (and wrap it in a contextual dead-letter
        /// structre)
        /// </summary>
        /// <param name="message">Dead letter message</param>
        /// <param name="ex">Exception that caused the dead-letter</param>
        /// <param name="reason">Reason for the dead-letter</param>
        public static Unit dead(object message, Exception ex, string reason) =>
            tell(
                ActorContext.DeadLetters,
                DeadLetter.create(
                    Sender,
                    Self,
                    ex,
                    reason,
                    message
            ));

        /// <summary>
        /// Forward the current message to dead-letters (and wrap it in a contextual dead-letter
        /// structre)
        /// </summary>
        /// <param name="reason">Reason for the dead-letter</param>
        public static Unit dead(string reason) =>
            tell(
                ActorContext.DeadLetters,
                DeadLetter.create(
                    Sender,
                    Self,
                    reason,
                    ActorContext.CurrentMsg
            ));

        /// <summary>
        /// Forward the current message to dead-letters (and wrap it in a contextual dead-letter
        /// structre)
        /// </summary>
        /// <param name="ex">Exception that caused the dead-letter</param>
        public static Unit dead(Exception ex) =>
            tell(
                ActorContext.DeadLetters,
                DeadLetter.create(
                    Sender,
                    Self,
                    ex,
                    ActorContext.CurrentMsg
            ));

        /// <summary>
        /// Forward a message to dead-letters (and wrap it in a contextual dead-letter
        /// structre)
        /// </summary>
        /// <param name="ex">Exception that caused the dead-letter</param>
        /// <param name="reason">Reason for the dead-letter</param>
        public static Unit dead(Exception ex, string reason) =>
            tell(
                ActorContext.DeadLetters,
                DeadLetter.create(
                    Sender,
                    Self,
                    ex,
                    reason,
                    ActorContext.CurrentMsg
            ));
    }
}
