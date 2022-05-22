using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.SysX.Diag;
using LanguageExt.SysX.Live;
using static LanguageExt.Prelude;

namespace TestBed
{
    public class IssueTests
    {
        public static void Run()
        {
            Issue1028_Alt().Wait();
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
        
        public static async Task Issue1028_Alt()
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
            var src = new ActivitySource("what?!");

            var q = span("outer", src,
                from d in delay(1)
                from n in operationName 
                from _ in writeLine(n)
                from rs in span<Unit>("inner", src,
                    from n in operationName 
                    from _ in writeLine(n)
                    select unit)
                select rs);

            var r = await q.Run();
        }

        public static Aff<A> span<A>(string name, ActivitySource? src, Aff<A> ma) =>
            AffMaybe(async () =>
            {
                var act = Activity.Current is null
                    ? src?.StartActivity(name)
                    : src?.StartActivity(name, ActivityKind.Internal, Activity.Current.Context);
                
                using (act)
                {
                    return await ma.Run().ConfigureAwait(false);
                }
            });

        public static Eff<Activity?> current =>
            Eff(() => Activity.Current);

        public static Eff<string?> operationName =>
            current.Map(a => a?.OperationName);

        public static Aff<Unit> delay(int ms) =>
            Aff(async () =>
            {
                await Task.Delay(ms);
                return unit;
            });

        public static Eff<Unit> writeLine(string line) =>
            Eff(() =>
            {
                Console.WriteLine(line);
                return unit;
            });
    }
}
