using LanguageExt;
using LanguageExt.Sys.Live;

namespace EffectsExamples;

class Program
{
    static void Main(string[] args) =>
        Menu<Runtime>
           .menu
           .Run(Runtime.New(), EnvIO.New())
           .ThrowIfFail();
}
