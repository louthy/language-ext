using LanguageExt.Sys.Live;
using System.Threading.Tasks;

namespace EffectsExamples
{
    class Program
    {
        static async Task Main(string[] args) =>
            await Menu<Runtime>.menu.Run(Runtime.New());
    }
}
