using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Reply functions
    /// 
    ///     The reply functions are used to send responses back to processes that have sent
    ///     a message using 'ask'.  The replyError variants are for bespoke error handling
    ///     but if you let the process throw an exception when something goes wrong, the 
    ///     Process system will auto-reply with an error response (and throw it for the
    ///     process that's asking).  If the asking process doesn't capture the error then
    ///     it will continue to cascade to the original asking process.
    /// 
    /// </summary>
    public static partial class Process
    {

        /// <summary>
        /// Reply to an ask
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit reply<T>(T message) =>
            InMessageLoop
                ? ActorContext.CurrentRequest == null
                    ? failwith<Unit>("You can't reply to this message.  It wasn't an 'ask'.  Use isAsk to confirm whether something is an 'ask' or a 'tell'")
                    : ActorContext.Tell(
                        ActorContext.CurrentRequest.ReplyTo, 
                            new ActorResponse(
                                message,
                                message.GetType().AssemblyQualifiedName,
                                ActorContext.CurrentRequest.ReplyTo, 
                                ActorContext.Self, 
                                ActorContext.CurrentRequest.RequestId
                            ), 
                            ActorContext.Self
                        )
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(reply));

        /// <summary>
        /// Reply if asked
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit replyIfAsked<T>(T message) =>
            InMessageLoop && isAsk
                ? reply(message)
                : unit;

        /// <summary>
        /// Reply to an ask with an error
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit replyError(Exception exception) =>
            InMessageLoop
                ? ActorContext.CurrentRequest == null
                    ? failwith<Unit>("You can't reply to this message.  It wasn't an 'ask'.  Use isAsk to confirm whether something is an 'ask' or a 'tell'")
                    : ActorContext.Tell(ActorContext.CurrentRequest.ReplyTo, 
                            new ActorResponse(
                                exception, 
                                exception.GetType().AssemblyQualifiedName, 
                                ActorContext.CurrentRequest.ReplyTo, 
                                ActorContext.Self, 
                                ActorContext.CurrentRequest.RequestId,
                                true
                            ), 
                            ActorContext.Self
                        )
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(reply));

        /// <summary>
        /// Reply to an ask with an error
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit replyError(string errorMessage) =>
            replyError(new Exception(errorMessage));

        /// <summary>
        /// Reply with an error if asked
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit replyErrorIfAsked(Exception exception) =>
            InMessageLoop && isAsk
                ? replyError(exception)
                : unit;

        /// <summary>
        /// Reply with an error if asked
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit replyErrorIfAsked(string errorMessage) =>
            InMessageLoop && isAsk
                ? replyError(errorMessage)
                : unit;
    }
}
