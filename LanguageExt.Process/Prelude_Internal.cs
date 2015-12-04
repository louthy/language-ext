using System.Reactive.Subjects;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Process
    {

        /// <summary>
        /// Use in message loop exception
        /// </summary>
        internal static T raiseUseInMsgLoopOnlyException<T>(string what) =>
            failwith<T>($"'{what}' should be used from within a process' message loop only");

        /// <summary>
        /// Not in message loop exception
        /// </summary>
        internal static T raiseDontUseInMessageLoopException<T>(string what) =>
            failwith<T>($"'{what}' should not be be used from within a process' message loop.");

        /// <summary>
        /// Returns true if in a message loop
        /// </summary>
        internal static bool InMessageLoop =>
            ActorContext.Self.IsValid && ActorContext.Self.Path != ActorContext.User.Path;

        static Subject<Unit> shutdownSubj = new Subject<Unit>();
        static Subject<CancelShutdown> preShutdownSubj = new Subject<CancelShutdown>();

        internal static void OnShutdown()
        {
            shutdownSubj.OnNext(unit);
            shutdownSubj.OnCompleted();
        }

        internal static void OnPreShutdown(CancelShutdown cancel)
        {
            preShutdownSubj.OnNext(cancel);
            if (!cancel.Cancelled)
            {
                preShutdownSubj.OnCompleted();
            }
        }
    }
}
