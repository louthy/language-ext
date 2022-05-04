using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.SysX.Diag;
using LanguageExt.SysX.Live;
using LanguageExt;
using LanguageExt.Sys;

namespace TestBed
{
    public class IssueTests
    {
        public static void Run()
        {
            Issue1028();
        }

        public static async Task RunAlwaysNull()
        {
            using var listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Start"),
                ActivityStopped = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Stop")
            };

            ActivitySource.AddActivityListener(listener);
 
            var activitySource = new ActivitySource("what?!");
            
            await Start(activitySource);
            Console.WriteLine(Activity.Current);
        }

        public static async Task<Activity?> Start(ActivitySource src)
        {
            await Task.Delay(1);
            return src.StartActivity("example");
        }

        public static void Issue1028()
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name);

            using var listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Start"),
                ActivityStopped = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Stop")
            };
            ActivitySource.AddActivityListener(listener);

            var q = Activity<Runtime>.span("outer",
                        from n in Activity<Runtime>.operationName 
                        from _ in Console<Runtime>.writeLine(n.IfNone("[not set]"))
                        from rs in Activity<Runtime>.span("inner",
                            from n in Activity<Runtime>.operationName 
                            from _ in Console<Runtime>.writeLine(n.IfNone("[not set]"))
                            select unit)
                        select rs);

            var r = q.Run(Runtime.New());
        }
    }
}
