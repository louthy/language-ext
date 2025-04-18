using LanguageExt.Sys.Test;
using Xunit;
using static LanguageExt.Pipes.Producer;
using static LanguageExt.Pipes.Consumer;

namespace LanguageExt.Tests;

public class PipesTests
{
    [Fact]
    public void MergeSynchronousProducersSucceeds()
    {
        using var rt = Runtime.New();
        
        (merge(yield<Runtime, int>(1), yield<Runtime, int>(1))
           | awaiting<Runtime, int>().Map(ignore))
               .Run().As()
               .Run(rt, EnvIO.New())
               .Ignore();
    }
}
