using LanguageExt.Sys.Live;
using LanguageExt;

namespace EffectsExamples;

class Program
{
    static void Main(string[] args) =>
        Menu<Runtime>
           .menu
           .Run(Runtime.New(), EnvIO.New())
           .ThrowIfFail();
}
