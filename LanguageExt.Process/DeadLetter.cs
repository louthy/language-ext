using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Dead letter message
    /// </summary>
    public class DeadLetter
    {
        private DeadLetter(ProcessId sender, ProcessId recipient, Exception ex, string reason, object message)
        {
            Sender = sender;
            Recipient = recipient;
            Exception = Optional(ex);
            Reason = Optional(reason);
            Message = Optional(message);
        }

        /// <summary>
        /// Create a new dead letter
        /// </summary>
        public static DeadLetter create(ProcessId sender, ProcessId recipient, Exception ex, string reason, object message) =>
            new DeadLetter(sender, recipient, ex, reason, message);

        /// <summary>
        /// Create a new dead letter
        /// </summary>
        public static DeadLetter create(ProcessId sender, ProcessId recipient, string reason, object message) =>
            new DeadLetter(sender, recipient, null, reason, message);

        /// <summary>
        /// Create a new dead letter
        /// </summary>
        public static DeadLetter create(ProcessId sender, ProcessId recipient, Exception ex, object message) =>
            new DeadLetter(sender, recipient, ex, null, message);

        /// <summary>
        /// Create a new dead letter
        /// </summary>
        public static DeadLetter create(ProcessId sender, ProcessId recipient, object message) =>
            new DeadLetter(sender, recipient, null, null, message);

        /// <summary>
        /// Sender of the letter that ended up 'dead'
        /// </summary>
        public readonly ProcessId Sender;

        /// <summary>
        /// Intended recipient of the message that ended up 'dead'
        /// </summary>
        public readonly ProcessId Recipient;

        /// <summary>
        /// Any exception that was thrown the cause the letter to die
        /// </summary>
        public readonly Option<Exception> Exception;

        /// <summary>
        /// An optional reason why the letter died
        /// </summary>
        public readonly Option<string> Reason;

        /// <summary>
        /// The content of the dead letter
        /// </summary>
        public Option<object> Message;

        /// <summary>
        /// Summary of the message content
        /// </summary>
        public string ContentDisplay =>
            Message.Match(
                Some: objmsg => map(objmsg.ToString(), msg =>
                                    msg.Length > 100
                                        ? msg.Substring(0, 100) + "..."
                                        : msg),
                None: ()     => "[null]"
            );

        /// <summary>
        /// Friendly type display
        /// </summary>
        public string ContentTypeDisplay =>
            Message.Match(
                Some: x => x.GetType().Name,
                None: () => "[null]"
            );

        private static string ProcessFmt(ProcessId pid) =>
            pid.IsValid
                ? pid.ToString()
                : "no-sender";


        /// <summary>
        /// Get a string representation of the dead letter
        /// </summary>
        public override string ToString() =>
            Exception.Match(
                Some: ex =>
                    Reason.Match(
                        Some: reason => String.Format("Dead letter from: {0} to: {1}, failed because: {2} {3}. Type: {4} Content: {5}", ProcessFmt(Sender), Recipient, reason, ex.Message, ContentTypeDisplay, ContentDisplay),
                        None: ()     => String.Format("Dead letter from: {0} to: {1}, failed because: {2}. Type: {3} Content: {4}", ProcessFmt(Sender), Recipient, ex.Message, ContentTypeDisplay, ContentDisplay)
                    ),
                None: () =>
                    Reason.Match(
                        Some: reason => String.Format("Dead letter from: {0} to: {1}, failed because: {2}.  Type: {3} Content: {4}", ProcessFmt(Sender), Recipient, reason, ContentTypeDisplay, ContentDisplay),
                        None: ()     => String.Format("Dead letter from: {0} to: {1}.  Type: {2} Content: {3}", ProcessFmt(Sender), Recipient, ContentTypeDisplay, ContentDisplay)
                    ));
    }
}
