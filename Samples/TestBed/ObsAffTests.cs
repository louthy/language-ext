using System;
using LanguageExt;
using LanguageExt.Sys;
using System.Reactive.Linq;
using LanguageExt.Sys.Live;
using System.Threading.Tasks;

namespace TestBed
{
    public class ObsAffTests
    {
        public static async Task Test()
        {
            var obs = Observable.Interval(TimeSpan.FromSeconds(1));

            var aff = obs.Fold<Runtime, int, long>(0, (s, x) => from n in Time<Runtime>.now
                                                                from _ in Console<Runtime>.writeLine($"{s}: {n}")
                                                                select s + 1);

            await aff.Run(Runtime.New());
        }
    }
}

