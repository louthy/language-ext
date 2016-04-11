using System;
using System.Threading.Tasks;
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
            ActorContext.InMessageLoop;

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

        internal static IDisposable safedelay(Action f, TimeSpan delayFor)
        {
            var savedContext = ActorContext.Request;
            var savedSession = ActorContext.SessionId;

            return (IDisposable)Task.Delay(delayFor).ContinueWith(_ =>
              {
                  try
                  {
                      if (savedContext == null)
                      {
                          f();
                      }
                      else
                      {
                          ActorContext.System(savedContext.Self.Actor.Id).WithContext(
                                       savedContext.Self,
                                       savedContext.Parent,
                                       savedContext.Sender,
                                       savedContext.CurrentRequest,
                                       savedContext.CurrentMsg,
                                       savedSession,
                                       () =>
                                       {
                                           f();

                                           // Run the operations that affect the settings and sending of tells
                                           // in the order which they occured in the actor
                                           ActorContext.Request.Ops.Run();
                                       });
                      }

                  }
                  catch (Exception e)
                  {
                      logErr(e);
                  }
              });
        }

        internal static IDisposable safedelay(Action f, DateTime delayUntil) =>
             safedelay(f, delayUntil - DateTime.UtcNow);

        /// <summary>
        /// Not advised to use this directly, but allows access to the underlying data-store.
        /// </summary>
        public static Option<ICluster> SystemCluster(SystemName system = default(SystemName)) => 
            ActorContext.System(system).Cluster;

        public static ProcessId resolvePID(ProcessId pid) =>
            ActorContext.ResolvePID(pid);
    }
}
