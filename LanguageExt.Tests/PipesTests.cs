using LanguageExt.Pipes;
using LanguageExt.Sys.Test;
using Xunit;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Tests;

public class PipesTests
{
    [Fact]
    public void MergeSynchronousProducersSucceeds() =>
        compose(
            Producer.merge<int, Eff<Runtime>>(
                yield(1),
                yield(1)),
            awaiting<int>().Map(ignore))
        .RunEffect().As()
        .Run(Runtime.New(), EnvIO.New())
        .Ignore();
}
