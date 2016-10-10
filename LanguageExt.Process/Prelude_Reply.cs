﻿using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    ///     Process: Reply functions
    /// </para>
    /// <para>
    ///     The reply functions are used to send responses back to processes that have sent
    ///     a message using 'ask'.  The replyError variants are for bespoke error handling
    ///     but if you let the process throw an exception when something goes wrong, the 
    ///     Process system will auto-reply with an error response (and throw it for the
    ///     process that's asking).  If the asking process doesn't capture the error then
    ///     it will continue to cascade to the original asking process.
    /// </para>
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Use this to cancel a reply in the proxy system
        /// </summary>
        public static readonly NoReturn noreply = 
            NoReturn.Default;

        /// <summary>
        /// Reply to an ask
        /// </summary>
        /// <remarks>
        /// This should be used from within a process' message loop only
        /// </remarks>
        public static Unit reply<T>(T message) =>
            (message is IReturn) && !((IReturn)message).HasValue
                ? unit
                : InMessageLoop
                    ? ActorContext.Request.CurrentRequest == null
                        ? failwith<Unit>("You can't reply to this message.  It wasn't an 'ask'.  Use isAsk to confirm whether something is an 'ask' or a 'tell'")
                        : ActorContext.System(default(SystemName)).Tell(
                            ActorContext.Request.CurrentRequest.ReplyTo, 
                                new ActorResponse(
                                    message,
                                    message.GetType().AssemblyQualifiedName,
                                    ActorContext.Request.CurrentRequest.ReplyTo, 
                                    ActorContext.Request.Self.Actor.Id, 
                                    ActorContext.Request.CurrentRequest.RequestId
                                ), 
                                ActorContext.Request.Self.Actor.Id
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
                ? ActorContext.Request.CurrentRequest == null
                    ? failwith<Unit>("You can't reply to this message.  It wasn't an 'ask'.  Use isAsk to confirm whether something is an 'ask' or a 'tell'")
                    : ActorContext.System(default(SystemName)).Tell(ActorContext.Request.CurrentRequest.ReplyTo, 
                            new ActorResponse(
                                exception, 
                                exception.GetType().AssemblyQualifiedName, 
                                ActorContext.Request.CurrentRequest.ReplyTo, 
                                ActorContext.Request.Self.Actor.Id, 
                                ActorContext.Request.CurrentRequest.RequestId,
                                true
                            ), 
                            ActorContext.Request.Self.Actor.Id
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

        /// <summary>
        /// Reply to the asker, or if it's not an ask then tell the sender
        /// via a message to their inbox.
        /// </summary>
        public static Unit replyOrTellSender<T>(T message) =>
            isAsk
                ? reply(message)
                : Sender.IsValid
                    ? (message is IReturn) && !((IReturn)message).HasValue
                        ? unit
                        : tell(Sender, message, Self)
                    : unit;
    }
}
