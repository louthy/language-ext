using BenchmarkDotNet.Running;

namespace LanguageExt.Benchmarks
{
    class Program
    {
        static void Main(string[] args) =>
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args);
    }
}
