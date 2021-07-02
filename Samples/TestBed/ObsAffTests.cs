using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using LanguageExt.Sys.IO;
using static LanguageExt.Prelude;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace TestBed
{
    public class ObsAffTests
    {
        public static async Task Test()
        {
            var obs = Observable.Interval(TimeSpan.FromSeconds(1), TaskPoolScheduler.Default);

            var aff = obs.Fold<Runtime, int, long>(0, (s, x) => from n in DateTime<Runtime>.now
                                                                from _ in Console<Runtime>.writeLine($"{s}: {n}")
                                                                select s + 1);

            await aff.Run(Runtime.New());
        }
    }
}

