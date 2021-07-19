using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Common;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace EffectsExamples
{
    class Program
    {
        static async Task Main(string[] args) =>
            await Menu<Runtime>.menu.Run(Runtime.New());
    }
}
