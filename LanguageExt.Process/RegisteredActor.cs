using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class RegisteredActor<T>
    {
        public static ProcessId Inbox(ProcessId pid, T message)
        {
            if (ActorContext.CurrentRequest == null)
            {
                // Redirect messages to the original
                tell(pid, message, Sender);
            }
            else
            {
                // Ask the question of the original and send it back
                reply(ask<T>(pid, message));
            }
            return pid;
        }
    }
}
